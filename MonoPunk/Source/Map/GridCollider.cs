using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoPunk
{
	public class GridCollider : ICollider
	{
		public int WidthPx { get; }
		public int HeightPx { get; }

		public int Width { get; }
		public int Height { get; }
		public int CellWidth { get; }
		public int CellHeight { get; }

		private readonly Tile[] grid;
		private readonly PixelMaskSet pixelMaskSet;

		public GridCollider(int width, int height, int cellWidth, int cellHeight, PixelMaskSet pixelMaskSet = null)
		{
			Width = width;
			Height = height;
			CellWidth = cellWidth;
			CellHeight = cellHeight;
			grid = new Tile[width * height];

			WidthPx = width * cellWidth;
			HeightPx = height * cellHeight;

			this.pixelMaskSet = pixelMaskSet;
		}

		public void AddTile(Tile tile)
		{
			grid[tile.Y * Width + tile.X] = tile;
		}

		public void SetTileAt(int x, int y, TileSolidType solidType)
		{
			if (solidType == TileSolidType.PixelMask)
			{
				throw new Exception("Invalid TileSolidType, use setPixelMaskAt instead.");
			}

			grid[y * Width + x] = new Tile(x, y, solidType);
		}

		public void SetPixelMaskAt(int x, int y, int id)
		{
			if (pixelMaskSet == null)
			{
				throw new Exception("Grid collider doesn't have a PixelMaskSet, set one in the GridCollider constructor.");
			}

			grid[y * Width + x] = new Tile(x, y, pixelMaskSet.GetMask(id));
		}

		public HitInfo CollideWithOther(float x, float y, float otherX, float otherY, Entity other)
		{
			var hit = HitInfo.None;

			QueryTiles(x, y, otherX, otherY, other.Width, other.Height, (tile, cellX, cellY, localX, localY) =>
			{
				switch (tile.SolidType)
				{
					case TileSolidType.Full:
						if (Mathf.IntersectRect(cellX, cellY, CellWidth, CellHeight, localX, localY, other.Width, other.Height))
						{
							hit = HitInfo.CreateHit(other, tile);
							return true;
						}
						break;
					case TileSolidType.HalfTop:
						if (Mathf.IntersectRect(cellX, cellY, CellWidth, CellHeight / 2, localX, localY, other.Width, other.Height))
						{
							hit = HitInfo.CreateHit(other, tile);
							return true;
						}
						break;
					case TileSolidType.HalfBottom:
						if (Mathf.IntersectRect(cellX, cellY + CellHeight / 2, CellWidth, CellHeight / 2, localX, localY, other.Width, other.Height))
						{
							hit = HitInfo.CreateHit(other, tile);
							return true;
						}
						break;
					case TileSolidType.HalfLeft:
						if (Mathf.IntersectRect(cellX, cellY, CellWidth / 2, CellHeight, localX, localY, other.Width, other.Height))
						{
							hit = HitInfo.CreateHit(other, tile);
							return true;
						}
						break;
					case TileSolidType.HalfRight:
						if (Mathf.IntersectRect(cellX + CellWidth / 2, cellY, CellWidth / 2, CellHeight, localX, localY, other.Width, other.Height))
						{
							hit = HitInfo.CreateHit(other, tile);
							return true;
						}
						break;
					case TileSolidType.PixelMask:
						if (tile.PixelMask.CollideWithOther(cellX, cellY, localX, localY, other))
						{
							hit = HitInfo.CreateHit(other, tile);
							return true;
						}
						break;
				}

				return false;
			});

			return hit;
		}

		public HitInfo CollideRect(float x, float y, float rectX, float rectY, int rectWidth, int rectHeight)
		{
			var hit = HitInfo.None;

			QueryTiles(x, y, rectX, rectY, rectWidth, rectHeight, (tile, cellX, cellY, localX, localY) =>
			{
				switch (tile.SolidType)
				{
					case TileSolidType.Full:
						if (Mathf.IntersectRect(cellX, cellY, CellWidth, CellHeight, localX, localY, rectWidth, rectHeight))
						{
							hit = HitInfo.CreateHit(tile);
							return true;
						}
						break;
					case TileSolidType.HalfTop:
						if (Mathf.IntersectRect(cellX, cellY, CellWidth, CellHeight / 2, localX, localY, rectWidth, rectHeight))
						{
							hit = HitInfo.CreateHit(tile);
							return true;
						}
						break;
					case TileSolidType.HalfBottom:
						if (Mathf.IntersectRect(cellX, cellY + CellHeight / 2, CellWidth, CellHeight / 2, localX, localY, rectWidth, rectHeight))
						{
							hit = HitInfo.CreateHit(tile);
							return true;
						}
						break;
					case TileSolidType.HalfLeft:
						if (Mathf.IntersectRect(cellX, cellY, CellWidth / 2, CellHeight, localX, localY, rectWidth, rectHeight))
						{
							hit = HitInfo.CreateHit(tile);
							return true;
						}
						break;
					case TileSolidType.HalfRight:
						if (Mathf.IntersectRect(cellX + CellWidth / 2, cellY, CellWidth / 2, CellHeight, localX, localY, rectWidth, rectHeight))
						{
							hit = HitInfo.CreateHit(tile);
							return true;
						}
						break;
					case TileSolidType.PixelMask:
						if (tile.PixelMask.CollideRect(cellX, cellY, localX, localY, rectWidth, rectHeight))
						{
							hit = HitInfo.CreateHit(tile);
							return true;
						}
						break;
				}

				return false;
			});

			return hit;
		}

		public void QueryTiles(float x, float y, float rectX, float rectY, int rectWidth, int rectHeight, Func<Tile, int, int, float, float, bool> callback)
		{
			var localX = rectX - x;
			var localY = rectY - y;

			var startX = Math.Max((int)(localX / CellWidth), 0);
			var startY = Math.Max((int)(localY / CellHeight), 0);
			var endX = Math.Min(startX + rectWidth / CellWidth + 2, Width); // FIXME (+2) ?
			var endY = Math.Min(startY + rectHeight / CellHeight + 2, Height); // FIXME (+2) ?

			var cellX = startX * CellWidth;
			for (var ix = startX; ix < endX; ix++)
			{
				var cellY = startY * CellHeight;
				for (var iy = startY; iy < endY; iy++)
				{
					var tile = grid[iy * Width + ix];
					if (tile != null)
					{
						if (callback(tile, cellX, cellY, localX, localY)) return;
					}
					cellY += CellHeight;
				}
				cellX += CellWidth;
			}
		}

		public void RenderDebug(float x, float y, SpriteBatch spriteBatch)
		{
			for (var ix = 0; ix < Width; ix++)
			{
				for (var iy = 0; iy < Height; iy++)
				{
					var rect = new RectangleF();
					var tile = grid[iy * Width + ix];
					if (tile != null)
					{
						switch (tile.SolidType)
						{
							case TileSolidType.Full:
								rect.X = x + ix * CellWidth;
								rect.Y = y + iy * CellHeight;
								rect.Width = CellWidth;
								rect.Height = CellHeight;
								break;
							case TileSolidType.HalfTop:
								rect.X = x + ix * CellWidth;
								rect.Y = y + iy * CellHeight;
								rect.Width = CellWidth;
								rect.Height = CellHeight / 2;
								break;
							case TileSolidType.HalfBottom:
								rect.X = x + ix * CellWidth;
								rect.Y = y + iy * CellHeight + CellHeight / 2;
								rect.Width = CellWidth;
								rect.Height = CellHeight / 2;
								break;
							case TileSolidType.HalfLeft:
								rect.X = x + ix * CellWidth;
								rect.Y = y + iy * CellHeight;
								rect.Width = CellWidth / 2;
								rect.Height = CellHeight;
								break;
							case TileSolidType.HalfRight:
								rect.X = x + ix * CellWidth + CellWidth / 2;
								rect.Y = y + iy * CellHeight;
								rect.Width = CellWidth / 2;
								rect.Height = CellHeight;
								break;
							case TileSolidType.PixelMask:
								rect.X = x + ix * CellWidth;
								rect.Y = y + iy * CellHeight;
								rect.Width = CellWidth;
								rect.Height = CellHeight;
								break;
						}
					}
					spriteBatch.FillRectangle(rect, Color.Green * 0.33f);
					spriteBatch.DrawRectangle(rect, Color.Green);
				}
			}
		}
	}
}
