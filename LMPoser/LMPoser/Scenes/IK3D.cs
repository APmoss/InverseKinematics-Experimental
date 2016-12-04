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
using LMPoser.Objects.Joint3D;

namespace LMPoser.Scenes {
	public class IK3D : Scene2DGui{
		private const int WIDTH = 1280;
		private const int HEIGHT = 720;

		private GraphicsDevice graphics;
		private BasicEffect effect;
		private RasterizerState rasterizer = new RasterizerState();
		private Camera3D camera;

		private List<VertexPositionColor> vertices = new List<VertexPositionColor>();

		private BallJoint baseJoint;

		#region Gui
		private TextBlock jointCountG;
		#endregion

		public IK3D()
			: base(0, 0, WIDTH, HEIGHT, "Content/GuiThemes/LightTheme.xml") {

			CapturesInput = true;
		}

		public override void Load(ContentManager content) {
			base.Load(content);

			graphics = SceneManager.Game.GraphicsDevice;
			effect = new BasicEffect(graphics);
			effect.VertexColorEnabled = true;
			rasterizer.FillMode = FillMode.WireFrame;
			rasterizer.CullMode = CullMode.None;
			camera = new Camera3D();

			InitGui();
			InitJoints();
		}

		public override void Input(InputManager input) {
			if (input.IsKeyPressed(Keys.Escape)) {
				Exit();
			}

			camera.Input(input, GameTime);

			base.Input(input);
		}

		public override void Update() {
			UpdateGui();
			UpdateEffect();

			vertices.Clear();

			AddJoints();

			base.Update();
		}

		public override void Draw() {
			graphics.Clear(Color.Black);

			if (vertices.Count > 0) {
				DrawVertices();
			}

			base.Draw();
		}

		private void InitGui() {
			var panel = new Panel(new Rectangle(0, 0, 300, 500));
			jointCountG = new TextBlock("", 0, 30);

			panel.AddChildren(new TextBlock("3D IK Example", 0, 0),
								jointCountG);

			Gui.BaseContainer.AddChildren(panel);
		}

		private void InitJoints() {
			baseJoint = new BallJoint() {
				Name = "base",
				RotZ = .060f
			};
			var j1 = new BallJoint() { Name = "j1", RotZ = 0f };
			var j2 = new BallJoint() { Name = "j2", RotY = 0f };
			var j3 = new BallJoint() { Name = "j3", RotX = 0f };

			baseJoint.AddChild(j1);
			j1.AddChild(j2);
			j2.AddChild(j3);
		}

		private void UpdateGui() {
			jointCountG.Text = "Joint Count: " + baseJoint.Count;
			jointCountG.AutoAdjustWidth();
			jointCountG.AutoAdjustHeight();
		}

		private void UpdateEffect() {
			effect.World = Matrix.CreateTranslation(Vector3.Zero);
			effect.View = camera.GetViewMatrix();
			effect.Projection = camera.GetPerspectiveProjectionMatrix();
		}

		private void AddJoints() {
			vertices.AddRange(baseJoint.AllVertices());
		}

		private void DrawVertices() {
			foreach (var pass in effect.CurrentTechnique.Passes) {
				pass.Apply();

				graphics.RasterizerState = rasterizer;
				graphics.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count() / 2);
			}
		}
	}
}
