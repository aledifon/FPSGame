using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] float maxHealth;
    public ParticleSystem hitEffect;
    [SerializeField] ParticleSystem deathEffect;

    private MeshRenderer meshRenderer;

    //private AudioSource audioSource;

    public float currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;

        meshRenderer = GetComponent<MeshRenderer>();
        //audioSource = GetComponent<AudioSource>();
    }        
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) 
            StartCoroutine(nameof(Death));
        else
        {            
            hitEffect.Play();
            //audioSource.Play();             // Play the audio shoot FX
        }
            
    }
    IEnumerator Death()
    {
        // Only when the Death anim. has been completely played then the GO will be destroyed
        yield return StartCoroutine(nameof(PlayDeathAnim));     
        Destroy(gameObject);
    }

    IEnumerator PlayDeathAnim()
    {
        // Disable the Mesh renderer of the GO to make it invisible
        meshRenderer.enabled = false;
        // Start playing the Death animation
        deathEffect.Play();
        // Wait till the Death Anim. has finished
        while(deathEffect.isPlaying)
            yield return null;
    }    
}
