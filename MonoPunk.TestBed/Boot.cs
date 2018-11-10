using MonoPunk;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace MonoPunk_TestBed
{
	class Boot : Engine
	{
		public static Type[] TestScenes;

		public Boot() : base(width: 320, height: 240, windowScale: 2)
		{
		}

		protected override void OnLoadContent()
		{
			Input.Define("previous", Keys.Left);
			Input.Define("next", Keys.Right);
			Input.Define("reset", Keys.R);

			Input.Define("up", Keys.W);
			Input.Define("down", Keys.S);
			Input.Define("left", Keys.A);
			Input.Define("right", Keys.D);

			Input.Define("test_multi_player", 0, Keys.D1);
			Input.Define("test_multi_player", 1, Keys.D2);
			Input.Define("test_multi_player", 2, Keys.D3);
			Input.Define("test_multi_player", 3, Keys.D4);

			// Use Linq query to retrieve all test scenes.
			TestScenes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
						  from assemblyType in domainAssembly.GetTypes()
						  where typeof(TestScene).IsAssignableFrom(assemblyType) && assemblyType != typeof(TestScene)						  
						  select assemblyType).ToArray();
		}

		protected override void OnStart()
		{
			TestScene.ShowTestScene(1);
		}
	}

#if WINDOWS || LINUX

	public static class Program
	{
		[STAThread]
		static void Main()
		{
			using (var game = new Boot())
				game.Run();
		}
	}
#endif
}
