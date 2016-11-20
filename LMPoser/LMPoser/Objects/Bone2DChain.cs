using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LMPoser.Objects {
	public class Bone2DChain {
		private const float DEFAULT_LENGTH = 100f;

		private Bone2DChain _child;

		public float Angle {
			get; set;
		}

		public float Length {
			get; set;
		}

		public Vector2 Position {
			get {
				return new Vector2(
					(float)Math.Cos(Angle),
					(float)Math.Sin(Angle)
				) * Length;
			}
		}

		public Vector2 GlobalPosition {
			get {
				if (Parent == null) {
					return Position;
				}
				else {
					return Position + Parent.GlobalPosition;
				}
			}
		}

		public Bone2DChain Parent {
			get; private set;
		}
		public Bone2DChain Child {
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

		public Bone2DChain(float angle)
			: this(angle, DEFAULT_LENGTH) {
		}
		public Bone2DChain(float angle, float length)
			: this(angle, length, null) {
		}
		public Bone2DChain(float angle, Bone2DChain child)
			: this(angle, DEFAULT_LENGTH, child) {
		}
		public Bone2DChain(float angle, float length, Bone2DChain child) {
			Angle = angle;
			Length = length;
			Child = child;
		}

		public List<VertexPositionColor> GetVertices() {
			var vertices = new List<VertexPositionColor>();

			if (Parent == null) {
				vertices.Add(new VertexPositionColor(
					Vector3.Zero,
					Color.Red
				));
				vertices.Add(new VertexPositionColor(
					new Vector3(Position, 0f),
					Color.Blue
				));
			}
			else {
				vertices.Add(new VertexPositionColor(
					new Vector3(Parent.GlobalPosition, 0f),
					Color.Red
				));
				vertices.Add(new VertexPositionColor(
					new Vector3(GlobalPosition, 0f),
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
	}
}
