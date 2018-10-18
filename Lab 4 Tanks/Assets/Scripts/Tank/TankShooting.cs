using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;   //player tag    
    public Rigidbody m_Shell;            //reference to shell prefab
    public Transform m_FireTransform;    //reference to fire transform
    public Slider m_AimSlider;           //reference to slider
    public AudioSource m_ShootingAudio;  //audio
    public AudioClip m_ChargingClip;     //audio
    public AudioClip m_FireClip;         //audio
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;//how long it takes to get from min to max launch force

    private string m_FireButton;         //input key
    private float m_CurrentLaunchForce;  //how much force built up
    private float m_ChargeSpeed;         
    private bool m_Fired;                //have we fired


    private void OnEnable()//scene start
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()//called once at start
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        //calculate charge speed
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    private void Update()//called every frame
    {
        // Track the current state of the fire button and make decisions based on the current launch force.

        m_AimSlider.value = m_MinLaunchForce;

        if(m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(m_FireButton))//if fire pressed
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)//if fire held and hasn't fired
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)//if fire not held and not fired
        {
            Fire();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.

        m_Fired = true;

        //as rigidbody assigns it as a rigidbody object bound by unity's physics engine
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}