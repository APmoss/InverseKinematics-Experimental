using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LMPoser.Objects.Joint3D {
	public class BallJoint : Joint3D {
		public float RotX {
			get; set;
		}
		public float RotY {
			get; set;
		}
		public float RotZ {
			get; set;
		}

		public override Matrix GetLocalTransformMatrix() {
			return Matrix.CreateRotationZ(RotZ) *
					Matrix.CreateRotationY(RotY) *
					Matrix.CreateRotationX(RotX);
		}

		public override Matrix GetGlobalTransformMatrix() {
			if (Parent == null) {
				return GetLocalTransformMatrix();
			}
			else {
				return GetLocalTransformMatrix() * Matrix.CreateTranslation(Parent.Resultant) * Parent.GetGlobalTransformMatrix();
			}
		}

		public override IEnumerable<VertexPositionColor> GetVertices() {
			return new List<VertexPositionColor>() {
				new VertexPositionColor(Position, Color.MediumVioletRed),
				new VertexPositionColor(GlobalResultant, Color.Pink)
			};
		}
	}
}
