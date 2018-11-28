using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace MonoPunk
{
	public struct QueryResult
	{
		public Entity Entity { get; private set; }
		public float DistanceSquared { get; private set; }
		public QueryResult(Entity entity = null, float distanceSquared = 0.0f)
		{
			Entity = entity;
			DistanceSquared = distanceSquared;
		}

		public static bool operator true(QueryResult queryResult)
		{
			return queryResult.Entity != null;
		}

		public static bool operator false(QueryResult queryResult)
		{
			return queryResult.Entity == null;
		}

		public static readonly QueryResult None = new QueryResult();
	}

	public struct QuerySelector
	{
		private readonly int[] types;
		private readonly Func<Entity, bool> matcher;

		public static QuerySelector Any { get; private set; } = new QuerySelector(null, null);

		public static QuerySelector Type(Func<Entity, bool> matcher, params int[] type)
		{
			return new QuerySelector(type, matcher);
		}

		public static QuerySelector Type(params int[] types)
		{
			return Type(null, types);
		}

		private QuerySelector(int[] types, Func<Entity, bool> matcher)
		{
			this.types = types;
			this.matcher = matcher;
		}

		internal void Select(Scene scene, Func<Entity, bool> callback)
		{
			if (types == null)
			{
				foreach (var entity in scene.GetEntities())
				{
					if (matcher == null || matcher(entity))
						if (callback(entity))
						{
							return;
						}
				}
			}
			else
			{
				foreach (var type in types)
				{
					var entities = scene.GetEntitiesByType(type);
					if (entities != null)
					{
						foreach (var entity in entities)
						{
							if (matcher == null || matcher(entity))
								if (callback(entity))
								{
									return;
								}
						}
					}
				}
			}
		}
	}

	public abstract class Scene
	{
		public Camera2D Camera { get; set; } = Engine.CreateCamera();

		private readonly RenderManager renderManager;
		private readonly Tweener tweener = new Tweener();
		private readonly SpriteBatch spriteBatch = new SpriteBatch(Engine.Instance.GraphicsDevice);

		#region Internal

		public Scene()
		{
			renderManager = new RenderManager(Camera);
		}
		internal void Begin()
		{
			FlushEntitiesAddList();
			OnBegin();
		}

		internal void End()
		{
			OnEnd();
		}

		internal void Update(GameTime gameTime)
		{
			OnUpdate((float)gameTime.ElapsedGameTime.TotalSeconds);
		}

		internal void Render()
		{
			OnRender();
		}

		internal void RenderDebug(SpriteBatch spriteBatch)
		{
			OnRenderDebug(spriteBatch);
		}

		#endregion

		#region Callback

		protected virtual void OnBegin() { }

		protected virtual void OnEnd() { }

		protected virtual void OnUpdate(float deltaTime)
		{
			tweener.Update(deltaTime);
			UpdateEntities(deltaTime);
		}

		protected virtual void OnRender()
		{
			renderManager.Render(spriteBatch, Camera);
		}

		protected virtual void OnRenderDebug(SpriteBatch spriteBatch)
		{
			foreach (var e in _entities.GetList())
			{
				e._RenderDebug(spriteBatch);
			}
		}

		#endregion

		#region Entity

		private readonly SmartList<Entity> _entities = new SmartList<Entity>();
		private readonly Dictionary<int, List<Entity>> _entitiesByType = new Dictionary<int, List<Entity>>();

		private void FlushEntitiesAddList()
		{
			_entities.FlushAddList((entity) =>
			{
				List<Entity> typeList;
				if (!_entitiesByType.TryGetValue(entity.Type, out typeList))
				{
					typeList = new List<Entity>();
					_entitiesByType[entity.Type] = typeList;
				}
				typeList.Add(entity);
			});
		}

		private void UpdateEntities(float deltaTime)
		{
			FlushEntitiesAddList();

			foreach (var entity in _entities.GetList())
			{
				entity._Update(deltaTime);
			}

			_entities.FlushRemoveList((entity) =>
			{
				_entitiesByType[entity.Type].Remove(entity);
				entity._Cleanup();
			});
		}

		public void Add(Entity entity)
		{
			entity._Init(this, renderManager);
			_entities.Add(entity);
		}

		public void Add(Entity entity, float x, float y)
		{
			entity.X = x;
			entity.Y = y;
			Add(entity);
		}

		public void Add(Entity entity, Vector2 position)
		{
			entity.Position = position;
			Add(entity);
		}

		public void Remove(Entity entity)
		{
			entity._Remove();
			_entities.Remove(entity);
		}

		public void RemoveByType(int type)
		{
			var entities = GetEntitiesByType(type);
			if (entities != null)
			{
				foreach (var entity in entities)
				{
					Remove(entity);
				}
			}
		}

		public Entity Add(Component component, float x = 0.0f, float y = 0.0f)
		{
			var entity = new Entity(x, y);
			entity.Add(component);
			Add(entity);
			return entity;
		}

		public Entity Add(Component component, Vector2 pos)
		{
			return Add(component, pos.X, pos.Y);
		}

		public List<Entity> GetEntities()
		{
			return _entities.GetList();
		}

		public int GetEntityCount()
		{
			return GetEntities().Count;
		}

		public void ForEach(Action<Entity> action)
		{
			foreach (var entity in GetEntities())
			{
				action(entity);
			}
		}

		public int GetEntityCount<T>()
		{
			var count = 0;
			foreach (var entity in GetEntities())
			{
				if (entity is T)
				{
					count++;
				}
			}
			return count;
		}

		public void ForEach<T>(Action<T> action) where T : Entity
		{
			foreach (var entity in GetEntities())
			{
				if (entity is T)
				{
					action((T)entity);
				}
			}
		}

		public Entity GetEntityByName(string name)
		{
			foreach (var entity in GetEntities())
			{
				if (entity.Name == name)
				{
					return entity;
				}
			}

			return null;
		}

		public Entity GetEntityByType(int type)
		{
			var entities = GetEntitiesByType(type);
			if (entities.Count == 0)
			{
				return null;
			}
			return entities[0];
		}

		public T GetEntity<T>() where T : Entity
		{
			foreach (var entity in GetEntities())
			{
				if (entity is T)
				{
					return (T)entity;
				}
			}

			return null;
		}

		public void IterateEntities<T>(Action<T> consumer) where T : Entity
		{
			foreach (var entity in GetEntities())
			{
				if (entity is T)
				{
					consumer((T)entity);
				}
			}
		}

		public List<Entity> GetEntitiesByType(int type)
		{
			List<Entity> result;
			if (_entitiesByType.TryGetValue(type, out result))
			{
				return result;
			}

			return null;
		}

		public void IterateEntitiesByType(int type, Action<Entity> callback)
		{
			List<Entity> entities;
			if (_entitiesByType.TryGetValue(type, out entities))
			{
				foreach (var e in entities)
				{
					callback(e);
				}
			}
		}

		public int GetEntityCountByType(int type)
		{
			var entities = GetEntitiesByType(type);
			return entities == null ? 0 : entities.Count;
		}

		#endregion

		#region Query

		public QueryResult QueryClosest(Vector2 position, int type, Func<Entity, bool> matcher = null)
		{
			var entities = GetEntitiesByType(type);
			if (entities == null)
			{
				return QueryResult.None;
			}

			Entity closest = null;
			float closestDist = 0.0f;

			foreach (var entity in entities)
			{
				if (matcher == null || matcher(entity))
				{
					var dist = (position - entity.Position).LengthSquared();
					if (closest == null || dist < closestDist)
					{
						closest = entity;
						closestDist = dist;
					}
				}
			}

			return new QueryResult(closest, closestDist);
		}

		public QueryResult QueryFurthest(Vector2 position, int type, Func<Entity, bool> matcher = null)
		{
			var entities = GetEntitiesByType(type);
			if (entities == null)
			{
				return QueryResult.None;
			}

			Entity furthest = null;
			float furthestDist = 0.0f;

			foreach (var entity in entities)
			{
				if (matcher == null || matcher(entity))
				{
					var dist = (position - entity.Position).LengthSquared();
					if (furthest == null || dist > furthestDist)
					{
						furthest = entity;
						furthestDist = dist;
					}
				}
			}

			return new QueryResult(furthest, furthestDist);
		}

		public QueryResult QueryRect(float x, float y, int width, int height, QuerySelector selector)
		{
			var result = QueryResult.None;
			QueryRect(x, y, width, height, selector, (entity) =>
			{
				result = new QueryResult(entity);
				return true;
			});
			return result;
		}

		public QueryResult QueryRect(Rectangle rect, QuerySelector selector)
		{
			return QueryRect(rect.X, rect.Y, rect.Width, rect.Height, selector);
		}

		public void QueryRect(float x, float y, int width, int height, QuerySelector selector, Func<Entity, bool> callback)
		{
			selector.Select(this, (entity) =>
			{
				if (entity.CollideRect(x, y, width, height) != HitInfo.None)
				{
					return callback(entity);
				}
				return false;
			});
		}

		public void QueryRect(Rectangle rect, QuerySelector selector, Func<Entity, bool> callback)
		{
			QueryRect(rect.X, rect.Y, rect.Width, rect.Height, selector, callback);
		}

		public void QueryCircle(Vector2 position, float radius, QuerySelector selector, Func<Entity, bool> callback)
		{
			var radiusSquared = radius * radius;
			selector.Select(this, (entity) =>
			{
				if (position.DistanceSquared(entity.Position) < radiusSquared)
				{
					return callback(entity);
				}
				return false;
			});
		}

		public QueryResult QueryCircle(Vector2 position, float radius, QuerySelector selector)
		{
			var result = QueryResult.None;
			QueryCircle(position, radius, selector, (entity) =>
			{
				result = new QueryResult(entity);
				return true;
			});
			return result;
		}

		public void QueryLine(float x1, float y1, float x2, float y2, QuerySelector selector, float width, Func<Entity, bool> callback)
		{
			var start = new Vector2(x1, y1);
			var line = new Vector2(x2 - x1, y2 - y1);
			var length = line.Length();
			var increment = 1.0f / (length / (width * 2));
			var pct = 0.0f;
			while (pct <= 1.0f)
			{
				var pos = start + line * pct;
				var stop = false;
				QueryRect(pos.X - width / 2, pos.Y - width / 2, Mathf.Ceiling(width), Mathf.Ceiling(width), selector, (entity) =>
				{
					if (callback(entity))
					{
						stop = true;
						return true;
					}
					return false;
				});
				if (stop)
				{
					return;
				}
				pct += increment;
			}
		}

		public QueryResult QueryLine(float x1, float y1, float x2, float y2, QuerySelector selector, float width = 1.0f)
		{
			var result = QueryResult.None;
			QueryLine(x1, y1, x2, y2, selector, width, (entity) =>
			{
				result = new QueryResult(entity);
				return true;
			});
			return result;
		}

		#endregion

		#region Tween

		public Tween Tween<T>(T target, object values, float duration, float delay = 0.0f, bool overwrite = true) where T : class
		{
			return tweener.Tween(target, values, duration, delay, overwrite);
		}

		public Tween Callback(float delay, Action callback)
		{
			return tweener.Timer(0.0f, delay).OnComplete(callback);
		}

		#endregion

		#region Render

		public void SetCamera(int layer, Camera2D camera)
		{
			renderManager.SetCamera(layer, camera);
		}

		public void SetVisible(int layer, bool visible)
		{
			renderManager.SetVisible(layer, visible);
		}

		public void SetScissor(int layer, int x, int y, int width, int height)
		{
			renderManager.SetScissor(layer, x, y, width, height);
		}

		#endregion
	}
}
