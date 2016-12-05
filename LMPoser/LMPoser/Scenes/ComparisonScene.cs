using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project_GE.Framework.Input;
using LMPoser.Objects.Joint2D;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using LMPoser.Objects;
using Project_GE.Framework.Gui.Controls;

namespace LMPoser.Scenes {
	public class ComparisonScene : PrimitiveScene {
		private const int HALF_W = WIDTH / 2;
		private const int HALF_H = HEIGHT / 2;

		private Vector2 goal = new Vector2(-HALF_W / 2, HALF_H / 2);

		private Hinge hingeJT;
		private Hinge hingeJI;
		private Hinge hingeDLS;
		private Hinge hingeCCD;

		private IK2DHelper ik = new IK2DHelper();

		public override void Load(ContentManager content) {
			base.Load(content);

			InitJoints();
			InitGui();
		}

		public override void Input(InputManager input) {
			base.Input(input);

			if (input.IsMouseDown(MouseButtons.Left)) {
				UpdateGoalPosition(input.CurrentMousePosition.ToVector2());
			}

			if (input.IsKeyDown(Keys.LeftShift) || input.IsKeyDown(Keys.RightShift)) {
				if (input.IsKeyPressed(Keys.Space)) {
					Console.WriteLine("JT: " + ik.SolveJT(hingeJT, goal));
					Console.WriteLine("JI: " + ik.SolveJI(hingeJI, goal + new Vector2(HALF_W, 0)));
					Console.WriteLine("DLS: " + ik.SolveDLS(hingeDLS, goal + new Vector2(0, -HALF_H)));
					Console.WriteLine("CCD: " + ik.SolveCCD(hingeCCD, goal + new Vector2(HALF_W, -HALF_H)));
					Console.WriteLine();
				}
			}
			else {
				if (input.IsKeyPressed(Keys.Space)) {
					ik.IterateJT(hingeJT, goal);
					ik.IterateJI(hingeJI, goal + new Vector2(HALF_W, 0));
					ik.IterateDLS(hingeDLS, goal + new Vector2(0, -HALF_H));
					ik.IterateCCD(hingeCCD, goal + new Vector2(HALF_W, -HALF_H));
				}
			}
		}

		public override void Update() {
			base.Update();

			// Dividers
			DrawLine(new Vector2(0, HALF_H), new Vector2(0, -HALF_H), Color.White);
			DrawLine(new Vector2(-HALF_W, 0), new Vector2(HALF_W, 0), Color.White);

			// Goal markers
			DrawCircle(goal, 10, 10, Color.Green);
			DrawCircle(goal + new Vector2(HALF_W, 0), 10, 10, Color.Green);
			DrawCircle(goal + new Vector2(0, -HALF_H), 10, 10, Color.Green);
			DrawCircle(goal + new Vector2(HALF_W, -HALF_H), 10, 10, Color.Green);

			// Solution Spaces
			DrawCircle(hingeJT.DefaultPosition, hingeJT.SolutionSpaceRadius(), 30, Color.DarkRed);
			DrawCircle(hingeJI.DefaultPosition, hingeJI.SolutionSpaceRadius(), 30, Color.DarkRed);
			DrawCircle(hingeDLS.DefaultPosition, hingeDLS.SolutionSpaceRadius(), 30, Color.DarkRed);
			DrawCircle(hingeCCD.DefaultPosition, hingeCCD.SolutionSpaceRadius(), 30, Color.DarkRed);

			// Joints
			Vertices.AddRange(hingeJT.GetVertices());
			Vertices.AddRange(hingeJI.GetVertices());
			Vertices.AddRange(hingeDLS.GetVertices());
			Vertices.AddRange(hingeCCD.GetVertices());
		}

		private void InitJoints() {
			hingeJT = new Hinge(MathHelper.PiOver4,
				new Hinge(
				MathHelper.PiOver4,
					new Hinge(
						MathHelper.PiOver4
					) { Length = 50f }
				) { Length = 50f }
			) { DefaultPosition = new Vector2(-HALF_W / 2, HALF_H / 2), Length = 50f };
			hingeJI = new Hinge(MathHelper.PiOver4,
				new Hinge(
				MathHelper.PiOver4,
					new Hinge(
						MathHelper.PiOver4
					) { Length = 50f }
				) { Length = 50f }
			) { DefaultPosition = new Vector2(HALF_W / 2, HALF_H / 2), Length = 50f };
			hingeDLS = new Hinge(MathHelper.PiOver4,
				new Hinge(
				MathHelper.PiOver4,
					new Hinge(
						MathHelper.PiOver4
					) { Length = 50f }
				) { Length = 50f }
			) { DefaultPosition = new Vector2(-HALF_W / 2, -HALF_H / 2), Length = 50f };
			hingeCCD = new Hinge(MathHelper.PiOver4,
				new Hinge(
				MathHelper.PiOver4,
					new Hinge(
						MathHelper.PiOver4
					) { Length = 50f }
				) { Length = 50f }
			) { DefaultPosition = new Vector2(HALF_W / 2, -HALF_H / 2), Length = 50f };
		}

		private void InitGui() {
			Gui.BaseContainer.AddChildren(
				new TextBlock("JT", 610, 320) { BackgroundTint = Color.White },
				new TextBlock("JI", 650, 320) { BackgroundTint = Color.White },
				new TextBlock("DLS", 600, 370) { BackgroundTint = Color.White },
				new TextBlock("CCD", 650, 370) { BackgroundTint = Color.White }
			);
		}

		private void UpdateGoalPosition(Vector2 mouse) {
			var newGoal = new Vector2(
				mouse.X % HALF_W,
				mouse.Y % HALF_H
			);

			newGoal -= new Vector2(HALF_W, HALF_H);
			newGoal.Y = -newGoal.Y;

			var def = hingeJT.DefaultPosition;
			var toGoal = newGoal - def;
			var dist = toGoal.Length();
			var rad = hingeJT.SolutionSpaceRadius();

			if (dist <= rad) {
				goal = def + toGoal;
			}
			else {
				goal = def + toGoal * (rad / dist);
			}
		}
	}
}
