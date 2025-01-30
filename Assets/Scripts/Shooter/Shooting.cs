using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Shooting : MonoBehaviour
{
    [SerializeField] float weaponDamage;
    [SerializeField] float weaponRange;             // The weapon range
    [SerializeField] float weaponImpactForce;       // Impact bullet force on the target
    [SerializeField] float weaponCadence;           // Weapon Cadence
    [SerializeField] ParticleSystem muzzleEffect;   // Weapon Particle system
    //[SerializeField] Target target;                 // Scripts intercomm. (Option A) 
    [SerializeField] private Transform rayOrigin;

    private AudioSource audioSource;
    private Camera cam;
    private Ray ray;
    private RaycastHit hit;
    private float timer;

    private LineRenderer lineRenderer;

    // Raycast Layers
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask grapplingPointLayer;

    // Ref. to the Player Movement Script
    [SerializeField] PlayerMovement playerMovement;        

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;

        //Line Renderer setup
        LineRendererSetup();        
    }
    
    void Update()
    {
        // Configure the raycast
        ray.origin = rayOrigin.position;
        ray.direction = cam.transform.forward;
        // Debug Weapon Target Direction
        //Debug.DrawRay(ray.origin, ray.direction * weaponRange, Color.red);        
        // Set the LineRenderer points
        lineRenderer.SetPosition(0, ray.origin); // Punto de inicio
        lineRenderer.SetPosition(1, ray.origin + ray.direction * weaponRange);

        // Timer update
        timer += Time.deltaTime;

        // Shooting
        if (Input.GetMouseButtonDown(0) && timer > weaponCadence)
            Shoot();        
    }

    void Shoot()
    {
        timer = 0;

        muzzleEffect.Play();            // Play the particle system
        audioSource.Play();             // Play the audio shoot FX

        // Set the Layer Mask
        int combinedLayerMask = targetLayer | grapplingPointLayer;

        //// Configure the raycast
        //ray.origin = cam.transform.position;        
        //ray.direction = cam.transform.forward;
        if (Physics.Raycast(ray,out hit, weaponRange, combinedLayerMask))
        {
            //// Scripts intercomm. (Option A) Target passed through the Inspector
            //target.TakeDamage(weaponDamage);

            // Get the hitted layerMask
            int hitLayer = hit.collider.gameObject.layer;
            // Scripts intercomm. (Option B)
            Target target = hit.collider.GetComponent<Target>();

            // Check if we hit the target Layer and it also contains a Target script Component
            if ((targetLayer & (1 << hitLayer)) != 0 && (target != null))
            {
                // Debugging
                Debug.Log($"Raycast impactó en {hit.collider.gameObject.name}, en la CAPA 1: {LayerMask.LayerToName(hitLayer)}");

                // Execute the TakeDamage method from the Target script
                target.TakeDamage(weaponDamage);
                // Place the hit particle system on the exact hit position
                target.hitEffect.transform.position = hit.point;
                // 
                if (hit.rigidbody != null)
                    hit.rigidbody.AddForce(ray.direction * weaponImpactForce);
                //hit.rigidbody.AddForce(-hit.normal * weaponImpactForce);
            }            
            // Otherwise, Check if we hit the grappling Point Layer
            else if ((grapplingPointLayer & (1 << hitLayer)) != 0)
            {
                Debug.Log($"Raycast impactó en {hit.collider.gameObject.name}, en la CAPA 2: {LayerMask.LayerToName(hitLayer)}");
                
                // Get the Grappling Point Transform
                Vector3 targetPosition =  hit.transform.position;                
                // Calls the coroutine which will perform the player movement towards the Grappling Point
                StartCoroutine(playerMovement.MoveTowardsPoint(targetPosition));
            }

            //// Scripts intercomm. (Option C) -> Not valid in this case as it will return errors
            //hit.collider.GetComponent<Target>().TakeDamage(weaponDamage);
        }                                
    }
    void LineRendererSetup()
    {
        //lineRenderer = GetComponent<LineRenderer>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2; // Dos puntos: inicio y fin
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;        
    }    
}
