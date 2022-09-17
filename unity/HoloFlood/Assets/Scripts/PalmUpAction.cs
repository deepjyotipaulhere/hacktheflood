using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmUpAction : MonoBehaviour
{
    public Player player;

    public void OnPalmUp(){
        Debug.Log("Go");
        player.OnPalmUp();
    }
}
