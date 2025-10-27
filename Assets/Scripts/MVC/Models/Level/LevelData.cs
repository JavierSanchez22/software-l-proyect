using UnityEngine;

namespace DinoRunner.MVC.Models.Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "DinoRunner/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Dimensions")]
        public float levelLength = 200f;

        [Header("Terrain Blocks")]
        public int startBlocksCount = 1;
        public BlockData[] startBlocks;

        public BlockData[] middleBlocks;
        public int endBlocksCount = 1;
        public BlockData[] endBlocks;

        [Header("Backgrounds")]
        public BackgroundLayerData[] backgroundLayers;

        [Header("Gameplay")]
        public int targetCoins = 50;
        public float timeLimit = 300f;
    }

    [System.Serializable]
    public class BlockData
    {
        public GameObject blockPrefab;
        [Range(0f, 1f)] public float probability = 1f;
    }

    [System.Serializable]
    public class BackgroundLayerData
    {
        public string layerName;
        public BlockData[] backgroundBlocks;
        public float parallaxFactor = 0.5f;
        public float minBlockWidth = 5f;
        public float maxBlockWidth = 20f;
    }
}