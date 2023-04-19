using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    public string charName;
    public int STR;
    public int MAG;
    public int DEF;
    public int RES;
    public int SPD;
    public int MOV; 

    public int movLeft;

    public void resetMove()
    {
        movLeft = MOV;
    }

    // Start is called before the first frame update
    void Start()
    {       
        resetMove();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
