using UnityEngine;
using System.Collections;

public class GunPowerup : PowerupChild {
    
    public GunPowerup(GameObject parent)
    {
        Parent = parent;
        PowerupObject = FindChildOfName("gun");
        EquipSound = Parent.GetComponent<PlayerControl>().GunEquipSound;
        Parent.GetComponent<PlayerControl>().CmdSetPowerup("gun");
    }

    public override void OnLeftClick()
    {
        if (!isOut)
            base.OnLeftClick();
        else
        {

        }
    }
}
