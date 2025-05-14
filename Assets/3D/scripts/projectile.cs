using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class projectile : MonoBehaviour
{
    public GameObject owner;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != owner)
        {
            if (other.gameObject.GetComponent<Health>())
            {
                impact(other);
                other.gameObject.GetComponent<Health>().TakeDamage(100);
                Debug.Log("Hello: " + other.gameObject.name);
            }
        }

    }

    public abstract void impact (Collision other);
}
