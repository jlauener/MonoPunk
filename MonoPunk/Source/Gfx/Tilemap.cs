using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;

namespace MonoPunk
{
	public class Tileset
	{
		public Texture2D Texture { get; }
		public int TileWidth { get; }
		public int TileHeight { get; }
		public int TileCount { get { return regions.Count; } }

		private readonly Dictionary<int, TextureRegion2D> regions = new Dictionary<int, TextureRegion2D>();

		public Tileset(Texture2D texture, int tileWidth, int tileHeight)
		{
			Texture = texture;
			TileWidth = tileWidth;
			TileHeight = tileHeight;

			var id = 0;
			for (var iy = 0; iy < Texture.Height; iy += TileHeight)
			{
				for (var ix = 0; ix < Texture.Width; ix += TileWidth)
				{
					regions.Add(id, new TextureRegion2D(texture, ix, iy, TileWidth, TileHeight));
					id++;
				}
			}
		}

		public Tileset(string textureName, int tileWidth, int tileHeight) : this(Asset.LoadTexture(textureName), tileWidth, tileHeight)
		{
		}

		public TextureRegion2D GetTileRegion(int id)
		{
			return id == -1 ? null : regions[id];
		}
	}

	public class Tilemap : Renderable
	{
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
		private readonly int[] map;

		public Tilemap(Tileset tileset, int width, int height, int defaultTileId = -1)
		{
			this.tileset = tileset;
			Width = width;
			Height = height;

			map = new int[height * width];
			for (int i = 0; i < height * width; ++i)
			{
				map[i] = defaultTileId;
			}
		}

		public Tilemap(string tilesetName, int width, int height, int defaultTileId = -1) : this(Asset.GetTileset(tilesetName), width, height, defaultTileId)
		{
		}

		public void SetTileAt(int x, int y, int tileId)
		{
			map[y * Width + x] = tileId;
		}

		public int GetTileAt(int x, int y)
		{
			return map[y * Width + x];
		}

		protected override void OnRender(SpriteBatch spriteBatch)
		{
			var offsetX = (int)GlobalPosition.X;
			var offsetY = (int)GlobalPosition.Y;

			for (var ix = 0; ix < Width; ix++)
			{
				for (var iy = 0; iy < Height; iy++)
				{
					var tileId = GetTileAt(ix, iy);
					if (tileId != -1)
					{
						var region = tileset.GetTileRegion(tileId);
						var destinationRectangle = new Rectangle(offsetX + ix * TileWidth, offsetY + iy * TileHeight, TileWidth, TileHeight);
						spriteBatch.Draw(region, destinationRectangle, Color * Alpha);
					}
				}
			}
		}
	}
}
