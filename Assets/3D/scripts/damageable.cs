using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class damageable : MonoBehaviour
{
    public float maxHealth = 100;
    public float health;

    public void setHealth(float f)
    {
        health = f;
    }
    private void Awake()
    {
        health = maxHealth;
    }

    public virtual void damage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            die();
        }
    }

    public abstract void die();
    
}
