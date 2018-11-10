using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace MonoPunk
{
    public class Music
    {
        private static float masterVolume = 1.0f;
        public static float MasterVolume
        {
            get { return masterVolume; }
            set
            {
                masterVolume = value;
                MediaPlayer.Volume = MasterVolume; // FIXME this breaks current song's volume...
            }
        }

        private readonly Song _song;

        public bool Loop { get; set; }        
        public float Volume { get; set; }

        public Music(Song song, bool loop = false, float volume = 1.0f)
        {
            _song = song;
            Loop = loop;
            Volume = volume;
        }

        public Music(string songName, bool loop = false, float volume = 1.0f) : this(Asset.LoadSong(songName), loop)
        {
        }

        public void Play()
        {
            MediaPlayer.Play(_song);
            MediaPlayer.IsRepeating = Loop;
            MediaPlayer.Volume = MasterVolume * Volume;
        }

        public void Play(float startPoint)
        {
            MediaPlayer.Play(_song, TimeSpan.FromMilliseconds(startPoint));
            MediaPlayer.IsRepeating = Loop;
            MediaPlayer.Volume = MasterVolume * Volume;
        }

        public void FadeIn(float duration, Action callback = null)
        {
            MediaPlayer.Play(_song);
            MediaPlayer.IsRepeating = Loop;
            MediaPlayer.Volume = 0.0f;

            Engine.Scene.Tween(typeof(MediaPlayer), new { Volume = MasterVolume * 1.0f }, duration).OnComplete(() =>
            {
                if(callback != null)
                {
                    callback();
                }
            });
        }

        public static void FadeOut(float duration, Action callback = null)
        {
            Engine.Scene.Tween(typeof(MediaPlayer), new { Volume = 0.0f }, duration).OnComplete(() =>
            {
                if(callback != null)
                {
                    callback();
                }
            });
        }

        public static void Stop()
        {
            MediaPlayer.Stop();
        }

        public static void Pause()
        {
            MediaPlayer.Pause();
        }

        public static void Resume()
        {
            MediaPlayer.Resume();
        }
    }
}
