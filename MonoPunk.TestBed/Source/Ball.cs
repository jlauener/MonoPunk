using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoPunk;
using System;

namespace MonoPunk_TestBed
{
    class Ball : Entity
    {
        public float MoveSpeed = 30.0f;
        public Vector2 Direction = new Vector2(1.0f, 0.0f);
		private readonly int[] solidType;

        public Ball(Texture2D spriteTexture, params int[] solidType) : base()
        {
            Name = "ball";
			this.solidType = solidType;

            var sprite = new Sprite(spriteTexture);            
            Add(sprite);
            Width = sprite.Width;
            Height = sprite.Height;
        }

        public void SetRandomDirection()
        {
            var radians = Rand.NextFloat() * 2.0f * (float)Math.PI;
            Direction.X = (float)Math.Cos(radians);
            Direction.Y = (float)Math.Sin(radians);
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            var velocity = Direction * MoveSpeed * deltaTime;
            MoveBy(velocity.X, velocity.Y, solidType);

            if (X < 0.0f)
            {
                Direction.X = -Direction.X;
            }
            else if (X > Engine.Width - Width)
            {
                Direction.X = -Direction.X;
            }

            if (Y < 0.0f)
            {
                Direction.Y = -Direction.Y;
            }
            else if (Y > Engine.Height - Height)
            {
                Direction.Y = -Direction.Y;
            }
        }

        protected override bool OnHit(HitInfo info)
        {
            info.OnHorizontalMovement(() => Direction.X = -Direction.X);
            info.OnVerticalMovement(() => Direction.Y = -Direction.Y);
            return true;
        }        
    }
}
