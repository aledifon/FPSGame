using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Lantern : MonoBehaviour
{
    [SerializeField] float maxTimeToFlicker;   // Max Time to elapse to enter in blinking state
    [SerializeField] float minTimeToFlicker;   // Min Time to elapse to enter in blinking state

    Light spotLight;
    float timer;
    bool lanternWorking;                        // To know if I can switch ON/OFF the lantern

    AudioSource audioSource;
    [SerializeField] AudioClip switchLantern;
    [SerializeField] AudioClip BlinkLantern;

    void Awake()
    {
        spotLight = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = switchLantern;
        lanternWorking = true;
    }
    
    void Update()
    {
        LanternOnOff();
        ChangeState();
    }
    void LanternOnOff()
    {
        if(Input.GetKeyDown(KeyCode.Q) && lanternWorking)
        {
            spotLight.enabled = !spotLight.enabled;
            audioSource.Play();
        }            
    }
    void ChangeState()
    {
        timer += Time.deltaTime;
        if (timer > Random.Range(minTimeToFlicker,maxTimeToFlicker))
        {
            timer = 0;
            StopCoroutine(nameof(Flicker));       // Recommended to always stop a coroutine
                                            // monitored by a timer before calling it again
                                            
            // Call to coroutine
            StartCoroutine(nameof(Flicker));
        }            
    }
    IEnumerator Flicker()
    {        
        // Set blinking variables
        lanternWorking = false;                
        float timeOnOff = Random.Range(0.05f, 0.7f);

        //float totalTimeBlink = Random.Range(1f, 2f);
        float totalTimeBlink = Random.Range(1f,2f);
        float startTime = Time.time;

        // Set the Blinking Clip & play it
        audioSource.clip = BlinkLantern;        

        while ((Time.time-startTime) < totalTimeBlink)
        {
            spotLight.enabled = !spotLight.enabled;
            if (!audioSource.isPlaying)
                audioSource.Play();
            yield return new WaitForSeconds(timeOnOff);                       
        }

        // Stop the audio Blinking clip
        audioSource.Stop();

        // Once blinking has finished then we'll set randomly the final state of the lantern.
        spotLight.enabled = (Random.value<=0.5f) ? true: false;

        // In case the final lantern state is switched off then we leave it in this sate
        // for a certain time and then we switch it on
        if (!spotLight.enabled)
        {
            yield return new WaitForSeconds(3);
            spotLight.enabled = true;               // Switch on the lantern
        }        

        // Set again the Lantern Switch audio clip
        audioSource.clip = switchLantern;

        // Allow again the Lantern control by the player
        lanternWorking = true;
    }
}
