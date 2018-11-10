using MonoPunk;

namespace MonoPunk_TestBed
{
    class SoundTest : TestScene
    {
        public SoundTest()
        {        
        }

        protected override void OnBegin()
        {
            base.OnBegin();

            // TODO play music
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            // TODO stop music
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if(Input.WasMousePressed(MouseButton.Left))
            {
                Asset.LoadSoundEffect("sfx/select").Play();
                
            }
        }        
    }
}
