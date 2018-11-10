using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoPunk
{
    public class Backdrop : Renderable
    {
        public float ScrollX { get; set; }
        public float ScrollY { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly TextureRegion2D _region;

        private readonly int _repeatX;
        private readonly int _repeatY;
        private readonly Vector2 _baseOffset;
        private readonly Point _clipSize;
        private Vector2 _scrollOffset = Vector2.Zero;

        public Backdrop(TextureRegion2D region, int width, int height)
        {
            _region = region;
            Width = width;
            Height = height;
            _baseOffset = new Vector2(-_region.Width, -_region.Height);
            _clipSize = new Point(width, height);
            _repeatX = Width / _region.Width + 2;
            _repeatY = Height / _region.Height + 3;
        }

        public Backdrop(Texture2D texture, int width, int height) : this(new TextureRegion2D(texture), width, height)
        {
        }

        public Backdrop(string textureName, int width, int height) : this(Asset.LoadTexture(textureName), width, height)
        {
        }

        public void ResetOffsets()
        {
            _scrollOffset = Vector2.Zero;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            _scrollOffset.X += ScrollX * deltaTime;
            if(_scrollOffset.X > _region.Width)
            {
                _scrollOffset.X -= _region.Width;
            }
            else if(_scrollOffset.X < -_region.Width)
            {
                _scrollOffset.X += _region.Width;
            }

            _scrollOffset.Y += ScrollY * deltaTime;
            if(_scrollOffset.Y > _region.Height)
            {
                _scrollOffset.Y -= _region.Height;
            }
            else if(_scrollOffset.Y < -_region.Height)
            {
                _scrollOffset.Y += _region.Height;
            }
        }

        protected override void OnRender(SpriteBatch spriteBatch)
        {
            for(var ix = 0; ix < _repeatX; ix++)
            {
                for(var iy = 0; iy < _repeatY; iy++)
                {
                    var position = GlobalPosition + _baseOffset + _scrollOffset;
                    position.X += ix * _region.Width;
                    position.Y += iy * _region.Height;
                    spriteBatch.Draw(_region.Texture, position, _region.Bounds, color: Color * Alpha);
                }
            }
        }
    }
}
