﻿using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using System.Collections.Generic;

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

		public GridCollider(int width, int height, int cellWidth, int cellHeight, string pixelMaskSetName) : this(width, height, cellWidth, cellHeight, Asset.GetPixelMaskSet(pixelMaskSetName))
		{
		}

		public void AddTile(Tile tile)
		{
			grid[tile.Y * Width + tile.X] = tile;
		}

		public void SetTileAt(int x, int y, TileSolidType solidType, Dictionary<string, string> properties = null)
		{
			if (solidType == TileSolidType.PixelMask)
			{
				throw new Exception("Invalid TileSolidType, use setPixelMaskAt instead.");
			}

			grid[y * Width + x] = new Tile(x, y, solidType, properties);
		}

		public void SetPixelMaskAt(int x, int y, int id, Dictionary<string, string> properties = null)
		{
			if (pixelMaskSet == null)
			{
				throw new Exception("Grid collider doesn't have a PixelMaskSet, set one in the GridCollider constructor.");
			}

			grid[y * Width + x] = new Tile(x, y, pixelMaskSet.GetMask(id), properties);
		}

		public void SetPixelMaskAt(int x, int y, PixelMask pixelMask, Dictionary<string, string> properties = null)
		{
			grid[y * Width + x] = new Tile(x, y, pixelMask, properties);
		}

		public HitInfo CollideWithOther(float x, float y, float otherX, float otherY, Entity other)
		{
			var hit = HitInfo.None;

			QueryTiles(x, y, otherX, otherY, other.Width, other.Height, (tile, cellX, cellY, cellWidth, cellHeight) =>
			{
				if (tile.PixelMask != null)
				{
					if (tile.PixelMask.CollideWithOther(cellX, cellY, otherX, otherY, other))
					{
						hit = HitInfo.CreateHit(other, tile, tile.PixelMask);
						return true;
					}
				}
				else if (Mathf.IntersectRect(cellX, cellY, cellWidth, cellHeight, otherX, otherY, other.Width, other.Height))
				{
					hit = HitInfo.CreateHit(other, tile);
					return true;
				}

				return false;
			});

			return hit;
		}

		public HitInfo CollideRect(float x, float y, float rectX, float rectY, int rectWidth, int rectHeight)
		{
			var hit = HitInfo.None;

			QueryTiles(x, y, rectX, rectY, rectWidth, rectHeight, (tile, cellX, cellY, cellWidth, cellHeight) =>
			{
				if (tile.PixelMask != null)
				{
					if (tile.PixelMask.CollideRect(cellX, cellY, rectX, rectY, rectWidth, rectHeight))
					{
						hit = HitInfo.CreateHit(tile, tile.PixelMask);
						return true;
					}
				}
				else if (Mathf.IntersectRect(cellX, cellY, cellWidth, cellHeight, rectX, rectY, rectWidth, rectHeight))
				{
					hit = HitInfo.CreateHit(tile);
					return true;
				}

				return false;

			});

			return hit;
		}

		public void QueryTiles(float x, float y, float rectX, float rectY, int rectWidth, int rectHeight, Func<Tile, float, float, int, int, bool> callback)
		{
			var startX = Math.Max((int)((rectX - x) / CellWidth), 0);
			var startY = Math.Max((int)((rectY - y) / CellHeight), 0);
			var endX = Math.Min(startX + rectWidth / CellWidth + 2, Width); // FIXME (+2) ?
			var endY = Math.Min(startY + rectHeight / CellHeight + 2, Height); // FIXME (+2) ?

			var cellX = x + startX * CellWidth;
			for (var ix = startX; ix < endX; ix++)
			{
				var cellY = y + startY * CellHeight;
				for (var iy = startY; iy < endY; iy++)
				{
					var tile = grid[iy * Width + ix];
					if (tile != null)
					{
						switch (tile.SolidType)
						{
							case TileSolidType.Full:
								if (callback(tile, cellX, cellY, CellWidth, CellHeight)) return;
								break;
							case TileSolidType.HalfTop:
								if (callback(tile, cellX, cellY, CellWidth, CellHeight / 2)) return;
								break;
							case TileSolidType.HalfBottom:
								if (callback(tile, cellX, cellY + CellHeight / 2, CellWidth, CellHeight / 2)) return;
								break;
							case TileSolidType.HalfLeft:
								if (callback(tile, cellX, cellY, CellWidth / 2, CellHeight)) return;
								break;
							case TileSolidType.HalfRight:
								if (callback(tile, cellX + CellWidth / 2, cellY, CellWidth / 2, CellHeight)) return;
								break;
							case TileSolidType.PixelMask:
								if (callback(tile, cellX, cellY, CellWidth, CellHeight)) return;
								break;
						}
					}
					cellY += CellHeight;
				}
				cellX += CellWidth;
			}
		}

		public void RenderDebug(float x, float y, SpriteBatch spriteBatch)
		{
			QueryTiles(x, y, x, y, WidthPx, HeightPx, (tile, cellX, cellY, cellWidth, cellHeight) =>
			{
				if (tile.PixelMask != null)
				{
					tile.PixelMask.RenderDebug(cellX, cellY, spriteBatch);
				}
				else
				{
					var rect = new RectangleF(cellX, cellY, cellWidth, cellHeight);
					spriteBatch.FillRectangle(rect, Color.Green * 0.5f);
					spriteBatch.DrawRectangle(rect, Color.Green);
				}
				return false;
			});

		}
	}
}
