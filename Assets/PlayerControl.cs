﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerControl : NetworkBehaviour
{
    //[SyncVar(hook = "UpdateRotation")]
    //float myRotation = 0;
    // Use this for initialization
    public AudioClip KnifeEquipSound;
    public AudioClip[] KnifeStabSounds;
    bool knifeOut = false;
    void Start()
    {


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
        if (!isLocalPlayer)
            return;
        Debug.Log("Collision with " + collision.gameObject.tag + collision.IsTouching(gameObject.GetComponentInChildren<Collider2D>(false)));
        if (collision.gameObject.tag == "Wall")
        {

            GetComponent<Rigidbody2D>().rotation += 180;
        }
        else if (collision.gameObject.GetComponent<Health>() && knifeOut)
        {
            
            CmdAttackGameObject(collision.gameObject);
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().color = Color.green;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {

            //GetComponent<Rigidbody2D>().velocity = transform.up * 2;
            return;
        }

        GetComponent<Rigidbody2D>().velocity = transform.up * 2;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawLine(transform.position, target, Color.red, .10f);
            Vector3 targetRotation = transform.position - target;
            float rot = Mathf.Atan2(targetRotation.x, -1 * targetRotation.y) * (180 / Mathf.PI);
            GetComponent<Rigidbody2D>().MoveRotation(rot);
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
                Debug.Log("We have changed our knife status to " + knifeStatus);
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
    void CmdAttackGameObject(GameObject target)
    {
        target.GetComponent<Health>().TakeDamage();
        RpcAttacked(target.transform.position);
    }
    [ClientRpc]
    public void RpcRespawn()
    {
        if (!isLocalPlayer)
            return;
        transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
    }
    [ClientRpc]
    void RpcAttacked(Vector3 attackSpot)
    {
        GetComponent<AudioSource>().PlayOneShot(KnifeStabSounds[Random.Range(0, KnifeStabSounds.Length)]);
    }
    //[Command]
    //void CmdSetRotation(float rot)
    //{
    //myRotation = rot;
    //}
}