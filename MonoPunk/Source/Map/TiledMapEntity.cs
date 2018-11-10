using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;

namespace MonoPunk
{
	public class TiledMapEntity : Entity
	{
		public int MapWidth
		{
			get { return Map.Width; }
		}

		public int MapHeight
		{
			get { return Map.Height; }
		}

		public int TileWidth { get; }
		public int TileHeight { get; }

		public float Scale { get; }

		public TiledMap Map { get; private set; }

		public TiledMapEntity(string mapName, float scale = 1.0f) : this(Asset.LoadTiledMap(mapName), scale)
		{
		}

		public TiledMapEntity(TiledMap map, float scale = 1.0f)
		{
			Map = map;
			Scale = scale;
			TileWidth = Mathf.Round(Map.TileWidth * Scale);
			TileHeight = Mathf.Round(Map.TileHeight * Scale);
			Width = Mathf.Round(Map.WidthInPixels * Scale);
			Height = Mathf.Round(Map.HeightInPixels * Scale);
		}

		public Tilemap LoadTilemap(string layerName, string textureName, Func<Tile, bool> tileSelector = null)
		{
			return LoadTilemap(layerName, new Tileset(textureName, Map.TileWidth, Map.TileHeight), tileSelector);
		}

		public Tilemap LoadTilemap(string layerName, Tileset tileset, Func<Tile, bool> tileSelector = null)
		{
			var tilemap = new Tilemap(tileset, MapWidth, MapHeight);
			tilemap.Scale = Scale;
			Add(tilemap);

			var layer = Map.GetLayer<TiledMapTileLayer>(layerName);
			if (layer == null) return tilemap;

			for (var ix = 0; ix < MapWidth; ix++)
			{
				for (var iy = 0; iy < MapHeight; iy++)
				{
					TiledMapTile? tiledTile;
					if (layer.TryGetTile(ix, iy, out tiledTile))
					{
						var tid = tiledTile.Value.GlobalIdentifier - 1;
						if (tid >= 0 && (tileSelector == null || tileSelector(GetTileAt(layer, ix, iy))))
						{
							tilemap.SetTileAt(ix, iy, tid);
						}
					}
				}
			}

			return tilemap;
		}

		public void LoadCollisionGrid(string layerName, Func<Tile, TileSolidType> solidSelector = null)
		{
			var layer = GetLayer(layerName);
			if (layer == null) return;

			var gridCollider = new GridCollider(MapWidth, MapHeight, TileWidth, TileHeight);
			Collider = gridCollider;

			for (var ix = 0; ix < MapWidth; ix++)
			{
				for (var iy = 0; iy < MapHeight; iy++)
				{
					var tile = GetTileAt(layer, ix, iy);
					if (tile != null)
					{
						if (solidSelector != null)
						{
							tile.SolidType = solidSelector(tile);
						}
						else
						{
							tile.SolidType = TileSolidType.Full;
						}
						gridCollider.AddTile(tile);
					}
				}
			}
		}

		public TiledMapTileLayer GetLayer(string name)
		{
			return Map.GetLayer<TiledMapTileLayer>(name);
		}

		public void Iterate(string layerName, Action<Tile> action)
		{
			var layer = GetLayer(layerName);
			if (layer == null) return;

			for (var ix = 0; ix < MapWidth; ix++)
			{
				for (var iy = 0; iy < MapHeight; iy++)
				{
					var tile = GetTileAt(layer, ix, iy);
					if (tile != null)
					{
						action(tile);
					}
				}
			}
		}

		public Tile GetTileAt(TiledMapTileLayer layer, int x, int y)
		{
			return Map.GetTileAt(layer, x, y);
			//if (x < 0 || x >= MapWidth || y < 0 || y >= MapHeight)
			//{
			//	return null;
			//}

			//TiledMapTile? tiledTile;
			//if (!layer.TryGetTile(x, y, out tiledTile) || tiledTile.Value.GlobalIdentifier == 0)
			//{
			//	return null;
			//}

			//var globalId = tiledTile.Value.GlobalIdentifier;
			//var tileset = Map.GetTilesetByTileGlobalIdentifier(globalId);
			//var localId = globalId - tileset.FirstGlobalIdentifier;
			//var tiledTilesetTile = tileset.GetTileByLocalTileIdentifier(localId);
			//if (tiledTilesetTile == null)
			//{
			//	return new Tile(globalId, localId, x, y);
			//}
			//else
			//{
			//	return new Tile(globalId, tiledTilesetTile.LocalTileIdentifier, x, y, tiledTilesetTile.Properties);
			//}
		}

		public override string ToString()
		{
			return "[TiledMap name=" + Map.Name + "]";
		}
	}
}
