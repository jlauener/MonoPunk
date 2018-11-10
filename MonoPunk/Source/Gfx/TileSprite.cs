using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using System;

namespace MonoPunk
{
    public class TileSprite : Renderable
    {
        public float Rotation { get; set; } = 0.0f;                
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;
       
        private Vector2 origin = Vector2.Zero;
        public int OriginX
        {
            get { return (int) origin.X; }
            set { origin.X = value; }
        }

        public int OriginY
        {
            get { return (int) origin.Y; }
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
            get { return tileset.TileWidth; }
        }

        public int Height
        {
            get { return tileset.TileHeight; }
        }

        public int Tile { get; set; }

        private readonly Tileset tileset;

        public TileSprite(Tileset tileset, int tile = 0, float x = 0.0f, float y = 0.0f)
        {
            X = x;
            Y = y;
            Tile = tile;
            this.tileset = tileset;
        }

        public TileSprite(string textureName, int width, int height, int tile = 0, float x = 0.0f, float y = 0.0f) : this(new Tileset(textureName, width, height), tile, x, y)
        {
        }

        public void CenterOrigin()
        {
            OriginX = Width / 2;
            OriginY = Height / 2;
        }        

        protected override void OnRender(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tileset.GetTileRegion(Tile), GlobalPosition, Color * Alpha, Rotation, origin, scale, Effects, 0.0f);
        }
    }
}
