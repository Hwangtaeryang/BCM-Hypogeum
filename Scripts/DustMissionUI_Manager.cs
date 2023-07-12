using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DustMissionUI_Manager : MonoBehaviour
{
    public GameObject candleObj;    //등잔모양토기
    CandleTiggerCtrl candle_Script;
    public GameObject[] fireObj;    //등잔 불
    public GameObject lightObj;    //동굴끝 빛
    public GameObject touchBtnObj;  //터치버튼
    public GameObject missionBar;   //미션바
    public GameObject missionPopup2; //미션 선택 팝업
    public GameObject textBack; //텍스트 나오는 박스
    public Text storyText;  //나레이션 텍스트

    public CinemachineVirtualCamera virtualCam; //가상카메라
    CinemachineBasicMultiChannelPerlin _virtalCam;
    public CinemachineDollyCart followObj;    //따라갈 오브젝트

    public GameObject fadeEffect;   //페이드 효과 스크립트
    FadeEffect fade_sripts;

    string beforeText;
    private static readonly string[] INVALID_CHARS = {
  " ", "　", "!", "?", "！", "？", "\"", "\'", "\\",
  ".", ",", "、", "。", "…", "・"
};



    void Start()
    {
        SoundMaixerManager.instance.DustBGMPlay();
        SoundMaixerManager.instance.FireSoundPlay();

        candle_Script = candleObj.GetComponent<CandleTiggerCtrl>();
        _virtalCam = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        fade_sripts = fadeEffect.GetComponent<FadeEffect>();
        beforeText = storyText.text;


        if ((PlayerPrefs.GetString("HPG_DustGame").Equals("No") &&
            PlayerPrefs.GetString("HPG_DustMisson").Equals("Fail")) ||
            PlayerPrefs.GetString("HPG_DustGame").Equals(""))
        {
            StartCoroutine(TextShowBox_Before());
        }
        else if (PlayerPrefs.GetString("HPG_DustGame").Equals("Start") &&
            PlayerPrefs.GetString("HPG_DustMisson").Equals("Success"))
        {
            StartCoroutine(TextShow_Conter());
        }
    }

    
    void Update()
    {
        //촛대 먼지 다 털었으면
        if (candle_Script.dustMission.Equals(true))
        {
            candle_Script.dustMission = false;
            StartCoroutine(TextShow_End());
        }
    }

    IEnumerator FadeStart()
    {
        yield return new WaitForSeconds(0.5f);
        fade_sripts.Fade(); //점점 어둡게 실행
    }

    //첫 게임 시 미션 나레이션
    IEnumerator TextShowBox_Before()
    {
        yield return new WaitForSeconds(1f);

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("여기는 너무 어두워...하나 있는 촛불은 다 녹아가는군.\n" +
            "뭔가 불을 밝힐 등잔 같은게 없을까?\n" +
            "촛불이 꺼지기 전에 잡아야겠어!", 8f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(9f);

        textBack.SetActive(false);

        if (fade_sripts.fadeStartState == false)
        {
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());    //점점 하얗게
        }

        yield return new WaitForSeconds(2f);
        UpButtonsCtrl.instance.DustGamePlay();
        //PlayerPrefs.SetString("HPG_DustGame", "Start");
        //SceneManager.LoadScene("Game_1");
    }

    //중간 미션 - 먼지털기
    IEnumerator TextShow_Conter()
    {
        candleObj.SetActive(true);  //등잔토기 활성화
        yield return new WaitForSeconds(1f);

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("와! 등잔 모양 토기를 찾았어! 이제 불을 밝힐 수 있겠군!\n" +
            "흠...흙이 많이 붙어있네...손으로 흙을 털어내서 사용해야겠어!", 6.5f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(7f);
        textBack.SetActive(false);

        yield return new WaitForSeconds(1f);
        touchBtnObj.SetActive(true);
        SoundMaixerManager.instance.MissionSoundPlay();
        missionPopup2.SetActive(true);
    }

    IEnumerator TextShow_End()
    {
        //candleObj.GetComponent<ObjectRotate>().enabled = false; //오브젝트 회전스크립트 끄기
        yield return new WaitForSeconds(1f);
        //불 활성화
        for (int i = 0; i < 4; i++)
            fireObj[i].SetActive(true);
        SoundMaixerManager.instance.FireCatchesSoundPlay();

        yield return new WaitForSeconds(1f);
        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("등잔 모양 토기에 불이 붙었어!\n" +
            "이젠 길을 찾아봐야지! 계속 걸어가면 분명 나가는 길이 나올거야!", 5f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(3f);
        SoundMaixerManager.instance.WalkSoundPlay();
        followObj.m_Speed = 5;

        yield return new WaitForSeconds(3.5f);
        lightObj.SetActive(true);
        storyText.text = "";
        storyText.DOText("어..? 저기 이 길의 끝에서 빛이 나는데...?", 3f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(5f);
        textBack.SetActive(false);

        yield return new WaitForSeconds(4f);
        SoundMaixerManager.instance.LightSoundPlay();
        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("으아아아앗!!", 1f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(0.5f);
        SoundMaixerManager.instance.WalkSoundStop();
        yield return new WaitForSeconds(1.5f);
        if (fade_sripts.fadeStartState == false)
        {
            textBack.SetActive(false);
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());    //점점 하얗게
        }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("EndingScene");
    }

}
