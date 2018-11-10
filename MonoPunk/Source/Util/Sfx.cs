using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace MonoPunk
{
	public class Sfx
	{
		public static float MasterVolume { get; set; } = 1.0f;

		private float _volume = 1.0f;
		public Sfx Volume(float value)
		{
			_volume = value;
			return this;
		}

		private float _minInterval = -1.0f;
		public Sfx MinInterval(float value)
		{
			_minInterval = value;
			return this;
		}

		//   private readonly string _name;
		private SoundEffect[] _sfxArray;
		private double _lastPlayStamp;

		public Sfx(string name, int count)
		{
			if (count == 0)
			{
				_sfxArray = new SoundEffect[1];
				_sfxArray[0] = Asset.LoadSoundEffect(name);
			}
			else
			{
				_sfxArray = new SoundEffect[count];
				for (var i = 0; i < count; ++i)
				{
					_sfxArray[i] = Asset.LoadSoundEffect(name + "_" + (i + 1));
				}
			}
		}

		public Sfx(params string[] names)
		{
			_sfxArray = new SoundEffect[names.Length];
			for (var i = 0; i < names.Length; i++)
			{
				_sfxArray[i] = Asset.LoadSoundEffect(names[i]);
			}
		}

		public void Play()
		{
			Play(_volume);
		}

		public void Play(float volume)
		{
			if (_minInterval != -1.0f)
			{
				if (Engine.Now - _lastPlayStamp < _minInterval)
				{
					return;
				}
			}

			_lastPlayStamp = Engine.Now;
			Rand.GetRandomElement(_sfxArray).Play(MasterVolume * volume, 0.0f, 0.0f);
		}
	}
}
