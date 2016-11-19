using Microsoft.Xna.Framework;
using Project_GE.Framework.SceneManagement;

namespace LMPoser {
	public class Entry : Game {
		private GraphicsDeviceManager graphics;
		private SceneManager sceneManager;

		public Entry() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			InitialSettings();

			sceneManager = new SceneManager(this);
			Components.Add(sceneManager);

			sceneManager.AddScene(new Scenes.MainScene());
		}

		private void InitialSettings() {
			IsMouseVisible = true;
			
			IsFixedTimeStep = false;

			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
		}
	}
}
