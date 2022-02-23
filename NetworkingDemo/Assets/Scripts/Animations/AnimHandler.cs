using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHandler : MonoBehaviour
{
    [SerializeField]
    List<Animation> AnimationList;

   void PlayAnimation(int i)
    {
        AnimationList[i].Play();
    }
}
