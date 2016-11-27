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

namespace LMPoser.Scenes {
	public class IK3D : Scene2DGui{
		private const int WIDTH = 1280;
		private const int HEIGHT = 720;

		public IK3D()
			: base(0, 0, WIDTH, HEIGHT, "Content/GuiThemes/LightTheme.xml") {

			CapturesInput = true;
		}

		public override void Load(ContentManager content) {
			base.Load(content);

			var panel = new Panel(new Rectangle(0, 0, 300, 500));

			panel.AddChildren(new TextBlock("3D IK Example", 0, 0));

			Gui.BaseContainer.AddChildren(panel);
		}

		public override void Input(InputManager input) {
			if (input.IsKeyPressed(Keys.Escape)) {
				Exit();
			}

			base.Input(input);
		}

		public override void Draw() {
			SceneManager.GraphicsDevice.Clear(Color.Black);

			base.Draw();
		}
	}
}
