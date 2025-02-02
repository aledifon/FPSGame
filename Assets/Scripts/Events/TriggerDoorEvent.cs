using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorEvent : MonoBehaviour
{
    //AudioSource audioSource;    

    bool isDoorOpened;

    [SerializeField] Transform doorTransform; //door (6)
    [SerializeField] float openingDuration;
    [SerializeField] AudioSource audioSource;

    private Quaternion openDoorRot = Quaternion.Euler(0, 0, 0);
    //private Quaternion closeDoorRot = Quaternion.Euler(0, -90f, 0);
    Quaternion targetDoorRot;
    Quaternion startDoorRot;

    private InteractuableObject doorObject;

    // Start is called before the first frame update
    void Start()
    {
        //audioSource = GetComponent<AudioSource>();                 
        //doorTransform = GetComponent<Transform>();
        //

        doorObject = doorTransform.GetComponent<InteractuableObject>();

        Debug.Log("");

        //doorTransform.Rotate(Vector3.up,150f);

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
        isDoorOpened = doorObject.IsObjectOpened;
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player") && !audioSource.isPlaying)
        {
            //audioSource.Play();
            //StartCoroutine(nameof(OpenDoor));
            doorObject.ActionOne();
            StartCoroutine(nameof(DestroyAfterDelay));
        }                
    }
    IEnumerator DestroyAfterDelay()
    {        
        //yield return new WaitWhile(() => audioSource.isPlaying || !isDoorOpened);
        yield return new WaitUntil(() => isDoorOpened && !audioSource.isPlaying);
        //Debug.Log("Before destroying the GO");
        Destroy(gameObject);
    }
    IEnumerator OpenDoor()
    {        
        float elapsedTime = 0f;
        startDoorRot = doorTransform.rotation;
        targetDoorRot = openDoorRot;
        //targetDoorRot = startDoorRot * Quaternion.Euler(48f, 130f, 260f);

        Debug.Log("Door started on: " + startDoorRot.eulerAngles); // Muestra la rotación en la consola
        Debug.Log("Target rotation is: " + targetDoorRot.eulerAngles); // Muestra la rotación en la consola

        while (elapsedTime < openingDuration) 
        {
            // Update the elapsed Time
            elapsedTime += Time.deltaTime;

            // Move door from point A towards point B  
            doorTransform.rotation =
                Quaternion.Lerp(startDoorRot, targetDoorRot, elapsedTime / openingDuration);

            Debug.Log("Door Tanform Rotation = " + doorTransform.rotation);
            // 1 frame Waiting
            yield return null;
        }
        // Assure the door is in the right target position
        doorTransform.rotation = targetDoorRot;
        // Set the Door opened flag to true
        isDoorOpened = true;
        Debug.Log("isDoorOpened = " + isDoorOpened);

        //Debug.Log("Door rotated to: " + doorTransform.rotation.eulerAngles); // Muestra la rotación en la consola
    }
}
