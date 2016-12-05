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

			var panelW = 400;
			var panelH = 450;

			var panel = new Panel(new Rectangle(WIDTH / 2 - panelW / 2, HEIGHT / 2 - panelH / 2, panelW, panelH));

			panel.AddChildren(new TextBlock("Inverse Kinematics Demos", 10, 0));

			var instructionButton = new Button("Instructions", 10, 60);
			instructionButton.Clicked += (s, e) => SceneManager.AddScene(new InstructionScene());

			var comparisonButton = new Button("Comparison Inverse Kinematics Demo", 10, 120);
			comparisonButton.Clicked += (s, e) => SceneManager.AddScene(new ComparisonScene());

			var angleLimitButton = new Button("Angle Limit Inverse Kinematics Demo", 10, 170);
			angleLimitButton.Clicked += (s, e) => SceneManager.AddScene(new AngleLimitScene());

			var varianceButton = new Button("Variance Inverse Kinematics Demo", 10, 220);
			varianceButton.Clicked += (s, e) => SceneManager.AddScene(new VarianceScene());

			var constraintButton = new Button("Constraint Inverse Kinematics Demo", 10, 270);
			constraintButton.Clicked += (s, e) => SceneManager.AddScene(new ConstraintScene());

			var ik2dButton = new Button("Live Demo Only", 10, 340);
			ik2dButton.Clicked += (s, e) => SceneManager.AddScene(new IK2D());

			panel.AddChildren(instructionButton, ik2dButton, comparisonButton, angleLimitButton, varianceButton, constraintButton);
			Gui.BaseContainer.AddChildren(panel);
		}

		public override void Input(InputManager input) {
			if (input.IsKeyPressed(Keys.Escape)) {
				SceneManager.Game.Exit();
			}

			base.Input(input);
		}

		public override void Draw() {
			SceneManager.Game.GraphicsDevice.Clear(new Color(0, 0, 50));

			base.Draw();
		}
	}
}
