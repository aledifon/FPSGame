using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTVEvent : MonoBehaviour
{           
    // GO Components
    [SerializeField] AudioSource audioSource;
    [SerializeField] TVWhiteNoise tVWhiteNoise;

    // Flags
    bool isTVTriggered;

    // Update is called once per frame
    void Update()
    {
        if (isTVTriggered)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player") && !audioSource.isPlaying)
        {
            audioSource.Play();
            tVWhiteNoise.ShowImage();
        }                
    }    
}
