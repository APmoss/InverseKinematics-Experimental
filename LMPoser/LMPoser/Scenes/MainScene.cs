using LMPoser.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_GE.Framework.Input;
using Project_GE.Framework.SceneManagement;
using System;

namespace LMPoser.Scenes {
	public class MainScene : Scene2D {
		private GraphicsDevice graphics;
		private BasicEffect effect;
		private RasterizerState rasterizer;

		private Matrix world = Matrix.Identity;

		private Camera3D camera;

		private Bone2DChain baseBone;

		public override void Load(ContentManager content) {
			graphics = SceneManager.GraphicsDevice;
			camera = new Camera3D();

			effect = new BasicEffect(graphics);
			effect.VertexColorEnabled = true;

			rasterizer = new RasterizerState();
			rasterizer.FillMode = FillMode.WireFrame;
			rasterizer.CullMode = CullMode.None;

			InitBones();

			base.Load(content);
		}

		public override void Input(InputManager input) {
			camera.Input(input, GameTime);

			// TODO: dimensions
			baseBone.Angle = (float)Math.Atan2(
				-(input.CurrentMousePosition.Y - (720 / 2)),
				input.CurrentMousePosition.X - (1280 / 2)
			);

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

		private void InitBones() {
			baseBone = new Bone2DChain(MathHelper.ToRadians(30f));

			baseBone.Child = new Bone2DChain(
				0f,
				new Bone2DChain(
					MathHelper.PiOver2
				)
			);
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

				var vertices = baseBone.GetVertices();

				graphics.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
			}
		}
	}
}
