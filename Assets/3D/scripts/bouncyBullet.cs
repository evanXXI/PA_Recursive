using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class bouncyBullet : projectile
{
    public GameObject particles;

    public int numberOfBounces;
    
    private int counter = 0;
    public override void impact (Collision other)
    {
        Instantiate(particles, other.contacts[0].point, transform.rotation);

        if (counter == numberOfBounces)
            Destroy(gameObject);
        counter++;
    }
}
