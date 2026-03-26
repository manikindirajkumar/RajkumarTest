using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Spawns objects just outside screen boundary edges.
    /// Asteroids appear from outside screen and move inward —
    /// gives player time to react before being hit.
    /// Picks a random edge then a random point along that edge.
    /// </summary>
    public class EdgeSpawnPositionProvider : IEdgeSpawnPositionProvider
    {
        // How far outside boundary to spawn
        private const float SpawnOffset = 1f;

        private enum SpawnEdge { Top, Bottom, Left, Right }

        public Vector2 GetRandomSpawnPosition(IBoundaries boundaries)
        {
            if (boundaries == null)
                throw new System.ArgumentNullException(
                    nameof(boundaries));

            SpawnEdge edge = (SpawnEdge)Random.Range(0, 4);

            return edge switch
            {
                SpawnEdge.Top    => new Vector2(
                    Random.Range(boundaries.MinX, boundaries.MaxX),
                    boundaries.MaxY + SpawnOffset),

                SpawnEdge.Bottom => new Vector2(
                    Random.Range(boundaries.MinX, boundaries.MaxX),
                    boundaries.MinY - SpawnOffset),

                SpawnEdge.Left   => new Vector2(
                    boundaries.MinX - SpawnOffset,
                    Random.Range(boundaries.MinY, boundaries.MaxY)),

                SpawnEdge.Right  => new Vector2(
                    boundaries.MaxX + SpawnOffset,
                    Random.Range(boundaries.MinY, boundaries.MaxY)),

                _ => Vector2.zero
            };
        }
    }
}