using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;


public class OenStepGamesUI_Manager : MonoBehaviour
{
    public static OenStepGamesUI_Manager instance { get; private set; }


    //public UnityEngine.UI.Image modelImage;    //미션 모델
    //public Sprite[] modelSprite;  //해당 미션 사진
    //public GameObject missionPopup; //미션 팝업
    //public UnityEngine.UI.Image missionPoupImg;    //미션팝업 안 이미지
    public Sprite[] outLineSprite;  //미션보기틀이미지
    public UnityEngine.UI.Image aHandOutLine;  //미션보기틀
    public GameObject missionBar;   //미션바
    public GameObject missionSuccessPopup;    //유물획득 팝업
    public Text relicNameText;  //유물이름텍스트
    public UnityEngine.UI.Image relicNameImg;
    public Sprite[] relicSprite;

    public GameObject[] knifeCount; //칼 종류(돌박힌칼, 그냥칼)

    public GameObject textBack; //텍스트 나오는 백배
    public Text storyText;  //나레이션 텍스트

    public GameObject fadeEffect;   //페이드 효과 스크립트
    FadeEffect fade_sripts;

    //public GameObject partical; //파티클
    public TrackableBehaviour[] imgTarget;


    bool missionStart;  //미션 시작 여부
    bool findRelicState;    //유물을 맞게 찾았는지
    bool noRelicOneShow;    //아니라는 텍스트 한번만 실행
    int vrState = 0; // 유물을 찾은 상태 0.무, 1.찾음, 2.찾은후

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
        fade_sripts = fadeEffect.GetComponent<FadeEffect>();
        beforeText = storyText.text;

        ShowAhandofImage(); //미리보기
        Invoke("ShowModelImage", 1f);
        KnifeKindChoice();  //칼 종류 선택
        //ShowModelImage();
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

    //미션시작 팝업
    public void MissionStartButtonOn()
    {
        missionStart = true;
    }


    void Update()
    {
        //Debug.Log("뭔가 이상하다 ..HPG_SendGame : " + PlayerPrefs.GetString("HPG_SendGame"));
        //Debug.Log("뭔가 이상하다 ..HPG_SendMission : " + PlayerPrefs.GetString("HPG_SendMission"));

        if(missionStart.Equals(true))
            RelicFindDistinction(); //해당 유물 찾았는지 판별하기 위한 함수

        //if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One"))
        //해당씬의 유물을 찾았을 때
        if (findRelicState.Equals(true))
        {
            //현재 유물을 찾지 않았음
            if (vrState != 2)
                vrState = 1;    //유물을 찾음!(1로변경)
        }

        if (vrState.Equals(1))
        {
            //Debug.Log("==== " + PlayerPrefs.GetString("HPG_TombstoneGame"));
            SoundMaixerManager.instance.ARFindSoundPlay();
            vrState = 2;    //찾았으니 찾은 후로 변경(2로)
            //1차 게임 AR
            if((PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
                PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No"))|| 
                (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
                PlayerPrefs.GetString("HPG_DustGame").Equals("No")) ||
                (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
                PlayerPrefs.GetString("HPG_SendGame").Equals("No")))
                StartCoroutine(ModelFindTextShow());
            //3차 게임 AR
            else if ((PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
                PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start")) ||
                (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
                PlayerPrefs.GetString("HPG_DustGame").Equals("Start")) ||
                (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
                PlayerPrefs.GetString("HPG_SendGame").Equals("Start")))
                StartCoroutine(ThreeGameModelFindTextShow());
        }
    }

    //AR오브젝트가 떳는지 상태확인
    private bool IsTrackingMarker(string imageTargetName)
    {
        var imageTarget = GameObject.Find(imageTargetName);
        var trackable = imageTarget.GetComponent<TrackableBehaviour>();
        var status = trackable.CurrentStatus;
        Debug.Log(trackable.name);
        return status == TrackableBehaviour.Status.TRACKED;
    }

    //칼 종류 선택함수
    void KnifeKindChoice()
    {
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start"))
        {
            knifeCount[0].SetActive(true);
            knifeCount[1].SetActive(false);
        }
        else
        {
            knifeCount[0].SetActive(false);
            knifeCount[1].SetActive(true);
        }
    }


    //해당씬에서 해당 유물을 찾았는지의 판별하기 위한 함수
    void RelicFindDistinction()
    {
        //return imgTarget[0].CurrentStatus == TrackableBehaviour.Status.TRACKED;
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") && 
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No"))
        {
            if ((imgTarget[0].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                findRelicState = true;  //해당 씬의 찾은 유물이 맞다
                relicNameImg.gameObject.SetActive(true);  relicNameImg.sprite = relicSprite[0];
            }
            else if((imgTarget[1].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                if(noRelicOneShow.Equals(false))    //해당 씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "굽다리 접시"));
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[1];
            }
            else if ((imgTarget[2].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "재갈이"));
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[2];
            }
            else if ((imgTarget[3].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                //게임2 - 투구
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "투구"));
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[3];
            }
            else if ((imgTarget[4].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                //게임2 - 목걸이
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "목걸이"));
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[4];
            }
            else if ((imgTarget[5].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                //게임2 - 화로
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "화로 모양 그릇 받침이"));
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[5];
            }
            else if ((imgTarget[6].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                Debug.Log("찾앗다");
                //게임3 - 환두대두
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "고리자루 큰 칼이"));
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[6];
            }
            else if ((imgTarget[7].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                //게임3 - 먼지털기
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "등잔 모양 토기"));
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[7];
            }
            else if ((imgTarget[8].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                //게임3 - 모래담기
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "큰 항아리"));
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[8];
            }
            else
            {
                relicNameImg.gameObject.SetActive(false);
            }
        }
        else if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("No"))
        {
            if ((imgTarget[0].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[0];
                if (noRelicOneShow.Equals(false))
                    StartCoroutine(RelicNoFind(53, "신발 모양 토기"));
            }
            else if ((imgTarget[1].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[1];
                findRelicState = true;
            }
            else if ((imgTarget[2].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[2];
                if (noRelicOneShow.Equals(false))
                    StartCoroutine(RelicNoFind(54, "재갈이"));
            }
            else if ((imgTarget[3].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[3];
                //게임2 - 투구
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "투구"));
            }
            else if ((imgTarget[4].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[4];
                //게임2 - 목걸이
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "목걸이"));
            }
            else if ((imgTarget[5].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[5];
                //게임2 - 화로
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "화로 모양 그릇 받침이"));
            }
            else if ((imgTarget[6].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[6];
                //게임3 - 환두대두
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "고리자루 큰 칼이"));
            }
            else if ((imgTarget[7].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[7];
                //게임3 - 먼지털기
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "등잔 모양 토기"));
            }
            else if ((imgTarget[8].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[8];
                //게임3 - 모래담기
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "큰 항아리"));
            }
            else
            {
                relicNameImg.gameObject.SetActive(false);
            }
        }
        else if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("No"))
        {
            if ((imgTarget[0].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[0];
                if (noRelicOneShow.Equals(false))
                    StartCoroutine(RelicNoFind(53, "신발 모양 토기"));
            }
            else if ((imgTarget[1].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[1];
                if (noRelicOneShow.Equals(false))
                    StartCoroutine(RelicNoFind(54, "굽다리 접시"));
            }
            else if ((imgTarget[2].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[2];
                findRelicState = true;
            }
            else if ((imgTarget[3].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[3];
                //게임2 - 투구
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "투구"));
            }
            else if ((imgTarget[4].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[4];
                //게임2 - 목걸이
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "목걸이"));
            }
            else if ((imgTarget[5].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[5];
                //게임2 - 화로
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "화로 모양 그릇 받침이"));
            }
            else if ((imgTarget[6].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[6];
                //게임3 - 환두대두
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "고리자루 큰 칼이"));
            }
            else if ((imgTarget[7].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[7];
                //게임3 - 먼지털기
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "등잔 모양 토기"));
            }
            else if ((imgTarget[8].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[8];
                //게임3 - 모래담기
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "큰 항아리"));
            }
            else
            {
                relicNameImg.gameObject.SetActive(false);
            }
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start"))
        {
            if ((imgTarget[0].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[0];
                if (noRelicOneShow.Equals(false))    //해당 씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "신발 모양 토기"));
            }
            else if ((imgTarget[1].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[1];
                if (noRelicOneShow.Equals(false))    //해당 씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "굽다리 접시"));
            }
            else if ((imgTarget[2].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[2];
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "재갈이"));
            }
            else if ((imgTarget[3].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[3];
                //게임2 - 투구
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "투구"));
            }
            else if ((imgTarget[4].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[4];
                //게임2 - 목걸이
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "목걸이"));
            }
            else if ((imgTarget[5].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[5];
                //게임2 - 화로
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "화로 모양 그릇 받침이"));
            }
            else if ((imgTarget[6].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[6];
                //게임3 - 환두대두
                findRelicState = true;  //해당 씬의 찾은 유물이 맞다
            }
            else if ((imgTarget[7].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[7];
                //게임3 - 먼지털기
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "등잔 모양 토기"));
            }
            else if ((imgTarget[8].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[8];
                //게임3 - 모래담기
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "큰 항아리"));
            }
            else
            {
                relicNameImg.gameObject.SetActive(false);
            }
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("Start"))
        {
            if ((imgTarget[0].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[0];
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "신발 모양 토기"));
            }
            else if ((imgTarget[1].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[1];
                if (noRelicOneShow.Equals(false))    //해당 씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "굽다리 접시"));
            }
            else if ((imgTarget[2].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[2];
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "재갈이"));
            }
            else if ((imgTarget[3].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[3];
                //게임2 - 투구
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "투구"));
            }
            else if ((imgTarget[4].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[4];
                //게임2 - 목걸이
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "목걸이"));
            }
            else if ((imgTarget[5].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[5];
                //게임2 - 화로
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "화로 모양 그릇 받침이"));
            }
            else if ((imgTarget[6].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[6];
                //게임3 - 환두대두
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "고리자루 큰 칼이"));
            }
            else if ((imgTarget[7].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[7];
                //게임3 - 먼지털기
                findRelicState = true;  //해당 씬의 찾은 유물이 맞다
            }
            else if ((imgTarget[8].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[8];
                //게임3 - 모래담기
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "큰 항아리"));
            }
            else
            {
                relicNameImg.gameObject.SetActive(false);
            }
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("Start"))
        {
            if ((imgTarget[0].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[0];
                if (noRelicOneShow.Equals(false))    //해당 씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "신발 모양 토기"));
            }
            else if ((imgTarget[1].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[1];
                if (noRelicOneShow.Equals(false))    //해당 씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "굽다리 접시"));
            }
            else if ((imgTarget[2].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[2];
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "재갈이"));
            }
            else if ((imgTarget[3].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[3];
                //게임2 - 투구
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "투구"));
            }
            else if ((imgTarget[4].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[4];
                //게임2 - 목걸이
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "목걸이"));
            }
            else if ((imgTarget[5].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[5];
                //게임2 - 화로
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(54, "화로 모양 그릇 받침이"));
            }
            else if ((imgTarget[6].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[6];
                //게임3 - 환두대두
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "고리자루 큰 칼이"));
            }
            else if ((imgTarget[7].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[7];
                //게임3 - 먼지털기
                if (noRelicOneShow.Equals(false))   //해당씬의 찾은 유물이 아니다
                    StartCoroutine(RelicNoFind(53, "등잔 모양 토기"));
            }
            else if ((imgTarget[8].CurrentStatus == TrackableBehaviour.Status.TRACKED).Equals(true))
            {
                relicNameImg.gameObject.SetActive(true); relicNameImg.sprite = relicSprite[8];
                //게임3 - 모래담기
                findRelicState = true;  //해당 씬의 찾은 유물이 맞다
            }
            else
            {
                relicNameImg.gameObject.SetActive(false);
            }
        }
    }

    //미션미리보기
    void ShowAhandofImage()
    {
        Debug.Log(" ::: " + PlayerPrefs.GetString("HPG_SendGame"));
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No"))
        {
            //1-1 신발모양 - 53호분
            aHandOutLine.sprite = outLineSprite[0];
            UpButtonsCtrl.instance.AhandofTimeRelic(0); //미리보기 이미지
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("No"))
        {
            //1-2굽다리 - 54호분
            aHandOutLine.sprite = outLineSprite[1];
            UpButtonsCtrl.instance.AhandofTimeRelic(1); //미리보기 이미지
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("No"))
        {
            //1-3재갈 - 54호분
            aHandOutLine.sprite = outLineSprite[1];
            UpButtonsCtrl.instance.AhandofTimeRelic(2); //미리보기 이미지
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start"))
        {
            //3-1고리자루 - 53호분
            aHandOutLine.sprite = outLineSprite[0];
            UpButtonsCtrl.instance.AhandofTimeRelic(3); //미리보기 이미지
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("Start"))
        {
            //3-2 등잔모양 - 53호분
            aHandOutLine.sprite = outLineSprite[0];
            UpButtonsCtrl.instance.AhandofTimeRelic(4); //미리보기 이미지
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("Start"))
        {
            //3-3 큰항아리 - 54호분
            aHandOutLine.sprite = outLineSprite[1];
            UpButtonsCtrl.instance.AhandofTimeRelic(5); //미리보기 이미지
        }
    }

    //선택 미션에 해당하는 모델 이미지 
    void ShowModelImage()
    {
        Debug.Log(" ::: " + PlayerPrefs.GetString("HPG_SendGame"));
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No"))
        {
            UpButtonsCtrl.instance.MissionPopup(0); //미션 이미지
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("No"))
        {
            UpButtonsCtrl.instance.MissionPopup(1);
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("No"))
        {
            UpButtonsCtrl.instance.MissionPopup(2);
        } 
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start"))
        {
            UpButtonsCtrl.instance.MissionPopup(3);
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("Start"))
        {
            UpButtonsCtrl.instance.MissionPopup(4);
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("Start"))
        {
            UpButtonsCtrl.instance.MissionPopup(5);
        } 
    }

    //해당 미션이 어떤건지 팝업 열어서 확인
    public void MissionLookPopupShow()
    {
        Debug.Log("클릭햇음--------");
        StartCoroutine(_MissionLookPopupShow());
    }

    IEnumerator _MissionLookPopupShow()
    {
        yield return null;

        //Debug.Log("HPG_FirstGateway " + PlayerPrefs.GetString("HPG_FirstGateway"));
        //Debug.Log("HPG_TombstoneGame " + PlayerPrefs.GetString("HPG_TombstoneGame"));

        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No"))
            UpButtonsCtrl.instance.MissionPopup(0);
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("No"))
            UpButtonsCtrl.instance.MissionPopup(1);
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("No"))
            UpButtonsCtrl.instance.MissionPopup(2);
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start"))
            UpButtonsCtrl.instance.MissionPopup(3);
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("Start"))
            UpButtonsCtrl.instance.MissionPopup(4);
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("Start"))
            UpButtonsCtrl.instance.MissionPopup(5);
    }

    //AR게임 1차  - 해당씬의 해당 유물을 찾았을 경우
    IEnumerator ModelFindTextShow()
    {
        yield return new WaitForSeconds(1f);
        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No"))
        {
            storyText.DOText("53호분에서 출토된 '신발 모양 토기'야!\n" +
                "이 유물이 문에 그려진 그림과 같군!", 4f).SetEase(Ease.Linear).
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
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("No"))
        {
            storyText.DOText("54호분에서 출토된 '굽다리 접시'야!\n" +
                "그릇 부분은 얕고, 다리는 원통형으로 긴 편이지.\n" +
                "이 유물이 문에 그려진 그림과 같군!", 6f).SetEase(Ease.Linear).
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
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("No"))
        {
            storyText.DOText("54호분에서 발견된 재갈이야!\n" +
                "이 유물이 문에 그려진 그림과 같군!", 4f).SetEase(Ease.Linear).OnUpdate(() => {
                    var currentText = storyText.text;
                    if (beforeText == currentText)
                        return;

                    var newChar = currentText[currentText.Length - 1].ToString();

                    if (!INVALID_CHARS.Contains(newChar))
                        SoundMaixerManager.instance.TextSoundPlay();
                    beforeText = currentText;
                });
            yield return new WaitForSeconds(5f);
        }
        
        textBack.SetActive(false);

        yield return new WaitForSeconds(1f);
        RelicAcquirePopup();
    }

    //AR게임 3차 - 해당씬의 해당 유물을 찾았을 경우
    IEnumerator ThreeGameModelFindTextShow()
    {
        Debug.Log("여기에 들어왓다");
        yield return new WaitForSeconds(1f);
        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start"))
        {
            storyText.DOText("이건 53호분의 출토된 '고리자루 큰 칼'이야.\n" +
                "칼집에 넣어진 채로 부장되었고, 칼의 몸체와 고리 부분이\n" +
                "하나의 철판으로 구성되어있군. 이게 비석 속 칼이 틀림없어!", 7f).SetEase(Ease.Linear).
                OnUpdate(() => {
                    var currentText = storyText.text;
                    if (beforeText == currentText)
                        return;

                    var newChar = currentText[currentText.Length - 1].ToString();

                    if (!INVALID_CHARS.Contains(newChar))
                        SoundMaixerManager.instance.TextSoundPlay();
                    beforeText = currentText;
                });

            yield return new WaitForSeconds(8f);

            //if (fade_sripts.fadeStartState == false)
            //{
            //    fadeEffect.SetActive(true);
            //    StartCoroutine(FadeStart());
            //}
            //yield return new WaitForSeconds(4f);
            //GameTakeSave3_ClickOn();
            //SceneManager.LoadScene("TombstoneGame");
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("Start"))
        {
            storyText.DOText("53호분에서 출토된 '등잔 모양 토기'야.\n" +
                "투창이 여러개 뚫린 굽다리 토기 위에 4개의 잔이 부착된 형태야.\n" +
                "촛불의 불을 옮겨서 불을 밝히면 되겠어!", 6f).SetEase(Ease.Linear).
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

            //if (fade_sripts.fadeStartState == false)
            //{
            //    fadeEffect.SetActive(true);
            //    StartCoroutine(FadeStart());
            //}
            //yield return new WaitForSeconds(4f);
            //GameTakeSave3_ClickOn();
            //SceneManager.LoadScene("DustGame");
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("Start"))
        {
            storyText.DOText("이건 54호분에서 출토된 '큰 항아리'야.\n" +
                "어깨 부분에는 두 개의 꼭지가 있고, 항아리 바닥은 뾰족한 편이야.\n" +
                "이거면 떨어지는 모래를 담기 충분하겠어.", 6f).SetEase(Ease.Linear).
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

            //if (fade_sripts.fadeStartState == false)
            //{
            //    fadeEffect.SetActive(true);
            //    StartCoroutine(FadeStart());
            //}

            //GameTakeSave3_ClickOn();
            //yield return new WaitForSeconds(4f);
            
            //SceneManager.LoadScene("SendGame");
        }
        textBack.SetActive(false);
        yield return new WaitForSeconds(1f);
        RelicAcquirePopup();
    }

    IEnumerator FadeStart()
    {
        yield return new WaitForSeconds(0.5f);
        fade_sripts.Fade(); //점점 어둡게 실행
    }


    //해당씬의 해당 유물이 아닌 다른 유물을 찾았을 경우 텍스트 함수
    IEnumerator RelicNoFind(int _number, string _name)
    {
        //업데이트에 들어가 있어서 한번만 적용하기 위해서 ture로 변경
        noRelicOneShow = true; 

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";

        Debug.Log("여기기기!!! " + PlayerPrefs.GetString("HPG_SendGame"));
        Debug.Log("여기기기!!! " + PlayerPrefs.GetString("HPG_FirstGateway"));
        SoundMaixerManager.instance.ARFailSoundPlay();
        if (PlayerPrefs.GetString("HPG_PuzzleGame").Equals("No"))
        {
            if(_name != "굽다리 접시")
            {
                storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                 "야!\n 이 유물은 문에 그려진 그림과 달라. 다시 찾아봐야겠어.", 4f).SetEase(Ease.Linear).
                 OnUpdate(() => {
                     var currentText = storyText.text;
                     if (beforeText == currentText)
                         return;

                     var newChar = currentText[currentText.Length - 1].ToString();

                     if (!INVALID_CHARS.Contains(newChar))
                         SoundMaixerManager.instance.TextSoundPlay();
                     beforeText = currentText;
                 });
            }
            else
            {
                storyText.DOText("이건 " + _number + "호분의 딸린덧널에서 출토된 " + _name +
                   "야!\n 이 유물은 문에 그려진 그림과 달라. 다시 찾아봐야겠어.", 4f).SetEase(Ease.Linear).
                   OnUpdate(() => {
                       var currentText = storyText.text;
                       if (beforeText == currentText)
                           return;

                       var newChar = currentText[currentText.Length - 1].ToString();

                       if (!INVALID_CHARS.Contains(newChar))
                           SoundMaixerManager.instance.TextSoundPlay();
                       beforeText = currentText;
                   });
            }
            yield return new WaitForSeconds(5f);
        }
        else
        {
            //고리자루칼
            if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
                PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start"))
            {
                if (_name != "굽다리 접시")
                {
                    storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                    "야!\n 이건 칼이 아니야. 조금 더 찾아봐야겠어.", 4f).SetEase(Ease.Linear).
                    OnUpdate(() => {
                        var currentText = storyText.text;
                        if (beforeText == currentText)
                            return;

                        var newChar = currentText[currentText.Length - 1].ToString();

                        if (!INVALID_CHARS.Contains(newChar))
                            SoundMaixerManager.instance.TextSoundPlay();
                        beforeText = currentText;
                    });
                }
                else
                {
                    storyText.DOText("이건 " + _number + "호분의 딸린덧널에서 출토된 " + _name +
                   "야!\n 이 유물은 칼이 아닌 토기잖아! 조금 더 찾아봐야겠어.", 4f).SetEase(Ease.Linear).
                   OnUpdate(() => {
                       var currentText = storyText.text;
                       if (beforeText == currentText)
                           return;

                       var newChar = currentText[currentText.Length - 1].ToString();

                       if (!INVALID_CHARS.Contains(newChar))
                           SoundMaixerManager.instance.TextSoundPlay();
                       beforeText = currentText;
                   });
                }
                yield return new WaitForSeconds(5f);
            }
            //등잔모양토기
            else if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") && 
                PlayerPrefs.GetString("HPG_DustGame").Equals("Start"))
            {
                if (_name == "굽다리 접시")
                {
                    storyText.DOText("이건 " + _number + "호분의 딸린덧널에서 출토된 " + _name +
                        "야!\n 이걸로는 안되겠어. 다시 찾아봐야겠어.", 4f).SetEase(Ease.Linear).
                        OnUpdate(() => {
                            var currentText = storyText.text;
                            if (beforeText == currentText)
                                return;

                            var newChar = currentText[currentText.Length - 1].ToString();

                            if (!INVALID_CHARS.Contains(newChar))
                                SoundMaixerManager.instance.TextSoundPlay();
                            beforeText = currentText;
                        });
                }
                else if(_name == "고리자루 큰 칼이")
                {
                    storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                        "야!\n 이걸로는 안되겠어. 다시 찾아봐야해!", 4f).SetEase(Ease.Linear).
                        OnUpdate(() => {
                            var currentText = storyText.text;
                            if (beforeText == currentText)
                                return;

                            var newChar = currentText[currentText.Length - 1].ToString();

                            if (!INVALID_CHARS.Contains(newChar))
                                SoundMaixerManager.instance.TextSoundPlay();
                            beforeText = currentText;
                        });
                }
                else if (_name == "신발 모양 토기")
                {
                    storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                        "야!\n 이걸로는 어렵겠는데? 다시 찾아봐야겠어.", 4f).SetEase(Ease.Linear).
                        OnUpdate(() => {
                            var currentText = storyText.text;
                            if (beforeText == currentText)
                                return;

                            var newChar = currentText[currentText.Length - 1].ToString();

                            if (!INVALID_CHARS.Contains(newChar))
                                SoundMaixerManager.instance.TextSoundPlay();
                            beforeText = currentText;
                        });
                }
                else if(_name == "화로 모양 그릇 받침이" || _name == "큰 항아리")
                {
                    storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                        "야!\n 이걸로는 어렵겠어. 다시 찾아보자.", 4f).SetEase(Ease.Linear).
                        OnUpdate(() => {
                            var currentText = storyText.text;
                            if (beforeText == currentText)
                                return;

                            var newChar = currentText[currentText.Length - 1].ToString();

                            if (!INVALID_CHARS.Contains(newChar))
                                SoundMaixerManager.instance.TextSoundPlay();
                            beforeText = currentText;
                        });
                }
                else
                {
                    storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                        "야!\n 이걸로는 불을 밝힐 순 없겠군. 다시 찾아봐야겠어.", 4f).SetEase(Ease.Linear).
                        OnUpdate(() => {
                            var currentText = storyText.text;
                            if (beforeText == currentText)
                                return;

                            var newChar = currentText[currentText.Length - 1].ToString();

                            if (!INVALID_CHARS.Contains(newChar))
                                SoundMaixerManager.instance.TextSoundPlay();
                            beforeText = currentText;
                        });
                }
                yield return new WaitForSeconds(5f);
            }
            //항아리
            else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") && 
                PlayerPrefs.GetString("HPG_SendGame").Equals("Start"))
            {
                if (_name == "굽다리 접시")
                {
                    storyText.DOText("이건 " + _number + "호분의 딸린덧널에서 출토된 " + _name +
                        "야!\n 이것도 좋지만 아무래도 담기에는 작아. 항아리를 찾아봐야겠어.", 5f).SetEase(Ease.Linear).
                        OnUpdate(() => {
                            var currentText = storyText.text;
                            if (beforeText == currentText)
                                return;

                            var newChar = currentText[currentText.Length - 1].ToString();

                            if (!INVALID_CHARS.Contains(newChar))
                                SoundMaixerManager.instance.TextSoundPlay();
                            beforeText = currentText;
                        });
                    yield return new WaitForSeconds(6f);
                }
                else if(_name.Equals("신발 모양 토기"))
                {
                    storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                        "야!\n 모래를 담기는 힘들어보여. 다시 찾아봐야겠어.", 4f).SetEase(Ease.Linear).
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
                }
                else if(_name.Equals("목걸이") || _name.Equals("투구"))
                {
                    storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                        "야!\n 에잇. 서둘러서 항아리를 찾아봐야겠어.", 4f).SetEase(Ease.Linear).
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
                }
                else if(_name.Equals("등잔 모양 토기"))
                {
                    storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                        "야!\n 여기에 모래를 담기는 힘들겠는데? 서둘러서 다시 찾아봐야겠어.", 4.5f).SetEase(Ease.Linear).
                        OnUpdate(() => {
                            var currentText = storyText.text;
                            if (beforeText == currentText)
                                return;

                            var newChar = currentText[currentText.Length - 1].ToString();

                            if (!INVALID_CHARS.Contains(newChar))
                                SoundMaixerManager.instance.TextSoundPlay();
                            beforeText = currentText;
                        });
                    yield return new WaitForSeconds(5.5f);
                }
                else if(_name.Equals("화로 모양 그릇 받침이"))
                {
                    storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                        "야!\n 여기에도 모래를 담을 수는 있지만, 난 항아리를 원해!\n" +
                        "다시 찾아봐야겠어.", 5f).SetEase(Ease.Linear).
                        OnUpdate(() => {
                            var currentText = storyText.text;
                            if (beforeText == currentText)
                                return;

                            var newChar = currentText[currentText.Length - 1].ToString();

                            if (!INVALID_CHARS.Contains(newChar))
                                SoundMaixerManager.instance.TextSoundPlay();
                            beforeText = currentText;
                        });
                    yield return new WaitForSeconds(6f);
                }
                else //환두대도, 재갈, 
                {
                    storyText.DOText("이건 " + _number + "호분에서 출토된 " + _name +
                        "야!\n 이건 모래를 담을 수 없어. 다시 찾아봐야겠어.", 4f).SetEase(Ease.Linear).
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
                }
            }
        }
        

        
        textBack.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        //해당 스토리가 전부 나왔기 때문에 다시 false로 변경
        noRelicOneShow = false;
    }

    //유물획득 시 획득 팝업
    void RelicAcquirePopup()
    {
        SoundMaixerManager.instance.MissionSceecssSoundPlay();
        missionSuccessPopup.SetActive(true);

        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") && 
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No"))
            relicNameText.text = "'신발 모양 토기'를 획득하였습니다.";
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("No"))
            relicNameText.text = "'굽다리 접시'를 획득하였습니다.";
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("No"))
            relicNameText.text = "'재갈'을 획득하였습니다.";
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start"))
            relicNameText.text = "'고리자루 큰 칼'을 획득하였습니다.";
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("Start"))
            relicNameText.text = "'등잔 모양 토기'를 획득하였습니다.";
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("Start"))
            relicNameText.text = "'큰 항아리'를 획득하였습니다.";
    }

    //1차 유물 획득 팝업에서 확인버튼 클릭 이벤트
    public void GameTakeSaveClickOn()
    {
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("No"))
        {
            PlayerPrefs.SetString("HPG_OneGame", "Success");
            StartCoroutine(SceneMovingParticel("GameChoiceScene"));
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("No"))
        {
            PlayerPrefs.SetString("HPG_TwoGame", "Success");
            StartCoroutine(SceneMovingParticel("GameChoiceScene"));
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("No"))
        {
            PlayerPrefs.SetString("HPG_ThreeGame", "Success");
            StartCoroutine(SceneMovingParticel("GameChoiceScene"));
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start"))
        {
            PlayerPrefs.SetString("HPG_TombstoneMission", "Success");
            StartCoroutine(SceneMovingParticel("TombstoneGame"));
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("Start"))
        {
            PlayerPrefs.SetString("HPG_DustMisson", "Success");
            StartCoroutine(SceneMovingParticel("DustGame"));
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("Start"))
        {
            PlayerPrefs.SetString("HPG_SendMission", "Success");
            StartCoroutine(SceneMovingParticel("SendGame"));
        }
        
    }
    //3차 유물 획득 팝업에서 확인 버튼 클릭 이벤트
    public void GameTakeSave3_ClickOn()
    {
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_TombstoneGame").Equals("Start"))
            PlayerPrefs.SetString("HPG_TombstoneMission", "Success");
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_DustGame").Equals("Start"))
            PlayerPrefs.SetString("HPG_DustMisson", "Success");
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_SendGame").Equals("Start"))
            PlayerPrefs.SetString("HPG_SendMission", "Success");
    }

    IEnumerator SceneMovingParticel(string _sceneName)
    {
        if (fade_sripts.fadeStartState == false)
        {
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());
        }
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(_sceneName);
    }
}
