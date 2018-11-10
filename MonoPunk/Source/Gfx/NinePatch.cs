using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoPunk
{
    public class NinePatch : Renderable
    {
        private const int TOP_LEFT = 0;
        private const int TOP = 1;
        private const int TOP_RIGHT = 2;
        private const int LEFT = 3;
        private const int CENTER = 4;
        private const int RIGHT = 5;
        private const int BOTTOM_LEFT = 6;
        private const int BOTTOM = 7;
        private const int BOTTOM_RIGHT = 8;

        public int Width { get; private set; }
        public int Height { get; private set; }        

        private readonly Tileset _tileset;
        private readonly int _right;
        private readonly int _bottom;

        public NinePatch(string textureName, int tileWidth, int tileHeight, int width, int height) : this(new Tileset(textureName, tileWidth, tileHeight), width, height)
        {
        }

        public NinePatch(Tileset tileset, int width, int height)
        {
            _tileset = tileset;
            _right = width - 1;
            _bottom = height - 1;
            Width = width * tileset.TileWidth;
            Height = height * tileset.TileHeight;
        }

        protected override void OnRender(SpriteBatch spriteBatch)
        {
            drawTile(spriteBatch, TOP_LEFT, 0, 0);
            drawTile(spriteBatch, BOTTOM_LEFT, 0, _bottom);
            for(var ix = 1; ix < _right; ix++)
            {
                drawTile(spriteBatch, TOP, ix, 0);
                drawTile(spriteBatch, BOTTOM, ix, _bottom);
                for(var iy = 1; iy < _bottom; iy++)
                {
                    drawTile(spriteBatch, CENTER, ix, iy);
                }
            }
            drawTile(spriteBatch, TOP_RIGHT, _right, 0);
            drawTile(spriteBatch, BOTTOM_RIGHT, _right, _bottom);

            for(var iy = 1; iy < _bottom; iy++)
            {
                drawTile(spriteBatch, LEFT, 0, iy);
                drawTile(spriteBatch, RIGHT, _right, iy);
            }
        }

        private void drawTile(SpriteBatch spriteBatch, int id, int tileX, int tileY)
        {
            var region = _tileset.GetTileRegion(id);
            var position = GlobalPosition + new Vector2(tileX * _tileset.TileWidth, tileY * _tileset.TileHeight);
            spriteBatch.Draw(region.Texture, position, sourceRectangle: region.Bounds, color: Color * Alpha);
        }
    }
}
