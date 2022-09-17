using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FloodSimulation : MonoBehaviour
{

    [SerializeField]
    Transform floodHeight;
    float floodSpeed = 0.001f;

    DateTime startTime;

    private void Start()
    {
        startTime = DateTime.Now;
    }

    void Update()
	{
        DateTime currentTime = DateTime.Now;
        float deltaTime = (currentTime.Ticks - startTime.Ticks) / 1000000f;

        if(deltaTime > 2)
		{
            floodHeight.localPosition = new Vector3(0, floodHeight.localPosition.y + floodSpeed, 0);
            startTime = currentTime;
		}
	}
}
