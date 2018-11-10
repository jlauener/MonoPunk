using MonoPunk;
using System;

namespace MonoPunk
{
    public class Button : Entity
    {
        public bool Enabled { get; set; } = true;
        public Action OnPressed { get; set; }

        public Button(float x = 0.0f, float y = 0.0f) : base(x, y)
        {        
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if(Enabled && Input.WasMousePressed(MouseButton.Left) && CollidePoint(Input.MouseX, Input.MouseY) != HitInfo.None)
            {
                OnPressed?.Invoke();
            }
        }
    }
}
