using Unity.VisualScripting;
using UnityEngine;

public class Bullet_script : MonoBehaviour
{
    [SerializeField] float move_speed;
    Vector3 movement;
    public Transform player;
    private Rigidbody rb;

    void Start()
    {
        movement = ((player.position - transform.position).normalized) * move_speed;
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        rb.AddForce(movement);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "hit") Destroy(gameObject);
    }
}
