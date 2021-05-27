using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour {
    int health = 100;

    void Update()
    {
        if(health <= 0)
        {
            Destroy(this);
        }
    }

    public void Damage(int dmg)
    {
        health -= dmg;
    }
}
