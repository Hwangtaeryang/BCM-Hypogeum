using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PuzzleGameUI_Manager : MonoBehaviour
{
    public static PuzzleGameUI_Manager instance { get; private set; }
    //public GameObject missionSuccessPopup;    //유물획득 팝업
    public Text relicNameText;  //유물이름텍스트
    public GameObject missionImg;   //미션바 이미지


    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else instance = this;

        
    }

    private void Start()
    {
        SoundMaixerManager.instance.PuzzleBGMPlay();
    }

    //미션바 시간 후 사라지게
    public void MissionBarDisappear()
    {
        StartCoroutine(_MissionBarDisappear());
    }

    IEnumerator _MissionBarDisappear()
    {
        yield return new WaitForSeconds(5f);
        missionImg.SetActive(false);
    }

    public void RelicAcquirePopup()
    {
        //유물획득 
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No"))
            relicNameText.text = "'투구'를 획득하였습니다.";
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("No"))
            relicNameText.text = "'목걸이'를 획득하였습니다.";
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("No"))
            relicNameText.text = "'화로모양 그릇받침'를 획득하였습니다.";
    }

    //내가 만든 함수 게임 종료 씬
    public void PuzzleGameEndSceneMove()
    {
        PlayerPrefs.SetString("HPG_PuzzleGame", "Yes");
        SceneManager.LoadScene("PuzzleGame");
    }
}
