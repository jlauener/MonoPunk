using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;

namespace MonoPunk
{
	public class Entity : Node
	{
		public int Type { get; set; } = -1;
		public string Name { get; set; } = "";
		public int Layer { get; set; } = -1;

		public Vector2 CenterPoint
		{
			get
			{
				return new Vector2(X - OriginX + Width / 2, Y - OriginY + Height / 2);
			}
		}

		public Scene Scene { get; private set; }
		private RenderManager renderManager;

		private bool removed = false;

		public Entity(float x = 0.0f, float y = 0.0f) : base(x, y)
		{
		}

		public Entity(Vector2 pos) : this(pos.X, pos.Y)
		{
		}

		public static Entity CreateBox(int x, int y, int width, int height, int type = -1)
		{
			var box = new Entity(x, y);
			box.Type = type;
			box.SetHitbox(width, height);
			return box;
		}

		public void SetHitbox(int width, int height, int originX = 0, int originY = 0)
		{
			Width = width;
			Height = height;
			OriginX = originX;
			OriginY = originY;
		}

		public void SetCenteredHitbox(int width, int height)
		{
			SetHitbox(width, height, width / 2, height / 2);
		}

		public void RemoveFromScene()
		{
			Scene.Remove(this);
		}

		#region Internal

		internal void _Init(Scene scene, RenderManager renderManager)
		{
			Scene = scene;
			this.renderManager = renderManager;
			OnAdded();
		}

		internal void _Remove()
		{
			Collidable = false;
			removed = true;
			OnRemoved();
		}

		internal void _Cleanup()
		{
			foreach (Component component in components.GetList())
			{
				if (component is Renderable)
				{
					renderManager.RemoveRenderable((Renderable)component);
				}
				component._Destroy();
			}
		}

		internal void _Update(float deltaTime)
		{
			if (removed /* || Paused*/)
			{
				return;
			}

			OnUpdate(deltaTime);
		}

		internal void _RenderDebug(SpriteBatch spriteBatch)
		{
			OnRenderDebug(spriteBatch);
		}

		#endregion

		#region Callback

		protected virtual void OnAdded()
		{
		}

		protected virtual void OnRemoved()
		{
		}

		protected virtual void OnRenderDebug(SpriteBatch spriteBatch)
		{
			if (Collider != null) Collider.RenderDebug(X - OriginX, Y - OriginY, spriteBatch);
			spriteBatch.DrawRectangle(new RectangleF(X - OriginX, Y - OriginY, Width, Height), Color.Blue);
			spriteBatch.DrawPoint(Position, Color.Red, 2.0f);
		}

		protected virtual void OnUpdate(float deltaTime)
		{
			UpdateComponents(deltaTime);
		}

		#endregion

		#region Component

		private readonly SmartList<Component> components = new SmartList<Component>();

		public void Add(Component component)
		{
			component._Init(this, this);
			components.Add(component);
		}

		public void Add(Component component, float x, float y)
		{
			component.X = x;
			component.Y = y;
			Add(component);
		}

		public void Add(Component component, Vector2 pos)
		{
			component.Position = pos;
			Add(component);
		}

		public void Remove(Component component)
		{
			component._Destroy();
			components.Remove(component);
		}

		public void Remove<T>() where T : Component
		{
			var component = Get<T>();
			if (component != null)
			{
				Remove(component);
			}
		}

		public List<Component> GetComponents()
		{
			return components.GetList();
		}

		public void ForEach<T>(Action<T> action) where T : Component
		{
			foreach (var component in GetComponents())
			{
				if (component is T)
				{
					action((T)component);
				}
			}
		}

		public T Get<T>() where T : Component
		{
			foreach (var component in GetComponents())
			{
				if (component is T)
				{
					return (T)component;
				}
			}

			return null;
		}

		private void UpdateComponents(float deltaTime)
		{
			components.FlushAddList(FlushAddComponent);
			foreach (Component component in components.GetList())
			{
				component._Update(deltaTime);
			}

			components.FlushRemoveList(FlushRemoveComponent);
		}

		private void FlushAddComponent(Component component)
		{
			component._CallOnAddedIfNeeded();
			if (component is Renderable)
			{
				Renderable renderable = (Renderable)component;
				if (renderable.Layer == Renderable.LAYER_NOT_SET)
				{
					renderable.Layer = Layer;
				}
				renderManager.AddRenderable(renderable);
			}
		}

		private void FlushRemoveComponent(Component component)
		{
			if (component is Renderable)
			{
				renderManager.RemoveRenderable((Renderable)component);
			}
		}

		#endregion

		#region Collision

		private Rectangle hitbox = new Rectangle();
		public int Width
		{
			get { return hitbox.Width; }
			set { hitbox.Width = value; }
		}

		public int Height
		{
			get { return hitbox.Height; }
			set { hitbox.Height = value; }
		}

		public int OriginX
		{
			get { return -hitbox.X; }
			set { hitbox.X = -value; }
		}

		public int OriginY
		{
			get { return -hitbox.Y; }
			set { hitbox.Y = -value; }
		}

		public float Left
		{
			get { return X - OriginX; }
		}

		public float Right
		{
			get { return X - OriginX + Width; }
		}

		public float Top
		{
			get { return Y - OriginY; }
		}

		public float Bottom
		{
			get { return Y - OriginY + Height; }
		}

		private bool collidable = true;
		public bool Collidable
		{
			get { return !removed && collidable; }
			set { collidable = value; }
		}

		private ICollider collider;
		public ICollider Collider
		{
			get { return collider; }
			set
			{
				collider = value;
				if (collider != null)
				{
					Width = collider.WidthPx;
					Height = collider.HeightPx;
				}
			}
		}

		public HitInfo CollideAt(float entityX, float entityY, params int[] types)
		{
			var result = HitInfo.None;
			CollideAt(entityX, entityY, types, (info) =>
			{
				result = info;
				return true;
			});
			return result;
		}

		public HitInfo CollideAt(float entityX, float entityY, int type)
		{
			var result = HitInfo.None;
			CollideAt(entityX, entityY, type, (info) =>
			{
				result = info;
				return true;
			});
			return result;
		}

		public bool CollideAt(float entityX, float entityY, int[] types, Func<HitInfo, bool> callback)
		{
			if (!Collidable)
			{
				return true;
			}

			foreach (var type in types)
			{
				if (CollideWithTypeAt(entityX, entityY, type, callback))
				{
					return true;
				}
			}

			return false;
		}

		public bool CollideAt(float entityX, float entityY, int type, Func<HitInfo, bool> callback)
		{
			return CollideWithTypeAt(entityX, entityY, type, callback);
		}

		private bool CollideWithTypeAt(float entityX, float entityY, int type, Func<HitInfo, bool> callback)
		{
			if (!Collidable)
			{
				return true;
			}

			List<Entity> entities = Scene.GetEntitiesByType(type);
			if (entities == null)
			{
				return false;
			}

			foreach (Entity other in entities)
			{

				if (!other.Collidable || other == this)
				{
					continue;
				}

				var info = CollideWithOtherAt(entityX, entityY, other);
				if (info != HitInfo.None)
				{
					if (callback(info))
					{
						return true;
					}
				}

				if (!Collidable)
				{
					return true;
				}
			}

			return false;
		}

		public HitInfo CollideWithOtherAt(float entityX, float entityY, Entity other)
		{
			entityX -= OriginX;
			entityY -= OriginY;

			if (!Mathf.IntersectRect(entityX, entityY, Width, Height, other.Left, other.Top, other.Width, other.Height))
			{
				return HitInfo.None;
			}

			if (Collider != null) return Collider.CollideWithOther(entityX, entityY, other.Left, other.Top, other);

			if (other.Collider != null)
			{
				var hit = other.Collider.CollideWithOther(other.Left, other.Top, entityX, entityY, this);
				if (hit) hit.Other = other;
				return hit;
			}

			return HitInfo.CreateHit(other);
		}

		public HitInfo CollideRect(float x, float y, int width, int height)
		{
			if(!Mathf.IntersectRect(Left, Top, Width, Height, x, y, width, height))
			{
				return HitInfo.None;
			}

			if (Collider != null) return Collider.CollideRect(Left, Top, x, y, width, height);

			return HitInfo.CreateHit();
		}

		public bool CollideRect(Rectangle rect)
		{
			return CollideRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public bool CollidePoint(float x, float y)
		{
			return CollideRect(x, y, 1, 1);
		}

		public bool CollidePoint(Vector2 point)
		{
			return CollidePoint(point.X, point.Y);
		}

		public bool InsideRect(float x, float y, int width, int height)
		{
			return Left >= x && Right < x + width && Top >= y && Bottom < y + height;
		}

		public bool Inside(Entity other)
		{
			return Left >= other.Left && Right < other.Right && Top >= other.Top && Bottom < other.Bottom;
		}

		#endregion

		#region Movement

		public float _moveX = 0.0f;
		public float _moveY = 0.0f;

		public void ResetMoveOffsets()
		{
			_moveX = 0.0f;
			_moveY = 0.0f;
		}

		public void MoveBy(float deltaX, float deltaY, CollisionFlags flags, params int[] solidTypes)
		{
			_moveX += deltaX;
			_moveY += deltaY;
			deltaX = Mathf.Round(_moveX);
			deltaY = Mathf.Round(_moveY);
			_moveX -= deltaX;
			_moveY -= deltaY;

			if (solidTypes.Length == 0)
			{
				X += deltaX;
				Y += deltaY;
				return;
			}

			if (deltaX != 0.0f)
			{
				if (Collidable)
				{
					var sign = deltaX > 0.0f ? 1 : -1;
					while (deltaX != 0.0f)
					{
						var stopMovement = false;
						var stop = CollideAt(X + sign, Y, solidTypes, (info) =>
						{
							info.Stopped = stopMovement;
							info.DeltaX = (int)deltaX;
							if (OnHit(info))
							{
								stopMovement = true;
							}
							return (flags & CollisionFlags.NonStop) == 0;
						});

						if (stop) return;
						if (stopMovement) break;
						X += sign;
						deltaX -= sign;
					}
				}
				else
				{
					X += deltaX;
				}
			}

			if (deltaY != 0.0f)
			{
				if (Collidable)
				{
					var sign = deltaY > 0.0f ? 1 : -1;
					while (deltaY != 0.0f)
					{
						bool stopMovement = false;
						var stop = CollideAt(X, Y + sign, solidTypes, (info) =>
						{
							info.Stopped = stopMovement;
							info.DeltaY = (int)deltaY;
							if (OnHit(info))
							{
								stopMovement = true;
							}
							return (flags & CollisionFlags.NonStop) == 0;
						});

						if (stop) return;
						if (stopMovement) break;
						Y += sign;
						deltaY -= sign;
					}
				}
				else
				{
					Y += deltaY;
				}
			}
		}

		public void MoveBy(Vector2 delta, CollisionFlags flags, params int[] solidTypes)
		{
			MoveBy(delta.X, delta.Y, flags, solidTypes);
		}

		public void MoveBy(float deltaX, float deltaY, params int[] solidTypes)
		{
			MoveBy(deltaX, deltaY, CollisionFlags.None, solidTypes);
		}

		public void MoveBy(Vector2 delta, params int[] solidTypes)
		{
			MoveBy(delta.X, delta.Y, CollisionFlags.None, solidTypes);
		}

		public void MoveTo(float x, float y, CollisionFlags flags, params int[] solidTypes)
		{
			MoveBy(x - X, y - Y, flags, solidTypes);
		}

		public void MoveTo(Vector2 pos, CollisionFlags flags, params int[] solidTypes)
		{
			MoveTo(pos.X, pos.Y, flags, solidTypes);
		}

		public void MoveTo(float x, float y, params int[] solidTypes)
		{
			MoveTo(x, y, CollisionFlags.None, solidTypes);
		}

		public void MoveTo(Vector2 pos, params int[] solidTypes)
		{
			MoveTo(pos.X, pos.Y, CollisionFlags.None, solidTypes);
		}

		public void TriggerCollisionDetection(params int[] solidTypes)
		{
			CollideAt(X, Y, solidTypes, (info) =>
			{
				OnHit(info);
				return false;
			});
		}

		protected virtual bool OnHit(HitInfo info)
		{
			return true;
		}

		#endregion
	}

	#region Collision

	public struct HitInfo
	{
		public static HitInfo None = new HitInfo(false, null, null, null);

		public bool Hit { get; }
		public Entity Other { get; internal set; }
		public Tile Tile { get; }
		public PixelMask PixelMask { get; }

		public int DeltaX { get; internal set; }
		public int DeltaY { get; internal set; }
		public bool Stopped { get; internal set; }
		public bool IsHorizontalMovement { get { return DeltaX != 0; } }
		public bool IsVerticalMovement { get { return DeltaY != 0; } }

		public static HitInfo CreateHit(Entity other = null, Tile tile = null, PixelMask pixelMask = null)
		{
			return new HitInfo(true, other, tile, pixelMask);
		}

		public static HitInfo CreateHit(Tile tile, PixelMask pixelMask = null)
		{
			return new HitInfo(true, null, tile, pixelMask);
		}

		public static implicit operator bool(HitInfo info)
		{
			return info.Hit;
		}

		private HitInfo(bool hit, Entity other, Tile tile, PixelMask pixelMask)
		{
			Hit = hit;
			Other = other;
			Tile = tile;
			PixelMask = pixelMask;

			DeltaX = 0;
			DeltaY = 0;
			Stopped = false;
		}

		public void OnHorizontalMovement(Action action)
		{
			if (IsHorizontalMovement)
			{
				action();
			}
		}

		public void OnVerticalMovement(Action action)
		{
			if (IsVerticalMovement)
			{
				action();
			}
		}

		public void Then(Action<Entity> action)
		{
			if (Hit)
			{
				action(Other);
			}
		}

		public void If<T>(Action<T> action) where T : Entity
		{
			if (Other is T)
			{
				action((T)Other);
			}
		}

		public static bool operator ==(HitInfo a, HitInfo b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(HitInfo a, HitInfo b)
		{
			return !a.Equals(b);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public interface ICollider
	{
		int WidthPx { get; }
		int HeightPx { get; }

		HitInfo CollideWithOther(float x, float y, float otherX, float otherY, Entity other);
		HitInfo CollideRect(float x, float y, float rectX, float rectY, int rectWidth, int rectHeight);
		void RenderDebug(float x, float y, SpriteBatch spriteBatch);
	}

	[Flags]
	public enum CollisionFlags
	{
		None = 0x00,
		NonStop = 0x01,
		PixelMaskDisalbed = 0x02
	}	

	#endregion
}

