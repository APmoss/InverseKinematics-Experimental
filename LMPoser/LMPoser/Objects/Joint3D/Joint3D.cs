using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LMPoser.Objects.Joint3D {
	public abstract class Joint3D {
		private List<Joint3D> _children = new List<Joint3D>();

		public string Name {
			get; set;
		} = "Unnamed Joint";

		public float Length {
			get; set;
		} = 1f;

		public Joint3D Parent {
			get; set;
		}
		public IEnumerable<Joint3D> Children {
			get {
				return _children.AsReadOnly();
			}
		}

		public Vector3 Position {
			get {
				if (Parent == null) {
					return Vector3.Zero;
				}
				else {
					return Parent.GlobalResultant;
				}
			}
		}

		public Vector3 Resultant {
			get {
				return Vector3.Transform(Vector3.UnitX * Length, GetLocalTransformMatrix());
			}
		}

		public Vector3 GlobalResultant {
			get {
				return Vector3.Transform(Vector3.UnitX * Length, GetGlobalTransformMatrix());
			}
		}

		public int Count {
			get {
				int count = 1;
				foreach (var child in _children) {
					count += child.Count;
				}

				return count;
			}
		}

		public Joint3D() {
		}

		public Joint3D(IEnumerable<Joint3D> children) {
			foreach (var child in children) {
				AddChild(child);
			}
		}

		public void AddChild(Joint3D child) {
			child.Parent = this;
			_children.Add(child);
		}

		public abstract Matrix GetLocalTransformMatrix();

		public abstract Matrix GetGlobalTransformMatrix();

		public abstract IEnumerable<VertexPositionColor> GetVertices();

		public IEnumerable<VertexPositionColor> AllVertices() {
			var vertices = new List<VertexPositionColor>();

			vertices.AddRange(GetVertices());

			foreach (var child in _children) {
				vertices.AddRange(child.AllVertices());
			}

			return vertices;
		}
	}
}
