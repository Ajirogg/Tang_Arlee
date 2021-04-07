using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningZone : MonoBehaviour
{
    //Check winning condition in trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PlayerController>().HasAllPartOfArmor())
            {
                Debug.Log("Do some stuff ! You Win !!!");
            }
        }
    }
}
