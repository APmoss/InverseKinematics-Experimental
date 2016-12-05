using LMPoser.Objects.Joint2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project_GE.Framework.Gui.Controls;
using Project_GE.Framework.Input;
using Project_GE.Framework.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace LMPoser.Scenes {
	public class IK2D : Scene2DGui {
		private const int WIDTH = 1280;
		private const int HEIGHT = 720;
		private const float BETA = 0.001f;

		private GraphicsDevice graphics;
		private BasicEffect effect;
		private RasterizerState rasterizer;

		private Matrix world = Matrix.Identity;

		private Camera3D camera;

		private Hinge baseJoint;
		private Hinge focusJoint;
		private bool updateFocusJoint = false;

		private List<VertexPositionColor> vertices = new List<VertexPositionColor>();
		private Vector2 goal = Vector2.Zero;

		TextBlock jointAngleText;
		TextBlock goalText;
		TextBlock sizeText;

		public IK2D()
			: base(new Rectangle(0, 0, WIDTH, HEIGHT), "Content/GuiThemes/LightTheme.xml") {

			CapturesInput = true;
		}

		public override void Load(ContentManager content) {
			graphics = SceneManager.GraphicsDevice;
			camera = new Camera3D();

			effect = new BasicEffect(graphics);
			effect.VertexColorEnabled = true;

			rasterizer = new RasterizerState();
			rasterizer.FillMode = FillMode.WireFrame;
			rasterizer.CullMode = CullMode.None;

			InitJoints();

			base.Load(content);

			var panel = new Panel(new Rectangle(10, 10, 300, 500));
			jointAngleText = new TextBlock("A", 0, 0);
			goalText = new TextBlock("A", 0, 30);
			sizeText = new TextBlock(baseJoint.Size().ToString(), 0, 60);

			panel.AddChildren(jointAngleText, goalText, sizeText);

			Gui.BaseContainer.AddChildren(
				panel
			);
		}

		public override void Input(InputManager input) {
			if (input.IsKeyPressed(Keys.Escape)) {
				Exit();
			}

			camera.Input(input, GameTime);

			if (updateFocusJoint) {
				var toMouse = new Vector2(
					input.CurrentMousePosition.X - (focusJoint.GlobalPosition.X + WIDTH / 2),
					-input.CurrentMousePosition.Y + (-focusJoint.GlobalPosition.Y + HEIGHT / 2)

				);

				focusJoint.Angle = (float)Math.Atan2(
					toMouse.Y,
					toMouse.X
				);

				if (input.IsKeyPressed(Keys.M)) {
					if (focusJoint.Child != null) {
						focusJoint = focusJoint.Child;
					}
					else {
						focusJoint = null;
						updateFocusJoint = false;
					}
				}
				if (input.IsKeyPressed(Keys.N)) {
					focusJoint = focusJoint.Parent != null ? focusJoint.Parent : focusJoint;
				}
			}

			if (input.IsKeyPressed(Keys.Space)) {
				updateFocusJoint = true;
				focusJoint = baseJoint;
			}

			if (input.IsKeyDown(Keys.LeftShift) || input.IsKeyDown(Keys.RightShift)) {
				if (input.IsKeyPressed(Keys.J)) {
					JTIterate();
				}
				if (input.IsKeyPressed(Keys.K)) {
					JIIterate();
				}
				if (input.IsKeyPressed(Keys.L)) {
					CCDIterate();
				}
				if (input.IsKeyPressed(Keys.OemSemicolon)) {
					DLSIterate();
				}
			}
			else {
				if (input.IsKeyDown(Keys.J)) {
					JTIterate();
				}
				if (input.IsKeyDown(Keys.K)) {
					JIIterate();
				}
				if (input.IsKeyDown(Keys.L)) {
					CCDIterate();
				}
				if (input.IsKeyDown(Keys.OemSemicolon)) {
					DLSIterate();
				}
			}
			
			if (input.IsMouseDown(MouseButtons.Left)) {
				goal = input.CurrentMousePosition.ToVector2();
				goal -= new Vector2(WIDTH / 2, HEIGHT / 2);
				goal.Y = -goal.Y;

				var dist = Vector2.Distance(goal, Vector2.Zero);
				var rad = baseJoint.SolutionSpaceRadius();

				if (dist > rad) {
					goal *= rad / dist;
				}
			}

			base.Input(input);
		}

		public override void Update() {
			vertices.Clear();

			vertices.AddRange(baseJoint.GetVertices());
			vertices.AddRange(GetCircle(goal, 5, 5, Color.Green));
			vertices.AddRange(GetCircle(Vector2.Zero, baseJoint.SolutionSpaceRadius(), 100, Color.DarkRed));

			UpdateEffect();

			if (updateFocusJoint) {
				jointAngleText.Text = "Current Joint Angle: " + focusJoint.Angle;
				jointAngleText.AutoAdjustWidth();

				vertices.AddRange(GetCircle(focusJoint.GlobalPosition, 10, 10, Color.Purple));
			}

			goalText.Text = "Goal Position: " + goal.X + ", " + goal.Y;
			goalText.AutoAdjustWidth();

			base.Update();
		}

		public override void Draw() {
			graphics.Clear(Color.Black);

			DrawBones();

			base.Draw();
		}

		private void InitJoints() {
			baseJoint = new Hinge(MathHelper.PiOver4,
				new Hinge(
				MathHelper.PiOver4,
					new Hinge(
						MathHelper.PiOver4,
						new Hinge(
							MathHelper.PiOver4
						) { AngleLimitLower = -2f, AngleLimitUpper = 2f }
					) { AngleLimitLower = -2f, AngleLimitUpper = 2f }
				) { AngleLimitLower = -2f, AngleLimitUpper = 2f }
			);

			focusJoint = baseJoint;
		}

		private void UpdateEffect() {
			effect.World = world;
			effect.View = camera.GetViewMatrix();
			effect.Projection = camera.GetOrthographicProjectionMatrix();
		}

		private void DrawBones() {
			foreach (var pass in effect.CurrentTechnique.Passes) {
				pass.Apply();

				graphics.RasterizerState = rasterizer;
				if (vertices.Count > 0) {
					graphics.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count() / 2);
				}
			}
		}

		private IEnumerable<VertexPositionColor> GetCircle(Vector2 center, float radius, int sides, Color color) {
			var verts = new List<VertexPositionColor>();
			var offset = new Vector3(center, 0);

			for (int i = 0; i < 360; i += 360 / sides) {
				var angle = MathHelper.ToRadians(i);
				var v1 = radius * new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);

				angle = MathHelper.ToRadians(i + 360 / sides);
				var v2 = radius * new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);

				verts.Add(new VertexPositionColor(
					offset + v1,
					color
				));
				verts.Add(new VertexPositionColor(
					offset + v2,
					color
				));
			}

			return verts;
		}

		private void JTIterate() {
			var jacobian = ComputeJacobian();
			var jacobianMat = CreateMatrix.Dense(2, jacobian.Count, (row, col) => row == 0 ? jacobian[col].X : jacobian[col].Y);
			var deltaE = SafeDeltaE(goal, baseJoint.EndEffector);

			var deltaEVec = CreateVector.Dense(new float[] { deltaE.X, deltaE.Y });
			var denom = (jacobianMat * jacobianMat.Transpose() * deltaEVec).L2Norm();

			var lambda = (deltaEVec * jacobianMat * jacobianMat.Transpose() * deltaEVec) / (denom * denom);

			var deltaPhiVec = (float)lambda * jacobianMat.Transpose() * deltaEVec;
			var deltaPhi = new LinkedList<float>(deltaPhiVec.ToArray());

			baseJoint.ApplyDofDeltas(deltaPhi);
		}
		private void JIIterate() {
			var jacobian = ComputeJacobian();
			var deltaE = SafeDeltaE(goal, baseJoint.EndEffector);

			var jacobianMat = CreateMatrix.Dense(2, jacobian.Count, (row, col) => row == 0 ? jacobian[col].X : jacobian[col].Y);

			var inverse = jacobianMat.GeneralizedInverse();
			var pInverse = jacobianMat.SVDPseudoInverse();

			var deltaPhi = new LinkedList<float>();

			foreach (var row in pInverse.ToRowArrays()) {
				deltaPhi.AddLast((deltaE.X * row[0]) + (deltaE.Y * row[1]));
			}

			baseJoint.ApplyDofDeltas(deltaPhi);
		}

		private void CCDIterate() {
			var focus = baseJoint;

			while (focus != null) {
				var toE = focus.EndEffector - focus.GlobalPosition;
				var toG = goal - focus.GlobalPosition;

				var eAngle = Math.Atan2(toE.Y, toE.X);
				var gAngle = Math.Atan2(toG.Y, toG.X);

				focus.Angle += ((float)(gAngle - eAngle));

				focus = focus.Child;
			}
		}

		private void DLSIterate() {
			var jacobian = ComputeJacobian();
			var deltaE = SafeDeltaE(goal, baseJoint.EndEffector);

			var jacobianMat = CreateMatrix.Dense(2, jacobian.Count, (row, col) => row == 0 ? jacobian[col].X : jacobian[col].Y);

			var lambda = .5f;
			var lambdaMatrix = lambda * lambda * CreateMatrix.DenseIdentity<float>(jacobianMat.RowCount);
			var deltaPhiVec =
				jacobianMat.Transpose() *
				(jacobianMat * jacobianMat.Transpose() + (lambdaMatrix)).Inverse() *
				CreateVector.DenseOfArray(new float[] { deltaE.X, deltaE.Y });
			var deltaPhi = new LinkedList<float>(deltaPhiVec.ToArray());

			baseJoint.ApplyDofDeltas(deltaPhi);
		}

		private List<Vector2> ComputeJacobian() {
			var jacobian = new List<Vector2>();
			var focus = baseJoint;

			while (focus != null) {
				jacobian.Add(focus.AnalyticalJacobianColumn());

				focus = focus.Child;
			}

			return jacobian;
		}

		private Vector2 SafeDeltaE(Vector2 goal, Vector2 endE) {
			var maxMag = 100;
			var deltaE = goal - endE;
			var length = deltaE.Length();


			if (length > maxMag) {
				var mult = maxMag / length;
				deltaE *= mult;
			}

			return deltaE;
		}

		private void SimpleLine(Vector2 start, Vector2 end) {
			vertices.Add(new VertexPositionColor(
				new Vector3(start, 0),
				Color.White
			));
			vertices.Add(new VertexPositionColor(
				new Vector3(end, 0),
				Color.White
			));
		}
	}
}
