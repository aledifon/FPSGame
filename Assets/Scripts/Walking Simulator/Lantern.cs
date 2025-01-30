using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    [SerializeField] float timeToChangeState;   // The lantern will blink every certain time

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
        if (timer > timeToChangeState)
        {
            timer = 0;
            // Call to coroutine
            StartCoroutine(Flicker());
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
        lanternWorking = true;
        //Debug.Log("Elapsed " + Time.time + "s out of " + totalTimeBlink + "s");

        // Stop the audio Blinking clip
        audioSource.Stop();
        // Set again the Lantern Switch audio clip
        audioSource.clip = switchLantern;
    }
}
