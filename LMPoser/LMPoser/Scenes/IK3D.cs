using Project_GE.Framework.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Project_GE.Framework.Gui.Controls;
using Microsoft.Xna.Framework;
using Project_GE.Framework.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace LMPoser.Scenes {
	public class IK3D : Scene2DGui{
		private const int WIDTH = 1280;
		private const int HEIGHT = 720;

		private GraphicsDevice graphics;
		private BasicEffect effect;
		private RasterizerState rasterizer = new RasterizerState();
		private Camera3D camera = new Camera3D();

		private List<VertexPositionColor> vertices = new List<VertexPositionColor>();

		public IK3D()
			: base(0, 0, WIDTH, HEIGHT, "Content/GuiThemes/LightTheme.xml") {

			CapturesInput = true;
		}

		public override void Load(ContentManager content) {
			base.Load(content);

			graphics = SceneManager.Game.GraphicsDevice;
			effect = new BasicEffect(graphics);
			rasterizer.FillMode = FillMode.WireFrame;
			rasterizer.CullMode = CullMode.None;

			InitGui();
		}

		public override void Input(InputManager input) {
			if (input.IsKeyPressed(Keys.Escape)) {
				Exit();
			}

			camera.Input(input, GameTime);

			base.Input(input);
		}

		public override void Update() {
			UpdateEffect();

			vertices.Clear();

			

			base.Update();
		}

		public override void Draw() {
			SceneManager.GraphicsDevice.Clear(Color.Black);

			if (vertices.Count > 0) {
				DrawJoints();
			}

			base.Draw();
		}

		private void InitGui() {
			var panel = new Panel(new Rectangle(0, 0, 300, 500));

			panel.AddChildren(new TextBlock("3D IK Example", 0, 0));

			Gui.BaseContainer.AddChildren(panel);
		}

		private void UpdateEffect() {
			effect.World = Matrix.CreateTranslation(Vector3.Zero);
			effect.View = camera.GetViewMatrix();
			effect.Projection = camera.GetPerspectiveProjectionMatrix();
		}

		private void DrawJoints() {
			foreach (var pass in effect.CurrentTechnique.Passes) {
				pass.Apply();

				graphics.RasterizerState = rasterizer;
				graphics.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count() / 2);
			}
		}
	}
}
