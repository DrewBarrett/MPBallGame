using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AIBall : NetworkBehaviour
{
    float timer;
    //[SyncVar(hook = "updateRotation")]
    float rotation;
    // Use this for initialization
    void Start()
    {
        if (!isServer)
            return;
        timer = Random.Range(3f, 10f);
        GetComponent<Rigidbody2D>().rotation = (Random.Range(1f, 359f));
        rotation = GetComponent<Rigidbody2D>().rotation;
        Debug.Log(rotation);
        GetComponent<NetworkTransform>().SetDirtyBit(1);
    }

    void updateRotation(float rot)
    {
        
        GetComponent<Rigidbody2D>().rotation = rot;
        rotation = rot;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer)
            return;
        if(collision.gameObject.tag == "Wall")
        {

            GetComponent<Rigidbody2D>().rotation -= 180 * Random.Range(0.9f,1.1f);
            rotation = GetComponent<Rigidbody2D>().rotation;
            GetComponent<NetworkTransform>().SetDirtyBit(1);
        }
        //GetComponent<Rigidbody2D>().MoveRotation(Random.Range(0, 360));
    }



    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * 2;
        if (!isServer)
            return;
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = Random.Range(3f, 10f);
            rotation = Random.Range(0f, 360f);
            GetComponent<Rigidbody2D>().rotation = rotation;
            GetComponent<NetworkTransform>().SetDirtyBit(1);
        }
    }
}
