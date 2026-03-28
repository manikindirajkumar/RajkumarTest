namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Central registry of all Addressable asset keys.
    /// Single place to manage asset addresses —
    /// no magic strings scattered across codebase.
    /// If an address changes, update it here only.
    /// </summary>
    public static class AddressableKeys
    {
        public static class Prefabs
        {
            public const string Player         = "Prefabs/Player";
            public const string Bullet         = "Prefabs/Bullet1";
            public const string LargeAsteroid  = "Prefabs/LargeAsteroid";
            public const string MediumAsteroid = "Prefabs/MediumAsteroid";
            public const string SmallAsteroid  = "Prefabs/SmallAsteroid";
        }
        // We can potentially add other asset types here like:
        //public static class Scenes { }
        //public static class Textures { }
        //public static class Audio { }
    }
}