﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanArea: MonoBehaviour
{
    //VARIABLES
    public GameObject player;
    private bool inZone;
    public GameObject camPos;

    //UPDATES
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        if (inZone)
        {
            Camera.main.transform.LookAt(player.transform);
        }
    }
    //METHODS
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (CameraFollowArea cf in FindObjectsOfType<CameraFollowArea>())
                cf.StopFollow();
            inZone = true;
            Camera.main.transform.position = camPos.transform.position;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        inZone = false;
    }
}
