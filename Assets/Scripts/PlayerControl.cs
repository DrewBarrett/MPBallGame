using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerControl : NetworkBehaviour
{
    //[SyncVar(hook = "UpdateRotation")]
    //float myRotation = 0;
    // Use this for initialization
    public AudioClip KnifeEquipSound;
    public AudioClip[] KnifeStabSounds;
    public AudioClip[] DeathSounds;
    public GameObject bloodPrefab;
    public GameObject _respawnText;
    bool knifeOut = false;
    bool dead;
    void Start()
    {
        
        dead = false;
        _respawnText = GameObject.Find("RespawnText");
        if(isLocalPlayer)
        {
            GetComponent<Rigidbody2D>().velocity = transform.up * 2;
            CmdRequestUpdate();
        }
    }

    void UpdateRotation(float target)
    {
        if (!isLocalPlayer)
        {
            //myRotation = target;
            //GetComponent<Rigidbody2D>().MoveRotation(target);
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLocalPlayer || dead)
            return;
        //Debug.Log("Collision with " + collision.gameObject.tag + collision.IsTouching(gameObject.GetComponentInChildren<Collider2D>(false)));
        if (collision.gameObject.tag == "Wall")
        {

            //GetComponent<Rigidbody2D>().rotation += 180;
            //Debug.Log(gameObject.transform.rotation.ToString());
            gameObject.transform.rotation = Quaternion.Euler(0,0,180 + gameObject.transform.rotation.eulerAngles.z);
            
            GetComponent<Rigidbody2D>().velocity = transform.up * 2;
            GetComponent<NetworkTransform>().SetDirtyBit(1);
        }
        else if (collision.gameObject.GetComponent<Health>() && knifeOut)
        {
            
            CmdAttackGameObject(collision.gameObject, gameObject);
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().color = Color.green;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer || dead)
        {

            //GetComponent<Rigidbody2D>().velocity = transform.up * 2;
            return;
        }

        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawLine(transform.position, target, Color.red, .10f);
            //Gizmos.DrawLine(transform.position, target);
            Vector3 targetRotation = transform.position - target;
            float rot = Mathf.Atan2(targetRotation.x, -1 * targetRotation.y) * (180 / Mathf.PI);
            //GetComponent<Rigidbody2D>().MoveRotation(rot);
            gameObject.transform.rotation = Quaternion.Euler(0,0, rot);
            GetComponent<Rigidbody2D>().velocity = transform.up * 2;
            GetComponent<NetworkTransform>().SetDirtyBit(1);
            //CmdSetRotation(rot);
        }
        if (Input.GetMouseButtonDown(1))
        {
            knifeOut = !knifeOut;
            if (knifeOut)
            {
                GetComponent<AudioSource>().PlayOneShot(KnifeEquipSound);
            }
            //Debug.Log("Mouse clicked!");
            if (SetKnifing(knifeOut))
            {
                CmdSetKnifing(knifeOut);
            }
            else
            {
                Debug.LogError("Where the fuck did the knife go?????");
            }
        }


    }
    bool SetKnifing(bool knifeStatus)
    {
        SpriteRenderer[] srs = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in srs)
        {
            //Debug.Log(sr.ToString());
            if (sr.gameObject.name == "knife")
            {
                sr.gameObject.SetActive(knifeStatus);
                //Debug.Log("We have changed our knife status to " + knifeStatus);
                return true;
            }
        }
        Debug.LogError("we have failed to change our knife status");
        return false;
    }
    [Command]
    void CmdSetKnifing(bool knifeStatus)
    {
        RpcSetKnifing(knifeStatus);
    }
    [ClientRpc]
    void RpcSetKnifing(bool knifeStatus)
    {
        if(!isLocalPlayer)
        {
            SetKnifing(knifeStatus);
            if(knifeStatus)
            {
                GetComponent<AudioSource>().PlayOneShot(KnifeEquipSound);
            }
        }else
        {
            Debug.Log("Server told us to change our knife but fuck the server");
        }
    }
    [Command]
    void CmdAttackGameObject(GameObject target, GameObject attacker)
    {
        target.GetComponent<Health>().TakeDamage();
        if(target.gameObject.tag == "Player")
        {
            //if you stabbed a player
            attacker.GetComponent<PlayerInfo>().score += 1;
        }
        else
        {
            //if you stabbed an AI
            attacker.GetComponent<PlayerInfo>().score -= 1;
        }
        RpcAttacked(target.transform.position);
    }
    [ClientRpc]
    public void RpcRespawn()
    {
        StopExisting(true);
        //gameObject.SetActive(false);
        if (!isLocalPlayer)
            return;
        transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
        GetComponent<NetworkTransform>().SetDirtyBit(1);
        CmdBeginRespawn();
    }
    [ClientRpc]
    void RpcAttacked(Vector3 attackSpot)
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(KnifeStabSounds[Random.Range(0, KnifeStabSounds.Length)]);
        GameObject blood = (GameObject)Instantiate(bloodPrefab, attackSpot, Quaternion.identity);
        Destroy(blood, 10f);
        GetComponent<AudioSource>().PlayOneShot(DeathSounds[Random.Range(0, KnifeStabSounds.Length)]);
    }
    [Command]
    void CmdBeginRespawn()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<AIManager>().Respawn(gameObject);
    }
    [ClientRpc]
    public void RpcEndRespawn()
    {
        //re-enable the player for everyone.
        Debug.Log("Rpc Respawning player now!");
        StopExisting(false);
        if (!isLocalPlayer)
            return;
        GetComponent<Rigidbody2D>().velocity = transform.up * 2;
        GetComponent<NetworkTransform>().SetDirtyBit(1);
    }
    [ClientRpc]
    public void RpcSetSpawnTime(float time)
    {
        if (!isLocalPlayer)
            return;
        _respawnText.GetComponent<RespawnTimer>().UpdateTime(time);
    }
    [Command]
    void CmdRequestUpdate()
    {
        RpcSendUpdate();
    }
    [ClientRpc]
    void RpcSendUpdate()
    {
        GetComponent<NetworkTransform>().SetDirtyBit(1);
    }

    void StopExisting(bool shouldStop)
    {
        
        dead = shouldStop;
        GetComponent<SpriteRenderer>().enabled = !shouldStop;
        GetComponent<CircleCollider2D>().enabled = !shouldStop;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        knifeOut = false;
        SetKnifing(knifeOut);
        CmdSetKnifing(knifeOut);
    }
    //[Command]
    //void CmdSetRotation(float rot)
    //{
    //myRotation = rot;
    //}
}
