using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Statistics : MonoBehaviour
{
    public static Statistics Instance;
    public static int scapes;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            // GameManager.onClick += FadeIn;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu") return;
        scapes = PlayerPrefs.GetInt("Scapes", 0);
        MainMenu.Instance.SetStatistics(scapes);
    }

    public void UpdateStatistics(string scape)
    {
        if(PlayerPrefs.GetInt(scape, 0) == 1)
        {
            return;
        }

        scapes++;
        PlayerPrefs.SetInt("Scapes", scapes);
        PlayerPrefs.SetInt(scape, 1);


    }

}
