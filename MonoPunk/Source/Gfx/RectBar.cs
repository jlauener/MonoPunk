using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework;

namespace MonoPunk
{
    public class RectBar : Renderable
    {
        private float percent;
        public float Percent
        {
            get { return percent; }
            set
            {
                percent = value;
                front.Width = Mathf.Floor(percent * Width);
            }
        }
        public bool Inverse { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly RectangleShape outline;
        public Color OutlineColor
        {
            get { return outline.Color; }
            set { outline.Color = value; }
        }
        private int outlineWIdth;
        public int OutlineWidth
        {
            get { return outlineWIdth; }
            set
            {
                outlineWIdth = value;
                outline.Width = Width + outlineWIdth * 2;
                outline.Height = Height + outlineWIdth * 2;
                outline.X = -outlineWIdth;
                outline.Y = -outlineWIdth;
                outline.Visible = outlineWIdth > 0;
            }
        }

        private readonly RectangleShape back;
        public Color BackColor
        {
            get { return back.Color; }
            set { back.Color = value; }
        }
        public bool BackVisible
        {
            get { return back.Visible; }
            set { back.Visible = value; }
        }

        private readonly RectangleShape front;
        public Color FrontColor
        {
            get { return front.Color; }
            set { front.Color = value; }
        }
        public bool FrontVisible
        {
            get { return front.Visible; }
            set { front.Visible = value; }
        }

        public RectBar(int width, int height)
        {
            Width = width;
            Height = height;

            outline = new RectangleShape(0, 0);
            outline.Visible = false;
            Add(outline);

            back = new RectangleShape(Width, Height);
            Add(back);

            front = new RectangleShape(0, Height);
            Add(front);
        }
    }
}
