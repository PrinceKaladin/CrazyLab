using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelmanager : MonoBehaviour
{
    public void sceneload(int level) {
        SceneManager.LoadScene(level);
    }
}
