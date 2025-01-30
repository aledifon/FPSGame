using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorEvent : MonoBehaviour
{
    AudioSource audioSource;    

    bool isDoorOpened;

    [SerializeField] Transform doorTransform; //door (6)
    [SerializeField] float openingDuration;
    //Quaternion targetDoorRot = Quaternion.Euler(0f, -360f, 0f);    
    //Quaternion rotationToAdd /*= Quaternion.Euler(0f, 90f, 0f)*/;
    Quaternion targetDoorRot;
    Quaternion startDoorRot;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();         
        
        doorTransform = GetComponent<Transform>();

        //doorTransform.rotation = Quaternion.Euler(0f, -450f, 0f);

        //if (doorTransform != null)
        //{
        //    // Gira la puerta al ángulo deseado
        //    doorTransform.rotation = Quaternion.Euler(0f, -450f, 0f); // Gira a -90 grados en Y
        //    Debug.Log("Door rotated to: " + doorTransform.rotation.eulerAngles); // Muestra la rotación en la consola
        //}
        //else
        //{
        //    Debug.LogError("doorTransform no está asignado.");
        //}
        //startDoorRot = doorTransform.rotation;        
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
            //StartCoroutine(OpenDoor());
            StartCoroutine(DestroyAfterDelay());
        }                
    }
    IEnumerator DestroyAfterDelay()
    {        
        yield return new WaitWhile(() => audioSource.isPlaying && !isDoorOpened);        
        Destroy(gameObject);
    }
    IEnumerator OpenDoor()
    {
        float elapsedTime = 0f;
        startDoorRot = doorTransform.rotation;
        targetDoorRot = Quaternion.Euler(0f, 360f, 0f);

        Debug.Log("Door started on: " + startDoorRot.eulerAngles); // Muestra la rotación en la consola
        Debug.Log("Target rotation is: " + targetDoorRot.eulerAngles); // Muestra la rotación en la consola

        while (elapsedTime < openingDuration) 
        {
            // Update the elapsed Time
            elapsedTime += Time.deltaTime;

            // Move door from point A towards point B  
            doorTransform.rotation = 
                Quaternion.Lerp(startDoorRot, targetDoorRot, elapsedTime / openingDuration);
            // 1 frame Waiting
            yield return null;
        }
        // Assure the door is in the right target position
        doorTransform.rotation = targetDoorRot;
        // Set the Door opened flag to true
        isDoorOpened = true;

        Debug.Log("Door rotated to: " + doorTransform.rotation.eulerAngles); // Muestra la rotación en la consola
    }
}
