using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Object;
using DG.Tweening;

public class InteractuableObject : Object
{
    [SerializeField] float impulse;

    // GO Components
    [SerializeField] PlayerActions playerActions;

    // Animate Type Objects vars.
    #region Animate_Drawer_Vars
    private Vector3 openedDrawerPos;
    private Vector3 closedDrawerPos;
    private float openingDrawerTime = 1f;
    #endregion

    #region Animate_Doors_Vars
    private Quaternion openDoorsRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion closeDoorsTurn = Quaternion.Euler(0, -90f, 0);
    private Quaternion closeDoorsInvTurn = Quaternion.Euler(0, 90f, 0);
    private List<Transform> transformDoors = new List<Transform>();    
    private List<Quaternion> openDoorRotList = new List<Quaternion>();
    private List<Quaternion> closeDoorRotList = new List<Quaternion>();
    private float openingDoorTime = 3f;
    [SerializeField] private Boolean isObjectOpened;
    public Boolean IsObjectOpened { get { return isObjectOpened; } }
    #endregion

    #region Read_Vars
    private Vector3 paperTablePos = new Vector3(-16.987f, 0.917f, 23.758f);
    private Quaternion paperTableRot = Quaternion.Euler(90f, 0f, 90f); 
    private Vector3 paperReadPos = new Vector3(0.32f,0.2f,-0.09f);
    private Quaternion paperReadRot = Quaternion.Euler(165f,180f,180f);
    private Boolean isReading;
    public Boolean IsReading { get { return isReading; } }
    #endregion

    private bool isCoroutineRunning;
    private AudioSource audioSource;

    private void Start()
    {
        if(objectSubType == InteractuableObject.ObjectSubType.AnimateDrawer)
            DrawerSetup();
        else if (objectSubType == InteractuableObject.ObjectSubType.AnimateDoors)
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
                {
                    if (!isCoroutineRunning)
                    {
                        audioSource.Play();
                        if (isObjectOpened)
                            StartCoroutine(nameof(CloseDrawer));
                        else
                            StartCoroutine(nameof(OpenDrawer));
                    }                      
                }
                else if (objectSubType == ObjectSubType.AnimateDoors)
                {
                    if (!isCoroutineRunning)
                    {
                        audioSource.Play();
                        if (isObjectOpened)
                            StartCoroutine(nameof(CloseDoors));
                        else
                            StartCoroutine(nameof(OpenDoors));
                    }                    
                } 
                else if(objectSubType == ObjectSubType.ClosedDoor)
                {
                    audioSource.Play();
                    ShakeDoors();
                }
                break;
            case ObjectType.Read:
                audioSource.Play();
                StartReading();
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
                break;
            case ObjectType.Read:
                audioSource.Play();
                StopReading();
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
        // Define the Opened & Closed Drawer positions
        closedDrawerPos = new Vector3(transform.position.x,transform.position.y, 12.928f);
        openedDrawerPos = new Vector3(transform.position.x, transform.position.y, 12.928f+0.3f);        
        // Set the current Drawer pos. in func. of the isObjectOpened setting
        if (isObjectOpened)
            transform.position = openedDrawerPos;
        else
            transform.position = closedDrawerPos;
    }
    IEnumerator OpenDrawer()
    {
        isCoroutineRunning = true;

        float elapsedTime = 0f;

        while (elapsedTime < openingDrawerTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(closedDrawerPos, openedDrawerPos, elapsedTime / openingDrawerTime);
            yield return null;
        }
        transform.position = openedDrawerPos;

        isObjectOpened = true;

        isCoroutineRunning = false;
    }    
    IEnumerator CloseDrawer()
    {
        isCoroutineRunning = true;

        float elapsedTime = 0f;

        while (elapsedTime < openingDrawerTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(openedDrawerPos, closedDrawerPos, elapsedTime / openingDrawerTime);
            yield return null;
        }
        transform.position = closedDrawerPos;

        // Set the current object as unselected
        IsObjectSelected(false);

        isObjectOpened = false;

        isCoroutineRunning = false;
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
                //2. Set the Initial Door Rotation
                if (isObjectOpened)
                    child.rotation = openDoorsRotation;
                else
                {
                    if (numDoors==1)
                        child.rotation = openDoorsRotation * closeDoorsTurn;
                    else
                        child.rotation = openDoorsRotation * closeDoorsInvTurn;
                }
                //3. Get the initial Door Childs Rotations
                openDoorRotList.Add(openDoorsRotation);
                //4. Get the target Door Childs Rotations
                if (numDoors == 1)
                    closeDoorRotList.Add(openDoorsRotation * closeDoorsTurn);
                else
                    closeDoorRotList.Add(openDoorsRotation * closeDoorsInvTurn);
            }
        }
        // The GO parent have no children (So himself is the GO's door and there is a single door)
        else
        {
            //1. Get the Door Transform Component
            transformDoors.Add(transform);
            //2. Set the Initial Door Rotation
            if (isObjectOpened)
                transformDoors[0].rotation = openDoorsRotation;
            else
                transformDoors[0].rotation = openDoorsRotation*closeDoorsTurn;
            //3. Get the initial Door Rotation
            openDoorRotList.Add(openDoorsRotation);
            //4. Get the target Door Rotation          
            closeDoorRotList.Add(openDoorsRotation * closeDoorsTurn);            
        }        
    }
    public IEnumerator OpenDoors()
    {
        isCoroutineRunning = true;

        float elapsedTime = 0f;

        while (elapsedTime < openingDoorTime)
        {
            elapsedTime += Time.deltaTime;
            for (int i = 0; i <= transformDoors.Count - 1; i++)
                transformDoors[i].rotation = Quaternion.Lerp(closeDoorRotList[i],
                                                        openDoorRotList[i],
                                                        elapsedTime / openingDoorTime);
            yield return null;
        }
        // Assure all the doors finish on their right position
        for (int i = 0; i <= transformDoors.Count - 1; i++)
            transformDoors[i].rotation = openDoorRotList[i];

        isObjectOpened = true;

        isCoroutineRunning = false;
    }
    IEnumerator CloseDoors()
    {
        isCoroutineRunning = true;

        float elapsedTime = 0f;

        while (elapsedTime < openingDoorTime)
        {
            elapsedTime += Time.deltaTime;
            for (int i = 0; i <= transformDoors.Count - 1; i++)
                transformDoors[i].rotation = Quaternion.Lerp(openDoorRotList[i],
                                                        closeDoorRotList[i],
                                                        elapsedTime / openingDoorTime);
            yield return null;
        }
        // Assure all the doors finish on their right position
        for (int i = 0; i <= transformDoors.Count - 1; i++)
            transformDoors[i].rotation = closeDoorRotList[i];

        // Set the current object as unselected
        IsObjectSelected(false);

        isObjectOpened = false;

        isCoroutineRunning = false;
    }
    #endregion

    #region Doors_Shaking
    void ShakeDoors() 
    {
        transform.DOShakePosition(1f,new Vector3(0.03f,0f,0f),10,40);
    }
    #endregion

    #region Reading_Methods
    void StartReading()
    {
        // Oscurecer pantalla y sacar cuadro texto        

        // Set the current object as unselected
        IsObjectSelected(false);

        isReading = true;

        // Set the current GO as child of the 'PosObject' GO which is a child of the 'Main Camera' GO
        //transform.SetParent(Camera.main.transform.GetChild(1).transform);
        //Transform posObjectTransform = Camera.main.transform.Find("PosObject");
        Transform posObjectTransform = playerActions.posObject;

        if (posObjectTransform != null)
            transform.SetParent(posObjectTransform);
        else
            Debug.Log("It was not found the 'PosObject' as a child of Camera.main");

        // Set the current Object Pos & Rotations respect to his father position ('PosObject')
        transform.localPosition = paperReadPos;
        transform.localRotation = paperReadRot;
    }
    void StopReading()
    {   
        // Remove the paper from the Camera hierarchy
        transform.SetParent(null);

        // Set again the paper init positions and rotations
        transform.position = paperTablePos;
        transform.rotation = paperTableRot;

        isReading = false;
    }
    #endregion
}
