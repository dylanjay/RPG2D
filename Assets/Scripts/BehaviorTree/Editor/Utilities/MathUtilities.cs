using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.Utilities
{
    public class MathUtilities : MonoBehaviour
    {
        /// <summary>
        /// Returns whether or not the given <paramref name="point"/> is inside of the line segment.
        /// Does not use rounded edges.
        /// </summary>
        /// <param name="startPosition">The start position of the line segment.</param>
        /// <param name="endPosition">The end position of the line segment.</param>
        /// <param name="width">The width of the line segment.</param>
        /// <param name="point">The point to check against for the line segment.</param>
        public static bool PointWithinLineSegment(Vector2 startPosition, Vector2 endPosition, float width, Vector2 point)
        {
            Vector2 segment = startPosition - endPosition;
            Vector2 relativePoint = startPosition - point;
            Vector2 projection = relativePoint.ProjectOnto(segment);
            float projectedDistance = projection.magnitude;
            // If point not within the bounds below, exit early.
            //  |          |
            //  |--------->|
            //  |          |
            if (projectedDistance < 0 || projectedDistance > segment.magnitude) { return false; }

            Vector2 vectorRejection = relativePoint - projection;
            return vectorRejection.sqrMagnitude < width * width + Vector2.kEpsilon;
        }
    }
}
