using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MonoPunk
{
	public enum ThumbStickType
	{
		Left,
		Right
	}

	public enum MouseButton
	{
		Left,
		Middle,
		Right
	}

	public static class Input
	{
		private static KeyboardState keyboardPrev;
		private static KeyboardState keyboard;
		private static MouseState mousePrev;
		private static MouseState mouse;

		private static Player[] InitPlayers()
		{
			var result = new Player[4];
			for (var i = 0; i < 4; i++)
			{
				result[i] = new Player(i);
			}
			return result;
		}
		private static Player[] _player = InitPlayers();

		public static bool Enabled { get; set; } = true;

		public static bool IsKeyDown(Keys key)
		{
			if (!Enabled)
			{
				return false;
			}

			return keyboard.IsKeyDown(key);
		}

		public static bool IsKeyUp(Keys key)
		{
			if (!Enabled)
			{
				return false;
			}

			return keyboard.IsKeyUp(key);
		}

		public static bool WasKeyPressed(Keys key)
		{
			if (!Enabled)
			{
				return false;
			}

			return keyboardPrev.IsKeyUp(key) && IsKeyDown(key);
		}

		public static bool WasKeyReleased(Keys key)
		{
			if (!Enabled)
			{
				return false;
			}

			return keyboardPrev.IsKeyDown(key) && IsKeyUp(key);
		}

		public static bool IsButtonDown(Buttons button, int playerId = 0)
		{
			if (!Enabled)
			{
				return false;
			}

			if (!_player[playerId].PadState.IsConnected) return false;
			return _player[playerId].PadState.IsButtonDown(button);
		}

		public static bool IsButtonUp(Buttons button, int playerId = 0)
		{
			if (!Enabled)
			{
				return false;
			}

			if (!_player[playerId].PadState.IsConnected) return true;
			return _player[playerId].PadState.IsButtonUp(button);
		}

		public static bool WasButtonPressed(Buttons button, int playerId = 0)
		{
			if (!Enabled)
			{
				return false;
			}

			if (!_player[playerId].PadState.IsConnected || !_player[playerId].PadStatePrev.IsConnected) return false;
			return _player[playerId].PadStatePrev.IsButtonUp(button) && IsButtonDown(button, playerId);
		}

		public static bool WasButtonReleased(Buttons button, int playerId = 0)
		{
			if (!Enabled)
			{
				return false;
			}

			if (!_player[playerId].PadState.IsConnected || !_player[playerId].PadStatePrev.IsConnected) return false;
			return _player[playerId].PadStatePrev.IsButtonDown(button) && IsButtonUp(button, playerId);
		}

		public static void Define(string mapping, params Keys[] keys)
		{
			Define(mapping, 0, keys);
		}

		public static void Define(string mapping, params Buttons[] buttons)
		{
			Define(mapping, 0, buttons);
		}

		public static void Define(string mapping, CustomMapping custom)
		{
			Define(mapping, 0, custom);
		}

		public static void Define(string mapping, int playerId, params Keys[] keys)
		{
			_player[playerId].Define(mapping, keys);
		}

		public static void Define(string mapping, int playerId, params Buttons[] buttons)
		{
			_player[playerId].Define(mapping, buttons);
		}

		public static void Define(string mapping, int playerId, CustomMapping custom)
		{
			_player[playerId].Define(mapping, custom);
		}

		public static Vector2 ThumbStick(ThumbStickType thumbType, int playerId = 0)
		{
			if (!Enabled)
			{
				return Vector2.Zero;
			}

			var padState = _player[playerId].PadState;
			if (!padState.IsConnected) return Vector2.Zero;
			return thumbType == ThumbStickType.Left ? padState.ThumbSticks.Left : padState.ThumbSticks.Right;
		}

		public static float ThumbStickX(ThumbStickType thumbType, int playerId = 0)
		{
			return ThumbStick(thumbType, playerId).X;
		}

		public static float ThumbStickY(ThumbStickType thumbType, int playerId = 0)
		{
			return ThumbStick(thumbType, playerId).Y;
		}

		public static bool IsDown(string mapping, int playerId = 0)
		{
			return _player[playerId].IsDown(mapping);
		}

		public static bool IsUp(string mapping, int playerId = 0)
		{
			return _player[playerId].IsUp(mapping);
		}

		public static bool WasPressed(string mapping, int playerId = 0)
		{
			return _player[playerId].WasPressed(mapping);
		}

		public static bool WasReleased(string mapping, int playerId = 0)
		{
			return _player[playerId].WasReleased(mapping);
		}

		private static Vector2 mousePosition = Vector2.Zero;
		public static Vector2 MousePosition
		{
			get { return mousePosition; }
		}

		public static float MouseX
		{
			get { return mousePosition.X; }
		}

		public static float MouseY
		{
			get { return mousePosition.Y; }
		}

		public static bool IsMouseDown(MouseButton button)
		{
			if (!Enabled)
			{
				return false;
			}

			return GetMouseButton(mouse, button) == ButtonState.Pressed;
		}

		public static bool IsMouseUp(MouseButton button)
		{
			if (!Enabled)
			{
				return false;
			}

			return GetMouseButton(mouse, button) == ButtonState.Released;
		}

		public static bool WasMousePressed(MouseButton button)
		{
			if (!Enabled)
			{
				return false;
			}

			return GetMouseButton(mousePrev, button) == ButtonState.Released && IsMouseDown(button);
		}

		public static bool WasMouseReleased(MouseButton button)
		{
			if (!Enabled)
			{
				return false;
			}

			return GetMouseButton(mousePrev, button) == ButtonState.Pressed && IsMouseUp(button);
		}

		private static ButtonState GetMouseButton(MouseState state, MouseButton button)
		{
			switch (button)
			{
				case MouseButton.Left:
					return state.LeftButton;
				case MouseButton.Middle:
					return state.MiddleButton;
				case MouseButton.Right:
					return state.RightButton;
				default:
					return state.LeftButton; // ?
			}
		}

		internal static void Update(GameTime gameTime)
		{
			keyboardPrev = keyboard;
			keyboard = Keyboard.GetState();

			foreach (var player in _player)
			{
				player.Update();
			}

			mousePrev = mouse;
			mouse = Mouse.GetState();
			mousePosition.X = Engine.ScreenToLocalX(mouse.X);
			mousePosition.Y = Engine.ScreenToLocalY(mouse.Y);
		}

		private delegate bool KeyCallback(Keys key);
		private delegate bool ButtonCallback(Buttons button);
		private delegate bool CustomCallback(CustomMapping custom);

		private class Player
		{
			public GamePadState PadStatePrev { get; private set; }
			public GamePadState PadState { get; private set; }

			private readonly Dictionary<string, Keys[]> keyMap = new Dictionary<string, Keys[]>();
			private readonly Dictionary<string, Buttons[]> buttonMap = new Dictionary<string, Buttons[]>();
			private readonly Dictionary<string, CustomMapping> customMap = new Dictionary<string, CustomMapping>();

			private readonly PlayerIndex index;

			public Player(int index)
			{
				this.index = (PlayerIndex)index;

			}

			public void Update()
			{
				foreach (var custom in customMap.Values)
				{
					custom.Update();
				}
				PadStatePrev = PadState;
				PadState = GamePad.GetState(index);
			}

			public void Define(string mapping, params Keys[] keys)
			{
				keyMap[mapping] = keys;
			}

			public void Define(string mapping, params Buttons[] buttons)
			{
				buttonMap[mapping] = buttons;
			}

			public void Define(string mapping, CustomMapping custom)
			{
				customMap.Add(mapping, custom);
			}

			private bool CheckMapping(string mapping, CustomCallback customCallback, KeyCallback keyCallback, ButtonCallback buttonCallback)
			{
				CustomMapping custom;
				if (customMap.TryGetValue(mapping, out custom))
				{
					if (customCallback(custom))
					{
						return true;
					}
				}

				Keys[] keys;
				if (keyMap.TryGetValue(mapping, out keys))
				{
					foreach (Keys key in keys)
					{
						if (keyCallback(key))
						{
							return true;
						}
					}
				}

				Buttons[] buttons;
				if (buttonMap.TryGetValue(mapping, out buttons))
				{
					foreach (var button in buttons)
					{
						if (buttonCallback(button))
						{
							return true;
						}
					}
				}
				return false;
			}

			public bool IsDown(string mapping)
			{
				return CheckMapping(mapping,
					(custom) => { return custom.IsDown(); },
					(key) => { return IsKeyDown(key); },
					(button) => { return IsButtonDown(button, (int)index); }
				);
			}

			public bool IsUp(string mapping)
			{
				return CheckMapping(mapping,
					(custom) => { return custom.IsUp(); },
					(key) => { return IsKeyUp(key); },
					(button) => { return IsButtonUp(button, (int)index); }
				);
			}

			public bool WasPressed(string mapping)
			{
				return CheckMapping(mapping,
					(custom) => { return custom.WasPressed(); },
					(key) => { return WasKeyPressed(key); },
					(button) => { return WasButtonPressed(button, (int)index); }
				);
			}

			public bool WasReleased(string mapping)
			{
				return CheckMapping(mapping,
					(custom) => { return custom.WasReleased(); },
					(key) => { return WasKeyReleased(key); },
					(button) => { return WasButtonReleased(button, (int)index); }
				);
			}
		}

		public interface CustomMapping
		{
			bool IsDown();
			bool IsUp();
			bool WasPressed();
			bool WasReleased();
			void Update();
		}
	}
}
