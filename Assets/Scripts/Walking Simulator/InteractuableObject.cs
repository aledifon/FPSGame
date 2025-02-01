using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Object;

public class InteractuableObject : Object
{
    [SerializeField] float impulse;

    // GO Components
    [SerializeField] PlayerActions playerActions;

    // Animate Type Objects vars.
    #region Animate_Drawer_Vars
    private Vector3 initDrawerPos;
    private Vector3 targetDrawerPos;
    private float openingDrawerTime = 1f;
    #endregion

    #region Animate_Doors_Vars
    private List<Transform> transformDoors = new List<Transform>();    
    private List<Quaternion> initDoorRotList = new List<Quaternion>();
    private List<Quaternion> targetDoorRotList = new List<Quaternion>();
    private float openingDoorTime = 3f;
    #endregion    

    private AudioSource audioSource;

    private void Start()
    {
        DrawerSetup();
        DoorsSetup();

        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        
    }
    #region Action_Methods
    public void ActionOne()
    {
        switch (objectType)
        {
            case ObjectType.TakeDrop:
                Take();
                break;
            case ObjectType.Animate:
                if (objectSubType == ObjectSubType.AnimateDrawer)                
                    StartCoroutine(nameof(OpenDrawer));                                    
                else if (objectSubType == ObjectSubType.AnimateDoors)
                {
                    audioSource.Play();
                    StartCoroutine(nameof(OpenDoors));
                }                                    
                break;
            case ObjectType.Read:
                break;
        }        
    }
    public void ActionTwo()
    {        
        switch (objectType)
        {
            case ObjectType.TakeDrop:
                Drop();
                break;
            case ObjectType.Animate:
                if (objectSubType == ObjectSubType.AnimateDrawer)                
                    StartCoroutine(nameof(CloseDrawer));                                    
                else if (objectSubType == ObjectSubType.AnimateDoors)
                {
                    audioSource.Play();
                    StartCoroutine(nameof(CloseDoors));
                }                                    
                break;
            case ObjectType.Read:
                break;
        }
    }
    #endregion

    #region TakeDrop_Methods
    void Take()
    {
        // Set the current object as unselected
        IsObjectSelected(false);

        // Set rb as kinematic to avoid to be affected by gravity
        rb.isKinematic = true;

        // Set the current GO as child of the 'PosObject' GO which is a child of the 'Main Camera' GO
        //transform.SetParent(Camera.main.transform.GetChild(1).transform);
        //Transform posObjectTransform = Camera.main.transform.Find("PosObject");
        Transform posObjectTransform = playerActions.posObject;

        if (posObjectTransform != null)                    
            transform.SetParent(posObjectTransform);                    
        else
            Debug.Log("It was not found the 'PosObject' as a child of Camera.main");

        // Set the current Object Pos & Rotations respect to his father position ('PosObject')
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 107f, 0f);        
    }
    void Drop()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.AddForce(transform.forward*impulse);
    }
    #endregion

    #region OpenClose_Drawers_Methods
    void DrawerSetup()
    {
        initDrawerPos = transform.position;
        targetDrawerPos = new Vector3(initDrawerPos.x, initDrawerPos.y, initDrawerPos.z + 0.3f);
    }
    IEnumerator OpenDrawer()
    {        
        float elapsedTime = 0f;

        while (elapsedTime < openingDrawerTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initDrawerPos, targetDrawerPos, elapsedTime / openingDrawerTime);
            yield return null;
        }
        transform.position = targetDrawerPos;
    }    
    IEnumerator CloseDrawer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openingDrawerTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(targetDrawerPos, initDrawerPos, elapsedTime / openingDrawerTime);
            yield return null;
        }
        transform.position = initDrawerPos;

        // Set the current object as unselected
        IsObjectSelected(false);
    }
    #endregion
    #region OpenClose_Doors_Methods
    void DoorsSetup()
    {        
        // The GO parent have any children (so this means the doors are GO's children)
        if (gameObject.transform.childCount > 0)
        {
            int numDoors = 0;

            foreach (Transform child in transform)
            {   // Increase the doors counter             
                numDoors++;                             
                //1. Get the Door Childs Transform Components
                transformDoors.Add(child);
                //2. Get the initial Door Childs Rotations
                initDoorRotList.Add(child.rotation);
                //2. Get the target Door Childs Rotations
                if (numDoors <= 1)
                    targetDoorRotList.Add(child.rotation * Quaternion.Euler(0, 100f, 0f));
                else
                    targetDoorRotList.Add(child.rotation * Quaternion.Euler(0, -100f, 0f));
            }
        }
        // The GO parent have no children (So himself is the GO's door and there is a single door)
        else
        {
            //1. Get the Door Transform Component
            transformDoors.Add(transform);
            //2. Get the initial Door Rotation
            initDoorRotList.Add(transform.rotation);
            //2. Get the target Door Rotation          
            targetDoorRotList.Add(transform.rotation * Quaternion.Euler(0, 100f, 0f));            
        }        
    }
    IEnumerator OpenDoors()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openingDoorTime)
        {
            elapsedTime += Time.deltaTime;
            for (int i = 0; i <= transformDoors.Count - 1; i++)
                transformDoors[i].rotation = Quaternion.Lerp(initDoorRotList[i],
                                                        targetDoorRotList[i],
                                                        elapsedTime / openingDoorTime);
            yield return null;
        }
        // Assure all the doors finish on their right position
        for (int i = 0; i <= transformDoors.Count - 1; i++)
            transformDoors[i].rotation = targetDoorRotList[i];        
    }
    IEnumerator CloseDoors()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openingDoorTime)
        {
            elapsedTime += Time.deltaTime;
            for (int i = 0; i <= transformDoors.Count - 1; i++)
                transformDoors[i].rotation = Quaternion.Lerp(targetDoorRotList[i],
                                                        initDoorRotList[i],
                                                        elapsedTime / openingDoorTime);
            yield return null;
        }
        // Assure all the doors finish on their right position
        for (int i = 0; i <= transformDoors.Count - 1; i++)
            transformDoors[i].rotation = initDoorRotList[i];

        // Set the current object as unselected
        IsObjectSelected(false);
    }
    #endregion

    #region Reading_Methods
    void StartReading()
    {
        // Oscurecer pantalla y sacar cuadro texto
        // Mostrar texto en pantalla

        // Hacer todo por Canvas?
    }
    void StopReading()
    {

    }
    #endregion
}
