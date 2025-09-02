using UnityEngine;

public class Projectile : MonoBehaviour
{
    private static Projectile instance1;
    [SerializeField] float speed = 30f;
    [SerializeField] GameObject projectileHITVFX;

    Rigidbody rb;

    int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    public void Init(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        playerHealth?.TakeDamage(damage);

        Instantiate(projectileHITVFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        
    }
}
