using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmDownAction : MonoBehaviour
{
    public Player player;
    public void OnPalmDown(){
        Debug.Log("Stop");
        player.OnPalmDown();
    }
}