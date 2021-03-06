using System;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapReader : ContentTypeReader<TiledMap>
    {
        protected override TiledMap Read(ContentReader input, TiledMap existingInstance)
        {
            if (existingInstance != null)
                return existingInstance;

            var value = ReadMetaData(input);
            ReadTilesets(input, value);
            ReadLayers(input, value);

            return value;
        }

        private static TiledMap ReadMetaData(ContentReader input)
        {
            var name = input.AssetName;
            var width = input.ReadInt32();
            var height = input.ReadInt32();
            var tileWidth = input.ReadInt32();
            var tileHeight = input.ReadInt32();
            var backgroundColor = input.ReadColor();
            var renderOrder = (TiledMapTileDrawOrder)input.ReadByte();
            var orientation = (TiledMapOrientation)input.ReadByte();

            var map = new TiledMap(name, width, height, tileWidth, tileHeight, renderOrder, orientation)
            {
                BackgroundColor = backgroundColor
            };

            input.ReadTiledMapProperties(map.Properties);

            return map;
        }

        private static void ReadTilesets(ContentReader input, TiledMap map)
        {
            var tilesetCount = input.ReadInt32();

            for (var i = 0; i < tilesetCount; i++)
            {
                var tileset = new TiledMapTileset(input);
                map.AddTileset(tileset);
            }
        }

        private static void ReadLayers(ContentReader input, TiledMap map)
        {
            var layerCount = input.ReadInt32();

            for (var i = 0; i < layerCount; i++)
            {
                TiledMapLayer layer;

                var layerType = (TiledMapLayerType)input.ReadByte();
                switch (layerType)
                {
                    case TiledMapLayerType.ImageLayer:
                        layer = new TiledMapImageLayer(input);
                        break;
                    case TiledMapLayerType.TileLayer:
                        layer = new TiledMapTileLayer(input, map);
                        break;
                    case TiledMapLayerType.ObjectLayer:
                        layer = new TiledMapObjectLayer(input, map);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                map.AddLayer(layer);
            }
        }
    }
}