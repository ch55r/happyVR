using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//raycast애니메이션
public class ani : MonoBehaviour {

    int[] door = new int[4];
    GameObject rayc;
    int doornum = 0;
    string doorname = "";

    int num=0;
    public Animation anim;
    Collider coll;
    AudioSource aud;

    private void Start()
    {
        rayc = GameObject.Find("OVRCameraRig");
        anim = GetComponent<Animation>();
        coll = GetComponent<Collider>();
        aud = GetComponent<AudioSource>();
    }
    void Update ()
    {
        for (int i = 0; i < 4; i++)
        {
            door[i] = rayc.GetComponent<GearVRPointer>().door[i];
        }

        switch (gameObject.name)
        {
            case "door1":
                doornum = 0;
                break;
            case "door2":
                doornum = 1;
                break;
            case "door3":
                doornum = 2;
                break;
            case "toilet":
                doornum = 3;
                break;

        }
        if (door[doornum]==1)
        {
            if (anim.isPlaying)
            { Debug.Log("playing"); }
            else if (num == 0)
            {

                anim["Take 001"].time = 0;
                anim["Take 001"].speed = 10f;
                anim.Play();
                aud.Play();
                coll.isTrigger = true;
                Debug.Log(num);
                num = 1;
            }
            else if (num == 1)
            {
                anim["Take 001"].speed = -10f;
                anim["Take 001"].time = anim["Take 001"].length;
                anim.Play();
                aud.Play();
                coll.isTrigger = false;
                Debug.Log(num);
                num = 0;

            }
        }
    }

}
