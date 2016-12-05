using LMPoser.Objects;
using LMPoser.Objects.Joint2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Project_GE.Framework.Input;
using System;

namespace LMPoser.Scenes {
	public class VarianceScene : PrimitiveScene {
		private Hinge baseJoint;
		private Vector2 goal = Vector2.Zero;

		private IK2DHelper ik = new IK2DHelper();

		public override void Load(ContentManager content) {
			base.Load(content);

			InitJoints();
		}

		public override void Input(InputManager input) {
			base.Input(input);

			if (input.IsMouseDown(MouseButtons.Left)) {
				UpdateGoalPosition(input.CurrentMousePosition.ToVector2());
			}

			if (input.IsKeyDown(Keys.LeftShift) || input.IsKeyDown(Keys.RightShift)) {
				if (input.IsKeyPressed(Keys.Space)) {
					Console.WriteLine("DLS: " + ik.SolveDLS(baseJoint, goal));
					Console.WriteLine();
				}
			}
			else {
				if (input.IsKeyDown(Keys.Space)) {
					ik.IterateDLS(baseJoint, goal);
				}
			}
		}

		public override void Update() {
			base.Update();

			DrawCircle(goal, 10, 10, Color.Green);
			DrawCircle(Vector2.Zero, baseJoint.SolutionSpaceRadius(), 100, Color.DarkRed);

			Vertices.AddRange(baseJoint.GetVertices());
		}

		private void InitJoints() {
			baseJoint = new Hinge(MathHelper.PiOver4,
				new Hinge(
				MathHelper.PiOver4,
					new Hinge(
						MathHelper.PiOver4,
						new Hinge(
							MathHelper.PiOver4
						) { Length = 40f }
					) { Length = 20f }
				) { Length = 80f }
			) { Length = 30f };
		}

		private void UpdateGoalPosition(Vector2 mouse) {
			goal = mouse;
			goal -= new Vector2(WIDTH / 2, HEIGHT / 2);
			goal.Y = -goal.Y;

			var dist = Vector2.Distance(goal, Vector2.Zero);
			var rad = baseJoint.SolutionSpaceRadius();

			if (dist > rad) {
				goal *= rad / dist;
			}
		}
	}
}
