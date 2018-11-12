using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using System;

namespace MonoPunk
{
	public class PixelMask : ICollider
	{
		public int WidthPx { get; }
		public int HeightPx { get; }

		private readonly bool[,] mask;

		private readonly TextureRegion2D region;

		public PixelMask(TextureRegion2D region)
		{
			this.region = region;

			WidthPx = region.Bounds.Width;
			HeightPx = region.Bounds.Height;

			Color[] textureData = new Color[region.Texture.Width * region.Texture.Height];
			region.Texture.GetData(textureData);
			mask = new bool[WidthPx, HeightPx];
			for (var ix = 0; ix < WidthPx; ix++)
			{
				for (var iy = 0; iy < HeightPx; iy++)
				{
					var color = textureData[region.Bounds.X + ix + (region.Bounds.Y + iy) * region.Texture.Width];
					mask[ix, iy] = color == Color.White;
				}
			}
		}

		public PixelMask(Texture2D texture) : this(new TextureRegion2D(texture))
		{
		}

		public PixelMask(string textureName) : this(Asset.LoadTexture(textureName))
		{
		}

		public HitInfo CollideWithOther(float x, float y, float otherX, float otherY, Entity other)
		{
			var hit = HitInfo.None;

			if (other.Collider is PixelMask)
			{
				// PixelMask to PixelMask collision.
				if (CollidePixelMaskToPixelMask(x, y, otherX, otherY, (PixelMask)other.Collider))
				{
					hit = HitInfo.CreateHit(other);
				}
			}
			else if (other.Collider is GridCollider)
			{
				// PixelMask to GridCollider collision.
				var gridCollider = (GridCollider)other.Collider;
				gridCollider.QueryTiles(otherX, otherY, x, y, WidthPx, HeightPx, (tile, cellX, cellY, cellWidth, cellHeight) =>
				{
				//	var tileX = otherX + tile.X * gridCollider.CellWidth;
					//var tileY = otherY + tile.Y * gridCollider.CellHeight;

					if (tile.PixelMask != null)
					{
						if (CollidePixelMaskToPixelMask(x, y, cellX, cellY, tile.PixelMask))
						{
							hit = HitInfo.CreateHit(other, tile);
							return true;
						}
					}
					else if (CollideRect(x, y, cellX, cellY, cellWidth, cellHeight))
					{
						hit = HitInfo.CreateHit(other, tile);
						return true;
					}

					return false;
				});
			}
			else if (CollideRect(x, y, otherX, otherY, other.Width, other.Height))
			{
				// Other collision: Use the other entity hitbox.
				hit = HitInfo.CreateHit(other);
			}

			return hit;
		}

		private bool CollidePixelMaskToPixelMask(float x, float y, float otherX, float otherY, PixelMask otherMask)
		{
			var otherOffsetX = (int)(otherX - x);
			var startX = otherOffsetX;
			if (startX < 0) startX = 0;

			var otherOffsetY = (int)(otherY - y);
			var startY = otherOffsetY;
			if (startY < 0) startY = 0;

			var endX = (int)(otherX + otherMask.WidthPx - x);
			if (endX > WidthPx) endX = WidthPx;

			var endY = (int)(otherY + otherMask.HeightPx - y);
			if (endY > HeightPx) endY = HeightPx;

			//Log.Debug("startX=" + startX + " startY=" + startY + " endX=" + endX + " endY=" + endY);			

			for (var ix = startX; ix < endX; ix++)
			{
				for (var iy = startY; iy < endY; iy++)
				{
					if (mask[ix, iy] && otherMask.mask[ix - otherOffsetX, iy - otherOffsetY])
					{
						return true;
					}
				}
			}

			return false;
		}

		public HitInfo CollideRect(float x, float y, float rectX, float rectY, int rectWidth, int rectHeight)
		{
			var startX = (int)(rectX - x);
			if (startX < 0) startX = 0;

			var startY = (int)(rectY - y);
			if (startY < 0) startY = 0;

			var endX = (int)(rectX + rectWidth - x);
			if (endX > WidthPx) endX = WidthPx;

			var endY = (int)(rectY + rectHeight - y);
			if (endY > HeightPx) endY = HeightPx;

			for (var ix = startX; ix < endX; ix++)
			{
				for (var iy = startY; iy < endY; iy++)
				{
					if (mask[ix, iy])
					{
						return HitInfo.CreateHit();
					}
				}
			}

			return HitInfo.None;
		}

		public void RenderDebug(float x, float y, SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(region, new Vector2(x, y), Color.Green);
		}
	}

	public class PixelMaskSet
	{
		private readonly PixelMask[] masks;

		public PixelMaskSet(Tileset tileset)
		{
			masks = new PixelMask[tileset.TileCount];
			for (var i = 0; i < tileset.TileCount; i++)
			{
				var region = tileset.GetTileRegion(i);
				masks[i] = new PixelMask(region);
			}
		}

		public PixelMaskSet(string textureName, int tileWidth, int tileHeight) : this(new Tileset(textureName, tileWidth, tileHeight))
		{
		}

		public PixelMaskSet(string tilesetName) : this(Asset.GetTileset(tilesetName))
		{
		}

		public PixelMask GetMask(int id)
		{
			return masks[id];
		}
	}
}
