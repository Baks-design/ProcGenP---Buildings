using System.Linq;
using UnityEngine;

public class Room
{
    public bool HasRoof { get; set; }
    public int FloorNumber { get; set; }
    public RoomRay RoomRay { get; private set; }
    public Wall[] Walls { get; set; } = new Wall[4];

    private Vector2 _position;

    public bool HasRoundedCorner => Walls.Where(w => w.WallCornerTypeSelected != Wall.WallCornerType.Normal).Any();

    public Room(Vector2 position, bool hasRoof = false, RoomRay roomRay = null)
    {
        this._position = position;
        this.HasRoof = hasRoof;
        this.RoomRay = roomRay;
    }

    public Vector2 RoomPosition => this._position;
}
