using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MonoPunk
{
	public class Component : Node
	{
		public Entity Entity { get; private set; }

		private bool added;

		public Scene Scene
		{
			get { return Entity.Scene; }
		}

		public Component(float x = 0.0f, float y = 0.0f) : base(x, y)
		{
		}

		public Component(Vector2 pos) : this(pos.X, pos.Y)
		{
		}

		internal void _Init(Node parent, Entity entity)
		{
			Parent = parent;
			Entity = entity;
			if (children != null)
			{
				foreach (var child in children)
				{
					child._Init(this, Entity);
				}
			}

			if (entity.Scene != null)
			{
				OnAdded();
				added = true;
			}
		}

		internal void _CallOnAddedIfNeeded()
		{
			if (!added)
			{
				OnAdded();
				added = true;
			}

			if (children != null)
			{
				foreach (var child in children)
				{
					child._CallOnAddedIfNeeded();
				}
			}
		}

		internal void _Destroy()
		{
			OnRemoved();
			if (children != null)
			{
				foreach (Component child in children)
				{
					child._Destroy();
				}
				children.Clear();
			}
		}

		internal void _Update(float deltaTime)
		{
			OnUpdate(deltaTime);
			if (children != null)
			{
				foreach (Component child in children)
				{
					child._Update(deltaTime);
				}
			}
		}

		protected virtual void OnAdded()
		{
		}

		protected virtual void OnRemoved()
		{
		}

		protected virtual void OnUpdate(float deltaTime)
		{
		}

		private List<Component> children;

		public void Add(Component child)
		{
			if (Entity != null)
			{
				child._Init(this, Entity);
			}

			if (children == null)
			{
				children = new List<Component>();
			}
			children.Add(child);
		}

		public void Remove(Component child)
		{
			child._Destroy();
			children.Remove(child);
		}

		public List<Component> GetChildren()
		{
			return children;
		}

		public T Get<T>() where T : Component
		{
			foreach (var child in children)
			{
				if (child is T)
				{
					return (T)child;
				}
			}

			return null;
		}

		public void ForEach<T>(Action<T> action) where T : Component
		{
			foreach (var child in children)
			{
				if (child is T)
				{
					action((T)child);
				}
			}
		}

		public void RemoveFromParent()
		{
			Entity.Remove(this);
		}
	}
}
