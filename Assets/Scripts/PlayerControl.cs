using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerControl : NetworkBehaviour
{
    //[SyncVar(hook = "UpdateRotation")]
    //float myRotation = 0;
    // Use this for initialization
    public AudioClip KnifeEquipSound;
    public AudioClip GunEquipSound;
    public AudioClip[] GunShootSounds;
    public AudioClip[] KnifeStabSounds;
    public AudioClip[] DeathSounds;
    public GameObject bloodPrefab;
    public GameObject _respawnText;
    [SyncVar(hook = "OnPowerUpChange")]
    string PowerUpName;
    public Powerup powerup;
    bool dead;
    void Start()
    {
        dead = false;
        powerup = new KnifePowerup(gameObject);
        //CmdSetPowerup("");
        _respawnText = GameObject.Find("RespawnText");
        if (isLocalPlayer)
        {
            GetComponent<Rigidbody2D>().velocity = transform.up * 2;
            CmdRequestUpdate();
        }
    }
    [Command]
    public void CmdSetPowerup(string name)
    {
        PowerUpName = name;
    }

    void OnPowerUpChange(string name)
    {
        //if we are the local player we know what powerup we have equiped!
        if (isLocalPlayer)
            return;

        PowerUpName = name;
        if (name == "knife")
        {
            powerup = new KnifePowerup(gameObject);
        }
        else if (name == "gun")
        {
            powerup = new GunPowerup(gameObject);
        }
        else
        {
            Debug.LogError("What the fuck powerup is this????? " + name);
        }
    }

    public void OnGUI()
    {
        if (!isLocalPlayer)
            return;
        if(GUI.Button(new Rect(10,200,200,20),"Debug Equip Knife"))
        {
            powerup.Destroy();
            powerup = new KnifePowerup(gameObject);
        }
        if (GUI.Button(new Rect(10, 230, 200, 20), "Debug Equip Gun"))
        {
            powerup.Destroy();
            powerup = new GunPowerup(gameObject);
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
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 180 + gameObject.transform.rotation.eulerAngles.z);

            GetComponent<Rigidbody2D>().velocity = transform.up * 2;
            GetComponent<NetworkTransform>().SetDirtyBit(1);
        }
        /*else if (collision.gameObject.GetComponent<Health>() && knifeOut)
        {
            
            CmdAttackGameObject(collision.gameObject, gameObject);
        }*/
        else
        {
            powerup.OnTriggerEnter(collision);
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
            powerup.OnLeftClick();

        }
        if (Input.GetMouseButtonDown(1))
        {
            powerup.OnRightClick();

            /*knifeOut = !knifeOut;
            if (knifeOut)
            {
                GetComponent<AudioSource>().PlayOneShot(KnifeEquipSound);
            }
            //Debug.Log("Mouse clicked!");
            if (SetKnifing(knifeOut))
            {
                CmdSetWeaponVisible(knifeOut);
            }
            else
            {
                Debug.LogError("Where the fuck did the knife go?????");
            }*/
        }


    }
    [Command]
    public void CmdSetWeaponVisible(bool weaponStatus)
    {
        RpcSetWeaponVisible(weaponStatus);
    }
    [ClientRpc]
    void RpcSetWeaponVisible(bool weaponStatus)
    {
        if (!isLocalPlayer)
        {
            powerup.SetVisible(weaponStatus);
            //SetKnifing(weaponStatus);
            if (weaponStatus)
            {
                GetComponent<AudioSource>().PlayOneShot(powerup.EquipSound);
            }
        }
        else
        {
            Debug.Log("Server told us to change our weapon but fuck the server");
        }
    }
    [Command]
    public void CmdAttackGameObject(GameObject target, GameObject attacker)
    {
        target.GetComponent<Health>().TakeDamage();
        if (target.gameObject.tag == "Player")
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
        //powerup.(knifeOut);
        //powerup.SetVisible(false);
        //CmdSetWeaponVisible(false);
        powerup.Destroy();
    }
    //[Command]
    //void CmdSetRotation(float rot)
    //{
    //myRotation = rot;
    //}
}
