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
        movement = ((player.position - transform.position).normalized * move_speed);
        movement += Vector3.up * Time.deltaTime * move_speed * 2;
        Debug.Log(movement);
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity =  Vector3.zero;
    }
    void Update()
    {
        rb.linearVelocity = movement;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "hit") Destroy(gameObject);
    }
}
