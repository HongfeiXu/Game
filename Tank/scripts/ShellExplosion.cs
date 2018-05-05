using UnityEngine;
using System.Collections;

public class ShellExplosion : MonoBehaviour {

    public LayerMask m_TankMask;                //  Used to filter what the explosion affects, this should be set to "Players".
    public ParticleSystem m_ExplosionParticles; // Reference to the particles that will play on explosion.
    public AudioSource m_ExplosionAudioSource;  // Reference to the audio that will play on explosion.
    public float m_MaxDamage = 100f;            // The amount of damage done if the explosion is centred on a tank.
    public float m_ExplosionForce = 1000f;      // The amount of force added to a tank at the centre of the explosion.
    public float m_MaxLifeTime = 2f;            // The time in seconds before the shell is removed
    public float m_ExplosionRadius = 5f;        // The maximum distance away from the explosion tanks can be and are still affected.

    private void Start () {
        // If it is not destroy by then, destroy the shell after it's lifetime
        Destroy(gameObject, m_MaxLifeTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        // Collect all the colliders in a  shpere from the shell's current position to a radius of the expolsion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        // Go through all the colliders...
        for (int i = 0; i < colliders.Length; ++i)
        {
            // ...and find their rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            // If they don't have a rigidbody, go on to the next collider
            if (!targetRigidbody)
            {
                continue;
            }

            // Add an explosion force
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Find the TankHealth script associated with rigidbody
            TankHealth tankHealth = colliders[i].GetComponent<TankHealth>();

            // If there is no TankHealth script attached to the gameobject, go on to the next collider
            if (!tankHealth)
            {
                continue;
            }

            // Calculate the amount of damage the target should take based on it's distance from the shell.
            float damage = CalculateDamage(targetRigidbody.position);

            // Deal this damage to the tank
            tankHealth.TakeDamage(damage);
        }

        // Unparent the particles from the shell
        // 分离是为了之后 Destroy 时，保证Shell能被立刻Destroy，而粒子系统在播放完之后才被Destroy。
        m_ExplosionParticles.transform.parent = null;

        // Play the particle system
        m_ExplosionParticles.Play();

        // Play the explosion sound effect
        m_ExplosionAudioSource.Play();

        // Once the particles have finished, destroy the gameobject they are on.
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);

        // Destroy the shell
        Destroy(gameObject);
    }
    
    // 计算子弹对 targetPosition处Tank 的伤害值
    private float CalculateDamage(Vector3 targetPosition)
    {
        // Create a vector from the shell to the target
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target
        float explosionDistance = explosionToTarget.magnitude;
        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate the damage as this proportion of the maximum possible damage.
        float damage = m_MaxDamage * relativeDistance;
        // Make sure that the minimum damage is always 0.
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}
