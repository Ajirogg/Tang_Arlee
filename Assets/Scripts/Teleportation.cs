using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Teleportation : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {
        // As needed
    }

    // Update is called once per frame
    void Update()
    {
        // As needed
    }
    
    public void OnTriggerEnter(Collider other)
    {
        // If the tag of the colliding object is the same, load the scene
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("MainLevel", LoadSceneMode.Single);
        }
    }
}