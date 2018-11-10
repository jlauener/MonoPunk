using System;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapLayer : IDisposable
    {
        public string Name { get; }
        public TiledMapProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float OffsetX { get; }
        public float OffsetY { get; }

        internal TiledMapLayer(ContentReader input)
        {
            Name = input.ReadString();
            Properties = new TiledMapProperties();
            IsVisible = input.ReadBoolean();
            Opacity = input.ReadSingle();
            OffsetX = input.ReadSingle();
            OffsetY = input.ReadSingle();

            input.ReadTiledMapProperties(Properties);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool diposing)
        {
            if (!diposing)
                return;
        }
    }
}