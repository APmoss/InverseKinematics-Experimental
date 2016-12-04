using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project_GE.Framework.Input;

namespace LMPoser {
	public class Camera3D {
		public Vector3 Position {
			get; set;
		} = new Vector3(0f, 0f, 10f);

		public Quaternion Orientation {
			get; set;
		} = Quaternion.Identity;

		public float FieldOfView {
			get; set;
		} = MathHelper.ToRadians(60f);

		public Vector2 Dimensions {
			get; set;
		} = new Vector2(1280, 720);

		public float AspectRatio {
			get {
				return Dimensions.X / Dimensions.Y;
			}
		}

		public float NearPlaneDistance {
			get; set;
		} = 1f;
		public float FarPlaneDistance {
			get; set;
		} = 1000f;

		public float Speed {
			get; set;
		} = 10f;

		public void Input(InputManager input, GameTime time) {
			var delta = (float)time.ElapsedGameTime.TotalSeconds;
			var speed = Speed * delta;

			if (input.IsKeyDown(Keys.W)) {
				Position += Vector3.Transform(Vector3.Forward, Orientation) * speed;
			}
			if (input.IsKeyDown(Keys.A)) {
				Position += Vector3.Transform(Vector3.Left, Orientation) * speed;
			}
			if (input.IsKeyDown(Keys.S)) {
				Position += Vector3.Transform(Vector3.Backward, Orientation) * speed;
			}
			if (input.IsKeyDown(Keys.D)) {
				Position += Vector3.Transform(Vector3.Right, Orientation) * speed;
			}
			if (input.IsKeyDown(Keys.E)) {
				Position += Vector3.Transform(Vector3.Up, Orientation) * speed;
			}
			if (input.IsKeyDown(Keys.Q)) {
				Position += Vector3.Transform(Vector3.Down, Orientation) * speed;
			}

			var amount = MathHelper.PiOver4 * delta;

			if (input.IsKeyDown(Keys.Up)) {
				Orientation *= Quaternion.CreateFromYawPitchRoll(0f, amount, 0f);
			}
			if (input.IsKeyDown(Keys.Down)) {
				Orientation *= Quaternion.CreateFromYawPitchRoll(0f, -amount, 0f);
			}
			if (input.IsKeyDown(Keys.Left)) {
				Orientation *= Quaternion.CreateFromYawPitchRoll(amount, 0f, 0f);
			}
			if (input.IsKeyDown(Keys.Right)) {
				Orientation *= Quaternion.CreateFromYawPitchRoll(-amount, 0f, 0f);
			}
		}

		public Matrix GetViewMatrix() {
			return Matrix.CreateLookAt(
				Position,
				Position + Vector3.Transform(Vector3.Forward, Orientation),
				Vector3.Up
			);
		}

		public Matrix GetOrthographicProjectionMatrix() {
			return Matrix.CreateOrthographic(Dimensions.X, Dimensions.Y, NearPlaneDistance, FarPlaneDistance);
		}

		public Matrix GetPerspectiveProjectionMatrix() {
			return Matrix.CreatePerspectiveFieldOfView(
				FieldOfView,
				AspectRatio,
				NearPlaneDistance,
				FarPlaneDistance
			);
		}
	}
}
