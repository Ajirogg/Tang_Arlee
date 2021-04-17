using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    public GameObject loot = null;
    private bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject OpenChest()
    {
        Debug.Log("OPEN (ANIM)");
        isOpen = true;
        return loot;
    }


}
