using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Project_GE.Framework.Gui.Controls;
using Project_GE.Framework.Input;
using Project_GE.Framework.SceneManagement;

namespace LMPoser.Scenes {
	public class MenuScene : Scene2DGui {
		private const int WIDTH = 1280;
		private const int HEIGHT = 720;
		public MenuScene()
			: base(0, 0, WIDTH, HEIGHT, "Content/GuiThemes/LightTheme.xml") {
		}

		public override void Load(ContentManager content) {
			base.Load(content);

			var panel = new Panel(new Rectangle(0, 0, 400, 300));
			var ik2dButton = new Button("2D Inverse Kinematics Demo", 10, 10);
			ik2dButton.Clicked += (s, e) => SceneManager.AddScene(new IK2D());

			var ik3dButton = new Button("3D Inverse Kinematics Demo", 10, 60);
			ik3dButton.Clicked += (s, e) => SceneManager.AddScene(new IK3D());

			panel.AddChildren(ik2dButton, ik3dButton);
			Gui.BaseContainer.AddChildren(panel);
		}

		public override void Input(InputManager input) {
			if (input.IsKeyPressed(Keys.Escape)) {
				SceneManager.Game.Exit();
			}

			base.Input(input);
		}
	}
}
