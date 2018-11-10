using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;

namespace MonoPunk
{
	public enum TextAlign
	{
		Left,
		Right,
		Top,
		Bottom,
		Center
	}

    public class Label : Renderable
    {     
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if(_text != value)
                {
                    _text = value;
                    _textSizeDirty = true;
                    _offsetDirty = true;
                }
           }
        }

        private BitmapFont _font;
        public BitmapFont Font
        {
            get { return _font; }
            set
            {
                if(_font != value)
                {
                    _font = value;
                    _textSizeDirty = true;
                    _offsetDirty = true;
                }
            }
        }

        private TextAlign _hAlign = TextAlign.Left;
        public TextAlign HAlign
        {
            get { return _hAlign; }
            set
            {
                if(_hAlign != value)
                {
                    _hAlign = value;
                    _offsetDirty = true;
                }
            }
        }

        private TextAlign _vAlign = TextAlign.Top;
        public TextAlign VAlign
        {
            get { return _vAlign; }
            set
            {
                if(_vAlign != value)
                {
                    _vAlign = value;
                    _offsetDirty = true;
                }
            }
        }

		public void SetAlign(TextAlign hAlign, TextAlign vAlign)
		{
			HAlign = hAlign;
			VAlign = vAlign;
		}

		public void Center()
		{
			HAlign = TextAlign.Center;
			VAlign = TextAlign.Center;
		}

        public int Width
        {
            get
            {
                RebuildIfNeeded();
                return Mathf.Ceiling(_unscaledSize.Width * Scale);
            }
        }

        public int Height
        {
            get
            {
                RebuildIfNeeded();
                return Mathf.Ceiling(_unscaledSize.Height * Scale);
            }
        }

        public float Rotation { get; set; } = 0.0f;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        private Vector2 _origin = Vector2.Zero;

        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        public int OriginX
        {
            get { return (int)_origin.X; }
            set { _origin.X = value; }
        }

        public int OriginY
        {
            get { return (int)_origin.Y; }
            set { _origin.Y = value; }
        }

        private float _scale = 1.0f;
        public float Scale
        {
            get { return _scale; }
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    _offsetDirty = true;
                }
            }
        }

        private bool _textSizeDirty = true;
        private bool _offsetDirty = true;
        private Vector2 _textOffset = Vector2.Zero;
        private Size _unscaledSize;

        public Label(BitmapFont font, Color color, string text = "")
        {
            _font = font;
            _text = text;
            Color = color;
            RebuildIfNeeded();
        }

        public Label(string fontName, Color color, string text = "") : this(Asset.LoadFont(fontName), color, text)
        {
        }

        public Label(BitmapFont font, string text = "") : this(font, Color.White, text)
        {
        }

        public Label(string fontName, string text = "") : this(Asset.LoadFont(fontName), text)
        {
        }

        private void RebuildIfNeeded()
        {
            if (_textSizeDirty)
            {
                _unscaledSize = Font.MeasureString(_text);
                _textSizeDirty = false;
            }

            if (_offsetDirty)
            {
                switch (HAlign)
                {
                    case TextAlign.Left:
                        {
                            _textOffset.X = 0.0f;
                            break;
                        }
                    case TextAlign.Center:
                        {
                            _textOffset.X = -_unscaledSize.Width * Scale / 2.0f;
                            break;
                        }
                    case TextAlign.Right:
                        {
                            _textOffset.X = -_unscaledSize.Width * Scale;
                            break;
                        }
                }
                switch (VAlign)
                {
                    case TextAlign.Top:
                        {
                            _textOffset.Y = 0.0f;
                            break;
                        }
                    case TextAlign.Center:
                        {
                            _textOffset.Y = -_unscaledSize.Height * Scale / 2.0f;
                            break;
                        }
                    case TextAlign.Bottom:
                        {
                            _textOffset.Y = -_unscaledSize.Height * Scale;
                            break;
                        }
                }
                _offsetDirty = false;
            }
        }

        protected override void OnRender(SpriteBatch spriteBatch)
        {
            RebuildIfNeeded();
            spriteBatch.DrawString(_font, _text, GlobalPosition + _textOffset, Color * Alpha, Rotation, _origin, Scale, Effects, 0.0f);
        }
    }
}
