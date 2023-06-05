using UnityEngine;
using System;
using static Generator;

public class DoorLocation : IEquatable<DoorLocation>
{
    private Grid<GridCell> _grid;
    public Vector2Int RoomCell { get; private set; }
    public Vector2Int HallwayCell { get; private set; }
    public Room Room { get; private set; }

    public DoorLocation(Grid<GridCell> grid, Vector2Int roomCell, Vector2Int hallwayCell, Room room)
    {
        _grid = grid;
        RoomCell = roomCell;
        HallwayCell = hallwayCell;
        Room = room;
    }

    public bool Equals(DoorLocation other)
    {
        return _grid[RoomCell].Index == _grid[other.RoomCell].Index
            && _grid[HallwayCell].Index == _grid[other.HallwayCell].Index;
    }

    public override bool Equals(object other)
    {
        if (other is DoorLocation) return Equals(other as DoorLocation);
        return false;
    }
}