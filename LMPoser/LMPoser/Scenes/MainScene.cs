using LMPoser.Objects.Joint2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project_GE.Framework.Input;
using Project_GE.Framework.SceneManagement;
using System;
using System.Linq;

namespace LMPoser.Scenes {
	public class MainScene : Scene2D {
		private const int WIDTH = 1280;
		private const int HEIGHT = 720;

		private GraphicsDevice graphics;
		private BasicEffect effect;
		private RasterizerState rasterizer;

		private Matrix world = Matrix.Identity;

		private Camera3D camera;

		private Hinge baseJoint;
		private Hinge focusJoint;

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
		}

		public override void Input(InputManager input) {
			camera.Input(input, GameTime);

			// TODO: dimensions
			var toMouse = new Vector2(
				input.CurrentMousePosition.X - (focusJoint.GlobalPosition.X + WIDTH / 2),
				-input.CurrentMousePosition.Y + (-focusJoint.GlobalPosition.Y + HEIGHT / 2)

			);
			focusJoint.Angle = (float)Math.Atan2(
				toMouse.Y,
				toMouse.X
			);

			if (input.IsKeyPressed(Keys.M)) {
				focusJoint = focusJoint.Child != null ? focusJoint.Child : focusJoint;
			}
			if (input.IsKeyPressed(Keys.N)) {
				focusJoint = focusJoint.Parent != null ? focusJoint.Parent : focusJoint;
			}

			base.Input(input);
		}

		public override void Update() {
			UpdateEffect();

			base.Update();
		}

		public override void Draw() {
			DrawBones();

			base.Draw();
		}

		private void InitJoints() {
			baseJoint = new Hinge(MathHelper.ToRadians(30f));

			baseJoint.Child = new Hinge(
				0f,
				new Hinge(
					MathHelper.PiOver2
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

				var vertices = baseJoint.GetVertices();

				graphics.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count() / 2);
			}
		}
	}
}
