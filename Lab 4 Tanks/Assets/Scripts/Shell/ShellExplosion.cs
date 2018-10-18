using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;//Check everything on players layer
    public ParticleSystem m_ExplosionParticles;       //particle system reference
    public AudioSource m_ExplosionAudio;              //audio clip reference
    public float m_MaxDamage = 100f;                  //max shell damage
    public float m_ExplosionForce = 1000f;            //force at centre
    public float m_MaxLifeTime = 2f;                  //max lifetime
    public float m_ExplosionRadius = 5f;              //how big is explosion effect


    private void Start()//start at scene
    {
        Destroy(gameObject, m_MaxLifeTime);//destroy object after 2 seconds
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.

        //Array checks all objects in explosion radius that are on players layer
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>(); //instance of tank health

            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.TakeDamage(damage);
        }

        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}