using Microsoft.Xna.Framework;
using MonoPunk;
using MonoGame.Extended.BitmapFonts;
using System;

namespace MonoPunk_TestBed
{
    class InputTest : TestScene
    {
        private readonly Label _label;

        public InputTest()
        {
            _label = new Label("font/04b03", "");
            Add(_label, 10, 10);
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if(Input.WasPressed("test_multi_player", 0))
            {
                _label.Text = "player_0";
            }

            if (Input.WasPressed("test_multi_player", 1))
            {
                _label.Text = "player_1";
            }

            if (Input.WasPressed("test_multi_player", 2))
            {
                _label.Text = "player_2";
            }

            if (Input.WasPressed("test_multi_player", 3))
            {
                _label.Text = "player_3";
            }
        }
    }
}
