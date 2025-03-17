using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class bullet : projectile
{
    public GameObject particles;
    public override void impact (Collision other)
    {
        Instantiate(particles, other.contacts[0].point, transform.rotation);
        
        Destroy(gameObject);
    }
}
