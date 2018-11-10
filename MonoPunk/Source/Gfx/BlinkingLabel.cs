using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace MonoPunk
{
    public class BlinkingLabel : Label
    {
        private readonly float _interval;
        private float _counter;

        public BlinkingLabel(BitmapFont font, float interval, Color color, string text = "") : base(font, color, text)
        {
            _interval = interval;
        }

        public BlinkingLabel(string fontName, float interval, Color color, string text = "") : this(Asset.LoadFont(fontName), interval, color, text)
        {
        }

        public BlinkingLabel(BitmapFont font, float interval, string text = "") : this(font, interval, Color.White, text)
        {
        }

        public BlinkingLabel(string fontName, float interval, string text = "") : this(Asset.LoadFont(fontName), interval, text)
        {
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            _counter -= deltaTime;
            if(_counter <= 0.0f)
            {
                Visible = !Visible;
                _counter += _interval;
            }
           
        }
    }
}
