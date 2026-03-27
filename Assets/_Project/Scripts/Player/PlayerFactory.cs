using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
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

        public void Create(Vector3 position)
        {
            // WaitForCompletion safe — cached in LoadingScene
            GameObject prefab = Addressables
                .LoadAssetAsync<GameObject>(
                    AddressableKeys.Prefabs.Player)
                .WaitForCompletion();

            GameObject playerGo = Object.Instantiate(
                prefab, position, Quaternion.identity);

            // VContainer injects all [Inject] methods
            // on all MonoBehaviours on this GameObject
            _container.InjectGameObject(playerGo);
        }
    }
}