using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Single access point for all player components.
    /// Attach to Player prefab root.
    /// Drag references in prefab Inspector —
    /// validated on Awake so missing components
    /// are caught immediately at startup.
    /// GameBootstrapper does one GetComponent
    /// on this class instead of multiple separate calls.
    /// </summary>
    public class PlayerComponents : MonoBehaviour, IPlayer
    {
         
        [SerializeField] private SpaceGun      _spaceGun;
        public ISpaceGun     SpaceGun      => _spaceGun;
         

        private void Awake()
        {
            
            Debug.Assert(_spaceGun      != null,
                "[PlayerComponents] SpaceGun not assigned " +
                "in Player prefab.");
             
        }
    }
}