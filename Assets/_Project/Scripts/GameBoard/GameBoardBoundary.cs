using System;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Calculates world-space screen boundaries
    /// from the main camera viewport.
    /// Camera injected via VContainer — no Camera.main needed.
    /// Calculated once in constructor — immutable after creation.
    /// </summary>
    public class GameBoardBoundary : IBoundaries
    {
        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float MinY { get; private set; }
        public float MaxY { get; private set; }

        /// <summary>
        /// Camera injected by VContainer.
        /// Boundaries calculated immediately in constructor.
        /// </summary>
        public GameBoardBoundary(Camera camera)
        {
            Calculate(camera);
        }

        private void Calculate(Camera camera)
        {
            // Viewport coordinates are always 0-1
            // regardless of screen resolution or aspect ratio
            // Bottom-left = (0,0,0), Top-right = (1,1,0)

            float depth = -camera.transform.position.z;

            Vector3 bottomLeft = camera.ViewportToWorldPoint(
                new Vector3(0, 0, depth));

            Vector3 topRight = camera.ViewportToWorldPoint(
                new Vector3(1, 1, depth));

            MinX = bottomLeft.x;
            MinY = bottomLeft.y;
            MaxX = topRight.x;
            MaxY = topRight.y;
        }
    }
}