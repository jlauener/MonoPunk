using System;
using MonoPunk;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoPunk
{
    public class Shaker : Entity
    {
		private Camera2D camera;
		private Vector2 originalCameraPosition;

        private List<Event> events = new List<Event>();

		public Shaker(Camera2D camera)
		{
			this.camera = camera;
			originalCameraPosition = camera.Position;
		}

        public void Shake(float strenghtX, float strenghtY, float decay = 0.8f)
        {
            events.Add(new ShakeEvent(Mathf.Abs(strenghtX), Mathf.Abs(strenghtY), decay));
        }

        public void Shake(Vector2 strenght, float decay = 0.8f)
        {
            Shake(strenght.X, strenght.Y, decay);
        }

        public void Bounce(float offsetX, float offsetY, float duration = 0.8f)
        {
            Bounce(new Vector2(offsetX, offsetY), duration);
        }

        public void Bounce(Vector2 offset, float duration)
        {
            events.Add(new BounceEvent(offset, duration));
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

			var offset = originalCameraPosition;
            for(var i = events.Count - 1; i >= 0; i--)
            {
                if(events[i].Apply(deltaTime, ref offset))
                {
                    events.RemoveAt(i);
                }
            }

            var camera = this.camera != null ? this.camera : Scene.Camera;
            camera.Position = offset;
        }

        private interface Event
        {
            bool Apply(float deltaTime, ref Vector2 offset);
        }

        private class ShakeEvent : Event
        {
            private float _strenghtX;
            private float _strenghtY;
            private readonly float _decay;

            public ShakeEvent(float strenghtX, float strenghtY, float decay)
            {
                _strenghtX = strenghtX;
                _strenghtY = strenghtY;
                _decay = decay;
            }

            public bool Apply(float deltaTime, ref Vector2 offset)
            {
                if (_strenghtX > 0.0f)
                {
                    offset.X += Rand.NextFloat(-_strenghtX, _strenghtX);
                    _strenghtX *= _decay;
                    _strenghtX = Mathf.Max(_strenghtX, 0.0f);
                }

                if (_strenghtY > 0.0f)
                {
                    offset.Y += Rand.NextFloat(-_strenghtY, _strenghtY);
                    _strenghtY *= _decay;
                    _strenghtY = Mathf.Max(_strenghtY, 0.0f);
                }

                return _strenghtX < Mathf.Epsilon && _strenghtY < Mathf.Epsilon;
            }
        }

        private class BounceEvent : Event
        {
            private Vector2 _offset;
            private float _counter;

            public BounceEvent(Vector2 offset, float duration)
            {
                _offset = offset;
                _counter = duration;
            }

            public bool Apply(float deltaTime, ref Vector2 offset)
            {
                offset += _offset;
                _counter -= deltaTime;
                return _counter <= 0.0f;
            }
        }
    }
}
