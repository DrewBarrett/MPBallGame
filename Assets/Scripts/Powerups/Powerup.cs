using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Powerup {

    public GameObject Parent;
    public GameObject PowerupObject;
    public AudioClip EquipSound = null;
    public AudioClip[] AttackSounds = null;

    public virtual void OnLeftClick()
    {
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.DrawLine(Parent.transform.position, target, Color.red, .10f);
        //Gizmos.DrawLine(transform.position, target);
        Vector3 targetRotation = Parent.transform.position - target;
        float rot = Mathf.Atan2(targetRotation.x, -1 * targetRotation.y) * (180 / Mathf.PI);
        //GetComponent<Rigidbody2D>().MoveRotation(rot);
        Parent.gameObject.transform.rotation = Quaternion.Euler(0, 0, rot);
        Parent.GetComponent<Rigidbody2D>().velocity = Parent.transform.up * 2;
        Parent.GetComponent<NetworkTransform>().SetDirtyBit(1);
        //CmdSetRotation(rot);
    }

    public virtual void OnRightClick()
    {

    }

    public virtual void SetVisible(bool status)
    {
        //do nothing because there are many behaviors for this
    }

    public virtual void OnTriggerEnter(Collider2D collision)
    {
        //who knows
    }

    public virtual void Destroy()
    {
        //put yourself away in preparation for next powerup
    }

    public virtual void Update()
    {
        //do updates here
    }

    public virtual void DoAttack()
    {

    }
}
