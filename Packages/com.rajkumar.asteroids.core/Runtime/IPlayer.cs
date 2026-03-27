namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Single access point for all player systems.
    /// Implemented by PlayerComponents on Player prefab.
    /// GameBootstrapper depends on IPlayer —
    /// never on concrete components directly.
    /// </summary>
    public interface IPlayer
    {
        //ShipMovement  ShipMovement  { get; }
        ISpaceGun     SpaceGun      { get; }
        //RespawnSystem RespawnSystem { get; }
    }
}