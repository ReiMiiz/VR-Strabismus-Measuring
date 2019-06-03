using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class boardScript : MonoBehaviour
{
    float[] pos2d = { -0.5f, -0.258819f, 0.0f, 0.258819f, 0.5f };
    float[] posz = { 1.0f, 0.965926f, 0.866025f, 0.933013f, 0.836516f, 0.750000f };
    int x, y;

    public GameObject target;
    public GameObject uTarget;
    public float sensitivity = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        new FileInfo(Application.persistentDataPath + "\\" + "asdasdkuykuy.txt");
        x = 2;
        y = 2;
    }

    // Update is called once per frame
    void Update()
    {


        target.transform.position = new Vector3(pos2d[x+2], pos2d[y+2], posz[getPosZ(x, y)]);

        //KEYBOARD CONTROL
        
        if (Input.GetButtonDown("Horizontal"))
        {
            Debug.Log("RIGHT!!");
            x = x < 2 ? x+1 : 2;
        }
        else if (Input.GetButtonDown("Horizontal"))
        {
            Debug.Log("LEFT!!");
            x = x > -2 ? x-1 : -2;
        }
        else if(Input.GetButtonDown("Vertical"))
        {
            Debug.Log("UP!!");
            y = y < 2 ? y+1 : 2;
        }
        else if(Input.GetButtonDown("Vertical"))
        {
            Debug.Log("DOWN!!");
            y = y > -2 ? y-1 : -2;
        }
    }

    int getPosZ(int x, int y)
    {
        int dX = Math.Abs(x);
        int dY = Math.Abs(y);
        if (dX == 0 && dY == 0) return 0;
        else if ((dX == 0 && dY == 1) || (dX == 1 && dY == 0)) return 1;
        else if ((dX == 2 && dY == 0) || (dX == 0 && dY == 2)) return 2;
        else if (dX == 1 && dY == 1) return 3;
        else if ((dX == 1 && dY == 2) || (dX == 2 && dY == 1)) return 4;
        else if (dX == 2 && dY == 2) return 5;
        else return 0;
    }
}
