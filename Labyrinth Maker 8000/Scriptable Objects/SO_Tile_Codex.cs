using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Codex", menuName = "Labyrinth Maker 8000/Tile Codex")]
public class SO_Tile_Codex : ScriptableObject
{
    [Tooltip("Holds tile wrappers, populate in-editor.")]
    [SerializeField]
    List<Labyrinth_Tile> tiles = default;

    public List<Labyrinth_Tile> Tiles
    {
        get { return tiles; }
    }

    [System.Serializable]
    public class Labyrinth_Tile
    {
        [Tooltip("Width in number of nodes in the X-axis. Actual width can be calculated as width * cellSize.")]
        [SerializeField]
        int widthX = 1;

        [Tooltip("Length in number of nodes in the Z-axis. Actual length can be calculated as length * cellSize.")]
        [SerializeField]
        int lengthZ = 1;

        [Tooltip("Minimum number of cells the tile should always be the edges.")]
        [SerializeField]
        int edgeBuffer = 0;

        [Tooltip("Number of times this tile should occur in the labyrinth. To exclude a tile, set this to 0.\nNote, if the labyrinth has limited room for tiles, occurences may be less than this number.")]
        [SerializeField]
        int occurences = 0;

        [Tooltip("Should the tile be scaled automatically in the X and Z axes? Useful when re-using tiles between different labyrinth scales.")]
        [SerializeField]
        bool autoScale = true;

        [Tooltip("The prefab to replace the node with.")]
        [SerializeField]
        GameObject tilePrefab = default;

        public int Width
        {
            get { return widthX; }
        }

        public int Length
        {
            get { return lengthZ; }
        }

        public int EdgeBuffer
        {
            get { return edgeBuffer; }
        }

        public int Occurences
        {
            get { return occurences; }
        }

        public bool AutoScale
        {
            get { return autoScale; }
        }

        public GameObject TilePrefab
        {
            get { return tilePrefab; }
        }
    }
}
