using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;      //approx time it takes to move to correct pos           
    public float m_ScreenEdgeBuffer = 4f;           //number added to size
    public float m_MinSize = 6.5f;                  //wont zoom in too much
    public Transform[] m_Targets; //hidden in inspector - array of transforms


    private Camera m_Camera;                        //represents the camera object
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              //position camera is trying to reach


    private void Awake()//on scene startup
    {
        m_Camera = GetComponentInChildren<Camera>();//Get child components - only gets one though
    }


    private void FixedUpdate()//doesnt update every frame
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();//finds average

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)//is tank dead or inactive
                continue;//break out of if

            averagePos += m_Targets[i].position;//add position to average position
            numTargets++;//increment tanks count
        }

        if (numTargets > 0)//if there are active targets
            averagePos /= numTargets;//devide position by no. of tanks

        averagePos.y = transform.position.y;//move camera to avg. position

        m_DesiredPosition = averagePos;//deseried position is the average
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);//find from desired position

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);//target in local pos. of camera space

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));//size is max of current value

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;//add on screen buffer

        size = Mathf.Max(size, m_MinSize);//apply minimum size to stop too much zoom

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}