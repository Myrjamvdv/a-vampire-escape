using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    public float restartDelay;

    void Start ()
    {
        StartCoroutine (RestartAfterDelay ());
    }

    IEnumerator RestartAfterDelay ()
    {
        yield return new WaitForSeconds (restartDelay);
        SceneManager.LoadScene ("Scene1");
    }
}
