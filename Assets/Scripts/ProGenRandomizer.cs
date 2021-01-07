using UnityEngine;

[RequireComponent(typeof(ProGen))]
public class ProGenRandomizer : MonoBehaviour 
{
    [SerializeField]
    [Range(0.0f, 20.0f)]
    private float m_randomizeInSeconds = 5.0f;

    [SerializeField]
    [Range(1, 20)]
    private int m_minRows = 1;

    [SerializeField]
    [Range(1, 20)]
    private int m_maxRows = 1;

    [SerializeField]
    [Range(1, 20)]
    private int m_minColumns = 1;

    [SerializeField]
    [Range(1, 20)]
    private int m_maxColumns = 1;

    [SerializeField]
    [Range(1, 20)]
    private int m_minFloors = 1;

    [SerializeField]
    [Range(1, 20)]
    private int m_maxFloors = 1;

    [SerializeField]
    [Range(1, 20)]
    private int m_minCellUnitSize = 1;

    [SerializeField]
    [Range(1, 20)]
    private int m_maxCellUnitSize = 1;

    [SerializeField]
    private bool m_randomizeRoofInclusion = false;

    private float _randomizerTimer = 0.0f;

    private ProGen _proGen;

    private void Awake() 
    {
        _proGen = GetComponent<ProGen>();
    }

    private void Update() 
    {
        if (_randomizerTimer >= m_randomizeInSeconds)
        {
            _proGen.m_Rows = Random.Range(m_minRows, m_maxRows);
            _proGen.m_Columns = Random.Range(m_minColumns, m_maxColumns);
            _proGen.m_NumberOfFloors = Random.Range(m_minFloors, m_maxFloors);
            _proGen.m_CellUnitSize = Random.Range(m_minCellUnitSize, m_maxCellUnitSize);

            if (m_randomizeRoofInclusion)
                _proGen.m_IncludeRoof = !_proGen.m_IncludeRoof;
            
            _proGen.Generate();
            _randomizerTimer = 0.0f;
        }

        _randomizerTimer += Time.deltaTime * 1.0f;
    }
}