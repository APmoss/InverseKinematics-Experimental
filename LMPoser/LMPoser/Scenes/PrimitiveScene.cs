using Microsoft.Xna.Framework.Graphics;
using Project_GE.Framework.SceneManagement;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using Project_GE.Framework.Input;
using Microsoft.Xna.Framework.Input;

namespace LMPoser.Scenes {
	public class PrimitiveScene : Scene2DGui {
		protected const int WIDTH = 1280;
		protected const int HEIGHT = 720;

		protected GraphicsDevice Graphics;
		protected BasicEffect Effect;
		protected RasterizerState Rasterizer;
		protected Camera3D Camera;
		protected List<VertexPositionColor> Vertices = new List<VertexPositionColor>();

		public PrimitiveScene()
			: base(0, 0, WIDTH, HEIGHT, "Content/GuiThemes/LightTheme.xml") {
			CapturesInput = true;
		}

		public override void Load(ContentManager content) {
			Graphics = SceneManager.Game.GraphicsDevice;
			Effect = new BasicEffect(Graphics);
			Effect.VertexColorEnabled = true;
			Rasterizer = new RasterizerState();
			Rasterizer.CullMode = CullMode.None;
			Rasterizer.FillMode = FillMode.WireFrame;
			Camera = new Camera3D();

			base.Load(content);
		}

		public override void Input(InputManager input) {
			if (input.IsKeyPressed(Keys.Escape)) {
				Exit();
			}

			base.Input(input);
		}

		public override void Update() {
			UpdateEffect();

			Vertices.Clear();

			base.Update();
		}

		public override void Draw() {
			Graphics.Clear(Color.Black);

			if (Vertices.Count > 0) {
				DrawVertices();
			}

			base.Draw();
		}

		protected void UpdateEffect() {
			Effect.World = Matrix.CreateTranslation(Vector3.Zero);
			Effect.View = Camera.GetViewMatrix();
			Effect.Projection = Camera.GetOrthographicProjectionMatrix();
		}

		protected void DrawVertices() {
			foreach (var pass in Effect.CurrentTechnique.Passes) {
				pass.Apply();

				Graphics.RasterizerState = Rasterizer;
				Graphics.DrawUserPrimitives(PrimitiveType.LineList, Vertices.ToArray(), 0, Vertices.Count / 2);
			}
		}

		protected void DrawCircle(Vector2 center, float radius, int sides, Color color) {
			var offset = new Vector3(center, 0);

			for (int i = 0; i < 360; i += 360 / sides) {
				var angle = MathHelper.ToRadians(i);
				var v1 = radius * new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);

				angle = MathHelper.ToRadians(i + 360 / sides);
				var v2 = radius * new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);

				Vertices.Add(new VertexPositionColor(
					offset + v1,
					color
				));
				Vertices.Add(new VertexPositionColor(
					offset + v2,
					color
				));
			}
		}

		protected void DrawLine(Vector2 start, Vector2 end, Color color) {
			Vertices.Add(new VertexPositionColor(
				new Vector3(start, 0),
				color
			));
			Vertices.Add(new VertexPositionColor(
				new Vector3(end, 0),
				color
			));
		}
	}
}
