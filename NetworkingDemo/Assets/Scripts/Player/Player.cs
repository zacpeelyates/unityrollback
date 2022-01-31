using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   public void PerformAction(PlayerAction action)
    {
        action.Execute(this);
    }
}
