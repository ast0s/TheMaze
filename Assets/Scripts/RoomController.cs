using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _columnPrefab;
    [SerializeField] private GameObject _arcPrefab;
    [SerializeField] private GameObject _floorPrefab;

    private readonly List<GameObject> _columns = new List<GameObject>();
    private readonly List<GameObject> _floors = new List<GameObject>();
    private readonly List<GameObject> _walls = new List<GameObject>();

    public void Init(int xWidth, int zLength)
    {
        Vector3 minZminXcorner = transform.position;

        for (float x = minZminXcorner.x; x <= xWidth + minZminXcorner.x; x++)
        {
            for (float z = minZminXcorner.z; z <= zLength + minZminXcorner.z; z++)
            {
                if (x != xWidth + minZminXcorner.x && z != zLength + minZminXcorner.z)
                {
                    // floor spawn
                    _floors.Add(Instantiate(_floorPrefab, new Vector3(x + .5f, minZminXcorner.y, z + .5f), Quaternion.identity, transform));
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
                        _x += .5f;
                    }
                    else if ((z == zLength + minZminXcorner.z) && !(x == minZminXcorner.x))
                    {
                        _x -= .5f;
                    }
                    if ((x == minZminXcorner.x) && !(z == minZminXcorner.z))
                    {
                        _z -= .5f;
                        rotation.y = 90;
                    }
                    else if ((x == xWidth + minZminXcorner.x) && !(z == zLength + minZminXcorner.z))
                    {
                        _z += .5f;
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

    public void DoorsInit()
    {

    }
}
