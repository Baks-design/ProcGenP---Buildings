using UnityEngine;

namespace Baks.Core.Theme
{
    [CreateAssetMenu(fileName = "ProGenThemeSO", menuName = "ProGen/Theme/CreateProGenTheme", order = 1)]
    public class ProGenThemeSO : ScriptableObject 
    {
        [Header("Wall Options")]
        public bool m_KeepInsideWalls = false;
        public GameObject m_WallPrefab;

        [Header("Window Options")]
        [Range(0, 1)]
        public float m_WindowPercentChance = 0.3f;
        public bool m_RandomizeWindowSelection = false;
        public GameObject[] m_WindowPrefabs;
        
        [Header("Roof Options")]
        public bool m_RandomizeRoofSelection = false;
        public bool m_IncludeRoof = false;
        public GameObject[] m_RootPrefabs;

        [Header("Corner Options")]
        public bool m_AllowCornerWalls = false;
        public GameObject m_CornerPrefab;

        [Header("Door Options")]
        public GameObject m_DoorPrefab = default;
        
        [Range(0, 1)]
        public float m_DoorPercentChance = 0.2f;

        [Header("Grid Options")]
        public bool m_RandomizerRows = false;
        public bool m_RandomizerColumns = false;

        [Range(1, 20)]
        public int m_Rows = 3;

        [Range(1, 20)]
        public int m_Columns = 3;

        [Range(1, 20)]
        public float m_CellUnitSize = 1.0f;
        
        [Range(1, 20)]
        public int m_NumberOfFloors = 1;
    }
}