using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    public int damagerPerShot = 10;
    public float energyPerShot = 5.0f;
    public float timeBetweenBullets = 0.5f;
    public float range = 1000f;
    public AudioSource blasterSource;
    public AudioClip blasterClip;


    PlayerShield playerShield;
    float timer;
    float effectsDisplayTime = 0.2f;
    Ray shootRay;
    RaycastHit shootHit;
    int shootableMask;
    LineRenderer gunLine;
    

    void Awake()
    {
        playerShield = GetComponentInParent<PlayerShield>();
        gunLine = GetComponent<LineRenderer>();
        blasterSource = GetComponent<AudioSource>();

        blasterSource.clip = blasterClip;
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (Input.GetButton("Fire") && timer >= timeBetweenBullets && playerShield.currentShield > energyPerShot)
        {
            Shoot();
        }

        if(timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects();
        }

    }

    public void DisableEffects()
    {
        gunLine.enabled = false;
    }

    void Shoot()
    {
        timer = 0f;
        RaycastHit hit;

        playerShield.ReduceShield(energyPerShot);

        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);

        //play Soundeffect
        blasterSource.Play(0);

        if(Physics.Raycast(shootRay.origin, shootRay.direction, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null) target.TakeDamage(damagerPerShot);
        }
        
    }
}
