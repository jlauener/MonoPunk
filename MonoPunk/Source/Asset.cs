using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MonoPunk
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AssetLoaderAttribute : Attribute
	{
	}

	public static class Asset
	{
		internal static void InitAll()
		{
			foreach (var type in Assembly.GetEntryAssembly().GetTypes())
			{
				foreach (var method in type.GetMethods())
				{
					if (method.GetCustomAttribute<AssetLoaderAttribute>() != null)
					{
						Log.Debug("calling asset loader method " + type + "." + method.Name + "()");
						method.Invoke(null, new object[] { });
					}
				}
			}
		}

		public static TextureRegion2D LoadRegion(string assetName)
		{
			return Load<TextureRegion2D>(assetName);
		}

		public static Texture2D LoadTexture(string assetName)
		{
			return Load<Texture2D>(assetName);
		}

		public static BitmapFont LoadFont(string assetName)
		{
			return Load<BitmapFont>(assetName);
		}

		public static Song LoadSong(string assetName)
		{
			return Load<Song>(assetName);
		}

		public static SoundEffect LoadSoundEffect(string assetName)
		{
			return Load<SoundEffect>(assetName);
		}

		public static TiledMap LoadTiledMap(string assetName)
		{
			return Load<TiledMap>(assetName);
		}

		public static TextureAtlas LoadTextureAltas(string assetName)
		{
			return Load<TextureAtlas>(assetName);
		}

		public static Effect LoadEffect(string assetName)
		{
			return Load<Effect>(assetName);
		}

		public static T Load<T>(string assetName)
		{
			return Engine.Instance.Content.Load<T>(assetName);
		}

		#region Tileset

		private static Dictionary<string, Tileset> TILESETS = new Dictionary<string, Tileset>();

		public static void AddTileset(string name, Tileset tileset)
		{
			if (TILESETS.ContainsKey(name))
			{
				throw new Exception("Tileset with name '" + name + "' already exists!");
			}
			TILESETS[name] = tileset;
		}

		public static void AddTileset(string name, string textureName, int tileWidth, int tileHeight)
		{
			AddTileset(name, new Tileset(LoadTexture(textureName), tileWidth, tileHeight));
		}

		public static Tileset GetTileset(string name)
		{
			Tileset tileset;
			if (!TILESETS.TryGetValue(name, out tileset))
			{
				throw new Exception("Tileset with name '" + name + "' not found!");
			}
			return tileset;
		}

		#endregion

		#region Animator

		private static Dictionary<string, AnimatorData> ANIMATORS = new Dictionary<string, AnimatorData>();

		public static void AddAnimatorData(string name, AnimatorData animatorData)
		{
			if (ANIMATORS.ContainsKey(name))
			{
				throw new Exception("AnimatorData with name '" + name + "' already exists!");
			}
			ANIMATORS[name] = animatorData;
		}

		public static AnimatorData GetAnimatorData(string name)
		{
			AnimatorData animatorData;
			if (!ANIMATORS.TryGetValue(name, out animatorData))
			{
				throw new Exception("AnimatorData with name '" + name + "' not found!");
			}
			return animatorData;
		}

		#endregion

		#region Sfx

		private static Dictionary<string, Sfx> SFXS = new Dictionary<string, Sfx>();

		public static void AddSfx(string name, Sfx sfx)
		{
			if (SFXS.ContainsKey(name))
			{
				throw new Exception("Sfx with name '" + name + "' already exists!");
			}
			SFXS[name] = sfx;
		}

		public static Sfx AddSfx(string name, string fileName, int count = 0)
		{
			var sfx = new Sfx(fileName, count);
			AddSfx(name, sfx);
			return sfx;
		}

		public static Sfx GetSfx(string name)
		{
			Sfx sfx;
			if (!SFXS.TryGetValue(name, out sfx))
			{
				return null;
			}
			return sfx;
		}

		#endregion

		#region Music

		private static Dictionary<string, Music> MUSICS = new Dictionary<string, Music>();

		public static void AddMusic(string name, Music music)
		{
			if (MUSICS.ContainsKey(name))
			{
				throw new Exception("Music with name '" + name + "' already exists!");
			}
			MUSICS[name] = music;
		}

		public static void AddMusic(string name, string assetName, bool loop = false, float volume = 1.0f)
		{
			AddMusic(name, new Music(assetName, loop, volume));
		}

		public static Music GetMusic(string name)
		{
			Music music;
			if (!MUSICS.TryGetValue(name, out music))
			{
				throw new Exception("Music with name '" + name + "' not found!");
			}
			return music;
		}

		#endregion

		#region PixelMask

		private static Dictionary<string, PixelMask> PIXEL_MASKS = new Dictionary<string, PixelMask>();

		public static void AddPixelMask(string name, PixelMask pixelMask)
		{
			if (PIXEL_MASKS.ContainsKey(name))
			{
				throw new Exception("PixelMask with name '" + name + "' already exists!");
			}
			PIXEL_MASKS[name] = pixelMask;
		}

		public static void AddPixelMask(string name, Texture2D texture)
		{
			AddPixelMask(name, new PixelMask(texture));
		}

		public static void AddPixelMask(string name, string textureName)
		{
			AddPixelMask(name, new PixelMask(textureName));
		}

		public static PixelMask GetPixelMask(string name)
		{
			PixelMask pixelMask;
			if (!PIXEL_MASKS.TryGetValue(name, out pixelMask))
			{
				throw new Exception("PixelMask with name '" + name + "' not found!");
			}
			return pixelMask;
		}

		private static Dictionary<string, PixelMaskSet> PIXEL_MASK_SETS = new Dictionary<string, PixelMaskSet>();

		public static void AddPixelMaskSet(string name, PixelMaskSet pixelMaskSet)
		{
			if (PIXEL_MASK_SETS.ContainsKey(name))
			{
				throw new Exception("PixelMaskSet with name '" + name + "' already exists!");
			}
			PIXEL_MASK_SETS[name] = pixelMaskSet;
		}

		public static void AddPixelMaskSet(string name, Tileset tileset)
		{
			AddPixelMaskSet(name, new PixelMaskSet(tileset));
		}

		public static void AddPixelMaskSet(string name, string textureName, int tileWidth, int tileHeight)
		{
			AddPixelMaskSet(name, new PixelMaskSet(textureName, tileWidth, tileHeight));
		}

		public static void AddPixelMaskSet(string name, string tilesetName)
		{
			AddPixelMaskSet(name, new PixelMaskSet(tilesetName));
		}

		public static PixelMaskSet GetPixelMaskSet(string name)
		{
			PixelMaskSet pixelMaskSet;
			if (!PIXEL_MASK_SETS.TryGetValue(name, out pixelMaskSet))
			{
				throw new Exception("PixelMaskSet with name '" + name + "' not found!");
			}
			return pixelMaskSet;
		}

		#endregion

		public static Dictionary<string, T> LoadAll<T>(string folder)
		{
			var contentDir = Engine.Instance.Content.RootDirectory;
			var dir = new DirectoryInfo(contentDir + Path.DirectorySeparatorChar + folder);
			if (!dir.Exists)
			{
				return new Dictionary<string, T>();
			}

			var result = new Dictionary<string, T>();
			var files = dir.GetFiles("*.*");
			foreach (FileInfo file in files)
			{
				var key = Path.GetFileNameWithoutExtension(file.Name);
				result[key] = Load<T>(folder + "/" + key);
			}
			return result;
		}
	}
}
