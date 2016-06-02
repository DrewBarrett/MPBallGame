using UnityEngine;
using System.Collections;

public class KnifePowerup : PowerupChild {

    public KnifePowerup(GameObject parent)
    {
        Parent = parent;
        PowerupObject = FindChildOfName("knife");
        EquipSound = Parent.GetComponent<PlayerControl>().KnifeEquipSound;
        Parent.GetComponent<PlayerControl>().CmdSetPowerup("knife");
    }

    public override void OnTriggerEnter(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Health>() && isOut)
        {
            Parent.GetComponent<PlayerControl>().CmdAttackGameObject(collision.gameObject, Parent.gameObject);
        }
    }
}
