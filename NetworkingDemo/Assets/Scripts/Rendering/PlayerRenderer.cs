using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


public class PlayerRenderer : MonoBehaviour
{
    [SerializeField] int ID;

    static Vector3 basePos = new Vector3(0,-1,0);

    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
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
        }
        
        
    }
}

