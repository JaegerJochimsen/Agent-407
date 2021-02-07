using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;

    // public so that we can call this function from another script
    public void TakeDamage (float amount)
    {
        health -= amount;
        if(health <= 0f)
        {
            Die();
        }
    }

    // local function, don't need other scripts to see it so not public
    void Die()
    {
        Destroy(gameObject);
    }
}
