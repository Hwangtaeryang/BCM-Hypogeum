using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTest : MonoBehaviour
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void FirstGatewayChoice()
    {
        PlayerPrefs.SetString("HPG_FirstGateway", "One");
        SceneManager.LoadScene("GameChoiceScene");
    }

    public void PuzzleTest()
    {
        PlayerPrefs.SetString("HPG_PuzzleGame", "Yes");
        SceneManager.LoadScene("PuzzleGame");
    }

    public void TombStoneTest()
    {
        PlayerPrefs.SetString("HPG_TombstoneGame", "Yes");
        SceneManager.LoadScene("TombstoneGame");
    }
}
