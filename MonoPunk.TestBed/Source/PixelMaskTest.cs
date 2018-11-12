using MonoPunk;
using Microsoft.Xna.Framework;

namespace MonoPunk_TestBed
{
	class PixelMaskTest : TestScene
	{
		private const int TypeSolid = 1;

		private readonly Entity player;

		public PixelMaskTest()
		{
			player = new Entity(80, 80);
			player.Collider = new PixelMask("gfx/mask2");
			player.OriginX = player.Width / 2;
			player.OriginY = player.Height / 2;
			var playerSprite = new Sprite("gfx/mask2");
			playerSprite.CenterOrigin();
			player.Add(playerSprite);
			Add(player);

			var entity1 = new Entity(40, 30);
			entity1.Type = TypeSolid;
			entity1.Collider = new PixelMask("gfx/mask1");
			entity1.Add(new Sprite("gfx/mask1"));
			Add(entity1);

			var tileset = new Tileset("gfx/mask_tiles", 16, 16);
			var tilemap = new Tilemap(tileset, 5, 5);
			var gridCollider = new GridCollider(8, 8, 16, 16, new PixelMaskSet(tileset));
			SetTile(tilemap, gridCollider, 0, 0, 0);
			SetTile(tilemap, gridCollider, 1, 0, 4);
			SetTile(tilemap, gridCollider, 2, 0, 1);
			SetTile(tilemap, gridCollider, 0, 1, 4);
			SetTile(tilemap, gridCollider, 1, 1, 4);
			SetTile(tilemap, gridCollider, 2, 1, 4);
			SetTile(tilemap, gridCollider, 0, 2, 2);
			SetTile(tilemap, gridCollider, 1, 2, 4);
			SetTile(tilemap, gridCollider, 2, 2, 3);

			var mapEntity = new Entity(100, 100);
			mapEntity.Type = TypeSolid;
			mapEntity.Add(tilemap);
			mapEntity.Collider = gridCollider;
			Add(mapEntity);
		}

		private void SetTile(Tilemap tilemap, GridCollider gridCollider, int x, int y, int id)
		{
			tilemap.SetTileAt(x, y, id);
			gridCollider.SetPixelMaskAt(x, y, id);
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);

			var dir = Vector2.Zero;
			if (Input.IsDown("left")) dir.X -= 1.0f;
			if (Input.IsDown("right")) dir.X += 1.0f;
			if (Input.IsDown("up")) dir.Y -= 1.0f;
			if (Input.IsDown("down")) dir.Y += 1.0f;

			if (dir != Vector2.Zero)
			{
				dir.Normalize();
				player.MoveBy(dir * 60.0f * deltaTime, TypeSolid);
			}
		}
	}
}
