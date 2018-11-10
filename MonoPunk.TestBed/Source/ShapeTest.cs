using Microsoft.Xna.Framework;
using MonoPunk;
using MonoGame.Extended.BitmapFonts;
using System;

namespace MonoPunk_TestBed
{
    class ShapeTest : TestScene
    {
        public ShapeTest()
        {
            var rect = new RectangleShape(200, 80, Color.DarkGray);
            rect.X = 10.0f;
            rect.Y = 20.0f;
            Add(rect);

            rect = new RectangleShape(50, 50, Color.Blue);
            rect.Mode = ShapeMode.Draw;
            rect.X = 30.0f;
            rect.Y = 10.0f;
            Add(rect);

            rect = new RectangleShape(20, 50, Color.Blue);
            rect.Mode = ShapeMode.Draw;
            rect.Thickness = 4.0f;
            rect.X = 70.0f;
            rect.Y = 50.0f;
            Add(rect);

            Add(new LineShape(5.0f, 5.0f, 50.0f, 40.0f, Color.Red));          
        }
    }
}
