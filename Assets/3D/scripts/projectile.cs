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
            impact(other);
        }
        
    }

    public abstract void impact (Collision other);
}
