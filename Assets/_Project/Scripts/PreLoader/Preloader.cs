using System;
using System.Collections;
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
    /// Runs in LoadingScene before GameScene starts.
    /// Loads all prefabs via AddressableAssetLoader,
    /// pre-instantiates pool objects under persistent parents,
    /// and registers itself in ServiceLocator so
    /// GameLifetimeScope can access pool parents without
    /// any scene searching.
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

        private AddressableAssetLoader<GameObject> _addressableAssetLoaderForGameObjcets;
        
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
            
            // Creating AssetLoader instance for gameobjects
            _addressableAssetLoaderForGameObjcets = new AddressableAssetLoader<GameObject>();
        }
        private async void Start()
        {
            // editor initialises this automatically, builds don't
            await Addressables.InitializeAsync().Task;
            await LoadAllAssetsAsync();
            // UI Updates
            _loadingView?.UpdateProgress(1f);
            // Creating pool of objects and attached to a parent
            InstantiatePoolObjects();
            // Loading next scene
            StartCoroutine(LoadGameSceneDelayed());
        }

        private IEnumerator LoadGameSceneDelayed()
        {
            yield return new WaitForSeconds(0.5f);
            LoadGameScene();
        }

        private void LoadGameScene()
        {
            SceneManager.LoadScene(_gameSceneName);
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
            var result = await _addressableAssetLoaderForGameObjcets.LoadAsync(address);

            if (result == null)
            {
                return false;
            }

            return true;
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
            GameObject prefab =  _addressableAssetLoaderForGameObjcets.LoadAsync(address).Result;

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

        private void OnDestroy()
        {
            if (_addressableAssetLoaderForGameObjcets != null)
            {
                _addressableAssetLoaderForGameObjcets.ReleaseAll();
            }
        }
    }
}