using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;

public class Generator : MonoBehaviour {
    public enum CellType {
        None,
        Room,
        Hallway
    }

    private class Room {
        public RectInt bounds;

        public Room(Vector2Int location, Vector2Int size) {
            bounds = new RectInt(location, size);
        }

        public static bool Intersect(Room a, Room b) {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
        }
    }

    [SerializeField] private int _seed = 0;
    [SerializeField] private Vector2Int _size;
    [SerializeField] private int _roomCount;
    [SerializeField] private Vector2Int _roomMinSize;
    [SerializeField] private Vector2Int _roomMaxSize;

    [SerializeField] private RoomController _roomPrefab;
    [SerializeField] private HallwayController _hallwayPrefab;

    [SerializeField] private int _roomWeight = 2;
    [SerializeField] private int _hallwayWeight = 1;
    [SerializeField] private int _newPathWeight = 7;

    [SerializeField] private float _loopability = .2f;

    private Random _random;
    private Grid<CellType> _grid;
    private List<Room> _rooms;
    private Delaunay _delaunay;
    private HashSet<Prim.Edge> _selectedEdges;

    void Start() {
        Generate();
    }

    void Generate() {
        _random = new Random(_seed);
        _grid = new Grid<CellType>(_size, Vector2Int.zero);
        _rooms = new List<Room>();

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
    }

    void PlaceRooms() {
        for (int i = 0; i < _roomCount; i++) {
            Vector2Int location = new Vector2Int(
                _random.Next(0, _size.x),
                _random.Next(0, _size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                _random.Next(_roomMinSize.x, _roomMaxSize.x),
                _random.Next(_roomMinSize.x, _roomMaxSize.y)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in _rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= _size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= _size.y) {
                add = false;
            }

            if (add) {
                _rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    _grid[pos] = CellType.Room;
                }
            }
        }
    }

    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in _rooms) {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        _delaunay = Delaunay.Triangulate(vertices);
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in _delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        _selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(_selectedEdges);

        foreach (var edge in remainingEdges) {
            if (_random.NextDouble() < _loopability) {
                _selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways() {
        DungeonPathfinder aStar = new DungeonPathfinder(_size);

        foreach (var edge in _selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder.Node a, DungeonPathfinder.Node b) => {
                var pathCost = new DungeonPathfinder.PathCost();
                
                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (_grid[b.Position] == CellType.Room) {
                    pathCost.cost += 2;
                } else if (_grid[b.Position] == CellType.None) {
                    pathCost.cost += 7;
                } else if (_grid[b.Position] == CellType.Hallway) {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (_grid[current] == CellType.None) {
                        _grid[current] = CellType.Hallway;
                    }

                    if (i > 0) {
                        var prev = path[i - 1];

                        var delta = current - prev;
                    }
                }

                foreach (var pos in path) {
                    if (_grid[pos] == CellType.Hallway) {
                        PlaceHallway(pos);
                    }
                }
            }
        }
    }

    void PlaceRoom(Vector2Int location, Vector2Int size) {
        RoomController room = Instantiate(_roomPrefab, new Vector3(location.x, 0, location.y), Quaternion.identity, transform);
        room.Init(size.x, size.y);
    }

    void PlaceHallway(Vector2Int location) {
        HallwayController hallway = Instantiate(_hallwayPrefab, new Vector3(location.x, 0, location.y), Quaternion.identity, transform);
        
        CellType left = CellType.None, up = CellType.None, right = CellType.None, down = CellType.None;

        left = _grid[new Vector2Int(location.x - 1, location.y)];
        up = _grid[new Vector2Int(location.x, location.y + 1)];
        right = _grid[new Vector2Int(location.x + 1, location.y)];
        down = _grid[new Vector2Int(location.x, location.y - 1)];

        hallway.Init(left, up, right, down);
    }
}
