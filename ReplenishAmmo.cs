using UnityEngine;

public class ReplenishAmmo : MonoBehaviour
{
    public CharacterController player;

    // Update is called once per frame
    void Update()
    {
        // if the player runs over the pickup
        if ((((transform.position - player.transform.position).z <= 1f && (transform.position - player.transform.position).x <= 1f)) && gunScript.bullets < 100)
        {
            gunScript.bullets = 100;

            // destroy pickup
            Destroy(gameObject);
        }
    }
}
