using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject Lobby;
    public static bool returned = false;
    /*
    public void PlayGame()
    {
        Debug.Log("Play");
        SceneManager.LoadScene("Game");
    }
    */
    public void QuitGame()
    {
        Debug.Log("exit");
        Application.Quit();
    }

    private void Start()
    {
        if (returned)
        {
            Lobby.SetActive(true);
            transform.gameObject.SetActive(false);
        }
    }

}
