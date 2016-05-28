using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class AIManager : NetworkBehaviour {
    public float SpawnTime;
    public GameObject AIBall;
    float remainingTime;
    public List<GameObject> RespawnQ = new List<GameObject>();
    // Use this for initialization
    void Start () {
        remainingTime = SpawnTime;
         
    }
	
	// Update is called once per frame
	void Update () {
        remainingTime -= Time.deltaTime;
        if(remainingTime <= 0)
        {
            GameObject ball = null;
            
            if(RespawnQ.Count > 0)
            {
                ball = RespawnQ[0];
                RespawnQ.RemoveAt(0);
            }
            remainingTime = SpawnTime;
            if (!ball || !ball.GetComponent<PlayerControl>())
            {
                //spawn an AI
                ball = (GameObject)Instantiate(AIBall, GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);
                NetworkServer.Spawn(ball);
                return;
            }
            ball.GetComponent<PlayerControl>().RpcEndRespawn();
        }
	}

    public void Respawn(GameObject go)
    {
        //add random amount of AI to q
        for (int i = 0; i < Random.Range(1,5); i++)
        {
            RespawnQ.Add(AIBall);
        }
        RespawnQ.Add(go);
        go.GetComponent<PlayerControl>().RpcSetSpawnTime( (RespawnQ.Count * SpawnTime) - remainingTime );
    }
}
