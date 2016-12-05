using Project_GE.Framework.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Project_GE.Framework.Input;
using Microsoft.Xna.Framework.Input;
using Project_GE.Framework.Gui.Controls;
using Microsoft.Xna.Framework;

namespace LMPoser.Scenes {
	public class InstructionScene : Scene2DGui {
		private const int WIDTH = 1280;
		private const int HEIGHT = 720;

		public InstructionScene()
			: base(0, 0, WIDTH, HEIGHT, "Content/GuiThemes/LightTheme.xml") {

			CapturesInput = true;
		}

		public override void Load(ContentManager content) {
			base.Load(content);

			InitGui();
		}

		public override void Input(InputManager input) {
			base.Input(input);

			if (input.IsKeyPressed(Keys.Escape)) {
				Exit();
			}
		}

		private void InitGui() {
			var panelW = 700;
			var panelH = 450;

			var panel = new Panel(new Rectangle(WIDTH / 2 - panelW / 2, HEIGHT / 2 - panelH / 2, panelW, panelH));
			var exitButton = new Button("Return to Menu", 10, 360);
			exitButton.Clicked += (s, e) => Exit();

			panel.AddChildren(new TextBlock(
				"Welcome to the Inverse Kinematics Demo!\n\n" +
				"General Controls for Demos-\n" +
				"  Left Click: Move the goal position.\n" +
				"  Spacebar: Run one IK iteration per frame (Demo 1 just runs 1 iteration)\n" +
				"  Shift+Spacebar: Run IK iterations until a solution is found (or a max is reached).\n" +
				"  Escape: Exits the individual demo, or the entire program (on the menu screen)\n\n" +
				"Notes-\n" +
				"  Descriptions for each demo can be found in the report.\n" +
				"  Live Demo is for the presentation only. No written documentation is provided.\n" +
				"  Some code is unused (like PseudoMatrix). This is from either testing or deprecation.\n" +
				"  Press Escape to return to the main menu!\n\n" +
				"Program by Alexander Pabst",
				10, 10),

				exitButton);

			Gui.BaseContainer.AddChildren(panel);
		}
	}
}
