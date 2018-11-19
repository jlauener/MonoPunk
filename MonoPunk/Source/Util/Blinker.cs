using System;

namespace MonoPunk
{
    public class Blinker : Component
    {
        private readonly float interval;
        private readonly Renderable[] renderables;

        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
				if (enabled != value)
				{
					enabled = value;
					if (enabled)
					{
						visible = false;
						counter = interval;
						UpdateRenderables();
					}
					else
					{
						visible = true;
						UpdateRenderables();
					}
				}
            }
        }

        private bool visible;
        private float counter;

        public Blinker(float interval, params Renderable[] renderables)
        {
            this.interval = interval;
            this.renderables = renderables;

			Enabled = true;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (Enabled)
            {
                counter -= deltaTime;
                if(counter <= 0.0f)
                {
                    visible = !visible;
                    UpdateRenderables();
                    counter += interval;
                }
            }
        }

        private void UpdateRenderables()
        {
            foreach(var renderable in renderables)
            {
                renderable.Visible = visible;
            }
        }
    }
}
