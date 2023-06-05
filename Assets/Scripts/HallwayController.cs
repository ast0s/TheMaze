using System.Collections.Generic;
using UnityEngine;
using static Generator;

public class HallwayController : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _columnPrefab;
    [SerializeField] private GameObject _floorPrefab;

    public int Index { get; set; }

    private readonly List<GameObject> _columns = new List<GameObject>();
    private readonly List<GameObject> _floors = new List<GameObject>();
    private readonly List<GameObject> _walls = new List<GameObject>();

    public void Init(CellType left, CellType up, CellType right, CellType down, int unitSize)
    {
        Vector3 minZminXcorner = transform.position;

        // floor spawn
        _floors.Add(Instantiate(_floorPrefab, new Vector3(minZminXcorner.x + .5f * unitSize, minZminXcorner.y, minZminXcorner.z + .5f * unitSize), Quaternion.identity, transform));

        // column spawn
        if ((left == CellType.Hallway && up == CellType.Hallway) || (left == CellType.None && up == CellType.None))
        {
            _columns.Add(Instantiate(_columnPrefab, new Vector3(minZminXcorner.x, minZminXcorner.y, minZminXcorner.z + unitSize), Quaternion.identity, transform));
        }
        if ((left == CellType.Hallway && down == CellType.Hallway) || (left == CellType.None && down == CellType.None))
        {
            _columns.Add(Instantiate(_columnPrefab, new Vector3(minZminXcorner.x, minZminXcorner.y, minZminXcorner.z), Quaternion.identity, transform));
        }
        if ((right == CellType.Hallway && up == CellType.Hallway) || (right == CellType.None && up == CellType.None))
        {
            _columns.Add(Instantiate(_columnPrefab, new Vector3(minZminXcorner.x + unitSize, minZminXcorner.y, minZminXcorner.z + unitSize), Quaternion.identity, transform));
        }
        if ((right == CellType.Hallway && down == CellType.Hallway) || (right == CellType.None && down == CellType.None))
        {
            _columns.Add(Instantiate(_columnPrefab, new Vector3(minZminXcorner.x + unitSize, minZminXcorner.y, minZminXcorner.z), Quaternion.identity, transform));
        }

        // wall spawn
        if (left != CellType.Hallway && left != CellType.Room)
        {
            _walls.Add(Instantiate(_wallPrefab, new Vector3(minZminXcorner.x, minZminXcorner.y, minZminXcorner.z + .5f * unitSize), Quaternion.Euler(new Vector3(0, 90, 0)), transform));
        }
        if (right != CellType.Hallway && right != CellType.Room)
        {
            _walls.Add(Instantiate(_wallPrefab, new Vector3(minZminXcorner.x + unitSize, minZminXcorner.y, minZminXcorner.z + .5f * unitSize), Quaternion.Euler(new Vector3(0, 90, 0)), transform));
        }
        if (up != CellType.Hallway && up != CellType.Room)
        {
            _walls.Add(Instantiate(_wallPrefab, new Vector3(minZminXcorner.x + .5f * unitSize, minZminXcorner.y, minZminXcorner.z + unitSize), Quaternion.identity, transform));
        }
        if (down != CellType.Hallway && down != CellType.Room)
        {
            _walls.Add(Instantiate(_wallPrefab, new Vector3(minZminXcorner.x + .5f * unitSize, minZminXcorner.y, minZminXcorner.z), Quaternion.identity, transform));
        }
    }
}
