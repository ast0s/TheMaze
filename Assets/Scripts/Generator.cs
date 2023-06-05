using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using System.Linq;

public partial class Generator : MonoBehaviour 
{
    public class GridCell
    {
        public int Index;
        public CellType Type;
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
    private Grid<GridCell> _grid;
    private List<Room> _rooms;
    private Delaunay _delaunay;
    private HashSet<Prim.Edge> _selectedEdges;
    private List<DoorLocation> _doors;

    private int _nextRoomIndex;
    private int _nextHallwayIndex;

    private List<RoomController> _roomControllers = new List<RoomController>();
    private List<HallwayController> _halwayControllers = new List<HallwayController>();

    public int UnitSize { get; set; } = 4;

    void Start() 
    {
        Generate();
    }

    void Generate() 
    {
        _random = new Random(_seed);
        _grid = new Grid<GridCell>(_size, Vector2Int.zero);
        _rooms = new List<Room>();

        CreateRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
        CreateDoors();
    }

    void CreateRooms() 
    {
        for (int i = 0; i < _roomCount; i++) 
        {
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

            foreach (var room in _rooms) 
            {
                if (Room.Intersect(room, buffer)) 
                {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= _size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= _size.y) 
            {
                add = false;
            }

            if (add) 
            {
                newRoom.Index = _nextRoomIndex;
                _rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size, newRoom);

                foreach (var pos in newRoom.bounds.allPositionsWithin) 
                {
                    _grid[pos].Type = CellType.Room;
                    _grid[pos].Index = _nextRoomIndex;
                }
                _nextRoomIndex++;
            }
        }
    }

    void Triangulate() 
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in _rooms) 
        {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        _delaunay = Delaunay.Triangulate(vertices);
    }

    void CreateHallways() 
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in _delaunay.Edges) 
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        _selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(_selectedEdges);

        foreach (var edge in remainingEdges) 
        {
            if (_random.NextDouble() < _loopability) 
            {
                _selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways() 
    {
        DungeonPathfinder aStar = new DungeonPathfinder(_size);

        foreach (var edge in _selectedEdges) 
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder.Node a, DungeonPathfinder.Node b) => 
            {
                var pathCost = new DungeonPathfinder.PathCost();

                pathCost.cost = Vector2Int.Distance(b.Position, endPos);

                if (_grid[b.Position].Type == CellType.Room) 
                {
                    pathCost.cost += 2;
                } else if (_grid[b.Position].Type == CellType.None) 
                {
                    pathCost.cost += 7;
                } else if (_grid[b.Position].Type == CellType.Hallway) 
                {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null) 
            {
                for (int i = 0; i < path.Count; i++) 
                {
                    var current = path[i];

                    if (_grid[current].Type == CellType.None) 
                    {
                        _grid[current].Type = CellType.Hallway;
                        _grid[current].Index = _nextHallwayIndex;
                    }

                    if (i > 0) 
                    {
                        var prev = path[i - 1];

                        var delta = current - prev;
                    }
                }
                _nextHallwayIndex++;

                foreach (var pos in path) 
                {
                    if (_grid[pos].Type == CellType.Hallway) 
                    {
                        PlaceHallway(pos);
                    }
                }
            }
        }
    }

    void CreateDoors() 
    {
        _doors = new List<DoorLocation>();
        for (int x = 0; x < _grid.Size.x; x++)
        {
            for (int y = 0; y < _grid.Size.y; y++)
            {
                var item = _grid[new Vector2Int(x, y)];                
                for (int x_offset = -1; x_offset <= 1; x_offset++)
                {
                    for (int y_offset = -1; y_offset <= 1; y_offset++)
                    {
                        if ((x_offset == 0 && y_offset == 0) || (x_offset != 0 && y_offset != 0)) continue;
                        if (x + x_offset < 0 || x + x_offset >= _grid.Size.x || y + y_offset < 0 || y + y_offset >= _grid.Size.y) continue;
                        var neighbour = _grid[new Vector2Int(x + x_offset, y + y_offset)];
                        
                        if ((item.Type == CellType.Room && neighbour.Type == CellType.Hallway)
                            || (item.Type == CellType.Hallway && neighbour.Type == CellType.Room))
                        {
                            var roomPosition = item.Type == CellType.Room ? new Vector2Int(x, y) : new Vector2Int(x + x_offset, y + y_offset);
                            var hallwayPosition = item.Type == CellType.Hallway ? new Vector2Int(x, y) : new Vector2Int(x + x_offset, y + y_offset);
                            DoorLocation door = new DoorLocation(_grid, roomPosition, hallwayPosition, 
                                _rooms.First(x => x.Index == _grid[roomPosition].Index));

                            if (!_doors.Contains(door))
                            {
                                _doors.Add(door);
                                PlaceDoor(door);
                            }
                        }
                    }
                }
            }
        }
    }

    void PlaceRoom(Vector2Int location, Vector2Int size, Room newRoom) 
    {
        RoomController room = Instantiate(_roomPrefab, new Vector3(location.x * UnitSize, 0, location.y * UnitSize), Quaternion.identity, transform);
        room.Index = newRoom.Index;
        room.Init(size.x, size.y, UnitSize);
        _roomControllers.Add(room);
    }

    void PlaceHallway(Vector2Int location)
    {
        HallwayController hallway = Instantiate(_hallwayPrefab, new Vector3(location.x * UnitSize, 0, location.y * UnitSize), Quaternion.identity, transform);
        hallway.Index = _grid[new Vector2Int(location.x, location.y)].Index;
        
        CellType left = CellType.None, up = CellType.None, right = CellType.None, down = CellType.None;

        left = _grid[new Vector2Int(location.x - 1, location.y)].Type;
        up = _grid[new Vector2Int(location.x, location.y + 1)].Type;
        right = _grid[new Vector2Int(location.x + 1, location.y)].Type;
        down = _grid[new Vector2Int(location.x, location.y - 1)].Type;

        hallway.Init(left, up, right, down, UnitSize);
        _halwayControllers.Add(hallway);
    }

    void PlaceDoor(DoorLocation door)
    {
        RoomController room = _roomControllers.First(x => x.Index == _grid[door.RoomCell].Index);
        room.DoorInit(door);
    }
}