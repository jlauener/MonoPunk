using Microsoft.Xna.Framework;
using MonoPunk;
using MonoGame.Extended.BitmapFonts;
using System;

namespace MonoPunk_TestBed
{
    class NinePatchTest : TestScene
    {
        public NinePatchTest()
        {
            var ninePatch = new NinePatch("gfx/ninepatch", 4, 4, 7, 5);
            ninePatch.X = 10;
            ninePatch.Y = 10;
            Add(ninePatch);
        }
    }
}
