using LMPoser.Objects.Joint2D;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMPoser.Objects {
	public class IK2DHelper {
		public float MaxDeltaE = 100f;
		public float SolutionThreshold = 10f;
		public int MaxIterations = 100;
		public float DLSLambda = .5f;

		public int SolveJT(Hinge joint, Vector2 goal) {
			var dist = Vector2.Distance(joint.EndEffector, goal);
			var iterations = 0;

			while (dist > SolutionThreshold && iterations < MaxIterations) {
				IterateJT(joint, goal);
				dist = Vector2.Distance(joint.EndEffector, goal);
				iterations++;
			}

			return iterations;
		}
		public void IterateJT(Hinge joint, Vector2 goal) {
			var jacobian = ComputeJacobian(joint);
			var jacobianMat = CreateMatrix.Dense(2, jacobian.Count, (row, col) => row == 0 ? jacobian[col].X : jacobian[col].Y);
			var deltaE = ClampDeltaE(goal, joint.EndEffector);

			if (deltaE.LengthSquared() < 0.0001) {
				return;
			}

			var deltaEVec = CreateVector.Dense(new float[] { deltaE.X, deltaE.Y });
			var denom = (jacobianMat * jacobianMat.Transpose() * deltaEVec).L2Norm();

			var lambda = (deltaEVec * jacobianMat * jacobianMat.Transpose() * deltaEVec) / (denom * denom);

			var deltaPhiVec = (float)lambda * jacobianMat.Transpose() * deltaEVec;
			var deltaPhi = new LinkedList<float>(deltaPhiVec.ToArray());

			joint.ApplyDofDeltas(deltaPhi);
		}

		public int SolveJI(Hinge joint, Vector2 goal) {
			var dist = Vector2.Distance(joint.EndEffector, goal);
			var iterations = 0;

			while (dist > SolutionThreshold && iterations < MaxIterations) {
				IterateJI(joint, goal);
				dist = Vector2.Distance(joint.EndEffector, goal);
				iterations++;
			}

			return iterations;
		}
		public void IterateJI(Hinge joint, Vector2 goal) {
			var jacobian = ComputeJacobian(joint);
			var deltaE = ClampDeltaE(goal, joint.EndEffector);

			var jacobianMat = CreateMatrix.Dense(2, jacobian.Count, (row, col) => row == 0 ? jacobian[col].X : jacobian[col].Y);

			var inverse = jacobianMat.GeneralizedInverse();
			var pInverse = jacobianMat.SVDPseudoInverse();

			var deltaPhi = new LinkedList<float>();

			foreach (var row in pInverse.ToRowArrays()) {
				deltaPhi.AddLast((deltaE.X * row[0]) + (deltaE.Y * row[1]));
			}

			joint.ApplyDofDeltas(deltaPhi);
		}

		public int SolveDLS(Hinge joint, Vector2 goal) {
			var dist = Vector2.Distance(joint.EndEffector, goal);
			var iterations = 0;

			while (dist > SolutionThreshold && iterations < MaxIterations) {
				IterateDLS(joint, goal);
				dist = Vector2.Distance(joint.EndEffector, goal);
				iterations++;
			}

			return iterations;
		}
		public void IterateDLS(Hinge joint, Vector2 goal) {
			var jacobian = ComputeJacobian(joint);
			var deltaE = ClampDeltaE(goal, joint.EndEffector);

			var jacobianMat = CreateMatrix.Dense(2, jacobian.Count, (row, col) => row == 0 ? jacobian[col].X : jacobian[col].Y);

			var lambda = DLSLambda;
			var lambdaMatrix = lambda * lambda * CreateMatrix.DenseIdentity<float>(jacobianMat.RowCount);
			var deltaPhiVec =
				jacobianMat.Transpose() *
				(jacobianMat * jacobianMat.Transpose() + (lambdaMatrix)).Inverse() *
				CreateVector.DenseOfArray(new float[] { deltaE.X, deltaE.Y });
			var deltaPhi = new LinkedList<float>(deltaPhiVec.ToArray());

			joint.ApplyDofDeltas(deltaPhi);
		}

		public int SolveCCD(Hinge joint, Vector2 goal) {
			var dist = Vector2.Distance(joint.EndEffector, goal);
			var iterations = 0;

			while (dist > SolutionThreshold && iterations < MaxIterations) {
				IterateCCD(joint, goal);
				dist = Vector2.Distance(joint.EndEffector, goal);
				iterations++;
			}

			return iterations;
		}
		public void IterateCCD(Hinge joint, Vector2 goal) {
			var focus = joint;

			while (focus != null) {
				var toE = focus.EndEffector - focus.GlobalPosition;
				var toG = goal - focus.GlobalPosition;

				var eAngle = Math.Atan2(toE.Y, toE.X);
				var gAngle = Math.Atan2(toG.Y, toG.X);

				focus.Angle += ((float)(gAngle - eAngle));

				focus = focus.Child;
			}
		}

		private List<Vector2> ComputeJacobian(Hinge joint) {
			var jacobian = new List<Vector2>();
			var focus = joint;

			while (focus != null) {
				jacobian.Add(focus.AnalyticalJacobianColumn());

				focus = focus.Child;
			}

			return jacobian;
		}

		private Vector2 ClampDeltaE(Vector2 goal, Vector2 endE) {
			var deltaE = goal - endE;
			var length = deltaE.Length();


			if (length > MaxDeltaE) {
				var mult = MaxDeltaE / length;
				deltaE *= mult;
			}

			return deltaE;
		}
	}
}
