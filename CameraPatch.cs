using Common;
using FezEngine.Components;
using FezEngine.Structure.Input;
using FezEngine.Tools;
using FezGame.Components;
using FezGame.Services;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FezCameraUnlocker
{
	public class CameraPatch : GameComponent
	{
		private static IDetour PlayerCameraControlUpdateDetour;

		[ServiceDependency]
		public IGameCameraManager CameraManager { private get; set; }

		[ServiceDependency]
		public IGameStateManager GameState { private get; set; }

		[ServiceDependency]
		public IInputManager InputManager { private get; set; }

		public bool freeCameraEnabled = true;

		public CameraPatch(Game game) : base(game)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ServiceHelper.InjectServices(this);
			PlayerCameraControlUpdateDetour = new Hook(
				typeof(PlayerCameraControl).GetMethod("Update"),
				(Action<Action<PlayerCameraControl, GameTime>, PlayerCameraControl, GameTime>)UpdateHooked);
		}

		private void UpdateHooked(Action<PlayerCameraControl, GameTime> original, PlayerCameraControl playerCameraControl, GameTime gameTime)
		{
			if (InputManager.ClampLook == FezButtonState.Pressed)
				freeCameraEnabled = !freeCameraEnabled;

			Vector2 freeLook = Vector2.Zero;
			if (freeCameraEnabled)
			{
				freeLook = InputManager.FreeLook;
				InputManager.GetType().GetProperty("FreeLook").SetValue(InputManager, Vector2.Zero);
			}

			original(playerCameraControl, gameTime);

			if (freeCameraEnabled)
			{
				if (freeLook.Length() > 0f)
				{
					Vector3 newDirection = Vector3.Transform(CameraManager.Direction, Quaternion.CreateFromAxisAngle(CameraManager.InverseView.Right, freeLook.Y * 0.1f));
					if (newDirection.Y > 0.7 || newDirection.Y < -0.7)
					{
						float damping = 0.7f / new Vector2(newDirection.X, newDirection.Z).Length();
						newDirection = new Vector3(newDirection.X * damping, 0.7f * Math.Sign(newDirection.Y), newDirection.Z * damping);
					}
					newDirection = Vector3.Transform(newDirection, Quaternion.CreateFromAxisAngle(CameraManager.InverseView.Up, freeLook.X * -0.1f));
					CameraManager.Direction = newDirection;
					if (CameraManager.Direction.Y < -0.625f)
						CameraManager.Direction = new Vector3(CameraManager.Direction.X, -0.625f, CameraManager.Direction.Z);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			PlayerCameraControlUpdateDetour.Dispose();
		}
	}
}
