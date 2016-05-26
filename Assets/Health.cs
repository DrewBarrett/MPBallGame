using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Health : NetworkBehaviour {
    public float MaxHealth;
    float currentHealth;
	// Use this for initialization
	void Start () {
        currentHealth = MaxHealth;
	}
	
	public void TakeDamage()
    {
        //take 10 damage
        currentHealth -= 10;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if(gameObject.GetComponent<PlayerControl>())
        {
            gameObject.GetComponent<PlayerControl>().RpcRespawn();
        }
        else
        {
            Destroy(gameObject);
        }
       
    }

}
