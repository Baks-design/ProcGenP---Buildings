using UnityEngine;

public class RoomComponent : MonoBehaviour 
{
    [SerializeField]
    private bool m_hitGenerated = false;

    [SerializeField]
    private bool m_showRay = true;

    [SerializeField]
    public float m_rayDistance = 0.0f;

    [SerializeField]
    public Vector3 m_rayPosition = default;

    [SerializeField]
    public Vector3 offset = default;

    private void Update() 
    {
        if (m_showRay)
            Debug.DrawRay(transform.localPosition - offset + m_rayPosition, transform.TransformDirection(Vector3.up) * m_rayDistance, Color.yellow);
        
        if (m_rayDistance > 0.0f && !m_hitGenerated)
        {
            if (Physics.Raycast(transform.localPosition - offset + m_rayPosition, transform.TransformDirection(Vector3.up), out RaycastHit hit, m_rayDistance))
                m_hitGenerated = true;
        }
    }
}