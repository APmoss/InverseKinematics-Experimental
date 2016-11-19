using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_GE.Framework.Input;
using Project_GE.Framework.SceneManagement;

namespace LMPoser.Scenes {
	public class MainScene : Scene2D {
		private GraphicsDevice graphics;
		private BasicEffect effect;
		private RasterizerState rasterizer;

		private Matrix world = Matrix.Identity;

		private VertexDeclaration vertexDec;
		private VertexPositionColor[] vertices;
		private VertexBuffer vertexBuffer;

		private short[] triangles;

		private Camera3D camera;
		private GameTime time;

		public override void Load(ContentManager content) {
			graphics = SceneManager.GraphicsDevice;
			camera = new Camera3D();
			
			InitVertices();
			InitTriangles();

			rasterizer = new RasterizerState();
			rasterizer.FillMode = FillMode.WireFrame;
			rasterizer.CullMode = CullMode.None;

			base.Load(content);
		}

		public override void Input(InputManager input) {
			if (time != null) {
				camera.Input(input, time);
			}

			base.Input(input);
		}

		public override void Update(GameTime time) {
			this.time = time;

			effect.World = world;
			effect.View = camera.GetViewMatrix();
			effect.Projection = camera.GetPerspectiveProjectionMatrix();

			base.Update(time);
		}

		public override void Draw() {
			foreach (var pass in effect.CurrentTechnique.Passes) {
				pass.Apply();

				graphics.RasterizerState = rasterizer;

				graphics.DrawUserIndexedPrimitives(
					PrimitiveType.TriangleList,
					vertices,
					0,
					4,
					triangles,
					0,
					2
				);
			}

			base.Draw();
		}

		private void InitVertices() {
			vertexDec = new VertexDeclaration(
				new VertexElement[] {
					new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
					new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
				}
			);

			effect = new BasicEffect(graphics);
			effect.VertexColorEnabled = true;

			vertices = new VertexPositionColor[4] {
				new VertexPositionColor(
					new Vector3(0, 0, 0), Color.Red
				),
				new VertexPositionColor(
					new Vector3(1, 0, 0), Color.Gray
				),
				new VertexPositionColor(
					new Vector3(1, -1, 0), Color.Green
				),
				new VertexPositionColor(
					new Vector3(0, -1, 0), Color.Blue
				)
			};

			vertexBuffer = new VertexBuffer(graphics, vertexDec, 4, BufferUsage.None);

			vertexBuffer.SetData(vertices);
		}

		private void InitTriangles() {
			triangles = new short[] {
				0, 1, 3, 1, 2, 3
			};
		}
	}
}
