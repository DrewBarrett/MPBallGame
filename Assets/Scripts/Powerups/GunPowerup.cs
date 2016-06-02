using UnityEngine;
using System.Collections;

public class GunPowerup : PowerupChild {

    int ammo = 9;

    public GunPowerup(GameObject parent)
    {
        Parent = parent;
        PowerupObject = FindChildOfName("gun");
        EquipSound = Parent.GetComponent<PlayerControl>().GunEquipSound;
        AttackSounds = Parent.GetComponent<PlayerControl>().GunShootSounds;
        //Parent.GetComponent<PlayerControl>().CmdSetPowerup("gun");
    }

    public override void DoAttack()
    {
        if(ammo >= 1)
        {
            Parent.GetComponent<AudioSource>().PlayOneShot(AttackSounds[Random.Range(0, AttackSounds.Length)]);
            ammo--;
        }
        else
        {
            Parent.GetComponent<AudioSource>().PlayOneShot(Parent.GetComponent<PlayerControl>().GunEmptySound);
        }
        if (AttackSounds.Length <= 0)
        {
            Debug.LogError("We havent synced the powerup right and there is no attack sounds!!!!");
        }
    }

    public override void OnLeftClick()
    {
        if (!isOut)
            base.OnLeftClick();
        else
        {
            if (ammo >= 1)
            {

                RaycastHit2D[] hit = Physics2D.RaycastAll(Parent.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - Parent.transform.position);
                foreach (RaycastHit2D target in hit)
                {
                    //Debug.Log(target.transform.gameObject.ToString());
                    if (target.transform.gameObject == Parent)
                    {
                        continue;
                    }
                    if (target.transform.gameObject.GetComponent<Health>())
                    {
                        Parent.GetComponent<PlayerControl>().CmdAttackGameObject(target.transform.gameObject, Parent);
                    }
                    else if (target.transform.gameObject.tag == "Wall")
                    {
                        break;
                    }
                }
            }
            //Parent.GetComponent<AudioSource>().PlayOneShot(Parent.GetComponent<PlayerControl>().GunShootSounds[Random.Range(0, Parent.GetComponent<PlayerControl>().GunShootSounds.Length)]);
            DoAttack();
            Parent.GetComponent<PlayerControl>().CmdDoAttack();
            
            
        }
    }
    public override void Update()
    {
        if (isOut)
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.DrawLine(Parent.transform.position, target, Color.red, .10f);
            Debug.DrawRay(Parent.transform.position, target - Parent.transform.position, Color.red, .05f);
            //Gizmos.DrawLine(transform.position, target);
            Vector3 targetRotation = Parent.transform.position - target;
            float rot = Mathf.Atan2(targetRotation.x, -1 * targetRotation.y) * (180 / Mathf.PI);
            //GetComponent<Rigidbody2D>().MoveRotation(rot);
            PowerupObject.transform.rotation = Quaternion.Euler(0, 0, rot + 90);
        }
    }
}
