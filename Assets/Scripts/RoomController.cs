using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _columnPrefab;
    [SerializeField] private GameObject _entrancePrefab;
    [SerializeField] private GameObject _floorPrefab;

    public int Index { get; set; }

    private int _unitSize;
    private readonly List<GameObject> _columns = new List<GameObject>();
    private readonly List<GameObject> _floors = new List<GameObject>();
    private readonly List<GameObject> _walls = new List<GameObject>();
    private readonly List<GameObject> _entrances = new List<GameObject>();

    public void Init(int xWidth, int zLength, int unitSize)
    {
        _unitSize = unitSize;
        xWidth *= unitSize;
        zLength *= unitSize;

        Vector3 minZminXcorner = transform.position;

        for (float x = minZminXcorner.x; x <= xWidth + minZminXcorner.x; x += unitSize)
        {
            for (float z = minZminXcorner.z; z <= zLength + minZminXcorner.z; z += unitSize)
            {
                if (x != xWidth + minZminXcorner.x && z != zLength + minZminXcorner.z)
                {
                    // floor spawn
                    _floors.Add(Instantiate(_floorPrefab, new Vector3(x + .5f * unitSize, minZminXcorner.y, z + .5f * unitSize), Quaternion.identity, transform));
                }

                if ((x == minZminXcorner.x || x == xWidth + minZminXcorner.x)
                    || (z == minZminXcorner.z || z == zLength + minZminXcorner.z))
                {
                    // wall spawn
                    float _x = x;
                    float _z = z;
                    Vector3 rotation = new Vector3();
                    if ((z == minZminXcorner.z) && !(x == xWidth + minZminXcorner.x))
                    {
                        _x += .5f * unitSize;
                    }
                    else if ((z == zLength + minZminXcorner.z) && !(x == minZminXcorner.x))
                    {
                        _x -= .5f * unitSize;
                    }
                    if ((x == minZminXcorner.x) && !(z == minZminXcorner.z))
                    {
                        _z -= .5f * unitSize;
                        rotation.y = 90;
                    }
                    else if ((x == xWidth + minZminXcorner.x) && !(z == zLength + minZminXcorner.z))
                    {
                        _z += .5f * unitSize;
                        rotation.y = 90;
                    }
                    _walls.Add(Instantiate(_wallPrefab, new Vector3(_x, minZminXcorner.y, _z), Quaternion.Euler(rotation), transform));
                }

                if ((x == minZminXcorner.x || x == xWidth + minZminXcorner.x)
                    || (z == minZminXcorner.z || z == zLength + minZminXcorner.z))
                {
                    // column spawn
                    _columns.Add(Instantiate(_columnPrefab, new Vector3(x, minZminXcorner.y, z), Quaternion.identity, transform));
                }
            }
        }
    }

    public void DoorInit(DoorLocation door)
    {
        var roomLocalInt = door.RoomCell - door.Room.bounds.position;
        var hallwayLocalInt = door.HallwayCell - door.Room.bounds.position;

        Vector3 roomLocal = new Vector3(roomLocalInt.x * _unitSize, 0, roomLocalInt.y * _unitSize);
        Vector3 hallwayLocal = new Vector3(hallwayLocalInt.x * _unitSize, 0, hallwayLocalInt.y * _unitSize);
        Vector3 center = ((roomLocal + hallwayLocal) / 2.0f) + new Vector3(_unitSize / 2.0f, 0, _unitSize / 2.0f);
        Vector3 roomSize = new Vector3(door.Room.bounds.size.x * _unitSize, 0, door.Room.bounds.size.y * _unitSize);
        
        Vector3 doorPosition = transform.position + center;
        Vector3 doorRotation = new Vector3();
        
        if (center.x == roomSize.x || center.x == 0) // right or left wall
        {
            doorRotation.y = 90;
        }
        
        GameObject wall = _walls.FirstOrDefault(x => x.transform.position == doorPosition);
        if (wall != null)   
        {
            _walls.Remove(wall);
            Destroy(wall);
        }
        _entrances.Add(Instantiate(_entrancePrefab, doorPosition, Quaternion.Euler(doorRotation), transform));
    }
}
