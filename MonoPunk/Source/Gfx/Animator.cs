using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;

namespace MonoPunk
{
    [Flags]
    public enum AnimatorMode
    {
        OneShot = 0x01,
        Loop = 0x02,
        Random = 0x08
    }

    public class AnimatorData
    {
        public int FrameWidth
        {
            get { return tileset.TileWidth; }
        }

        public int FrameHeight
        {
            get { return tileset.TileHeight; }
        }

        private readonly Tileset tileset;
        private readonly Dictionary<string, AnimationData> animations = new Dictionary<string, AnimationData>();

        public AnimatorData(Tileset tileset)
        {
            this.tileset = tileset;
        }

        public AnimatorData(string tilesetName) : this(Asset.GetTileset(tilesetName))
        {
        }

        public AnimatorData(string textureName, int frameWidth, int frameHeight) : this(new Tileset(textureName, frameWidth, frameHeight))
        {
        }

        public void Add(string name, AnimatorMode mode, float interval, params int[] frames)
        {
            if (animations.ContainsKey(name))
            {
                throw new Exception("Animation '" + name + "' already exists.");
            }

            if (frames.Length == 0)
            {
                throw new Exception("Animation must have at least one frame.");
            }

            animations.Add(name, new AnimationData(name, mode, interval, frames));
        }

        public void Add(string name, int frame)
        {
            Add(name, AnimatorMode.Loop, 0.0f, frame);
        }

        public bool Contains(string name)
        {
            return animations.ContainsKey(name);
        }

        internal TextureRegion2D GetTileRegion(int tileId)
        {
            return tileset.GetTileRegion(tileId);
        }

        internal AnimationData GetAnimation(string name)
        {
            AnimationData animData;
            if(!animations.TryGetValue(name, out animData))
            {
                Engine.Throw("Animation '" + name + "' not found.");
            }
            return animData;
        }
    }

    internal class AnimationData
    {
        internal readonly string Name;
        internal readonly AnimatorMode Mode;
        internal readonly float Interval;
        internal readonly int[] Frames;

        internal AnimationData(string name, AnimatorMode mode, float interval, int[] frames)
        {
            Name = name;
            Mode = mode;
            Interval = interval;
            Frames = frames;
        }

        internal bool Loop
        {
            get { return (Mode & AnimatorMode.Loop) > 0; }
        }

        internal bool OneShot
        {
            get { return (Mode & AnimatorMode.OneShot) > 0; }
        }
    }

    public class Animator : Renderable
    {
        public float Rotation { get; set; } = 0.0f;
		public bool FlipH { get; set; }

        private Vector2 origin = Vector2.Zero;

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public int OriginX
        {
            get { return (int)origin.X; }
            set { origin.X = value; }
        }

        public int OriginY
        {
            get { return (int)origin.Y; }
            set { origin.Y = value; }
        }

        public void CenterOrigin()
        {
            OriginX = Width / 2;
            OriginY = Height / 2;
        }

        private Vector2 scale = Vector2.One;
        public float ScaleX
        {
            get { return scale.X; }
            set { scale.X = value; }
        }

        public float ScaleY
        {
            get { return scale.Y; }
            set { scale.Y = value; }
        }

		public float Scale
		{
			get { return Mathf.Max(scale.X, scale.Y); }
			set { scale.X = value; scale.Y = value; }
		}

        public int Width
        {
            get { return data.FrameWidth; }
        }

        public int Height
        {
            get { return data.FrameHeight; }
        }

        public string CurrentAnim
        {
            get { return lastAnimation != null ? lastAnimation.Name : null; }
        }

        public bool Playing
        {
            get { return currentAnimation != null; }
        }

        public float SpeedFactor { get; set; } = 1.0f;

        public int CurrentFrame { get; private set; }
        public int CurrentTile { get; private set; }

        private readonly AnimatorData data;
        private Action animCompleteCallback;

        private AnimationData currentAnimation;
        private AnimationData lastAnimation;
        private float frameCounter;

        public Animator(AnimatorData data)
        {
            this.data = data;
        }

        public Animator(string animatorDataName) : this(Asset.GetAnimatorData(animatorDataName))
        {
        }

        public Animator(Tileset tileset) : this(new AnimatorData(tileset))
        {
        }

        public Animator(string textureName, int frameWidth, int frameHeight) : this(new Tileset(textureName, frameWidth, frameHeight))
        {
        }

        public void Add(string name, AnimatorMode mode, float frameRate, params int[] frames)
        {
            data.Add(name, mode, frameRate, frames);
        }

        public void Add(string name, int frame)
        {
            data.Add(name, frame);
        }

		public bool Contains(string name)
		{
			return data.Contains(name);
		}

        public void Play(string name, Action callback = null)
        {
            if(currentAnimation != null && (currentAnimation.Name == name || currentAnimation.OneShot))
            {
                // same anim or current anim is one shot, don't change
                return;
            }

            currentAnimation = data.GetAnimation(name);
            lastAnimation = currentAnimation;     
            CurrentFrame = 0;
            frameCounter = 0.0f;
            animCompleteCallback = callback;
            UpdateFrame();
        }

        public void Stop()
        {
            currentAnimation = null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (currentAnimation == null || currentAnimation.Interval == 0.0f)
            {
                return;
            }

            frameCounter += deltaTime * SpeedFactor;
            if (frameCounter > currentAnimation.Interval)
            {
                if (++CurrentFrame == currentAnimation.Frames.Length)
                {
                    if (currentAnimation.Loop)
                    {
                        CurrentFrame = 0;
                        frameCounter -= currentAnimation.Interval;
                    }
                    else
                    {
                        var animation = currentAnimation;
                        currentAnimation = null;
                        frameCounter = 0.0f;
                        if (animCompleteCallback != null)
                        {
                            var callback = animCompleteCallback;
                            animCompleteCallback = null;                          
                            callback();
                        }
                    }
                }
                else
                {
                    frameCounter -= currentAnimation.Interval;
                }

                if(currentAnimation != null)
                {
                    UpdateFrame();
                }
            }
        }

        private void UpdateFrame()
        {
            if((currentAnimation.Mode & AnimatorMode.Random) > 0)
            {
                CurrentTile = currentAnimation.Frames[Rand.NextInt(currentAnimation.Frames.Length)];
            }
            else
            {
                CurrentTile = currentAnimation.Frames[CurrentFrame];
            }
        }

        protected override void OnRender(SpriteBatch spriteBatch)
        {
            if(currentAnimation != null)
            {
                var region = data.GetTileRegion(CurrentTile);
				var effects = FlipH ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(region.Texture, GlobalPosition, region.Bounds, Color * Alpha, Rotation, origin, scale, effects, 0.0f);
            }
        }
    }
}
