using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework;

namespace MonoPunk
{
    public class RectBar : Renderable
    {
        private float _percent;
        public float Percent
        {
            get { return _percent; }
            set
            {
                _percent = value;
                _front.Width = Mathf.Floor(_percent * Width);
            }
        }
        public bool Inverse { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly RectangleShape _outline;
        public Color OutlineColor
        {
            get { return _outline.Color; }
            set { _outline.Color = value; }
        }
        private int _outlineWidth;
        public int OutlineWidth
        {
            get { return _outlineWidth; }
            set
            {
                _outlineWidth = value;
                _outline.Width = Width + _outlineWidth * 2;
                _outline.Height = Height + _outlineWidth * 2;
                _outline.X = -_outlineWidth;
                _outline.Y = -_outlineWidth;
                _outline.Visible = _outlineWidth > 0;
            }
        }

        private readonly RectangleShape _back;
        public Color BackColor
        {
            get { return _back.Color; }
            set { _back.Color = value; }
        }
        public bool BackVisible
        {
            get { return _back.Visible; }
            set { _back.Visible = value; }
        }

        private readonly RectangleShape _front;
        public Color FrontColor
        {
            get { return _front.Color; }
            set { _front.Color = value; }
        }
        public bool FrontVisible
        {
            get { return _front.Visible; }
            set { _front.Visible = value; }
        }

        public RectBar(int width, int height)
        {
            Width = width;
            Height = height;

            _outline = new RectangleShape(0, 0);
            _outline.Visible = false;
            Add(_outline);

            _back = new RectangleShape(Width, Height);
            Add(_back);

            _front = new RectangleShape(0, Height);
            Add(_front);
        }
    }
}
