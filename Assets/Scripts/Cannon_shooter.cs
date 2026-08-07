using System;
using UnityEngine;

public class Cannon_shooter : Enemy_checks
{
    [SerializeField] float cooldown;
    float timer = 0;
    [SerializeField] GameObject bullet;
    void Update()
    {
        timer += Time.deltaTime;
        if (in_range())
        {
            if (timer >= cooldown)
            {
                GameObject proj = Instantiate(bullet, transform.position, transform.rotation);
                proj.GetComponent<Bullet_script>().player = player;
                timer = 0;
                Debug.Log("Spawn");
            }
        }
    }
}
