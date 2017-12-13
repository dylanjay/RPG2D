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

        public static Vector2 WithMagnitude(this Vector2 vector, float magnitude)
        {
            return vector.normalized * magnitude;
        }

        /// <summary>
        /// Returns the Scalar Projection value.
        /// </summary>
        /// <param name="baseVector">The vector to project onto.</param>
        public static float ScalarProjectionOnto(this Vector2 vector, Vector2 baseVector)
        {
            //      _
            // this /|
            //     / :
            //    /  :
            //   / r : baseVector
            //  ----->-------->
            //     ^ returns magnitude of this vector.
            return Vector2.Dot(vector, baseVector.normalized);
        }

        /// <summary>
        /// Returns the Scalar Projection value.
        /// </summary>
        /// <param name="baseVector">The vector to project onto.</param>
        public static Vector2 ProjectOnto(this Vector2 vector, Vector2 baseVector)
        {
            //      _
            // this /|
            //     / :
            //    /  :
            //   / r : baseVector
            //  ----->-------->
            //     ^ returns this vector.
            Vector2 baseNormal = baseVector.normalized;
            return Vector2.Dot(vector, baseNormal) * baseNormal;
        }

        /// <summary>
        /// Same as Vector2.Scale(Vector2), but instead returns a new Vector2.
        /// </summary>
        /// <param name="scaleVector">The vector to scale by.</param>
        public static Vector2 ScaledBy(this Vector2 vector, Vector2 scaleVector)
        {
            return new Vector2(vector.x * scaleVector.x, vector.y * scaleVector.y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <remarks>Techincally not a Vector2 extension. If there are more Rect 
        /// extensions, we should move this.</remarks>
        public static Rect WithOffset(this Rect rect, Vector2 offset)
        {
            return new Rect(rect.position + offset, rect.size);
        }
    }
}
