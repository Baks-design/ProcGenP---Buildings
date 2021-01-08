using UnityEngine;

[System.Serializable]
public class Floor
{    
    public Room[,] Rooms;

    public int FloorNumber { get; private set; }
    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public Floor(int floorNumber, Room[,] rooms)
    {
        FloorNumber = floorNumber;
        this.Rooms = rooms;
        Rows = rooms.GetLength(0);
        Columns = rooms.GetLength(1);
    }
}
