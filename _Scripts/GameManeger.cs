using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManeger : MonoBehaviour
{
    public Text textTime, textResult;
    public GameObject enemies, player;
    float counter = 0.0f;
    public float levelMaxTime = 0;
    int enemiesCounter = 0;
    public bool start = false;
    public LevelLoadScript levelLoader;

    void Start()
    {
        textTime.text = counter.ToString() + "s";
    }

    void Update()
    {
        if (player == null) StartCoroutine(WaitReload(1f));
        if ((Input.GetKey("r"))) levelLoader.ReloadLevel();

        if ((Input.GetKey("w") || Input.GetMouseButtonDown(0)) && !start) 
        {
            start = true;
        }

        if (start)
        {
            enemiesCounter = 0;
            for (int i = 0; i < enemies.transform.childCount; i++) enemiesCounter ++;
            if (enemiesCounter == 0 && player != null) 
            {
                if (counter <= levelMaxTime) 
                {
                    textResult.text = "THE FLASH! YOU PASSED.";
                    StartCoroutine(WaitNext(3f));
                }
                else 
                {
                    textResult.text = "TOO SLOW! MAX LEVEL TIME IS " + levelMaxTime.ToString() + "s";
                    StartCoroutine(WaitReload(3f));
                }
            }
            else
            {
                counter += Time.deltaTime;
                textTime.text = counter.ToString("F2") + "s";
            }
        }
    }

    IEnumerator WaitNext(float time)
    {
        yield return new WaitForSeconds(time);
        levelLoader.LoadNextLevel();
    }

    IEnumerator WaitReload(float time)
    {
        yield return new WaitForSeconds(time);
        levelLoader.ReloadLevel();
    }
}
