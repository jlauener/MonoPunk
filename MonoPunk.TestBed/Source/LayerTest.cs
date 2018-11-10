using Microsoft.Xna.Framework.Graphics;
using MonoPunk;

namespace MonoPunk_TestBed
{
    class LayerTest : TestScene
    {
        private static readonly int BALL_COUNT_PER_LAYER = 50;

        public LayerTest()
        {
            for(var i = 0; i < BALL_COUNT_PER_LAYER; ++i)
            {
                createBall(0, "gfx/ball_red");
            }

            for (var i = 0; i < BALL_COUNT_PER_LAYER; ++i)
            {
                createBall(1, "gfx/ball_green");
            }

            for (var i = 0; i < BALL_COUNT_PER_LAYER; ++i)
            {
                createBall(2, "gfx/ball_blue");
            }
        }

        private void createBall(int layer, string spriteName)
        {
            var ball = new Ball(Asset.LoadTexture(spriteName));
            ball.Layer = layer;
            ball.Collidable = false;
            ball.X = Rand.NextFloat(Engine.Width - ball.Width);
            ball.Y = Rand.NextFloat(Engine.Height - ball.Height);
            ball.SetRandomDirection();
            ball.MoveSpeed = Rand.NextFloat(20.0f, 40.0f);
            Add(ball);
        }
    }
}
