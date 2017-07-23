using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public string level1SceneName;

    void Update ()
    {
        if (Input.GetMouseButtonUp (0)) {
            SceneManager.LoadScene (level1SceneName);
        }
    }
}
