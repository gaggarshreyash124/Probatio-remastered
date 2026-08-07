using System.Collections;
using UnityEngine;

public class Breakable_wall : MonoBehaviour
{
    public bool hit = false;
    Animator anim;
    Collider col;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (hit)
        {
            anim.SetBool("Disappear", hit);
            col.enabled = false;
        }
    }
}
