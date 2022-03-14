using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Renderer : MonoBehaviour
{
    [SerializeField]
    GameObject p1;

    [SerializeField]
    GameObject p2;

    [SerializeField]
    float zPos;

    // Update is called once per frame
    void Update()
    {
        GameState current = Transport.current;
        if (current != null)
        {
            FVec2 pos1 = current.players[0].pos;
            FVec2 pos2 = current.players[1].pos;

            p1.transform.position = new Vector3(pos1.x.ToFloat, pos1.y.ToFloat, zPos);
            p2.transform.position = new Vector3(pos2.x.ToFloat, pos2.y.ToFloat, zPos);
            //can add lots of nice undertiminestic float lerps and such here but testing for now
        }
        
    }
}

