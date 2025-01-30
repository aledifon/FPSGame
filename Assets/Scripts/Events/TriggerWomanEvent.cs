using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWomanEvent : MonoBehaviour
{
    AudioSource audioSource;    

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player") && !audioSource.isPlaying)
        {
            audioSource.Play();
            StartCoroutine(DestroyAfterDelay());
        }                
    }
    IEnumerator DestroyAfterDelay()
    {        
        yield return new WaitWhile(() => audioSource.isPlaying);
        Destroy(gameObject);
    }
}
