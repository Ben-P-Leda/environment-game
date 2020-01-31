using UnityEngine;

namespace PlayingField
{
    public class PlayingFieldGrid : MonoBehaviour
    {
        [SerializeField] private GameObject _tilePrefab = null;
        [SerializeField] private int _gridWidth = 40;
        [SerializeField] private int _gridDepth = 40;

        private PlayingFieldTile[][] _tileGrid;

        public Vector3 GetRandomTileCenter()
        {
            return _tileGrid[Random.Range(0, _gridWidth)][Random.Range(0, _gridDepth)].Position;
        }

        private void Awake()
        {
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
                    _tileGrid[x][z].SetHealthyColour(Tile_Healthy_Colors[(x + z) % Tile_Healthy_Colors.Length]);
                }
            }
        }

        private static readonly Color[] Tile_Healthy_Colors = {new Color(0.0f, 0.6f, 0.0f), new Color(0.0f, 0.4f, 0.0f)};
    }
}