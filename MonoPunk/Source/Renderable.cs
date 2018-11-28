using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using System;

namespace MonoPunk
{
	public class Renderable : Component, IComparable<Renderable>
	{
		public const int LAYER_NOT_SET = int.MinValue;

		public Color Color { get; set; } = Color.White;
		public float Alpha { get; set; } = 1.0f;

		public bool Visible { get; set; } = true;
		public void Show()
		{
			Visible = true;
		}
		public void Hide()
		{
			Visible = false;
		}

		private int layer = LAYER_NOT_SET;
		public int Layer
		{
			get { return layer; }
			set { if (value == LAYER_NOT_SET) Engine.Throw("Invalid layer " + value + "."); layer = value; }
		}

		private int sortOrder = -1;
		public int SortOrder
		{
			get { return sortOrder; }
			set { sortOrder = value; if (renderManager != null) renderManager.SortOrderChanged(this); }
		}

		private RenderManager renderManager;

		public Renderable(float x = 0.0f, float y = 0.0f) : base(x, y)
		{
		}

		public Renderable(Vector2 pos) : this(pos.X, pos.Y)
		{
		}

		internal void _Init(RenderManager renderManager)
		{
			this.renderManager = renderManager;
		}

		internal void _Render(SpriteBatch spriteBatch)
		{
			if (Visible)
			{
				OnRender(spriteBatch);
				if (GetChildren() != null)
				{
					foreach (var child in GetChildren())
					{
						if (child is Renderable)
						{
							((Renderable)child)._Render(spriteBatch);
						}
					}
				}
			}
		}

		protected virtual void OnRender(SpriteBatch spriteBatch)
		{
		}

		public int CompareTo(Renderable other)
		{
			return SortOrder - other.SortOrder;
		}

		public RectangleF GetVisibleBounds()
		{
			return renderManager.GetVisibleBounds(Layer);
		}
	}
}
