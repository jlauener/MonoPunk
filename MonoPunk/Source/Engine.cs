using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Shapes;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace MonoPunk
{
	public delegate void ViewportChanged(int width, int height, bool fullscreen);

	public abstract class Engine : Game
	{
		private static Engine INSTANCE;
		internal static Engine Instance
		{
			get { return INSTANCE; }
		}

		public static Scene Scene
		{
			get { return Instance.scene; }
			set { Instance.nextScene = value; }
		}

		public static Camera2D CreateCamera()
		{
			return new Camera2D(Instance.GraphicsDevice);
		}

		public static int Width
		{
			get { return Instance.width; }
		}

		public static int Height
		{
			get { return Instance.height; }
		}

		public static int HalfWidth
		{
			get { return Instance.width / 2; }
		}

		public static int HalfHeight
		{
			get { return Instance.height / 2; }
		}

		public static float WindowScale
		{
			get { return Instance.windowScale; }
			set { Instance.windowScale = value; }
		}

		public static float ScreenToLocalX(float x)
		{
			return x * (Width / (float)Instance.graphics.PreferredBackBufferWidth);
		}

		public static float ScreenToLocalY(float y)
		{
			return y * (Height / (float)Instance.graphics.PreferredBackBufferHeight);
		}

		public double now;
		public static double Now
		{
			get { return Instance.now; }
		}

		public static bool Pause
		{
			get { return Instance.pause; }
			set { Instance.pause = value; }
		}

		public static void Throw(Exception ex)
		{
			Log.Fatal(ex.Message);
			throw ex;
		}

		public static void Throw(string errorMessage)
		{
			Throw(new Exception(errorMessage));
		}

		public static void Track(object source, string name)
		{
			Instance.trackedMembers.Remove(name);
			Instance.trackedMembers.Add(name, new TrackedMember(source, source.GetType(), name));
		}

		public static void Track(Type type, string name)
		{
			Instance.trackedMembers.Remove(name);
			Instance.trackedMembers.Add(name, new TrackedMember(null, type, name));
		}

		public static void Quit()
		{
			Instance.Exit();
		}

		public static event ViewportChanged OnViewportChanged;

		private Exception exception;

		private readonly GraphicsDeviceManager graphics;

		private Scene scene;
		private Scene nextScene;

		private readonly int width;
		private readonly int height;
		private float windowScale;
		private bool fullScreen;

		private SpriteBatch spriteBatch;
		private RenderTarget2D renderTarget;
		private Rectangle renderToScreenRect;

		private Color backColor = Color.Black;
		public static Color BackColor
		{
			get { return Instance.backColor; }
			set { Instance.backColor = value; }
		}

		public static Effect PostProcessor { get; set; }

		private string debugFontName;

		public Engine(int width, int height, float windowScale = 1.0f, bool fullScreen = false, string debugFont = "font/04b03")
		{
			INSTANCE = this;
			Content.RootDirectory = "Content";

			this.width = width;
			this.height = height;
			this.windowScale = windowScale;
			this.fullScreen = fullScreen;
			this.debugFontName = debugFont;

			graphics = new GraphicsDeviceManager(this);
			fpsCounter = new FramesPerSecondCounter();

			Log.Debug("Engine started width=" + width + " height=" + height + " fixedTimeStep=" + IsFixedTimeStep);
		}

		private void ToggleFullScreen()
		{
			fullScreen = !fullScreen;
			if (fullScreen)
			{
				SetFullScreen();
			}
			else
			{
				SetWindowed();
			}
		}

		private void SetFullScreen()
		{
			var screenWidth = GraphicsDevice.DisplayMode.Width;
			var screenHeight = GraphicsDevice.DisplayMode.Height;
			graphics.PreferredBackBufferWidth = screenWidth;
			graphics.PreferredBackBufferHeight = screenHeight;
			graphics.IsFullScreen = true;
			graphics.ApplyChanges();

			var virtualRatio = Width / (float)Height;
			var screenRatio = screenWidth / (float)screenHeight;

			if (virtualRatio > screenRatio)
			{
				// letter box
				var scale = screenWidth / (float)Width;
				var height = (int)(scale * Height);
				renderToScreenRect = new Rectangle(0, (screenHeight - height) / 2, screenWidth, height);
			}
			else if (virtualRatio < screenRatio)
			{
				// pillar box                
				var scale = screenHeight / (float)Height;
				var width = (int)(scale * Width);
				renderToScreenRect = new Rectangle((screenWidth - width) / 2, 0, width, screenHeight);
			}
			else
			{
				// no box
				renderToScreenRect = new Rectangle(0, 0, screenWidth, screenHeight);
			}

			OnViewportChanged?.Invoke(renderToScreenRect.Width, renderToScreenRect.Height, true);
		}

		private void SetWindowed()
		{
			var windowWidth = (int)(Width * windowScale);
			var windowHeight = (int)(Height * windowScale);

			graphics.PreferredBackBufferWidth = windowWidth;
			graphics.PreferredBackBufferHeight = windowHeight;
			renderToScreenRect = new Rectangle(0, 0, windowWidth, windowHeight);
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();

			OnViewportChanged?.Invoke(renderToScreenRect.Width, renderToScreenRect.Height, false);
		}

		protected override void Initialize()
		{
			base.Initialize();

			spriteBatch = new SpriteBatch(GraphicsDevice);
			renderTarget = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
		}

		protected override void LoadContent()
		{
			debugSpriteBatch = new SpriteBatch(GraphicsDevice);
			debugFont = Content.Load<BitmapFont>(debugFontName);

			OnLoadContent();

			try
			{
				Asset.InitAll();
			}
			catch (Exception ex)
			{
				exception = ex;
				Log.Fatal(ex.ToString());
			}

			if (fullScreen)
			{
				SetFullScreen();
			}
			else
			{
				SetWindowed();
			}

			OnStart();
		}

		protected override void Update(GameTime gameTime)
		{
			now = gameTime.TotalGameTime.TotalMilliseconds / 1000.0;

			Input.Update(gameTime);
			if (Input.WasKeyPressed(Keys.Enter) && (Input.IsKeyDown(Keys.LeftAlt) || Input.IsKeyDown(Keys.RightAlt)))
			{
				ToggleFullScreen();
			}

			if (Input.WasKeyPressed(Keys.OemTilde))
			{
				ToggleDebug();
			}

			if (exception != null)
			{
				if (Input.WasKeyPressed(Keys.Delete))
				{
					exception = null;
				}

				return;
			}

			if (Input.WasKeyPressed(Keys.Home))
			{
				TogglePause();
			}

			if (pause)
			{
				return;
			}

			try
			{
				if (nextScene != null)
				{
					if (scene != null)
					{
						scene.End();
					}
					scene = nextScene;
					nextScene = null;
					scene.Begin();
				}

				scene.Update(gameTime);
			}
			catch (Exception ex)
			{
				exception = ex;
				Log.Fatal(ex.ToString());
			}

			if (debugLevel > 0)
			{
				fpsCounter.Update(gameTime);
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.SetRenderTarget(renderTarget);
			GraphicsDevice.Clear(backColor);
			try
			{
				scene.Render();
			}
			catch (Exception ex)
			{
				exception = ex;
				Log.Fatal(ex.ToString());
			}

			if (exception != null)
			{
				DrawException();
			}

			if (debugLevel > 0)
			{
				fpsCounter.Draw(gameTime);
				DrawDebug();
			}

			GraphicsDevice.SetRenderTarget(null);

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, effect: PostProcessor);
			spriteBatch.Draw(renderTarget, renderToScreenRect, Color.White);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void DrawException()
		{
			debugSpriteBatch.Begin(samplerState: SamplerState.LinearClamp, blendState: BlendState.AlphaBlend);
			debugSpriteBatch.FillRectangle(new RectangleF(0.0f, 0.0f, Width, Height), Color.Blue * 0.2f);
			debugSpriteBatch.DrawString(debugFont, exception.ToString(), new Vector2(1.0f, 0.0f), Color.White);
			debugSpriteBatch.End();
		}

		private void DrawDebug()
		{
			debugSpriteBatch.Begin(samplerState: SamplerState.LinearClamp, blendState: BlendState.AlphaBlend, transformMatrix: scene.Camera.GetViewMatrix());
			if (debugLevel == 2)
			{
				scene.RenderDebug(debugSpriteBatch);
			}
			debugSpriteBatch.End();

			debugSpriteBatch.Begin(samplerState: SamplerState.LinearClamp, blendState: BlendState.AlphaBlend);
			debugSpriteBatch.FillRectangle(new RectangleF(0.0f, 0.0f, Width, debugFont.LineHeight), Color.Black * 0.5f);

			debugSpriteBatch.DrawString(debugFont, "FPS:" + Mathf.Round(fpsCounter.FramesPerSecond), new Vector2(1.0f, 0.0f), Color.White);

			var str = scene.GetEntityCount().ToString();
			var size = debugFont.MeasureString(str);
			debugSpriteBatch.DrawString(debugFont, str, new Vector2(Width - size.Width - 1.0f, 0.0f), Color.White);

			if (trackedMembers.Count > 0)
			{
				// TODO calculate size
				// debugSpriteBatch.FillRectangle(new RectangleF(0.0f, debugFont.LineHeight, Width * 0.5f, Height), Color.Black * 0.5f);
				var y = debugFont.LineHeight + 2.0f;
				foreach (var prop in trackedMembers.Values)
				{
					debugSpriteBatch.DrawString(debugFont, prop.Name + "=" + prop.Value, new Vector2(2.0f, y), Color.White);
					y += 6.0f;
				}
			}

			debugSpriteBatch.End();
		}

		protected virtual void OnLoadContent()
		{
		}

		protected abstract void OnStart();

		//
		// DEBUG
		//

		private int debugLevel = 0;
		private SpriteBatch debugSpriteBatch;
		private BitmapFont debugFont;
		private readonly FramesPerSecondCounter fpsCounter;

		private void ToggleDebug()
		{
			if (++debugLevel == 3)
			{
				debugLevel = 0;
			}
		}

		private bool pause;

		private void TogglePause()
		{
			pause = !pause;
		}

		#region Property Track

		private class TrackedMember
		{
			public string Name { get; private set; }

			public object Value
			{
				get { return property != null ? property.GetValue(source) : field.GetValue(source); }
			}

			private readonly object source;
			private readonly PropertyInfo property;
			private readonly FieldInfo field;

			public TrackedMember(object source, Type type, string memberName)
			{
				Name = memberName;
				this.source = source;
				property = type.GetProperty(Name);
				if (property == null)
				{
					property = type.GetProperty(Name, BindingFlags.NonPublic | BindingFlags.Instance);
				}

				field = type.GetField(Name);
				if (field == null)
				{
					field = type.GetField(Name, BindingFlags.NonPublic | BindingFlags.Instance);
				}

				if (property == null && field == null)
				{
					Throw("Member '" + memberName + "' not found on " + type);
				}
			}
		}

		private readonly Dictionary<string, TrackedMember> trackedMembers = new Dictionary<string, TrackedMember>();

		#endregion

		public static Texture2D LoadTextureFromDisk(string fileName)
		{
			using (var stream = new FileStream(fileName, FileMode.Open))
			{
				return Texture2D.FromStream(Instance.GraphicsDevice, stream);
			}
		}

		public static Texture2D CreateTexture(int width, int height)
		{
			return new Texture2D(Instance.GraphicsDevice, width, height);
		}

		public static string WindowTitle
		{
			get { return Instance.Window.Title; }
			set { Instance.Window.Title = value; }
		}

		public static void SetRenderTarget(RenderTarget2D renderTarget)
		{
			if (renderTarget != null)
			{
				Instance.GraphicsDevice.SetRenderTarget(renderTarget);
			}
			else
			{
				Instance.GraphicsDevice.SetRenderTarget(Instance.renderTarget);
			}
		}
	}
}
