using Entities;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private GameObject hitFX;
    [SerializeField] private Transform trailFX;
    
    [Header("Settings")] 
    [SerializeField] private float damage;
    [SerializeField] private float speed;

    private Rigidbody _rigidbody;
    
    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = transform.forward * speed;
    }
    
    //Damage any health on contact and destroy this projectile
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.TryGetComponent(out Health health))
            health.TakeDamage(damage);
        
        trailFX.SetParent(null);
        Instantiate(hitFX, transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(gameObject);
    }
}
