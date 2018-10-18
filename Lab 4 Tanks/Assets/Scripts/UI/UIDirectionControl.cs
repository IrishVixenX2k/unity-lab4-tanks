using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;  


    private Quaternion m_RelativeRotation;     


    private void Start()
    {
        m_RelativeRotation = transform.parent.localRotation;//find local rotation of canvas
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;//sets its own rotation to local rotation
    }
}
