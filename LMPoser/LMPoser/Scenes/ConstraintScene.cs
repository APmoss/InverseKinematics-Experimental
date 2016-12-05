using LMPoser.Objects.Joint2D;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Project_GE.Framework.Input;
using LMPoser.Objects;
using Microsoft.Xna.Framework.Input;

namespace LMPoser.Scenes {
	public class ConstraintScene : PrimitiveScene {
		protected const int HALF_W = WIDTH / 2;
		protected const int HALF_H = HEIGHT / 2;

		private Vector2 goal = Vector2.Zero;
		private Hinge leftJoint;
		private Hinge rightJoint;

		private IK2DHelper ik = new IK2DHelper();

		public override void Load(ContentManager content) {
			base.Load(content);

			ik.SolutionThreshold = 5f;

			InitJoints();

			ik.SolveCCD(leftJoint, goal);
			ik.SolveCCD(rightJoint, goal);
		}

		public override void Input(InputManager input) {
			base.Input(input);

			if (input.IsMouseDown(MouseButtons.Left)) {
				UpdateGoalPosition(input.CurrentMousePosition.ToVector2());
			}

			if (input.IsKeyDown(Keys.LeftShift) || input.IsKeyDown(Keys.RightShift)) {
				if (input.IsKeyPressed(Keys.Space)) {
					Console.WriteLine("Left CCD: " + ik.SolveCCD(leftJoint, goal));
					Console.WriteLine("Right CCD: " + ik.SolveCCD(rightJoint, goal));
					Console.WriteLine();
				}
			}
			else {
				if (input.IsKeyDown(Keys.Space)) {
					ik.IterateCCD(leftJoint, goal);
					ik.IterateCCD(rightJoint, goal);
				}
			}
		}

		public override void Update() {
			base.Update();

			// Goal Marker
			DrawCircle(goal, 5, 5, Color.Green);

			// Solution Spaces
			DrawCircle(leftJoint.DefaultPosition, leftJoint.SolutionSpaceRadius(), 30, new Color(50, 0, 0));
			DrawCircle(rightJoint.DefaultPosition, rightJoint.SolutionSpaceRadius(), 30, new Color(50, 0, 0));

			// Joints
			Vertices.AddRange(leftJoint.GetVertices());
			Vertices.AddRange(rightJoint.GetVertices());
		}

		private void InitJoints() {
			leftJoint = new Hinge(MathHelper.PiOver4,
				new Hinge(
				MathHelper.PiOver4,
					new Hinge(
						MathHelper.PiOver4,
						new Hinge(
							MathHelper.PiOver4
						)
					)
				)
			) { DefaultPosition = new Vector2(-HALF_W / 4, 0) };
			rightJoint = new Hinge(MathHelper.PiOver4,
				new Hinge(
				MathHelper.PiOver4,
					new Hinge(
						MathHelper.PiOver4,
						new Hinge(
							MathHelper.PiOver4
						)
					)
				)
			) { DefaultPosition = new Vector2(HALF_W / 4, 0) };
		}

		private void UpdateGoalPosition(Vector2 mouse) {
			var newGoal = mouse;

			newGoal -= new Vector2(HALF_W, HALF_H);
			newGoal.Y = -newGoal.Y;

			var def = leftJoint.DefaultPosition;
			var toGoal = newGoal - def;
			var dist = toGoal.Length();
			var rad = leftJoint.SolutionSpaceRadius();

			if (dist <= rad) {
				goal = def + toGoal;
			}
			else {
				goal = def + toGoal * (rad / dist);
			}

			def = rightJoint.DefaultPosition;
			toGoal = goal - def;
			dist = toGoal.Length();
			rad = rightJoint.SolutionSpaceRadius();

			if (dist <= rad) {
				goal = def + toGoal;
			}
			else {
				goal = def + toGoal * (rad / dist);
			}
		}
	}
}
