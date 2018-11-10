using Microsoft.Xna.Framework;
using System;

namespace MicroPunk
{
    public class ProtoSprite : RectangleShape
    {
        private readonly Label label;

        public ProtoSprite(Color color) : base(0, 0, color)
        {
            label = new Label(Engine.DebugFont);
            label.Color = Color.Black;
            Add(label);
        }

        public ProtoSprite() : this(Color.White)
        {
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            Width = Entity.Width;
            Height = Entity.Height;
            label.Text = Entity.Name;
        }
    }
}
