using LMPoser.Objects.Joint2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project_GE.Framework.Gui.Controls;
using Project_GE.Framework.Input;
using Project_GE.Framework.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace LMPoser.Scenes {
	public class MainScene : Scene2DGui {
		private const int WIDTH = 1280;
		private const int HEIGHT = 720;

		private GraphicsDevice graphics;
		private BasicEffect effect;
		private RasterizerState rasterizer;

		private Matrix world = Matrix.Identity;

		private Camera3D camera;

		private Hinge baseJoint;
		private Hinge focusJoint;
		private bool updateFocusJoint = false;

		private List<VertexPositionColor> vertices = new List<VertexPositionColor>();
		private Vector2 goal = Vector2.Zero;

		TextBlock jointAngleText;
		TextBlock goalText;
		TextBlock sizeText;

		public MainScene()
			: base(new Rectangle(0, 0, WIDTH, HEIGHT), "Content/GuiThemes/LightTheme.xml") {
		}

		public override void Load(ContentManager content) {
			graphics = SceneManager.GraphicsDevice;
			camera = new Camera3D();

			effect = new BasicEffect(graphics);
			effect.VertexColorEnabled = true;

			rasterizer = new RasterizerState();
			rasterizer.FillMode = FillMode.WireFrame;
			rasterizer.CullMode = CullMode.None;

			InitJoints();

			base.Load(content);

			var panel = new Panel(new Rectangle(10, 10, 300, 500));
			jointAngleText = new TextBlock("A", 0, 0);
			goalText = new TextBlock("A", 0, 30);
			sizeText = new TextBlock(baseJoint.Size().ToString(), 0, 60);

			panel.AddChildren(jointAngleText, goalText, sizeText);

			Gui.BaseContainer.AddChildren(
				panel
			);
		}

		public override void Input(InputManager input) {
			if (input.IsKeyPressed(Keys.Escape)) {
				SceneManager.Game.Exit();
			}

			camera.Input(input, GameTime);

			// TODO: dimensions
			if (updateFocusJoint) {
				var toMouse = new Vector2(
					input.CurrentMousePosition.X - (focusJoint.GlobalPosition.X + WIDTH / 2),
					-input.CurrentMousePosition.Y + (-focusJoint.GlobalPosition.Y + HEIGHT / 2)

				);

				focusJoint.Angle = (float)Math.Atan2(
					toMouse.Y,
					toMouse.X
				);

				if (input.IsKeyPressed(Keys.M)) {
					if (focusJoint.Child != null) {
						focusJoint = focusJoint.Child;
					}
					else {
						focusJoint = null;
						updateFocusJoint = false;
					}
				}
				if (input.IsKeyPressed(Keys.N)) {
					focusJoint = focusJoint.Parent != null ? focusJoint.Parent : focusJoint;
				}
			}

			if (input.IsKeyPressed(Keys.Space)) {
				updateFocusJoint = true;
				focusJoint = baseJoint;
			}

			if (input.IsKeyDown(Keys.J)) {
				JTIterate();
			}
			if (input.IsKeyDown(Keys.K)) {
				JIIterate();
			}
			if (input.IsKeyDown(Keys.U)) {
				goal = input.CurrentMousePosition.ToVector2();
				goal -= new Vector2(WIDTH / 2, HEIGHT / 2);
				goal.Y = -goal.Y;
			}

			base.Input(input);
		}

		public override void Update() {
			vertices.Clear();

			vertices.AddRange(baseJoint.GetVertices());
			vertices.AddRange(GetCircle(goal, 5, 5, Color.Green));

			UpdateEffect();

			if (updateFocusJoint) {
				jointAngleText.Text = "Current Joint Angle: " + focusJoint.Angle;
				jointAngleText.AutoAdjustWidth();

				vertices.AddRange(GetCircle(focusJoint.GlobalPosition, 10, 10, Color.Purple));
			}

			goalText.Text = "Goal Position: " + goal.X + ", " + goal.Y;
			goalText.AutoAdjustWidth();

			base.Update();
		}

		public override void Draw() {
			graphics.Clear(Color.Black);

			DrawBones();

			base.Draw();
		}

		private void InitJoints() {
			baseJoint = new Hinge(MathHelper.ToRadians(30f),
				new Hinge(
				0f,
					new Hinge(
						MathHelper.PiOver2,
						new Hinge(
							MathHelper.PiOver4
						)
					)
				)
			);

			focusJoint = baseJoint;
		}

		private void UpdateEffect() {
			effect.World = world;
			effect.View = camera.GetViewMatrix();
			effect.Projection = camera.GetOrthographicProjectionMatrix();
		}

		private void DrawBones() {
			foreach (var pass in effect.CurrentTechnique.Passes) {
				pass.Apply();

				graphics.RasterizerState = rasterizer;

				graphics.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count() / 2);
			}
		}

		private IEnumerable<VertexPositionColor> GetCircle(Vector2 center, float radius, int sides, Color color) {
			var verts = new List<VertexPositionColor>();
			var offset = new Vector3(center, 0);

			for (int i = 0; i < 360; i += 360 / sides) {
				var angle = MathHelper.ToRadians(i);
				var v1 = radius * new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);

				angle = MathHelper.ToRadians(i + 360 / sides);
				var v2 = radius * new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);

				verts.Add(new VertexPositionColor(
					offset + v1,
					color
				));
				verts.Add(new VertexPositionColor(
					offset + v2,
					color
				));
			}

			return verts;
		}

		private void JTIterate() {
			var deltaE = goal - baseJoint.EndEffector;
			deltaE *= 0.01f;
			var jacobian = new List<Vector2>();
			var focus = baseJoint;

			while (focus != null) {
				jacobian.Add(focus.NumericalJacobianColumn());

				focus = focus.Child;
			}

			var deltaPhi = new LinkedList<float>();

			foreach (var column in jacobian) {
				deltaPhi.AddLast((deltaE.X * column.X) + (deltaE.Y * column.Y));
			}

			baseJoint.ApplyDofDeltas(deltaPhi);
		}
		private void JIIterate() {
			var deltaE = goal - baseJoint.EndEffector;
			deltaE *= 0.001f;
			var jacobian = new List<Vector2>();
			var focus = baseJoint;

			while (focus != null) {
				jacobian.Add(focus.NumericalJacobianColumn());

				focus = focus.Child;
			}

			var jacobianMat = CreateMatrix.Dense(2, jacobian.Count, (row, col) => row == 0 ? jacobian[col].X : jacobian[col].Y);

			var inverse = jacobianMat.Transpose() * (jacobianMat * jacobianMat.Transpose()).Inverse();

			var deltaPhi = new LinkedList<float>();

			foreach (var row in inverse.ToRowArrays()) {
				deltaPhi.AddLast((deltaE.X * row[0]) + (deltaE.Y * row[1]));
			}

			baseJoint.ApplyDofDeltas(deltaPhi);
		}
	}
}
