using Microsoft.Xna.Framework;

namespace MonoPunk
{
	public class Node
	{
		private Vector2 position = Vector2.Zero;
		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public void SetPosition(float x, float y)
		{
			position.X = x;
			position.Y = y;
		}

		public float X
		{
			get { return position.X; }
			set { position.X = value; }
		}

		public float Y
		{
			get { return position.Y; }
			set { position.Y = value; }
		}

		public Vector2 GlobalPosition
		{
			get { return Parent != null ? Parent.GlobalPosition + Position : Position; }
		}

		public float GlobalX
		{
			get { return Parent != null ? Parent.GlobalX + X : X; }
		}

		public float GlobalY
		{
			get { return Parent != null ? Parent.GlobalY + Y : Y; }
		}

		public Node Parent { get; set; }

		public Node(float x, float y)
		{
			X = x;
			Y = y;
		}
	}
}
