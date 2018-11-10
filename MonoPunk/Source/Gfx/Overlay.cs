using MonoPunk;
using Microsoft.Xna.Framework;
using System;

namespace MonoPunk
{
	public class Fader : Entity
	{
		public int RetroResolution { get; set; }

		private readonly RectangleShape overlay;
		private Action callback;

		private float alpha;

		public Fader(int width, int height, Color color, int layer = -1)
		{
			Layer = layer;
			overlay = new RectangleShape(width, height);
			overlay.Visible = false;
			overlay.Color = color;
			Add(overlay);
		}

		public Fader(Color color, int layer = -1) : this(Engine.Width, Engine.Height, color, layer)
		{
		}

		public void FadeOut(float duration, Action callback = null)
		{
			this.callback = callback;
			alpha = 0.0f;
			overlay.Alpha = alpha;
			overlay.Visible = true;

			Scene.Tween(this, new { alpha = 1.0f }, duration).OnComplete(() =>
			{
				callback?.Invoke();
			});
		}

		public void FadeIn(float duration, Action callback = null)
		{
			this.callback = callback;
			overlay.Visible = true;
			alpha = 1.0f;
			overlay.Alpha = alpha;

			Scene.Tween(this, new { alpha = 0.0f }, duration).OnComplete(() =>
			{
				overlay.Visible = false;
				callback?.Invoke();
			});
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);

			if(RetroResolution == 0)
			{
				overlay.Alpha = alpha;
			}
			else
			{
				overlay.Alpha = Mathf.Round(alpha * RetroResolution) / ((float)RetroResolution);
			}
		}
	}

	public class Flash : Entity
	{
		private readonly RectangleShape overlay;
		private readonly float flashDuration;
		private float counter;
		private Action callback;

		public bool Active
		{
			get { return counter > 0.0f; }
		}

		public Flash(int width, int height, Color color, float flashDuration, int layer = -1)
		{
			Layer = layer;
			this.flashDuration = flashDuration;

			overlay = new RectangleShape(width, height);
			overlay.Visible = false;
			overlay.Color = color;
			Add(overlay);
		}

		public Flash(Color color, float flashDuration, int layer = -1) : this(Engine.Width, Engine.Height, color, flashDuration, layer)
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);

			if (counter > 0.0f)
			{
				counter -= deltaTime;
				if (counter <= 0.0f)
				{
					overlay.Visible = false;
					callback?.Invoke();
				}
			}
		}

		public void Show(float duration, Action callback = null)
		{
			overlay.Visible = true;
			counter = duration;
			this.callback = callback;
		}

		public void Show(Action callback = null)
		{
			Show(flashDuration, callback);
		}
	}
}
