using UnityEngine;

public class gunScript : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float impactForce = 60f;

    static public int bullets = 100;
    public int bulletsInMag = 10;
    public int magSize = 10;
    public float reloadDelay = -1f;
    
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private float nextTimeToFire = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire) 
        {
            // determine fire rate-based delay
            nextTimeToFire = Time.time + 1f / fireRate;

            // if there are still bullets in the mag and we have reloaded
            if (bulletsInMag > 0 && reloadDelay <= 0f)
            {
                Shoot();
            }
            else
            {
                Reload();
                // after each reload is initiated we need to set the reload delay
                if(reloadDelay <= 0)
                    reloadDelay = 1.5f;
            }
        }

        // preemptively call reload
        if (Input.GetKeyDown(KeyCode.R) && bullets > 0)
        {
            Reload();
            if (reloadDelay <= 0)
                reloadDelay = 1.5f;
        }

        if(reloadDelay >= 0)
            // keep track of delay
            reloadDelay -= Time.deltaTime;
        
    }
    void Reload()
    {
        if (reloadDelay < 0)
        {
            // find out how many bullets have been expended from mag
            int diff = magSize - bulletsInMag;
            
            // if we have enough reserve bullets to satisfy reload then refill mag and deduct from total bullets
            if (bullets > diff)
            {
                bullets -= diff;
                bulletsInMag += diff;
            }
            // otherwise we fill mag with the remainder of our stores
            else
            {
                bulletsInMag += bullets;
                bullets = 0;
            }
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        // check to see if we hit our target
        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            // if the thing we hit is an enemy then do damage to it 
            enemyAI target = hit.transform.GetComponent<enemyAI>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            // add impact force to our hit
            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            // these are the impact effects, instantiate and then destroy after short delay
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 0.5f);
        }
        bulletsInMag -= 1;
    }
}
