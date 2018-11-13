using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using System;

namespace MonoPunk
{
	public class DrawLayer : Renderable
	{
		private readonly RenderTarget2D renderTarget;
		private readonly SpriteBatch spriteBatch;		

		public DrawLayer(int width, int height, Color initialColor)
		{
			renderTarget = new RenderTarget2D(Engine.Instance.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
			spriteBatch = new SpriteBatch(Engine.Instance.GraphicsDevice);

			Engine.SetRenderTarget(renderTarget);
			Engine.Instance.GraphicsDevice.Clear(initialColor);
			Engine.SetRenderTarget(null);
		}

		public DrawLayer(int width, int height) : this(width, height, Color.Transparent)
		{
		}

		public void BeginDraw()
		{
			Engine.SetRenderTarget(renderTarget);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
		}

		public void EndDraw()
		{
			spriteBatch.End();
			Engine.SetRenderTarget(null);
		}

		public void Clear(Color color)
		{
			Engine.Instance.GraphicsDevice.Clear(color);
		}

		public void Draw(Texture2D texture, float x, float y, float originX = 0.0f, float originY = 0.0f, float rotation = 0.0f, float scale = 1.0f, SpriteEffects effects = SpriteEffects.None)
		{
			Draw(texture, x, y, Color.White, originX, originY, rotation, scale, effects);
		}

		public void Draw(Texture2D texture, float x, float y, Color color, float originX = 0.0f, float originY = 0.0f, float rotation = 0.0f, float scale = 1.0f, SpriteEffects effects = SpriteEffects.None)
		{
			var source = new Rectangle(0, 0, texture.Width, texture.Height);
			spriteBatch.Draw(texture, new Vector2(x, y), source, color, rotation, new Vector2(originX, originY), scale, effects, 0.0f);
		}

		public void DrawRectangle(float x, float y, float width, float height, Color color)
		{
			spriteBatch.DrawRectangle(new RectangleF(x, y, width, height), color);
		}

		protected override void OnRender(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(renderTarget, GlobalPosition, Color * Alpha);
		}
	}
}
