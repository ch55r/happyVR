using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class controller : MonoBehaviour {
    GameObject rayc;
    string state="";
    public Text inputText;
    // Use this for initialization
    void Start () {
        rayc = GameObject.Find("GearVRPointer");
    }
	
	// Update is called once per frame
	void Update () {
        state=rayc.GetComponent<GearVRPointer>().state;

        inputText.text = state;
    }
}
