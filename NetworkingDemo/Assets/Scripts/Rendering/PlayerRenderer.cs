using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


public class PlayerRenderer : MonoBehaviour
{
    [SerializeField] int ID;

    static Vector3 basePos = new Vector3(0,-1,0);
    [SerializeField] static int rot = 150; 
    static Vector3 baseRot;
    static Vector3 flipRot;
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
        baseRot = new Vector3(0, rot, 0);
        flipRot = new Vector3(0, rot - 180, 0);
    }

    
    private void Update()
    {
        if (Transport.current != null)
        {
            SimPlayer simPlayer = Transport.current.players[ID];
            gameObject.transform.position = basePos + simPlayer.pos.ToVec2();

            anim.SetBool("kick", simPlayer.state == PlayerState.PS_KICK);
            anim.SetBool("walk", simPlayer.state == PlayerState.PS_WALK);
            anim.SetBool("crouch", simPlayer.state == PlayerState.PS_CROUCH);
            anim.SetBool("jump", simPlayer.state == PlayerState.PS_AIRBORNE);
            anim.SetBool("punch", simPlayer.state == PlayerState.PS_PUNCH);
            anim.SetBool("slash", simPlayer.state == PlayerState.PS_SLASH);
            anim.SetBool("heavyslash", simPlayer.state == PlayerState.PS_HSLASH);
            anim.SetBool("land", simPlayer.IsGrounded);

            transform.rotation = Quaternion.Euler(simPlayer.facingLeft ? flipRot : baseRot);
        }
        
    }
}

