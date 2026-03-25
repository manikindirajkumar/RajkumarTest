using System.Threading.Tasks;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Generic async asset loader abstraction.
    /// Supports any Unity asset type —
    /// GameObject, Sprite, AudioClip, TextAsset etc.
    /// 
    /// Concrete implementation uses Addressables.
    /// Tests use MockAssetLoader with pre-loaded assets —
    /// no Addressables catalog needed in tests.
    /// 
    /// Usage:
    ///   IAssetLoader[GameObject] for prefabs
    ///   IAssetLoader[Sprite]     for sprites
    ///   IAssetLoader[AudioClip]  for sounds
    /// </summary>
    /// <typeparam name="T">Unity asset type to load</typeparam>
    public interface IAssetLoader<T> where T : class
    {
        /// <summary>
        /// Load asset by Addressable address.
        /// Returns cached result if already loaded.
        /// </summary>
        Task<T> LoadAsync(string address);

        /// <summary>
        /// Release loaded asset handle.
        /// Always call when asset no longer needed
        /// to prevent memory leaks.
        /// </summary>
        void Release(string address);

        /// <summary>
        /// Release all loaded asset handles.
        /// Call on scene unload or game shutdown.
        /// </summary>
        void ReleaseAll();
    }
}