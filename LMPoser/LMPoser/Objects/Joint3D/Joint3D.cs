using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace LMPoser.Objects.Joint3D {
	public abstract class Joint3D {
		private List<Joint3D> _children = new List<Joint3D>();

		public string Name {
			get; set;
		} = "Unnamed Joint";

		public float Length {
			get; set;
		} = 10f;

		public Joint3D Parent {
			get; set;
		}
		public IEnumerable<Joint3D> Children {
			get {
				return _children.AsReadOnly();
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
	}
}
