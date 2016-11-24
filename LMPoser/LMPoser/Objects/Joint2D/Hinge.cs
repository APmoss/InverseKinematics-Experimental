using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LMPoser.Objects.Joint2D {
	public class Hinge {
		private const float DEFAULT_LENGTH = 100f;

		private Hinge _child;

		public float Angle {
			get; set;
		}

		public float Length {
			get; private set;
		}

		public Vector2 Resultant {
			get {
				return new Vector2(
					(float)Math.Cos(Angle),
					(float)Math.Sin(Angle)
				) * Length;
			}
		}

		public Vector2 GlobalResultant {
			get {
				if (Parent == null) {
					return Resultant;
				}
				else {
					return Resultant + Parent.GlobalResultant;
				}
			}
		}

		public Vector2 GlobalPosition {
			get {
				if (Parent == null) {
					return Vector2.Zero;
				}
				else {
					return Parent.GlobalResultant;
				}
			}
		}

		public Hinge Parent {
			get; private set;
		}
		public Hinge Child {
			get {
				return _child;
			}
			set {
				_child = value;
				if (_child != null) {
					_child.Parent = this;
				}
			}
		}

		public Vector2 EndEffector {
			get {
				if (Child == null) {
					return GlobalResultant;
				}
				else {
					return Child.EndEffector;
				}
			}
		}

		public Hinge(float angle)
			: this(angle, DEFAULT_LENGTH) {
		}
		public Hinge(float angle, float length)
			: this(angle, length, null) {
		}
		public Hinge(float angle, Hinge child)
			: this(angle, DEFAULT_LENGTH, child) {
		}
		public Hinge(float angle, float length, Hinge child) {
			Angle = angle;
			Length = length;
			Child = child;
		}
		public Hinge(Hinge hinge) {
			Angle = hinge.Angle;
			Length = hinge.Length;
			
			if (hinge.Child != null) {
				Child = new Hinge(hinge.Child);
			}
		}

		public IEnumerable<VertexPositionColor> GetVertices() {
			var vertices = new List<VertexPositionColor>();

			if (Parent == null) {
				vertices.Add(new VertexPositionColor(
					Vector3.Zero,
					Color.Red
				));
				vertices.Add(new VertexPositionColor(
					new Vector3(Resultant, 0f),
					Color.Blue
				));
			}
			else {
				vertices.Add(new VertexPositionColor(
					new Vector3(Parent.GlobalResultant, 0f),
					Color.Red
				));
				vertices.Add(new VertexPositionColor(
					new Vector3(GlobalResultant, 0f),
					Color.Blue
				));
			}

			if (_child != null) {
				vertices.AddRange(_child.GetVertices());
			}

			return vertices;
		}

		public List<short> GetLines() {
			var lines = new List<short>();

			lines.Add(0);
			lines.Add(1);

			if (_child != null) {
				lines.AddRange(_child.GetLines(2));
			}

			return lines;
		}

		private List<short> GetLines(int depth) {
			var lines = new List<short>();

			lines.Add((short)(depth - 1));
			lines.Add((short)depth);
			
			if (_child != null) {
				lines.AddRange(_child.GetLines(depth + 1));
			}

			return lines;
		}

		public void UpdateDoFChain(LinkedList<float> dofs) {
			Angle = dofs.First.Value;

			dofs.RemoveFirst();

			if (dofs.Count > 0 && Child != null) {
				Child.UpdateDoFChain(dofs);
			}
		}
		public void ApplyDofDeltas(LinkedList<float> deltas) {
			Angle += deltas.First.Value;

			deltas.RemoveFirst();

			if (deltas.Count > 0 && Child != null) {
				Child.ApplyDofDeltas(deltas);
			}
		}

		public int Size() {
			if (Child == null) {
				return 1;
			}
			else {
				return 1 + Child.Size();
			}
		}

		public Vector2 NumericalJacobianColumn() {
			var copy = new Hinge(this);
			copy.Parent = Parent;

			copy.Angle += MathHelper.ToRadians(0.1f);

			var oldE = EndEffector;
			var newE = copy.EndEffector;

			return newE - oldE;
		}
	}
}
