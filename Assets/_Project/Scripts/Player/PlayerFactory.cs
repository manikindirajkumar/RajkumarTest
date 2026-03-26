using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using RajkumarTest.Asteroid.Core;
using VContainer.Unity;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Responsible for spawning and injecting
    /// the player at runtime.
    /// Single responsibility — player creation only.
    /// Uses VContainer.InjectGameObject to wire
    /// all [Inject] methods on player MonoBehaviours.
    /// </summary>
    public class PlayerFactory
    {
        private readonly IObjectResolver _container;

        public PlayerFactory(IObjectResolver container)
        {
            _container = container;
        }

        public IPlayer Create(Vector3 position)
        {
            // WaitForCompletion safe — cached in LoadingScene
            GameObject prefab = Addressables
                .LoadAssetAsync<GameObject>(
                    AddressableKeys.Prefabs.Player)
                .WaitForCompletion();

            if (prefab == null)
            {
                Debug.LogError("[PlayerFactory] " +
                               "Failed to load player prefab. " +
                               $"Address: {AddressableKeys.Prefabs.Player}");
                return null;
            }

            GameObject playerGo = Object.Instantiate(
                prefab, position, Quaternion.identity);

            // VContainer injects all [Inject] methods
            // on all MonoBehaviours on this GameObject
            _container.InjectGameObject(playerGo);

            IPlayer player =
                playerGo.GetComponent<IPlayer>();

            Debug.Assert(player != null,
                "[PlayerFactory] IPlayer (PlayerComponents) " +
                "missing from Player prefab root.");

            return player;
        }
    }
}