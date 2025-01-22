using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] ParticleSystem hitEffect;

    private float currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
    }        
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) 
            Death();            
    }
    void Death()
    {
        Destroy(gameObject);
    }
}
