using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWomanEvent : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource tvAudioSource;
    [SerializeField] InteractuableObject paperEntrance;

    // Start is called before the first frame update
    void Start()
    {
        
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
            tvAudioSource.Stop();
            paperEntrance.enabled = true;
            StartCoroutine(DestroyAfterDelay());
        }                
    }
    IEnumerator DestroyAfterDelay()
    {        
        yield return new WaitWhile(() => audioSource.isPlaying);
        Destroy(gameObject);
    }
}
