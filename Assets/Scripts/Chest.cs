using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    //Part of armor
    public GameObject loot = null;

    //Need to avoid re-openning
    public bool isOpen = false;


    public GameObject OpenChest()
    {
        Debug.Log("OPEN (ANIM)");
        isOpen = true;
        return loot;
    }
}
