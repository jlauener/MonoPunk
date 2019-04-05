using System;
using System.Collections.Generic;

namespace MonoPunk
{
	public class EntityData
	{
		public string Name { get; }

		public int Type { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int OriginX { get; set; }
		public int OriginY { get; set; }

		public EntityData(string name, int type = 0)
		{
			Name = name;
			Type = type;
		}

		private static readonly List<EntityData> DATA_STORE = new List<EntityData>();

		public static T Create<T>(params dynamic[] args) where T : EntityData
		{
			T data = (T) Activator.CreateInstance(typeof(T), args);
			DATA_STORE.Add(data);
			return data;
		}

		public static T Get<T>(string name) where T : EntityData
		{
			foreach (var data in DATA_STORE)
			{
				if (data.Name == name) return (T)data;
			}

			return null;
		}

		public static void Iterate<T>(Action<T> action) where T : EntityData
		{
			foreach (var data in DATA_STORE)
			{
				if (data is T) action((T) data);
			}
		}
	}
}
