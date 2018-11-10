using MonoPunk;

namespace MonoPunk_TestBed
{
    class AnimatorTest : TestScene
    {
        private readonly Animator animator;

        public AnimatorTest()
        {           
            animator = new Animator("bat");
			Add(animator, 20, 30);
            animator.Play("idle");
        }
     
        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if(Input.WasMousePressed(MouseButton.Left))
            {
				animator.Play("attack", () => animator.Play("idle"));
            }
        }        

		[AssetLoader]
		public static void LoadAssets()
		{
			var anim = new AnimatorData("gfx/bat", 24, 24);
			anim.Add("idle", AnimatorMode.Loop, 0.5f, 0, 1);
			anim.Add("attack", AnimatorMode.OneShot, 0.25f, 2, 3);
			Asset.AddAnimatorData("bat", anim);
		}
    }
}
