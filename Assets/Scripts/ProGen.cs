using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Baks.Core.Components;
using Baks.Core.Theme;

namespace Baks.Core
{
    public class ProGen : MonoBehaviour
    {
        [SerializeField]
        private ProGenThemeSO m_proGenThemeSO = default;

        private int _prefabCounter = 0;
        private Floor[] _floors;
        private List<GameObject> _rooms = new List<GameObject>();

        public ProGenThemeSO ProGenThemeSO => m_proGenThemeSO;
        private bool CanFlagCornerWalls => m_proGenThemeSO.m_Rows >= 2
                                        && m_proGenThemeSO.m_CornerPrefab != null
                                        && m_proGenThemeSO.m_AllowCornerWalls;

        private void Awake() => Generate();

        public void Generate()
        {
            _prefabCounter = 0;

            // clear
            Clear();

            // create data structure
            BuildDataStructure();

            // generate prefabs
            Render();

            // removes inside walls
            if (!m_proGenThemeSO.m_KeepInsideWalls)
                RemoveInsideWalls();
        }

        private void Clear()
        {
            for (int i = 0; i < _rooms.Count; i++)
                DestroyImmediate(_rooms[i]);

            _rooms.Clear();
        }

        private void BuildDataStructure()
        {
            _floors = new Floor[m_proGenThemeSO.m_NumberOfFloors];

            int floorCount = 0;
            
            int intialRows = m_proGenThemeSO.m_Rows;
            int initialColumns = m_proGenThemeSO.m_Columns;

            foreach (Floor floor in _floors)
            {
                Room[,] rooms = new Room[intialRows, initialColumns];

                for (int row = 0; row < intialRows; row++)
                {
                    for (int column = 0; column < initialColumns; column++)
                    {
                        var roomPosition = new Vector3(row * m_proGenThemeSO.m_CellUnitSize, floorCount, column * m_proGenThemeSO.m_CellUnitSize);
                        rooms[row, column] = new Room(roomPosition, m_proGenThemeSO.m_IncludeRoof ? (floorCount == _floors.Length - 1) : false);
                        
                        rooms[row, column].Walls[0] = new Wall(roomPosition, Quaternion.Euler(0, 0, 0));
                        rooms[row, column].Walls[1] = new Wall(roomPosition, Quaternion.Euler(0, 90, 0));
                        rooms[row, column].Walls[2] = new Wall(roomPosition, Quaternion.Euler(0, 180, 0));
                        rooms[row, column].Walls[3] = new Wall(roomPosition, Quaternion.Euler(0, -90, 0));

                        FlagCornerWalls(rooms, roomPosition, row, column);

                        if (m_proGenThemeSO.m_RandomizerRows || m_proGenThemeSO.m_RandomizerColumns)
                            rooms[row, column].HasRoof = true;
                    }
                }

                _floors[floorCount] = new Floor(floorCount++, rooms);

                // rule if random column or rows let´s experiment with different values
                if (m_proGenThemeSO.m_RandomizerRows)
                    intialRows = Random.Range(1, m_proGenThemeSO.m_Rows);
                
                if (m_proGenThemeSO.m_RandomizerColumns)
                    initialColumns = Random.Range(1, m_proGenThemeSO.m_Columns);
            }
        }

        private void FlagCornerWalls(Room[,] rooms, Vector3 roomPosition, int row, int column)
        {
            if (CanFlagCornerWalls)
            {
                if (row == 0 && column == 0) // left upper corner
                {   
                    rooms[row, column].Walls[0].WallCornerTypeSelected = Wall.WallCornerType.LeftBottom;
                }
                else if (row == 0 && column == m_proGenThemeSO.m_Columns - 1) // left bottom corner
                { 
                    rooms[row, column].Walls[1].WallCornerTypeSelected = Wall.WallCornerType.LeftUpper;
                }
                else if (row == m_proGenThemeSO.m_Rows - 1 && column == 0) // right upper corner
                { 
                    rooms[row, column].Walls[2].WallCornerTypeSelected = Wall.WallCornerType.RightBottom;
                }
                else if (row == m_proGenThemeSO.m_Rows - 1 && column == m_proGenThemeSO.m_Columns - 1) //right bottom corner
                {
                    rooms[row, column].Walls[3].WallCornerTypeSelected = Wall.WallCornerType.RightUpper;
                }
            }
        }

        private void Render()
        {
            foreach (Floor floor in _floors)
            {
                for (int row = 0; row < floor.Rows; row++)
                {
                    for (int column = 0; column < floor.Columns; column++)
                    {
                        Room room = floor.Rooms[row, column];
                        room.FloorNumber = floor.FloorNumber;

                        GameObject roomGo = new GameObject($"Room_{row}_{column}");
                        _rooms.Add(roomGo);
                        roomGo.transform.parent = transform;

                        // corner logic takes precedence
                        if (room.HasRoundedCorner)
                            RoomPlacementWithRoundedCorners(room, roomGo);
                        else
                        {
                            if (floor.FloorNumber == 0)
                                RoomPlacement(Random.Range(0.0f, 1.0f) <= m_proGenThemeSO.m_DoorPercentChance ? 
                                                                          m_proGenThemeSO.m_DoorPrefab : m_proGenThemeSO.m_WallPrefab, room, roomGo);
                            else
                            {
                                // rule: if window coverage percent is within
                                // threshold add a window otherwise add a basic wall
                                if (Random.Range(0.0f, 1.0f) <= m_proGenThemeSO.m_WindowPercentChance)
                                {
                                    if (ProGenThemeSO.m_RandomizeWindowSelection)
                                    {
                                        int windowIndex = Random.Range(0, m_proGenThemeSO.m_WindowPrefabs.Length);
                                        RoomPlacement(m_proGenThemeSO.m_WindowPrefabs[windowIndex], room, roomGo);
                                    }
                                    else
                                        RoomPlacement(m_proGenThemeSO.m_WindowPrefabs[0], room, roomGo);
                                }
                                else
                                    RoomPlacement(m_proGenThemeSO.m_WallPrefab, room, roomGo);
                            }
                        }
                    }
                }
            }
        }

        private void RoomPlacementWithRoundedCorners(Room room, GameObject roomGo)
        {
            if (room.Walls.Any(w => w.WallCornerTypeSelected == Wall.WallCornerType.LeftBottom))
            {
                SpawnPrefab(m_proGenThemeSO.m_CornerPrefab, roomGo.transform, room.Walls[0].Position, room.Walls[0].Rotation);
                SpawnPrefab(m_proGenThemeSO.m_WallPrefab, roomGo.transform, room.Walls[2].Position, room.Walls[2].Rotation);
                SpawnPrefab(m_proGenThemeSO.m_WallPrefab, roomGo.transform, room.Walls[3].Position, room.Walls[3].Rotation);
            }
            else if (room.Walls.Any(w => w.WallCornerTypeSelected == Wall.WallCornerType.LeftUpper))
            {
                SpawnPrefab(m_proGenThemeSO.m_CornerPrefab, roomGo.transform, room.Walls[1].Position, room.Walls[1].Rotation);
                SpawnPrefab(m_proGenThemeSO.m_WallPrefab, roomGo.transform, room.Walls[0].Position, room.Walls[0].Rotation);
                SpawnPrefab(m_proGenThemeSO.m_WallPrefab, roomGo.transform, room.Walls[3].Position, room.Walls[3].Rotation);
            }
            else if (room.Walls.Any(w => w.WallCornerTypeSelected == Wall.WallCornerType.RightUpper))
            {
                SpawnPrefab(m_proGenThemeSO.m_CornerPrefab, roomGo.transform, room.Walls[2].Position, room.Walls[2].Rotation);
                SpawnPrefab(m_proGenThemeSO.m_WallPrefab, roomGo.transform, room.Walls[0].Position, room.Walls[0].Rotation);
                SpawnPrefab(m_proGenThemeSO.m_WallPrefab, roomGo.transform, room.Walls[1].Position, room.Walls[1].Rotation);
            }
            else if (room.Walls.Any(w => w.WallCornerTypeSelected == Wall.WallCornerType.RightBottom))
            {
                SpawnPrefab(m_proGenThemeSO.m_CornerPrefab, roomGo.transform, room.Walls[3].Position, room.Walls[3].Rotation);
                SpawnPrefab(m_proGenThemeSO.m_WallPrefab, roomGo.transform, room.Walls[2].Position, room.Walls[2].Rotation);
                SpawnPrefab(m_proGenThemeSO.m_WallPrefab, roomGo.transform, room.Walls[1].Position, room.Walls[1].Rotation);
            }
        }

        private void RoomPlacement(GameObject prefab, Room room, GameObject roomGo)
        {
            SpawnPrefab(prefab, roomGo.transform, room.Walls[0].Position, room.Walls[0].Rotation);
            SpawnPrefab(prefab, roomGo.transform, room.Walls[1].Position, room.Walls[1].Rotation);
            SpawnPrefab(prefab, roomGo.transform, room.Walls[2].Position, room.Walls[2].Rotation);
            SpawnPrefab(prefab, roomGo.transform, room.Walls[3].Position, room.Walls[3].Rotation);

            AddRoof(room, roomGo);
        }

        private void AddRoof(Room room, GameObject roomGo)
        {
            // rule: if we need to randomize roof and we´re at the top
            if (m_proGenThemeSO.m_RandomizeRoofSelection && room.FloorNumber == _floors.Count() - 1)
            {
                int roofIndex = Random.Range(0, m_proGenThemeSO.m_RootPrefabs.Length);
                SpawnPrefab(m_proGenThemeSO.m_RootPrefabs[roofIndex], roomGo.transform, room.Walls[0].Position, room.Walls[0].Rotation);
            }
            else
                SpawnPrefab(m_proGenThemeSO.m_RootPrefabs[0], roomGo.transform, room.Walls[0].Position, room.Walls[0].Rotation);
        }

        private void SpawnPrefab(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            var gameObject = Instantiate(prefab, transform.position + position, rotation);
            gameObject.transform.parent = parent;
            gameObject.AddComponent<WallComponent>();
            gameObject.name = $"{gameObject.name}_{_prefabCounter}";
            
            _prefabCounter++;
        }

        private void RemoveInsideWalls()
        {
            var wallComponents = GameObject.FindObjectsOfType<WallComponent>();
            var childs = wallComponents.Select(c => c.transform.GetChild(0).position.ToString()).ToList();
            var dupPositions = childs.GroupBy(c => c)
                .Where(c => c.Count() > 1)
                .Select(grp => grp.Key)
                .ToList();
            
            foreach (WallComponent w in wallComponents)
            {
                var childTransform = w.transform.GetChild(0);
                if (dupPositions.Contains(childTransform.position.ToString()))
                    DestroyImmediate(childTransform.gameObject);
            }
        }
    }
}