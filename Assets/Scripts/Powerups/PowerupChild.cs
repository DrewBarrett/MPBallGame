using UnityEngine;
using System.Collections;

public class PowerupChild : Powerup {
    //this class is used when the powerup is a gameobject attached to the player

    //is the attached gameObject visible and ready?
    public bool isOut = false;
    

    public override void OnRightClick()
    {
        base.OnRightClick();
        isOut = !isOut;
        if (isOut)
        {
            Parent.GetComponent<AudioSource>().PlayOneShot(EquipSound);
        }
        SetVisible(isOut);
        Parent.GetComponent<PlayerControl>().CmdSetWeaponVisible(isOut);
    }

    public override void SetVisible(bool status)
    {
        //if we die and our character tells us to put our knife away we damn well better do it
        isOut = status;
        PowerupObject.SetActive(status);
    }

    public override void Destroy()
    {
        SetVisible(false);
        Parent.GetComponent<PlayerControl>().CmdSetWeaponVisible(false);
    }

    public GameObject FindChildOfName(string name)
    {
        SpriteRenderer[] srs = Parent.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in srs)
        {
            if(sr.gameObject.name == name)
            {
                return sr.gameObject;
            }
        }
        Debug.LogError("Could not find child of name " + name + " in parent " + Parent.ToString());
        return null;
    }

    public override void DoAttack()
    {
        Parent.GetComponent<AudioSource>().PlayOneShot(AttackSounds[Random.Range(0, AttackSounds.Length)]);
    }

}
