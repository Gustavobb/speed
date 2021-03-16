using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScript : MonoBehaviour
{
    public LevelLoadScript levelLoader;
    bool start = false;
    void Update()
    {
        if (Input.GetKey("space") && !start) 
        {
            start = true;
            GetComponent<AudioSource>().Play();
            levelLoader.LoadNextLevel();
        }
    }
}
