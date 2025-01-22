using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] float weaponDamage;
    [SerializeField] float weaponRange;             // The weapon range
    [SerializeField] float weaponImpactForce;       // Impact bullet force on the target
    [SerializeField] float weaponCadence;           // Weapon Cadence
    [SerializeField] ParticleSystem muzzleEffect;   // Weapon Particle system
    [SerializeField] Target target;
    [SerializeField] private Transform rayOrigin;

    private AudioSource audioSource;
    private Camera cam;
    private Ray ray;
    private RaycastHit hit;
    private float timer;

    private LineRenderer lineRenderer;
    

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

        //// Configure the raycast
        //ray.origin = cam.transform.position;        
        //ray.direction = cam.transform.forward;
        if (Physics.Raycast(ray,out hit, weaponRange))                    
            target.TakeDamage(weaponDamage);                
    }
    void LineRendererSetup()
    {
        //lineRenderer = GetComponent<LineRenderer>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2; // Dos puntos: inicio y fin
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;        
    }
}
