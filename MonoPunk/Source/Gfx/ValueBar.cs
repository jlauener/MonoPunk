using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework;

namespace MonoPunk
{
    public class ValueBar : RectBar
    {
        private readonly Label _label;

        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                UpdateValue();
            }
        }

        private int _maxValue;
        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                UpdateValue();
            }
        }

        public Color LabelColor
        {
            get { return _label.Color; }
            set { _label.Color = value; }
        }

        private Vector2 _labelOffset;
        public Vector2 LabelOffset
        {
          get { return _labelOffset; }
            set
            {
                _labelOffset = value;
                _label.X = Width / 2 + _labelOffset.X;
                _label.Y = Height / 2 + _labelOffset.Y;
            }
        }

        public ValueBar(int width, int height, string font) : base(width, height)
        {
            _label = new Label(font);
            _label.HAlign = TextAlign.Center;
            _label.VAlign = TextAlign.Center;
            _label.X = Width / 2;
            _label.Y = Height / 2;
            Add(_label);
        }

        private void UpdateValue()
        {
            if(MaxValue == 0)
            {
                // prevent division by zero
                return;
            }

            _label.Text = Value + "/" + MaxValue;
            Percent = _value / (float)MaxValue;
        }
    }
}
