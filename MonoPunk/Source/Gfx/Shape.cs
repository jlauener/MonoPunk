using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using System;

namespace MonoPunk
{
    public enum ShapeMode
    {
        Draw,
        Fill
    }

    public class RectangleShape : Renderable
    {
        public ShapeMode Mode { get; set; } = ShapeMode.Fill;

        private Vector2 origin = Vector2.Zero;
        public int OriginX
        {
            get { return (int)origin.X; }
            set { origin.X = value; }
        }

        public int OriginY
        {
            get { return (int)origin.Y; }
            set { origin.Y = value; }
        }

        private Vector2 scale = Vector2.One;
        public float ScaleX
        {
            get { return scale.X; }
            set { scale.X = value; }
        }

        public float ScaleY
        {
            get { return scale.Y; }
            set { scale.Y = value; }
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public float Thickness { get; set; } = 1.0f;

        public RectangleShape(int width, int height, Color color) : this(width, height)
        {
            Color = color;
        }

        public RectangleShape(int width, int height)
        {
            Width = width;
            Height = height;
        }

        protected override void OnRender(SpriteBatch spriteBatch)
        {
            var rectF = new RectangleF(GlobalX - OriginX * ScaleX, GlobalY - OriginY * ScaleY, Width * ScaleX, Height * ScaleY);
            switch (Mode)
            {
                case ShapeMode.Draw:
                    spriteBatch.DrawRectangle(rectF, Color * Alpha, Thickness);
                    break;
                case ShapeMode.Fill:
                    spriteBatch.FillRectangle(rectF, Color * Alpha);
                    break;
            }
        }
    }

    public class CircleShape : Renderable
    {
        private Vector2 origin = Vector2.Zero;
        public int OriginX
        {
            get { return (int)origin.X; }
            set { origin.X = value; }
        }

        public int OriginY
        {
            get { return (int)origin.Y; }
            set { origin.Y = value; }
        }

        public float Radius { get; set; }
        public float Scale { get; set; } = 1.0f;
        public float Thickness { get; set; } = 1.0f;
        public int Sides { get; set; } = 32;

        public CircleShape(float radius, Color color) : this(radius)
        {
            Color = color;
        }

        public CircleShape(float radius)
        {
            Radius = radius;
        }

        protected override void OnRender(SpriteBatch spriteBatch)
        {
            var circleF = new CircleF((GlobalPosition - origin) * Scale, Radius * Scale);
            spriteBatch.DrawCircle(circleF, Sides, Color * Alpha, Thickness);
        }
    }

    public class LineShape : Renderable
    {
        public Vector2 PointA { get; set; }
        public Vector2 PointB { get; set; }

        public float Thickness { get; set; } = 1.0f;

        public LineShape(Vector2 pointA, Vector2 pointB, Color color)
        {
            PointA = pointA;
            PointB = pointB;
            Color = color;
        }

        public LineShape(Vector2 pointA, Vector2 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }

        public LineShape(float x1, float y1, float x2, float y2, Color color) : this(new Vector2(x1, y1), new Vector2(x2, y2), color)
        {
        }

        public LineShape(float x1, float y1, float x2, float y2) : this(new Vector2(x1, y1), new Vector2(x2, y2))
        {
        }

        protected override void OnRender(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(PointA, PointB, Color * Alpha, Thickness);
        }
    }
}
