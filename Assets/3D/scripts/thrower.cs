using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class thrower : MonoBehaviour
{
    [Header("References")] 
    public Transform cam;
    public Transform attackPoint;
    private GameObject objectToThrow;
    public GameObject Ammo1;
    public GameObject Ammo2;
    public GameObject Ammo3;

    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public KeyCode changeType = KeyCode.Mouse1;
    public float throwForce;
    public float throwUpwardForce;

    private Rigidbody rb;
    private int AmmoTypeNumb=1;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectToThrow = Ammo1;
    }

    public void Update()
    {
        if (Input.GetKeyDown(throwKey))
            Shoot();
        if (Input.GetKeyDown(changeType))
            OnSwitchAmmoType();
    }

    public void Shoot()
    {
        // instantiate object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // calculate direction
        Vector3 forceDirection = cam.transform.forward;
        
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        { 
            forceDirection = (hit.point - attackPoint.position).normalized;
        }
        // add force
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        rb.AddForce(-forceToAdd, ForceMode.Impulse);

        totalThrows--;
    }

    public void OnSwitchAmmoType()
    {
        if (AmmoTypeNumb == 1)
        {
            objectToThrow = Ammo1;
            AmmoTypeNumb = 2;
            throwForce = 15;
        }
        else if (AmmoTypeNumb == 2)
        {
            objectToThrow = Ammo2;
            AmmoTypeNumb = 3;
            throwForce = 7;
        }
        else
        {
            objectToThrow=Ammo3;
            AmmoTypeNumb = 1;
            throwForce = 10;
        }
    }

    public void AddAmmo()
    {
        totalThrows += 100;
    }
}
