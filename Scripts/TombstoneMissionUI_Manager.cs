using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Linq;

public class TombstoneMissionUI_Manager : MonoBehaviour
{
    public static TombstoneMissionUI_Manager instance { get; private set; }


    //public GameObject missionPopup; //미션 선택 찾기 팝업
    public GameObject missionBar;   //미션바
    public GameObject missionPopup2; //미션 선택 뽑기 팝업
    public GameObject successPopup; //미션 성공 팝업
    public Text suceesText; //성공팝업텍스트
    public GameObject textBack; //텍스트 나오는 박스
    public Text storyText;  //나레이션 텍스트
    public GameObject ston; //비석
    public GameObject knife;    //환두대도

    public Transform mainCamera;
    public Transform[] cameraPos;   //카메라 전진 타겟

    public GameObject partical; //빛나는 효과
    public GameObject fadeEffect;   //페이드 효과 스크립트
    FadeEffect fade_sripts;

    float cameraSpeed = 2f;

    bool cameraMovingState; //카메라 무빙

    string beforeText;
    private static readonly string[] INVALID_CHARS = {
  " ", "　", "!", "?", "！", "？", "\"", "\'", "\\",
  ".", ",", "、", "。", "…", "・"
};


    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else instance = this;
    }

    void Start()
    {
        SoundMaixerManager.instance.TombastoneBGMPlay();
        SoundMaixerManager.instance.FireSoundPlay();
        beforeText = storyText.text;
        fade_sripts = fadeEffect.GetComponent<FadeEffect>();

        //PlayerPrefs.SetString("HPG_TombstoneGame", "No");

        Debug.Log("HPG_TombstoneGame " + PlayerPrefs.GetString("HPG_TombstoneGame"));
        Debug.Log("HPG_TombstoneMission " + PlayerPrefs.GetString("HPG_TombstoneMission"));

        if ((PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No") &&
            PlayerPrefs.GetString("HPG_TombstoneMission").Equals("Fail")) ||
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals(""))
        {
            StartCoroutine(TextShowBox_Before());
        }
        //환두대도 뽑아야하는 미션 지령
        else if(PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start") &&
            PlayerPrefs.GetString("HPG_TombstoneMission").Equals("Success"))
        {
            StartCoroutine(MissionTextBoxShow());
        }
        
    }

    
    void Update()
    {
        if (PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No") ||
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals(""))
        {
            if (cameraMovingState.Equals(true))
                mainCamera.position = Vector3.MoveTowards(mainCamera.position, cameraPos[0].position, cameraSpeed * Time.deltaTime);
        }

        //환두대도 뽑기 성공하면 게임 끝으로 
        if (PlayerPrefs.GetString("HPG_TombstoneMission").Equals("End"))
        {
            PlayerPrefs.SetString("HPG_TombstoneMission", "Success");
            TextShowBox_End();
        }

    }

    //처음 비석씬에 들어왔을때 스토리 진행 텍스트
    IEnumerator TextShowBox_Before()
    {
        yield return new WaitForSeconds(1f);

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("어.. 저기 희미한 빛 아래에 뭔가가 놓여져 있어.\n" +
            "가까이 가보자.", 3.5f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(4.5f);
        textBack.SetActive(false);

        yield return new WaitForSeconds(1f);
        cameraMovingState = true;
        SoundMaixerManager.instance.WalkSoundPlay();
        yield return new WaitForSeconds(3f);

        SoundMaixerManager.instance.WalkSoundStop();
        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("[고리자루 큰 칼을 찾아서 칼을 뽑으면 길이 열릴지니]라고 적혀있어.\n" +
            "비석에 그려진 고리자루 큰 칼을 찾아봐야겠군!", 6f).SetEase(Ease.Linear).
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
        //UpButtonsCtrl.instance.MissionPopup(0);

        if (fade_sripts.fadeStartState == false)
        {
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());    //점점 하얗게
        }

        yield return new WaitForSeconds(2f);
        UpButtonsCtrl.instance.TombStoneGamePlay();
    }

    //중간 미션 - 환두대도 뽑기 미션
    IEnumerator MissionTextBoxShow()
    {
        mainCamera.position = cameraPos[0].position;

        ston.SetActive(false);  //비석 없애기 
        knife.SetActive(true);  //환두대도 활성화
        yield return new WaitForSeconds(1f);

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("고리자루 큰 칼을 찾았어! 그런데 이 칼... 돌에 꽂혀있잖아?\n" +
            "비석에 적힌대로 돌에 꽂힌 칼을 뽑아야 길이 열리겠군...!", 6f).SetEase(Ease.Linear).
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
        SoundMaixerManager.instance.MissionSoundPlay();
        missionPopup2.SetActive(true);
    }

    //미션바 시간 후 사라지게
    public void MissionBarDisappear()
    {
        StartCoroutine(_MissionBarDisappear());
    }

    IEnumerator _MissionBarDisappear()
    {
        missionBar.SetActive(true);
        yield return new WaitForSeconds(5f);
        missionBar.SetActive(false);
    }

    //미션 성공 팝업 
    public void MissionSuccessPopup()
    {
        successPopup.SetActive(true);

        //if (PlayerPrefs.GetString("HPG_OneGame").Equals("Success"))
            suceesText.text = "'고리자루 큰 칼'을 획득하였습니다.";
        //else if(PlayerPrefs.GetString("HPG_TwoGame").Equals("Success"))
        //    suceesText.text = "'등잔모양토기'를 획득하였습니다.";
        //else if (PlayerPrefs.GetString("HPG_ThreeGame").Equals("Success"))
        //    suceesText.text = "'짧은 목 항아리'를 획득하였습니다.";
    }

    IEnumerator FadeStart()
    {
        yield return new WaitForSeconds(0.5f);
        fade_sripts.Fade(); //점점 어둡게 실행
    }

    
    public void TextShowBox_End()
    {
        StartCoroutine(_TextShowBox_End());
    }

    //미션 성공 후 스토리 진행
    IEnumerator _TextShowBox_End()
    {
        mainCamera.position = cameraPos[0].position;

        yield return new WaitForSeconds(1f);
        

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("드디어 고리자루 큰 칼을 뽑았어..!!!\n" +
            "엇! 갑자기 빛이..!어디서 나오는 빛이지...? 으아아앗!!!!", 4f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });
        yield return new WaitForSeconds(2f);
        partical.SetActive(true);
        yield return new WaitForSeconds(2f);
        if (fade_sripts.fadeStartState == false)
        {
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());    //점점 하얗게
        }

        yield return new WaitForSeconds(2f);
        textBack.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("EndingScene");
    }
}
