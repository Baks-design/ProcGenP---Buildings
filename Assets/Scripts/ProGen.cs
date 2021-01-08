using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Baks.Core.Components;

namespace Baks.Core
{
    public class ProGen : MonoBehaviour
    {
        [Header("Prefab Options")]
        [SerializeField]
        private GameObject m_wallPrefab = default;

        [SerializeField]
        private GameObject[] m_rootPrefabs = default;

        [SerializeField]
        private bool m_randomizeRoofSelection = false;

        [SerializeField]
        private GameObject[] m_windowPrefabs = default;

        [SerializeField]
        private bool m_randomizeWindowSelection = false;

        [SerializeField]
        private GameObject m_doorPrefab = default;

        [SerializeField]
        public bool m_IncludeRoof = false;

        [SerializeField]
        private bool m_keepInsideWalls = false;

        [SerializeField]
        [Range(0, 1)]
        public float m_WindowPercentChance = 0.3f;

        [SerializeField]
        [Range(0, 1)]
        public float m_DoorPercentChance = 0.2f;

        [Header("Grid Options")]
        [SerializeField]
        [Range(1, 20)]
        public int m_Rows = 3;

        [SerializeField]
        [Range(1, 20)]
        public int m_Columns = 3;

        [SerializeField]
        public bool m_RandomizerRows = false;

        [SerializeField]
        public bool m_RandomizerColumns = false;

        [SerializeField]
        [Range(1, 20)]
        public float m_CellUnitSize = 1.0f;

        [SerializeField]
        [Range(1, 20)]
        public int m_NumberOfFloors = 1;

        private int _prefabCounter = 0;
        private Floor[] _floors;
        //TODO: fix a bug with creating rooms and unable to delete them
        private List<GameObject> _rooms = new List<GameObject>();

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
            if (!m_keepInsideWalls)
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
            _floors = new Floor[m_NumberOfFloors];

            int floorCount = 0;

            int intialRows = m_Rows;
            int initialColumns = m_Columns;

            foreach (Floor floor in _floors)
            {
                Room[,] rooms = new Room[intialRows, initialColumns];

                for (int row = 0; row < intialRows; row++)
                {
                    for (int column = 0; column < initialColumns; column++)
                    {
                        var roomPosition = new Vector3(row * m_CellUnitSize, floorCount, column * m_CellUnitSize);
                        rooms[row, column] = new Room(roomPosition, m_IncludeRoof ? (floorCount == _floors.Length - 1) : false);
                        rooms[row, column].Walls[0] = new Wall(roomPosition, Quaternion.Euler(0, 0, 0));
                        rooms[row, column].Walls[1] = new Wall(roomPosition, Quaternion.Euler(0, 90, 0));
                        rooms[row, column].Walls[2] = new Wall(roomPosition, Quaternion.Euler(0, 180, 0));
                        rooms[row, column].Walls[3] = new Wall(roomPosition, Quaternion.Euler(0, -90, 0));
                    
                        if (m_RandomizerRows || m_RandomizerColumns)
                            rooms[row, column].HasRoof = true;
                    }
                }

                _floors[floorCount] = new Floor(floorCount++, rooms);

                // rule if random column or rows let´s experiment with different values
                if (m_RandomizerRows)
                    intialRows = Random.Range(1, m_Rows);
                
                if (m_RandomizerColumns)
                    initialColumns = Random.Range(1, m_Columns);
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
                        //TODO: move this to BuildStructure
                        room.FloorNumber = floor.FloorNumber;

                        GameObject roomGO = new GameObject($"Room_{row}_{column}");
                        _rooms.Add(roomGO);
                        roomGO.transform.parent = transform;

                        if (floor.FloorNumber == 0)
                            RoomPlacement(Random.Range(0.0f, 1.0f) <= m_DoorPercentChance ? m_doorPrefab : m_wallPrefab, room, roomGO);
                        else
                        {
                            // rule: if window coverage percent is within
                            // threshold add a window otherwise add a basic wall
                            if (Random.Range(0.0f, 1.0f) <= m_WindowPercentChance)
                            {
                                if (m_randomizeWindowSelection)
                                {
                                    int windowIndex = Random.Range(0, m_windowPrefabs.Length);
                                    RoomPlacement(m_windowPrefabs[windowIndex], room, roomGO);
                                }
                                else
                                    RoomPlacement(m_windowPrefabs[0], room, roomGO);
                            }
                            else
                                RoomPlacement(m_wallPrefab, room, roomGO);
                        }
                    }
                }
            }
        }

        private void RoomPlacement(GameObject prefab, Room room, GameObject roomGO)
        {
            SpawnPrefab(prefab, roomGO.transform, room.Walls[0].Position, room.Walls[0].Rotation);
            SpawnPrefab(prefab, roomGO.transform, room.Walls[1].Position, room.Walls[1].Rotation);
            SpawnPrefab(prefab, roomGO.transform, room.Walls[2].Position, room.Walls[2].Rotation);
            SpawnPrefab(prefab, roomGO.transform, room.Walls[3].Position, room.Walls[3].Rotation);

            if (room.HasRoof)
            {
                // rule if we need to randomize roof and we´re at the top
                if (m_randomizeRoofSelection && room.FloorNumber == _floors.Count() - 1)
                {
                    int roofIndex = Random.Range(0, m_rootPrefabs.Length);
                    SpawnPrefab(m_rootPrefabs[roofIndex], roomGO.transform, room.Walls[0].Position, room.Walls[0].Rotation);
                }
                else
                    SpawnPrefab(m_rootPrefabs[0], roomGO.transform, room.Walls[0].Position, room.Walls[0].Rotation);
            }
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