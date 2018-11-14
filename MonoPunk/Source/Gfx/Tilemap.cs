using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using System;

namespace MonoPunk
{
	public class Tilemap : Renderable
	{
		private class Tile
		{
			public int TileId { get; set; }

			internal SpriteEffects Effects { get; private set; }

			public bool FlipX
			{
				get { return Effects == SpriteEffects.FlipHorizontally; }
				set
				{
					if (value)
					{
						if (FlipY) throw new Exception("Cannot flip a sprite horizontally and vertically.");
						Effects = SpriteEffects.FlipHorizontally;
					}
					else if (!FlipY)
					{
						Effects = SpriteEffects.None;
					}
				}
			}

			public bool FlipY
			{
				get { return Effects == SpriteEffects.FlipVertically; }
				set
				{
					if (value)
					{
						if (FlipX) throw new Exception("Cannot flip a sprite horizontally and vertically.");
						Effects = SpriteEffects.FlipVertically;
					}
					else if (!FlipX)
					{
						Effects = SpriteEffects.None;
					}
				}
			}

			public Tile(int tileId)
			{
				TileId = tileId;
			}
		}

		public readonly int Width;
		public readonly int Height;

		public int WidthPx
		{
			get { return Mathf.Round(Width * TileWidth * ScaleX); }
		}

		public int HeightPx
		{
			get { return Mathf.Round(Width * TileWidth * ScaleY); }
		}

		public int TileWidth
		{
			get { return Mathf.Round(tileset.TileWidth * ScaleX); }
		}

		public int TileHeight
		{
			get { return Mathf.Round(tileset.TileHeight * ScaleY); }
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

		private Tileset tileset;
		private readonly Tile[,] map;

		public Tilemap(Tileset tileset, int width, int height, int defaultTileId = -1)
		{
			this.tileset = tileset;
			Width = width;
			Height = height;

			map = new Tile[width, height];
			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)

				{
					map[ix,iy] = new Tile(defaultTileId);
				}
			}
		}

		public Tilemap(string tilesetName, int width, int height, int defaultTileId = -1) : this(Asset.GetTileset(tilesetName), width, height, defaultTileId)
		{
		}

		public void SetTileAt(int x, int y, int tileId)
		{
			map[x, y].TileId = tileId;
		}

		public int GetTileAt(int x, int y)
		{
			return map[x, y].TileId;
		}

		protected override void OnRender(SpriteBatch spriteBatch)
		{
			var offsetX = (int)GlobalPosition.X;
			var offsetY = (int)GlobalPosition.Y;

			for (var ix = 0; ix < Width; ix++)
			{
				for (var iy = 0; iy < Height; iy++)
				{
					var tile = map[ix, iy];
					if (tile.TileId != -1)
					{
						var region = tileset.GetTileRegion(tile.TileId);
						var position = new Vector2(offsetX + ix * TileWidth, offsetY + iy * TileHeight);
						spriteBatch.Draw(region, position, Color * Alpha, 0.0f, Vector2.Zero, Vector2.One, tile.Effects, 0.0f);
					}
				}
			}
		}
	}
}
