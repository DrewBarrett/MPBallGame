using UnityEngine;
using System.Collections;

public class KnifePowerup : PowerupChild {

    public KnifePowerup(GameObject parent)
    {
        Parent = parent;
        PowerupObject = FindChildOfName("knife");
        EquipSound = Parent.GetComponent<PlayerControl>().KnifeEquipSound;
        AttackSounds = Parent.GetComponent<PlayerControl>().KnifeStabSounds;
        //Parent.GetComponent<PlayerControl>().
    }

    public override void OnTriggerEnter(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Health>() && isOut)
        {
            DoAttack();
            Parent.GetComponent<PlayerControl>().CmdDoAttack();
            Parent.GetComponent<PlayerControl>().CmdAttackGameObject(collision.gameObject, Parent.gameObject);
        }
    }

    
}
