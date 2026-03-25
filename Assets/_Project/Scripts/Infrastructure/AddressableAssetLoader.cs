using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Generic Addressables loader.
    /// One implementation handles all asset types.
    /// 
    /// Examples:
    ///   new AddressableAssetLoader[GameObject]()
    ///   new AddressableAssetLoader[Sprite]()
    ///   new AddressableAssetLoader[AudioClip]()
    /// </summary>
    public class AddressableAssetLoader<T>
        : IAssetLoader<T> where T : class
    {
        private readonly Dictionary<string,
            AsyncOperationHandle<T>> _handles
            = new Dictionary<string,
                AsyncOperationHandle<T>>();

        public async Task<T> LoadAsync(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(
                    nameof(address),
                    "Address cannot be null or empty.");

            // Return cached if already loaded
            if (_handles.TryGetValue(
                address, out var cachedHandle))
            {
                if (cachedHandle.Status ==
                    AsyncOperationStatus.Succeeded)
                    return cachedHandle.Result;
            }

            // Load from Addressables
            var handle = Addressables
                .LoadAssetAsync<T>(address);

            _handles[address] = handle;
            await handle.Task;

            if (handle.Status !=
                AsyncOperationStatus.Succeeded)
            {
                Debug.LogError(
                    $"[AddressableAssetLoader] " +
                    $"Failed to load: {address} " +
                    $"Type: {typeof(T).Name}");
                return null;
            }

            return handle.Result;
        }

        public void Release(string address)
        {
            if (!_handles.TryGetValue(address,
                out var handle)) return;

            Addressables.Release(handle);
            _handles.Remove(address);
        }

        public void ReleaseAll()
        {
            foreach (var handle in _handles.Values)
                Addressables.Release(handle);

            _handles.Clear();
        }
    }
}