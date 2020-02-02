using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace PlayingField
{
    public class PlayingFieldGrid : MonoBehaviour
    {
        [SerializeField] private GameObject _tilePrefab = null;
        [SerializeField] private GameObject _plantPrefab = null;

        [SerializeField] private int _gridWidth = 40;
        [SerializeField] private int _gridDepth = 40;
        [SerializeField] private int _startingPlantCount = 4;

        private List<PlayingFieldTile> _tileList;

        public PlayingFieldTile[][] TileGrid;

        public PlayingFieldTile GetRandomTile(bool onlyIncludeAvailable)
        {
            if (onlyIncludeAvailable)
            {
                List<PlayingFieldTile> validTiles = _tileList.Where(x => x.AvailableForObjectPlacement).ToList();
                return validTiles.Count > 0
                    ? validTiles[Random.Range(0, validTiles.Count)]
                    : null;
            }

            return _tileList[Random.Range(0, _tileList.Count)];
        }

        public Vector3 GetCarouselPosition(bool playingFromLeft)
        {
            return TileGrid[playingFromLeft ? 1 : _gridWidth - 2][_gridDepth / 2].Position;
        }

        public Vector3 GetPlayerStartPosition(bool playingFromLeft)
        {
            int offset = _gridWidth / 4;
            PlayingFieldTile startTile = TileGrid[playingFromLeft ? offset : _gridWidth - (offset + 1)][_gridDepth / 2];
            startTile.ObstructedBy = TileBlockers.PlayerStartPoint;

            return startTile.Position;
        }

        private void Awake()
        {
            _tileList = new List<PlayingFieldTile>();

            Vector3 frontLeft = new Vector3(_tilePrefab.transform.localScale.x * (_gridWidth - 1), 0.0f, _tilePrefab.transform.localScale.z * (_gridDepth - 1)) * -0.5f;
            Vector3 step = new Vector3(_tilePrefab.transform.localScale.x, 0.0f, _tilePrefab.transform.localScale.z);

            TileGrid = new PlayingFieldTile[_gridWidth][];
            for (int x = 0; x < _gridWidth; x++)
            {
                TileGrid[x] = new PlayingFieldTile[_gridDepth];
                for (int z = 0; z < _gridDepth; z++)
                {
                    GameObject tile = Instantiate(_tilePrefab);
                    tile.name = $"GroundTile {x}:{z}";

                    tile.transform.position = new Vector3(frontLeft.x + (step.x * x), 0.0f, frontLeft.z + (step.z * z));
                    tile.transform.parent = transform;

                    TileGrid[x][z] = tile.GetComponent<PlayingFieldTile>();
                    if ((x < 2) || (z == 0) || (x >= _gridWidth - 2) || (z == _gridDepth - 1))
                    {
                        TileGrid[x][z].SetAsPathTile(Tile_Path_Colors[(x + z) % Tile_Path_Colors.Length]);
                    }
                    else
                    {
                        TileGrid[x][z].SetHealthyColour(Tile_Healthy_Colors[(x + z) % Tile_Healthy_Colors.Length]);
                    }

                    _tileList.Add(TileGrid[x][z]);
                }
            }
        }

        private static readonly Color[] Tile_Healthy_Colors = {new Color(0.0f, 0.6f, 0.0f), new Color(0.0f, 0.4f, 0.0f)};
        private static readonly Color[] Tile_Path_Colors = {new Color(0.6f, 0.6f, 0.6f), new Color(0.4f, 0.4f, 0.4f)};
    }
}