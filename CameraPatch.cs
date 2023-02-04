using Common;
using FezEngine.Components;
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
		public IGameCameraManager CameraManager;

		[ServiceDependency]
		public IGameStateManager GameState;

		[ServiceDependency]
		public IInputManager InputManager;

		public bool freeCameraEnabled = false;

		public CameraPatch(Game game) : base(game)
		{
		}

		public override void Initialize()
		{
			PlayerCameraControlUpdateDetour = new Hook(
				typeof(PlayerCameraControl).GetMethod("Update"),
				(Action<Action<PlayerCameraControl, GameTime>, PlayerCameraControl, GameTime>)UpdateHooked);
		}

		private void UpdateHooked(Action<PlayerCameraControl, GameTime> original, PlayerCameraControl playerCameraControl, GameTime gameTime)
		{
			//if (InputManager.ClampLook == FezEngine.Structure.Input.FezButtonState.Pressed)
			//	freeCameraEnabled = !freeCameraEnabled;

			Vector2 freeLook = Vector2.Zero;
			if (freeCameraEnabled)
			{
				freeLook = InputManager.FreeLook;
				Vector2 reflectedFreeLook = (Vector2)InputManager.GetType().GetField("FreeLook").GetValue(InputManager);
				InputManager.GetType().GetField("FreeLook").SetValue(InputManager, Vector2.Zero);
			}

			original(playerCameraControl, gameTime);

			if (freeCameraEnabled)
			{
				if (freeLook.Length() > 0f)
				{
					Vector3 value = Vector3.Transform(CameraManager.Direction, Quaternion.CreateFromAxisAngle(CameraManager.InverseView.Right, InputManager.FreeLook.Y * 0.4f));
					value = Vector3.Transform(value, Quaternion.CreateFromAxisAngle(CameraManager.InverseView.Up, (0f - InputManager.FreeLook.X) * 0.5f));

				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			PlayerCameraControlUpdateDetour.Dispose();
		}
	}
}
