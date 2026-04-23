
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 3f;

    [SerializeField] private ParticleSystem impactEffect; //reference to prefab
    private ParticleSystem impactEffectInstance; //reference to instance of each individiual prefab we are spawning in case you want to change any settings

    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = -transform.right * speed;
        Destroy(gameObject, lifeTime);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        

        Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));


        //SpawnImpactParticles();

        Destroy(gameObject);
    }

    private void SpawnImpactParticles()
    {
        
       
        
        //impactEffectInstance = Instantiate(impactEffect, spawnPos , transform.rotation);
    }


}
