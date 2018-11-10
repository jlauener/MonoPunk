using MonoPunk;

namespace MonoPunk_TestBed
{
    class BackdropTest : TestScene
    {
        public BackdropTest()
        {
            SetScissor(10, 40, 40, 100, 100);

            var tileset = new Tileset(Asset.LoadTexture("gfx/tileset"), 24, 24);

            var backdrop = new Backdrop(tileset.GetTileRegion(0), 100, 100);
            backdrop.ScrollX = 130.0f;
            backdrop.ScrollY = 130.0f;

            var entity = new Entity(40.0f, 40.0f);
            entity.Layer = 10;
            entity.Width = backdrop.Width;
            entity.Height = backdrop.Height;
            entity.Add(backdrop);
            Add(entity);
        }
    }
}
