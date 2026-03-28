using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    [CreateAssetMenu(
        fileName = "WaveConfig",
        menuName = "Asteroids/WaveConfig")]
    public class WaveConfig : ScriptableObject, IWaveConfig
    {
        [SerializeField] private int   _initialAsteroidCount = 4;
        [SerializeField] private int   _asteroidsPerWave     = 2;
        [SerializeField] private int   _maxAsteroidCount     = 12;
        [SerializeField] private float _minSpeed             = 1f;
        [SerializeField] private float _maxSpeed             = 3f;
        [SerializeField] private float _speedIncreasePerWave = 0.2f;

        // Add to WaveConfig ScriptableObject
        [Header("Score")]
        [SerializeField] private int _largeAsteroidScore  = 20;
        [SerializeField] private int _mediumAsteroidScore = 50;
        [SerializeField] private int _smallAsteroidScore  = 100;

      
        
        public int   InitialAsteroidCount => _initialAsteroidCount;
        public float MinSpeed             => _minSpeed;
        public float MaxSpeed             => _maxSpeed;

        public int GetAsteroidCountForWave(int wave)
        {
            int count = _initialAsteroidCount +
                        (wave - 1) * _asteroidsPerWave;
            return Mathf.Min(count, _maxAsteroidCount);
        }

        public float GetMaxSpeedForWave(int wave)
        {
            return _maxSpeed +
                   (wave - 1) * _speedIncreasePerWave;
        }

        public int GetScoreForSize(AsteroidSize size) => size switch
        {
            AsteroidSize.Large  => _largeAsteroidScore,
            AsteroidSize.Medium => _mediumAsteroidScore,
            AsteroidSize.Small  => _smallAsteroidScore,
            _                   => 0
        };
    }
}