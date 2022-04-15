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

    [SerializeField] float scale;

    private void Start()
    {
        p1.gameObject.transform.localScale *= scale;
        p2.gameObject.transform.localScale *= scale;
    }

    // Update is called once per frame
    void Update()
    {
        GameState current = Transport.current;
        if (current != null)
        {
            FVec2 pos1 = current.players[0].pos;
            FVec2 pos2 = current.players[1].pos;

            p1.transform.position = new Vector3(pos1.x.ToFloat, pos1.y.ToFloat, zPos) * scale;
            p2.transform.position = new Vector3(pos2.x.ToFloat, pos2.y.ToFloat, zPos) * scale;
            //can add lots of nice undertiminestic float lerps and such here but testing for now

            Transport.current = null;
        }
        
    }
}

