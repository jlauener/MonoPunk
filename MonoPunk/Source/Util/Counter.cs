using System;
using MonoPunk;

namespace MonoPunk
{
	public class Counter : Component
	{
		public bool Enabled { get; set; } = true;
		public float Delay { get; set; }

		private float intervalMin;
		private float intervalMax;
		private float intervalModifier;

		private float interval;
		private float counter;

		public event Action OnTrigger;

		public Counter(float interval)
		{		
			intervalMin = interval;
			intervalMax = interval;
			intervalModifier = interval;
			Reset();
		}

		public Counter(float intervalMin, float intervalMax, float intervalModifier)
		{
			this.intervalMin = intervalMin;
			this.intervalMax = intervalMax;
			this.intervalModifier = intervalModifier;
			Reset();
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);

			if(!Enabled)
			{
				return;
			}

			if(Delay > 0.0f)
			{
				Delay -= deltaTime;
				return;
			}

			counter += deltaTime;
			while(counter >= interval)
			{
				OnTrigger?.Invoke();				
				counter -= interval;
				interval *= intervalModifier;
				interval = Mathf.Clamp(interval, intervalMin, intervalMax);
			}
		}

		public void Reset()
		{
			interval = intervalMin;
		}
	}
}
