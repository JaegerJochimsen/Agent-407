using UnityEngine;

public class RestoreHealth : MonoBehaviour
{
    public CharacterController player;
    public float healthRestore = 50f;

    // Update is called once per frame
    void Update()
    {
        // if the player runs over the pickup
        if((((transform.position - player.transform.position).z <= 1f && (transform.position - player.transform.position).x <= 1f)) && MovePlayer.health < 100)
        {
            // if health is above half then restore to full
            if(MovePlayer.health > 50)
            {
                MovePlayer.health = 100;
            }
            // otherwise re-heal 50hp
            else
            {
                MovePlayer.health += healthRestore;
            }

            // destroy pickup
            Destroy(gameObject);
        }
    }
}
