using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LMPoser.Objects.Joint2D {
	public class Hinge {
		private const float DEFAULT_LENGTH = 100f;

		private float _angle;
		private Hinge _child;

		public float Angle {
			get {
				return _angle;
			}
			set {
				var lower = AngleLimitLower ?? value;
				var upper = AngleLimitUpper ?? value;

				_angle = MathHelper.Clamp(value, lower, upper);
			}
		}
		public float GlobalAngle {
			get {
				if (Parent == null) {
					return Angle;
				}
				else {
					return Angle + Parent.GlobalAngle;
				}
			}
		}

		public float? AngleLimitUpper {
			get; set;
		} = null;
		public float? AngleLimitLower {
			get; set;
		} = null;

		public float Length {
			get; set;
		}

		public Vector2 DefaultPosition {
			get; set;
		} = Vector2.Zero;

		public Vector2 Resultant {
			get {
				return DefaultPosition +
					new Vector2(
						(float)Math.Cos(GlobalAngle),
						(float)Math.Sin(GlobalAngle)
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
					return DefaultPosition;
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
					new Vector3(GlobalPosition, 0f),
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

		public Vector2 AnalyticalJacobianColumn() {
			var e = EndEffector;
			var r = GlobalPosition;
			var a = new Vector3(0, 0, 1);

			var er = new Vector3(e - r, 0);

			var partial = Vector3.Cross(a, er);

			return new Vector2(partial.X, partial.Y);
		}

		public float SolutionSpaceRadius() {
			var radius = Length;

			if (Child != null) {
				radius += Child.SolutionSpaceRadius();
			}

			return radius;
		}
	}
}
