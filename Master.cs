using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour {

    public int[] door = new int[2] { 0,0};
    void Update()
    {
        door[0] = 0;
        door[1] = 0;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {

            Debug.Log(hit.transform.name);
            if (hit.transform.name == "door1")
            {
                door[0] = 1;
            }
            else if (hit.transform.name == "door2")
            {
                door[1] = 1;
            }
        }


    }

}
