using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;

namespace MonoPunk
{
	public enum TileSolidType
	{
		None,
		Full,
		HalfTop,
		HalfBottom,
		HalfLeft,
		HalfRight,
		PixelMask
	}

	public class Tile
	{
		public int GlobalId { get; private set; }
		public int Id { get; private set; }
		public int X { get; private set; }
		public int Y { get; private set; }

		public PixelMask PixelMask { get; set; }
		public TileSolidType SolidType { get; set; }

		private readonly Dictionary<string, string> properties;

		public Tile(int x, int y, TileSolidType solidType, Dictionary<string, string> properties = null)
		{
			X = x;
			Y = y;
			SolidType = solidType;
			this.properties = properties;
		}

		public Tile(int x, int y, PixelMask pixelMask, Dictionary<string, string> properties = null) : this(x, y, TileSolidType.PixelMask, properties)
		{
			PixelMask = pixelMask;
		}

		public Tile(int globalId, int localId, int tileX, int tileY, Dictionary<string, string> properties = null)
		{
			GlobalId = globalId;
			Id = localId;
			X = tileX;
			Y = tileY;
			this.properties = properties;
		}

		public bool HasProperty(string name)
		{
			return properties != null && properties.ContainsKey(name);
		}

		public string GetString(string name, string defaultValue = null)
		{
			if (properties == null)
			{
				return defaultValue;
			}

			string result;
			if (properties.TryGetValue(name, out result))
			{
				return result;
			}

			return defaultValue;
		}

		public bool GetBool(string name, bool defaultValue = false)
		{
			if (properties == null)
			{
				return defaultValue;
			}

			string str;
			if (properties.TryGetValue(name, out str))
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
			if (properties == null)
			{
				return defaultValue;
			}

			string str;
			if (properties.TryGetValue(name, out str))
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
			if (properties == null)
			{
				return defaultValue;
			}

			string str;
			if (properties.TryGetValue(name, out str))
			{
				float result;
				if (float.TryParse(str, out result))
				{
					return result;
				}
			}

			return defaultValue;
		}

		public override string ToString()
		{
			return "[TiledTile pos=" + X + "," + Y + " localId=" + Id + " globalId=" + GlobalId + " SolidType=" + SolidType + "]";
		}
	}
}
