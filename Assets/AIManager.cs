using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AIManager : NetworkBehaviour {
    public float SpawnTime;
    public GameObject AIBall;
    float remainingTime;
	// Use this for initialization
	void Start () {
        remainingTime = SpawnTime;
	}
	
	// Update is called once per frame
	void Update () {
        remainingTime -= Time.deltaTime;
        if(remainingTime <= 0)
        {
            remainingTime = SpawnTime;
            GameObject ball = (GameObject)Instantiate(AIBall, GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);
            NetworkServer.Spawn(ball);
        }
	}
}
