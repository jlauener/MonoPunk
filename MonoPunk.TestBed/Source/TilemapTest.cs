using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoPunk;

namespace MonoPunk_TestBed
{
    class TilemapTest : TestScene
    {
		private const int TypeMap = 1;
		public const int TypeBall = 2;

        private const int LayerBall = 5;
        private const int LayerMap = 10;

		private const float MoveSpeedMin = 60.0f;
		private const float MoveSpeedMax = 120.0f;
        private const int BallCount = 100;

        public TilemapTest()
        {
			var map = new TiledMapEntity("map/test");
			map.Type = TypeMap;
            map.Layer = LayerMap;
            map.LoadCollisionGrid("main", (tile) => { return tile.GetBool("solid") ? TileSolidType.Full : TileSolidType.None; });
            map.LoadTilemap("main", "gfx/tileset");
            Add(map);
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (GetEntityCountByType(TypeBall) < BallCount)
            {
                TryToCreateBall();
            }
        }

        private void TryToCreateBall()
        {
            var spriteTexture = Asset.LoadTexture("gfx/ball_red");

            var map = GetEntity<TiledMapEntity>();
            var bounds = new Rectangle((int)map.X, (int)map.Y, map.Width, map.Height);
            Vector2 location;
            if (TryGetFreeLocation(out location, spriteTexture, bounds, TypeMap, TypeBall))
            {
                var ball = new Ball(spriteTexture, TypeMap, TypeBall);
                ball.Layer = LayerBall;
				ball.Type = TypeBall;
                ball.Position = location;
                ball.SetRandomDirection();
                ball.MoveSpeed = Rand.NextFloat(MoveSpeedMin, MoveSpeedMax);
                Add(ball);
            }
        }

        private bool TryGetFreeLocation(out Vector2 result, Texture2D spriteTexture, Rectangle bounds, params int[] solidType)
        {
            result.X = Rand.NextFloat(bounds.Left, bounds.Right - spriteTexture.Width);
            result.Y = Rand.NextFloat(bounds.Top, bounds.Bottom - spriteTexture.Height);
			return QueryRect(result.X, result.Y, spriteTexture.Width, spriteTexture.Height, QuerySelector.Type(solidType)).Entity == null;		
        }
    }
}
