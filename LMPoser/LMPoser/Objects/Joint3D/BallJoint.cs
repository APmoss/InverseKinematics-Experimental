using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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
					Matrix.CreateRotationZ(RotY) *
					Matrix.CreateRotationZ(RotX);
		}
	}
}
