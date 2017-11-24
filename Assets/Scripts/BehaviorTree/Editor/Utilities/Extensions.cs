using UnityEngine;

namespace Benco.Utilities
{
    /// <summary>
    /// Extensions to the Vector2 struct.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Returns a copy of <paramref name="vector"/>, rotated by <paramref name="degrees"/>
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        public static Vector2 RotatedBy(this Vector2 vector, float degrees)
        {
            float radAngle = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radAngle);
            float sin = Mathf.Sin(radAngle);
            return new Vector2(
                vector.x * cos - vector.y * sin,
                vector.y * cos + vector.x * sin
            );
        }
    }
}
