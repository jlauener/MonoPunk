using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework;

namespace MonoPunk
{
	public class Bar : Renderable
	{
		private readonly TextureRegion2D _region;

		public float Percent { get; set; } = 1.0f;
		public bool Inverse { get; set; }

		public int Width { get { return _region.Width; } }
		public int Height { get { return _region.Height; } }

		public Bar(TextureRegion2D region)
		{
			_region = region;
		}

		public Bar(Texture2D texture)
		{
			_region = new TextureRegion2D(texture);
		}

		public Bar(string textureName) : this(Asset.LoadTexture(textureName))
		{
		}

		protected override void OnRender(SpriteBatch spriteBatch)
		{
			var texture = _region.Texture;
			var sourceRectangle = _region.Bounds;
			sourceRectangle.Width = Mathf.Round(_region.Width * Percent);
			if (Inverse)
			{
				sourceRectangle.X += _region.Width - sourceRectangle.Width;
			}
			var position = GlobalPosition;
			if (Inverse)
			{
				position.X += _region.Width - sourceRectangle.Width;
			}
			spriteBatch.Draw(texture, position, sourceRectangle, Color * Alpha, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
		}
	}
}
