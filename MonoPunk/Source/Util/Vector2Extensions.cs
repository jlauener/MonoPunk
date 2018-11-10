using System.IO;
using Microsoft.Xna.Framework;

namespace MonoPunk
{
    public static class Vector2Extensions
    {
        public static float Distance(this Vector2 a, Vector2 b)
        {
            return (a - b).Length();
        }

        public static float DistanceSquared(this Vector2 a, Vector2 b)
        {
            return (a - b).LengthSquared();
        }

        public static float AngleWith(this Vector2 a, Vector2 b)
        {
            a.Normalize();
            b.Normalize();
            return AngleWithNormalized(a, b);
        }

        public static float AngleWithNormalized(this Vector2 a, Vector2 b)
        {
            return Mathf.Acos(a.X * b.X + a.Y * b.Y);
        }

        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            var cs = Mathf.Cos(angle);
            var sn = Mathf.Sin(angle);
            return new Vector2(v.X * cs - v.Y * sn, v.X * sn + v.Y * cs);
        }
    }
}