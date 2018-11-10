using Microsoft.Xna.Framework;
using MonoPunk;
using MonoGame.Extended.BitmapFonts;
using System;

namespace MonoPunk_TestBed
{
    class LabelTest : TestScene
    {
        public LabelTest()
        {
            var rect = new RectangleShape(200, 80, Color.DarkGray);
            rect.X = 10.0f;
            rect.Y = 20.0f;
            Add(rect);

            Add(new LineShape(rect.X + rect.Width / 2, rect.Y, rect.X + rect.Width / 2, rect.Y + rect.Height, Color.Red));

            var font = Asset.LoadFont("font/04b03");

            var labelLeft = new Label(font, "Left label");
            labelLeft.HAlign = TextAlign.Left;
            labelLeft.X = rect.X;
            labelLeft.Y = rect.Y + 10.0f;
            Add(labelLeft);

            var labelCenter = new Label(font, "Center label");
            labelCenter.HAlign = TextAlign.Center;
            labelCenter.X = rect.X + rect.Width / 2;
            labelCenter.Y = rect.Y + 10.0f;
            Add(labelCenter);

            var labelRight = new Label(font, "Right label");
            labelRight.HAlign = TextAlign.Right;
            labelRight.X = rect.X + rect.Width;
            labelRight.Y = rect.Y + 10.0f;
            Add(labelRight);
        }
    }
}
