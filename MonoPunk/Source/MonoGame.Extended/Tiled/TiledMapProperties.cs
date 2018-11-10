using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapProperties : Dictionary<string, string>
    {
		public string GetString(string name, string defaultValue = null)
		{
			string result;
			if (TryGetValue(name, out result))
			{
				return result;
			}

			return defaultValue;
		}

		public bool GetBool(string name, bool defaultValue = false)
		{
			string str;
			if (TryGetValue(name, out str))
			{
				bool result;
				if (bool.TryParse(str, out result))
				{
					return result;
				}
			}

			return defaultValue;
		}

		public int GetInt(string name, int defaultValue = 0)
		{
			string str;
			if (TryGetValue(name, out str))
			{
				int result;
				if (int.TryParse(str, out result))
				{
					return result;
				}
			}

			return defaultValue;
		}

		public float GetFloat(string name, float defaultValue = 0)
		{
			string str;
			if (TryGetValue(name, out str))
			{
				float result;
				if (float.TryParse(str, out result))
				{
					return result;
				}
			}

			return defaultValue;
		}

		public T GetEnum<T>(string name)
		{
			return (T) Enum.Parse(typeof(T), GetString(name));
		}
	}
}