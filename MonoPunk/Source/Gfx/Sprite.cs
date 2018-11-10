using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using System;

namespace MonoPunk
{
    public class Sprite : Renderable
    {
        public float Rotation { get; set; } = 0.0f;                
        public bool FlipH { get; set; }

        private Vector2 origin = Vector2.Zero;

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public int OriginX
        {
            get { return (int)origin.X; }
            set { origin.X = value; }
        }

        public int OriginY
        {
            get { return (int)origin.Y; }
            set { origin.Y = value; }
        }

        private Vector2 scale = Vector2.One;
        public float ScaleX
        {
            get { return scale.X; }
            set { scale.X = value; }
        }

        public float ScaleY
        {
            get { return scale.Y; }
            set { scale.Y = value; }
        }

		public float Scale
		{
			get { return Mathf.Max(scale.X, scale.Y); }
			set { scale.X = value; scale.Y = value; }
		}

		public int Width
        {
            get { return region.Width; }
        }

        public int Height
        {
            get { return region.Height; }
        }

        private readonly TextureRegion2D region;

        public Sprite(TextureRegion2D region, float x = 0.0f, float y = 0.0f)
        {
            X = x;
            Y = y;
            this.region = region;
        }

        public Sprite(Texture2D texture, float x = 0.0f, float y = 0.0f)
        {
            X = x;
            Y = y;
            region = new TextureRegion2D(texture);
        }

        public Sprite(string textureName, float x = 0.0f, float y = 0.0f) : this(Asset.LoadTexture(textureName), x, y)
        {
        }

        public void CenterOrigin()
        {
            OriginX = Width / 2;
            OriginY = Height / 2;
        }        

        protected override void OnRender(SpriteBatch spriteBatch)
        {
			var effects = FlipH ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			spriteBatch.Draw(region, GlobalPosition, Color * Alpha, Rotation, origin, scale, effects, 0.0f);
        }
    }
}
