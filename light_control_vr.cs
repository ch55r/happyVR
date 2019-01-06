using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class light_control_vr : MonoBehaviour
{
    int[] swit = new int[6];
    GameObject rayc;
    AudioSource aud;
    public GameObject light1;
    int swnum;
    int ck = 1;
    // Use this for initialization
    void Start()
    {
        aud = GetComponent<AudioSource>();
        rayc = GameObject.Find("OVRCameraRig");
    }

    private void Update()
    {
        for (int i = 0; i < 6; i++)
        {
            swit[i] = rayc.GetComponent<GearVRPointer>().swit[i];
        }

        switch (gameObject.name)
        {
            case "swit_liv":
                swnum = 0;
                break;
            case "swit_room1":
                swnum = 1;
                break;
            case "swit_room2":
                swnum = 2;
                break;
            case "swit_room3":
                swnum = 3;
                break;
            case "swit_kitch":
                swnum = 4;
                break;
            case "swit_toilet":
                swnum = 5;
                break;
        }
        if (swit[swnum] == 1)
        {
            if (ck == 1)
            {
                light1.GetComponent<Light>().enabled = false;
                ck = 0;
                aud.Play();
            }
            else
            {
                light1.GetComponent<Light>().enabled = true;
                ck = 1;
                aud.Play();
            }

        }
    }
    public void Empty()
    { }
}