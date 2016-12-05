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
	public class AngleLimitScene : PrimitiveScene {
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
					Console.WriteLine("JT: " + ik.SolveJT(baseJoint, goal));
					Console.WriteLine();
				}
			}
			else {
				if (input.IsKeyDown(Keys.Space)) {
					ik.IterateJT(baseJoint, goal);
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
						MathHelper.PiOver4
					) { AngleLimitLower = -MathHelper.PiOver2, AngleLimitUpper = MathHelper.PiOver2 }
				) { AngleLimitLower = -MathHelper.PiOver2, AngleLimitUpper = MathHelper.PiOver2 }
			);
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
