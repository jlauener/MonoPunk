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
}
