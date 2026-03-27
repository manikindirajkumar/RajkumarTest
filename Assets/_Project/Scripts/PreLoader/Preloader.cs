using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Loads and instantiates all pooled objects
    /// before GameScene starts.
    /// Uses DontDestroyOnLoad to carry pool parents
    /// into GameScene — no instantiation needed there.
    /// </summary>
    public class Preloader : MonoBehaviour
    {
        [SerializeField] private LoadingView _loadingView;
        [SerializeField] private string _gameSceneName = "GameScene";

        [Header("Pool Settings")]
        [SerializeField] private int _bulletPoolSize   = 10;
        [SerializeField] private int _largePoolSize    = 12;
        [SerializeField] private int _mediumPoolSize   = 24;
        [SerializeField] private int _smallPoolSize    = 48;
        

        public Transform BulletPoolParent        { get; private set; }
        public Transform LargeAsteroidParent     { get; private set; }
        public Transform MediumAsteroidParent    { get; private set; }
        public Transform SmallAsteroidParent     { get; private set; }
        
        
        private readonly List<string> _addresses =
            new List<string>
            {
                AddressableKeys.Prefabs.Player,
                AddressableKeys.Prefabs.Bullet1,
                AddressableKeys.Prefabs.LargeAsteroid,
                AddressableKeys.Prefabs.MediumAsteroid,
                AddressableKeys.Prefabs.SmallAsteroid
            };

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            // Register into ServiceLocator —
            // available before GameScene builds VContainer
            ServiceLocator.Register<Preloader>(this);
        }
        private async void Start()
        {
            // Step 1 — load all assets into cache
            await LoadAllAssetsAsync();
            _loadingView?.UpdateProgress(0.5f);

            // Step 2 — instantiate pool objects
            InstantiatePoolObjects();
            _loadingView?.UpdateProgress(1f);

            // Step 3 — load game scene
            await Task.Delay(500); // brief pause so 100% shows
            LoadGameScene();
        }

        private async Task LoadAllAssetsAsync()
        {
            int total   = _addresses.Count;
            int current = 0;

            foreach (string address in _addresses)
            {
                await LoadAssetAsync(address);
                current++;

                // First half of progress bar = loading
                float progress = (float)current / total * 0.5f;
                _loadingView?.UpdateProgress(progress);
            }
        }

        private async Task<bool> LoadAssetAsync(string address)
        {
            var handle = Addressables
                .LoadAssetAsync<GameObject>(address);

            await handle.Task;

            return handle.Status ==
                AsyncOperationStatus.Succeeded;
        }

        private void InstantiatePoolObjects()
        {
            BulletPoolParent     = CreateParent("[Pool] Bullets");
            LargeAsteroidParent  = CreateParent("[Pool] LargeAsteroids");
            MediumAsteroidParent = CreateParent("[Pool] MediumAsteroids");
            SmallAsteroidParent  = CreateParent("[Pool] SmallAsteroids");

            PreInstantiate(
                AddressableKeys.Prefabs.Bullet1,
                _bulletPoolSize,
                BulletPoolParent);

            PreInstantiate(
                AddressableKeys.Prefabs.LargeAsteroid,
                _largePoolSize,
                LargeAsteroidParent);

            PreInstantiate(
                AddressableKeys.Prefabs.MediumAsteroid,
                _mediumPoolSize,
                MediumAsteroidParent);

            PreInstantiate(
                AddressableKeys.Prefabs.SmallAsteroid,
                _smallPoolSize,
                SmallAsteroidParent);
        }

        private Transform CreateParent(string name)
        {
            GameObject parent = new GameObject(name);
            parent.transform.SetParent(this.transform);
            return parent.transform;
        }

        private void PreInstantiate(
            string address,
            int count,
            Transform parent)
        {
            // WaitForCompletion safe — just loaded above
            GameObject prefab = Addressables
                .LoadAssetAsync<GameObject>(address)
                .WaitForCompletion();

            if (prefab == null)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                GameObject go = Instantiate(
                    prefab,
                    Vector3.zero,
                    Quaternion.identity,
                    parent);

                go.SetActive(false);
            }
        }
        private void LoadGameScene()
        {
            SceneManager.LoadScene(_gameSceneName);
        }
    }
}