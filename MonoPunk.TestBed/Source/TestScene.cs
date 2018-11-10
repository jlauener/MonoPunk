using MonoPunk;
using System;

namespace MonoPunk_TestBed
{
	class TestScene : Scene
	{
		protected override void OnBegin()
		{
			base.OnBegin();
			Engine.WindowTitle = GetType().ToString();
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);

			if (Input.WasPressed("previous") && CurrentTestScene > 0)
			{
				ShowTestScene(CurrentTestScene - 1);
			}

			if (Input.WasPressed("next") && CurrentTestScene < Boot.TestScenes.Length - 1)
			{
				ShowTestScene(CurrentTestScene + 1);
			}

			if (Input.WasPressed("reset"))
			{
				ShowTestScene(CurrentTestScene);
			}
		}

		private static int CurrentTestScene;

		public static void ShowTestScene(int index)
		{
			CurrentTestScene = index;
			Engine.Scene = (Scene)Activator.CreateInstance(Boot.TestScenes[index]);
		}
	}
}
