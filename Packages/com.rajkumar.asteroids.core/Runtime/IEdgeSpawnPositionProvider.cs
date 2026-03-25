using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Spawns objects just outside screen boundary edges.
    /// Asteroids appear from outside screen and move inward —
    /// gives player time to react before being hit.
    ///
    /// Picks a random edge (top, bottom, left, right)
    /// then picks a random point along that edge.
    /// Position is slightly outside boundary so asteroid
    /// enters screen naturally via its movement direction.
    /// </summary>
    public class EdgeSpawnPositionProvider
        : ISpawnPositionProvider
    {
        // How far outside the boundary to spawn
        // Prevents asteroid popping in at screen edge
        private const float SpawnOffset = 1f;

        private enum SpawnEdge
        {
            Top    = 0,
            Bottom = 1,
            Left   = 2,
            Right  = 3
        }

        public Vector2 GetRandomSpawnPosition(
            IBoundaries boundaries)
        {
            if (boundaries == null)
                throw new System.ArgumentNullException(
                    nameof(boundaries),
                    "Boundaries cannot be null.");

            // Pick random edge
            SpawnEdge edge = (SpawnEdge)Random.Range(0, 4);

            return edge switch
            {
                SpawnEdge.Top    => SpawnOnTopEdge(boundaries),
                SpawnEdge.Bottom => SpawnOnBottomEdge(boundaries),
                SpawnEdge.Left   => SpawnOnLeftEdge(boundaries),
                SpawnEdge.Right  => SpawnOnRightEdge(boundaries),
                _                => Vector2.zero
            };
        }

        // ── private edge helpers ─────────────────────────────────

        private Vector2 SpawnOnTopEdge(IBoundaries b)
        {
            return new Vector2(
                Random.Range(b.MinX, b.MaxX),  // random X
                b.MaxY + SpawnOffset);          // just above top
        }

        private Vector2 SpawnOnBottomEdge(IBoundaries b)
        {
            return new Vector2(
                Random.Range(b.MinX, b.MaxX),  // random X
                b.MinY - SpawnOffset);          // just below bottom
        }

        private Vector2 SpawnOnLeftEdge(IBoundaries b)
        {
            return new Vector2(
                b.MinX - SpawnOffset,           // just left of screen
                Random.Range(b.MinY, b.MaxY));  // random Y
        }

        private Vector2 SpawnOnRightEdge(IBoundaries b)
        {
            return new Vector2(
                b.MaxX + SpawnOffset,           // just right of screen
                Random.Range(b.MinY, b.MaxY));  // random Y
        }
    }
}