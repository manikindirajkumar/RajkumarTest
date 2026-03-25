using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Calculates and stores world-space screen boundaries
    /// based on the main camera's viewport.
    /// Used for screen wrapping and asteroid spawn positioning.
    /// 
    /// Important: Initialise this after the camera is ready —
    /// call Initialise() from a MonoBehaviour's Start(), not Awake().
    /// </summary>
    public class GameBoardBoundary : IBoundaries
    {
        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float MinY { get; private set; }
        public float MaxY { get; private set; }

        /// <summary>
        /// Calculate world-space boundaries from the main camera viewport.
        /// Must be called after the camera is initialised.
        /// </summary>
        /// <param name="camera">The camera to calculate boundaries from</param>
        public void Initialise(Camera camera)
        {
            if (camera == null)
            {
                Debug.LogError("[GameBoardBoundary] Camera is null. " +
                               "Cannot calculate boundaries.");
                return;
            }

            // Calculate once and reuse — avoids redundant ScreenToWorldPoint calls
            float depth = -camera.transform.position.z;

            Vector3 bottomLeft = camera.ScreenToWorldPoint(
                new Vector3(0, 0, depth));

            Vector3 topRight = camera.ScreenToWorldPoint(
                new Vector3(Screen.width, Screen.height, depth));

            MinX = bottomLeft.x;
            MinY = bottomLeft.y;
            MaxX = topRight.x;
            MaxY = topRight.y;
        }
    }
}