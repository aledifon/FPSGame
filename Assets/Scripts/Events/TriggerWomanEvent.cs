using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWomanEvent : MonoBehaviour
{
    [SerializeField] AudioSource generalAudioSource;
    [SerializeField] AudioSource womanSongAudioSource;
    [SerializeField] AudioSource tvAudioSource;

    [SerializeField] InteractuableObject firstDocumentObject;
    [SerializeField] InteractuableObject lastDocumentObject;

    private void Start()
    {
        // Subscription to the Start & Stop reading events from InteractuableObject
        InteractuableObject.OnReadingFinished += PrepareEndOfScene;
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player") && !womanSongAudioSource.isPlaying)
        {
            //audioSource.Play();
            //tvAudioSource.Stop();
            //lastDocumentObject.enabled = true;
            //StartCoroutine(DestroyAfterDelay());
        }                
    }

    void PrepareEndOfScene(InteractuableObject sender)
    {
        if (sender == firstDocumentObject)
        {
            // Reduce the general volume to 10%
            generalAudioSource.volume = 0.1f;

            womanSongAudioSource.Play();
            tvAudioSource.Stop();
            lastDocumentObject.gameObject.SetActive(true);
            //StartCoroutine(DestroyAfterDelay());

            // Unscription of the even
            InteractuableObject.OnReadingFinished -= PrepareEndOfScene;
        }
    }
    IEnumerator DestroyAfterDelay()
    {        
        yield return new WaitWhile(() => womanSongAudioSource.isPlaying);
        Destroy(gameObject);
    }
}
