using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class explode : projectile
{
    public float radius;
    public float power;
    public GameObject particles;
    
    public override void impact (Collision other)
    {
        Vector3 hitpoint = other.GetContact(0).point;
        Collider[] hits = Physics.OverlapSphere(hitpoint, radius);

        Instantiate(particles, transform.position, transform.rotation);
        
        foreach (Collider hitted in hits)
        {
            Rigidbody rb = hitted.GetComponent<Rigidbody>();

            if (rb != null)
            { rb.AddForce((hitted.transform.position - hitpoint).normalized * power, ForceMode.Impulse);
            }
        }
        Destroy(gameObject);
    }
}
