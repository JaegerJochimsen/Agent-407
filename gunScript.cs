using UnityEngine;

public class gunScript : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float impactForce = 30f;
    static public int bullets = 100;
    public int bulletsInMag = 10;
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
            nextTimeToFire = Time.time + 1f / fireRate;
            if (bulletsInMag > 0 && reloadDelay <= 0f)
            // && reloadDelay <= 0f
            {
                Shoot();
                Debug.Log("bullets: " + bullets);
            }
            else
            {
                Reload();
                if(reloadDelay <= 0)
                    reloadDelay = 1.5f;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && bullets > 0)
        {
            Reload();
            if (reloadDelay <= 0)
                reloadDelay = 1.5f;
        }

            reloadDelay -= Time.deltaTime;
    }
    void Reload()
    {
        if (reloadDelay < 0)
        {
            int diff = 10 - bulletsInMag;
            if (bullets > diff)
            {
                bullets -= diff;
                bulletsInMag += diff;
            }
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

        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 0.5f);
        }
        bulletsInMag -= 1;
    }
}
