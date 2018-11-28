using System;
using MonoPunk;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoPunk
{
    public class Shaker : Entity
    {
		public bool Enabled { get; set; } = true;

		//private Camera2D camera;
		//public Vector2 Origin { get; set; }
		public Vector2 Offset { get; private set; }

        private List<Event> events = new List<Event>();

		public Shaker(Camera2D camera = null)
		{
			//this.camera = camera;
			//Origin = camera.Position;
		}

		public void StopAll()
		{
			events.Clear();
		}

		//public void ResetCameraOrigin()
		//{
		//	Origin = camera.Position;
		//}

        public void Shake(float strenghtX, float strenghtY, float decay = 0.8f)
        {
			if (Enabled)
			{
				events.Add(new ShakeEvent(Mathf.Abs(strenghtX), Mathf.Abs(strenghtY), decay));
			}
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
			if (Enabled)
			{
				events.Add(new BounceEvent(offset, duration));
			}
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
			if (!Enabled) return;

			var offset = Vector2.Zero;
            for(var i = events.Count - 1; i >= 0; i--)
            {
                if(events[i].Apply(deltaTime, ref offset))
                {
                    events.RemoveAt(i);
                }
            }
			Offset = offset;

          //  var camera = this.camera != null ? this.camera : Scene.Camera;
          //  camera.Position = offset;
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
