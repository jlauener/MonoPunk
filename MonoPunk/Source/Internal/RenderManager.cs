using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;

namespace MonoPunk
{
    public class RenderManager
    {
		private readonly Camera2D sceneCamera;
        private readonly SortedDictionary<int, Layer> _layers;

        private class MyComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return y - x;
            }
        }

        public RenderManager(Camera2D sceneCamera)
        {
			this.sceneCamera = sceneCamera;
            _layers = new SortedDictionary<int, Layer>(new MyComparer());
        }

        public void AddRenderable(Renderable renderable)
        {
            var layer = GetOrCreateLayer(renderable.Layer);            
            layer.AddRenderable(renderable);
            renderable._Init(this);
        }

        public void RemoveRenderable(Renderable renderable)
        {
            Layer layer;
            if(!_layers.TryGetValue(renderable.Layer, out layer))
            {
                throw new Exception("Layer with ID " + renderable.Layer + " not found.");
            }
            layer.RemoveRenderable(renderable);
        }

        public void SetCamera(int layerId, Camera2D camera)
        {
            var layer = GetOrCreateLayer(layerId);
            layer.CustomCamera = camera == null ? new Camera2D(Engine.Instance.GraphicsDevice) : camera;
            layer.UseCustomCamera = true;
        }

        public void SetVisible(int layerId, bool visible)
        {
            var layer = GetOrCreateLayer(layerId);
            layer.Visible = visible;
        }

        public void SetScissor(int layerId, int x, int y, int width, int height)
        {
            var layer = GetOrCreateLayer(layerId);

            layer.RasterizerState = new RasterizerState();
            layer.RasterizerState.ScissorTestEnable = true;
            layer.ScissorRectangle = new Rectangle(new Point(x, y), new Point(width, height));
        }

        public void Render(SpriteBatch spriteBatch, Camera2D camera)
        {            
            foreach(var layer in _layers.Values)
            {
                if(!layer.Visible)
                {
                    continue;
                }

                layer.BeginRender(spriteBatch, camera);
                layer.Render(spriteBatch);
                layer.EndRender(spriteBatch);
            }
        }

		public RectangleF GetVisibleBounds(int layerId)
		{
			Layer layer;
			if (!_layers.TryGetValue(layerId, out layer))
			{
				return RectangleF.Empty;
			}

			var camera = layer.UseCustomCamera ? layer.CustomCamera : sceneCamera;
			return camera.BoundingRectangle;
		}

        private Layer GetOrCreateLayer(int layerId)
        {
            Layer layer;
            if(!_layers.TryGetValue(layerId, out layer))
            {
                layer = new Layer();
                _layers[layerId] = layer;
            }
            return layer;
        }

        internal void SortOrderChanged(Renderable renderable)
        {
            Layer layer;
            if(_layers.TryGetValue(renderable.Layer, out layer))
            {
                layer.SortOrderChanged(renderable);
            }
        }

        private class Layer
        {
            public Camera2D CustomCamera { get; set; }
            public bool UseCustomCamera { get; set; }
            public bool Visible { get; set; } = true;
            public RasterizerState RasterizerState { get; set; }
            public Rectangle ScissorRectangle { get; set; }

            private readonly List<Renderable> _renderables = new List<Renderable>();
            private bool needToSort = false;

            public void BeginRender(SpriteBatch spriteBatch, Camera2D camera)
            {
                Camera2D layerCamera = UseCustomCamera ? CustomCamera : camera;
                spriteBatch.GraphicsDevice.ScissorRectangle = ScissorRectangle;    
                spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: layerCamera.GetViewMatrix(), rasterizerState: RasterizerState);
            }

            public void Render(SpriteBatch spriteBatch)
            {                
                if(needToSort)
                {
                    _renderables.Sort();
                }
                needToSort = false;

                foreach(var renderable in _renderables)
                {
                    renderable._Render(spriteBatch);
                }
            }

            public void EndRender(SpriteBatch spriteBatch)
            {
                spriteBatch.End();
            }

            public void SortOrderChanged(Renderable renderable)
            {
                needToSort = true;
            }

            public void AddRenderable(Renderable renderable)
            {
                _renderables.Add(renderable);
                if (renderable.SortOrder != -1)
                {
                    needToSort = true;
                }
            }

            public void RemoveRenderable(Renderable renderable)
            {
                _renderables.Remove(renderable);
            }
        }
    }
}
