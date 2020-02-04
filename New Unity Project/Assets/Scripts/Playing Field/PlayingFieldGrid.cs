using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace PlayingField
{
    public class PlayingFieldGrid : MonoBehaviour
    {
        [SerializeField] private GameObject _tilePrefab = null;
        [SerializeField] private GameObject _wallPrefab = null;
        [SerializeField] private GameObject _barrierPrefab = null;

        [SerializeField] private int _gridWidth = 40;
        [SerializeField] private int _gridDepth = 40;
        [SerializeField] private int _startingPlantCount = 4;

        private PlayingFieldTile[][] _tileGrid;
        private List<PlayingFieldTile> _tileList;

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

        public PlayingFieldTile GetRandomClearPatch()
        {
            PlayingFieldTile center = GetRandomTile(true);
            bool isClear = true;

            if (center != null)
            {
                for (int x = center.GridX - 2; x < center.GridX + 3; x++)
                {
                    for (int z = center.GridZ - 2; z < center.GridZ + 3; z++)
                    {
                        if ((x >= 0) && (z >= 0) && (x < _gridWidth) && (z < _gridDepth))
                        {
                            isClear &= _tileGrid[x][z].ObstructedBy != TileBlockers.Plant;
                        }
                    }
                }
            }

            return isClear ? center : null;
        }

        public Vector3 GetCarouselPosition(bool playingFromLeft)
        {
            return _tileGrid[playingFromLeft ? 2 : _gridWidth - 3][_gridDepth / 2].Position;
        }

        public Vector3 GetPlayerStartPosition(bool playingFromLeft)
        {
            int offset = _gridWidth / 4;
            PlayingFieldTile startTile = _tileGrid[playingFromLeft ? offset : _gridWidth - (offset + 1)][_gridDepth / 2];
            startTile.ObstructedBy = TileBlockers.PlayerStartPoint;

            return startTile.Position;
        }

        private void Awake()
        {
            _tileList = new List<PlayingFieldTile>();

            Vector3 frontLeft = new Vector3(_tilePrefab.transform.localScale.x * (_gridWidth - 1), 0.0f, _tilePrefab.transform.localScale.z * (_gridDepth - 1)) * -0.5f;
            Vector3 step = new Vector3(_tilePrefab.transform.localScale.x, 0.0f, _tilePrefab.transform.localScale.z);

            _tileGrid = new PlayingFieldTile[_gridWidth][];
            for (int x = 0; x < _gridWidth; x++)
            {
                _tileGrid[x] = new PlayingFieldTile[_gridDepth];
                for (int z = 0; z < _gridDepth; z++)
                {
                    GameObject tile = Instantiate(_tilePrefab);
                    tile.name = $"GroundTile {x}:{z}";

                    tile.transform.position = new Vector3(frontLeft.x + (step.x * x), 0.0f, frontLeft.z + (step.z * z));
                    tile.transform.parent = transform;

                    _tileGrid[x][z] = tile.GetComponent<PlayingFieldTile>();
                    _tileGrid[x][z].GridX = x;
                    _tileGrid[x][z].GridZ = z;
                    if ((x < 2) || (z == 0) || (x >= _gridWidth - 2) || (z == _gridDepth - 1))
                    {
                        _tileGrid[x][z].SetAsPathTile(Tile_Path_Colors[(x + z) % Tile_Path_Colors.Length]);
                    }
                    else
                    {
                        _tileGrid[x][z].SetHealthyColour(Tile_Healthy_Colors[(x + z) % Tile_Healthy_Colors.Length]);
                    }

                    _tileList.Add(_tileGrid[x][z]);
                }

                CreateWall(new Vector3(frontLeft.x + (step.x * x), 0.0f, frontLeft.z + (step.z * _gridDepth)), "North wall", x);
            }

            for (int z = 0; z < _gridDepth + 1; z++)
            {
                CreateWall(new Vector3(frontLeft.x + (step.x * -1), 0.0f, frontLeft.z + (step.z * z)), "West wall", z);
                CreateWall(new Vector3(frontLeft.x + (step.x * _gridWidth), 0.0f, frontLeft.z + (step.z * z)), "East wall", z);
            }

            CreateBarrier(_barrierPrefab, _tileGrid[0][0].Position + new Vector3(-1.0f, 0.0f, -1.5f), _tileGrid[0][_gridDepth - 1].Position + new Vector3(-1.0f, 0.0f, 1.5f));
            CreateBarrier(_barrierPrefab, _tileGrid[_gridWidth - 1][0].Position + new Vector3(1.0f, 0.0f, -1.5f), _tileGrid[_gridWidth - 1][_gridDepth - 1].Position + new Vector3(1.0f, 0.0f, 1.5f));
            CreateBarrier(_barrierPrefab, _tileGrid[0][0].Position + new Vector3(-0.5f, 0.0f, -1.0f), _tileGrid[_gridWidth - 1][0].Position + new Vector3(0.5f, 0.0f, -1.0f));
            CreateBarrier(_barrierPrefab, _tileGrid[0][_gridDepth - 1].Position + new Vector3(-0.5f, 0.0f, 1.0f), _tileGrid[_gridWidth - 1][_gridDepth - 1].Position + new Vector3(0.5f, 0.0f, 1.0f));
        }

        private void CreateBarrier(GameObject prefab, Vector3 endOne, Vector3 endTwo)
        {
            GameObject wall = Instantiate(prefab);

            Transform wallTransform = wall.transform;

            Vector3 position = (endOne.x == endTwo.x)
                ? new Vector3(endOne.x, 0.0f, 0.0f)
                : new Vector3(0.0f, 0.0f, endOne.z);

            Vector3 scale = (endOne.x == endTwo.x)
                ? new Vector3(1.0f, wallTransform.localScale.y, Mathf.Abs(endOne.z - endTwo.z))
                : new Vector3(Mathf.Abs(endOne.x - endTwo.x), wallTransform.localScale.y, 1.0f);

            wallTransform.parent = transform;
            wallTransform.position = position;
            wallTransform.localScale = scale;
        }

        private void CreateWall(Vector3 position, string wallName, int segmentIndex)
        {
            GameObject wall = Instantiate(_wallPrefab);
            wall.name = $"{wallName} {segmentIndex}";
            wall.transform.position = position;
        }

        private static readonly Color[] Tile_Healthy_Colors = {new Color(0.0f, 0.6f, 0.0f), new Color(0.0f, 0.4f, 0.0f)};
        private static readonly Color[] Tile_Path_Colors = {new Color(0.6f, 0.6f, 0.6f), new Color(0.4f, 0.4f, 0.4f)};
    }
}