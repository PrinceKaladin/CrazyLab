using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class draglevel : MonoBehaviour
{
    private void Update()
    {
        CheckLevelComplete();
    }
    void CheckLevelComplete()
    {
        if (GameObject.Find("pop") == null)
        {
            SceneManager.LoadScene(2);
        }


    }
}
