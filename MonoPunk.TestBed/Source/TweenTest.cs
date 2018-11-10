using MonoPunk;
using System;

namespace MonoPunk_TestBed
{
    class TweenTest : TestScene
    {
        protected override void OnBegin()
        {
            base.OnBegin();            

            var sprite = new Sprite("gfx/ball_red");            
            Add(sprite);
            Tween(sprite, new { X = 30.0f, Y = 80.0f }, 1.0f, 2.0f).Ease(Ease.QuadOut);
        }
    }    
}
