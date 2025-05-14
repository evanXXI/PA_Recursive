using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class playerDamageable : damageable
{
    private Vector3 spawnPoint;
    public Slider slider;

    private void Start()
    {
        spawnPoint = transform.position;
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public override void die()
    {
         transform.position = spawnPoint;
         setHealth(maxHealth);
    }
    
    public override void damage (float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            die();
        }
        
        slider.value = health;
        Debug.Log(health);
    }

    public void Addhealth()
    {
        health += 50;
        slider.value = health;
        if(health > 100)
        {
            health = 100;
        }
    }
}
