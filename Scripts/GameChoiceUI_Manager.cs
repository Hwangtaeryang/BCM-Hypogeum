using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;
using System.Linq;

public class GameChoiceUI_Manager : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    public GameObject missionPopup; //미션 선택 팝업
    public GameObject pausePopup;   //일시정지 팝업
    //public GameObject choicePopup;    //문 선택 팝업
    public GameObject doorChoice;   //문선택 UI
    //public GameObject titleImg; //씬 타이틀
    public GameObject missionBar;   //미션바
    //public Image missionImage;  //미션이미지
    //public Sprite[] missionSprite;
    public GameObject textBack; //텍스트 나오는 백배
    public Text storyText;  //나레이션 텍스트
    public CinemachineVirtualCamera virtualCam; //가상카메라 / 움직힐 카메라
    public CinemachineDollyCart[] followObj;    //따라갈 오브젝트

    public GameObject[] gatewayDoor;    //관문
    public GameObject[] gatewayDust;    //문 먼지
    public Transform[] targetPos; //문열릴 위치

    //public GameObject movingCamera; //움직힐 카메라
    public Transform[] movingTarget;    //카메라 움직일 위치
    public GameObject fadeEffect;   //페이드 효과 스크립트
    FadeEffect fade_sripts;


    int missionNum; //미션 번호
    float posDis;   //포지션 거리
    bool textShowState; //텍스트박스가 끝났는지 상태확인
    int dustOpenState = 0; //먼지 활성화 상태 0.무 1.오픈, 2.오픈 후 

    bool doorNearby;    //열리는 문 가까이 가는 상태
    bool soundPlayState;    //사운드 재생 상태

    float cameraSpeed = 5f;

    string beforeText;
    private static readonly string[] INVALID_CHARS = {
  " ", "　", "!", "?", "！", "？", "\"", "\'", "\\",
  ".", ",", "、", "。", "…", "・"
};

    void Start()
    {
        Time.timeScale = 1;

        SoundMaixerManager.instance.FirstBGMPlay();
        SoundMaixerManager.instance.FireSoundPlay();
        //PlayerPrefs.SetString("HPG_FirstGateway", "No");
        //Debug.Log(PlayerPrefs.GetString("HPG_FirstGateway"));
        //Debug.Log("뭔가 이상하다 ..HPG_SendGame : " + PlayerPrefs.GetString("HPG_SendGame"));
        //Debug.Log("뭔가 이상하다 ..HPG_SendMission : " + PlayerPrefs.GetString("HPG_SendMission"));

        fade_sripts = fadeEffect.GetComponent<FadeEffect>();
        beforeText = storyText.text;

        //첫번째 관문을 한번씩 선택하지 않았을 경우(게임 처음시작했을때)
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("No") || PlayerPrefs.GetString("HPG_FirstGateway").Equals(""))
            StartCoroutine(TextShowBox());
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") && 
            PlayerPrefs.GetString("HPG_OneGame").Equals("Success"))
            StartCoroutine(OneGatewayCameraMoving());
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_TwoGame").Equals("Success"))
            StartCoroutine(TwoGatewayCameraMoving());
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_ThreeGame").Equals("Success"))
            StartCoroutine(ThreeGatewayCameraMoving());
    }


    //미션에 관한 스토리 진행
    IEnumerator TextShowBox()
    {
        yield return new WaitForSeconds(1f);

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("세 가지 갈림길이 나타났어. \n 원하는 길로 가려면 그림 속 유물을 찾아야 하나 봐.\n" +
            "먼저 길을 선택해보자.", 6f).SetEase(Ease.Linear).
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
        textShowState = true;   //텍스트박스 확인햇음

        textBack.SetActive(false);
        //choicePopup.SetActive(true);    //문선택 팝업
        doorChoice.SetActive(true); //문선택 UI
        StartCoroutine(WalkMovingSound(0f, 3f));
        ChoiceGameCameraStartMoving();
    }

    //처음 선택 나레이션 끝나고 카메라 전진 무빙
    void ChoiceGameCameraStartMoving()
    {
        missionBar.SetActive(true);
        virtualCam.Follow = followObj[0].transform;
        followObj[0].m_Speed = 3f;
    }

    IEnumerator FireSoundPlay(float _standbySecond)
    {
        yield return new WaitForSecondsRealtime(_standbySecond);
        SoundMaixerManager.instance.FireSoundStop();
    }

    IEnumerator WalkMovingSound(float _standbySecond, float _second)
    {
        yield return new WaitForSecondsRealtime(_standbySecond);
        SoundMaixerManager.instance.WalkSoundPlay();
        yield return new WaitForSecondsRealtime(_second);
        SoundMaixerManager.instance.WalkSoundStop();
    }

    IEnumerator WalkPlaySound()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        soundPlayState = true;
        SoundMaixerManager.instance.WalkSoundPlay();
    }

    
    void Update()
    {
        RayCastHit();

        
        //Debug.Log(followObj[0].m_Position); //1-16.8315 / 2-10.6 / 3-16.35583
        //Debug.Log(" !!!++++++++  " + PlayerPrefs.GetString("HPG_SandGame"));
        //첫번째 관문 통과 후 카메라 무빙이 끝났는지 확인 후 문열기
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            PlayerPrefs.GetString("HPG_OneGame").Equals("Success"))
        {
            //문에 가깝지 않다. 걷는 사운드 재생 조건식
            if (doorNearby.Equals(true) && soundPlayState.Equals(false))
            {
                StartCoroutine(WalkPlaySound());
            }
            else if (doorNearby.Equals(false) && soundPlayState.Equals(true))
            {
                SoundMaixerManager.instance.WalkSoundStop();
                soundPlayState = false;
            }

            //choicePopup.SetActive(false);    //문선택 팝업
            doorChoice.SetActive(false); //문선택 UI

            posDis = followObj[1].m_Position;
            //Debug.Log(" ===  " + posDis);
            if (posDis >= 16.5246f)
            {
                doorNearby = false; //문에 가까워짐.
                //먼저 오픈되지 않음. 현재 0(무)
                if (dustOpenState != 2)
                {
                    SoundMaixerManager.instance.OpenDoorSound();
                    dustOpenState = 1; //오픈 상태로 변경(1로)
                }

                //철문 오픈
                gatewayDoor[0].transform.position = Vector3.MoveTowards(gatewayDoor[0].transform.position,
            targetPos[0].position, 0.078f);
            }
            else
            {
                doorNearby = true;  //문에 가까워지지 않음 
            }

            //먼지 발생 상태일 때
            if(dustOpenState.Equals(1))
            {
                StartCoroutine(SuccessText());  //결과 텍스트
                gatewayDust[0].SetActive(true);
                dustOpenState = 2;  //오픈 후 쑈한 상태
                StartCoroutine(FireSoundPlay(1.5f));
                StartCoroutine(WalkMovingSound(3.5f, 4f));
            }

            if (dustOpenState.Equals(2))
                StartCoroutine(OneGatewayCameraStraight()); 
        }
        else if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            PlayerPrefs.GetString("HPG_TwoGame").Equals("Success"))
        {
            //문에 가깝지 않다.
            if (doorNearby.Equals(true) && soundPlayState.Equals(false))
            {
                StartCoroutine(WalkPlaySound());
            }
            else if (doorNearby.Equals(false))
            {
                SoundMaixerManager.instance.WalkSoundStop();
            }

            doorChoice.SetActive(false); //문선택 UI

            posDis = followObj[2].m_Position;
            if(posDis >= 10.5f)
            {
                doorNearby = false;

                //먼저 오픈되지 않음. 현재 0(무)
                if (dustOpenState != 2)
                {
                    SoundMaixerManager.instance.OpenDoorSound();
                    dustOpenState = 1; //오픈 상태로 변경(1로)
                }
                gatewayDoor[1].transform.position = Vector3.MoveTowards(gatewayDoor[1].transform.position,
                    targetPos[1].position, 0.078f);
            }
            else
            {
                doorNearby = true;
            }

            //먼지 발생 상태일 때
            if(dustOpenState.Equals(1))
            {
                StartCoroutine(SuccessText());  //결과 텍스트
                gatewayDust[1].SetActive(true);
                dustOpenState = 2;
                StartCoroutine(FireSoundPlay(1.5f));
                StartCoroutine(WalkMovingSound(1f, 4f));
            }

            if (dustOpenState.Equals(2))
                StartCoroutine(TwoGatewayCameraStraight());
        }
        else if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            PlayerPrefs.GetString("HPG_ThreeGame").Equals("Success"))
        {
            //문에 가깝지 않다.
            if (doorNearby.Equals(true) && soundPlayState.Equals(false))
            {
                StartCoroutine(WalkPlaySound());
            }
            else if (doorNearby.Equals(false))
            {
                SoundMaixerManager.instance.WalkSoundStop();
            }

            doorChoice.SetActive(false); //문선택 UI

            posDis = followObj[3].m_Position;
            if(posDis >= 16.35583f)
            {
                doorNearby = false;
                //먼저 오픈되지 않음. 현재 0(무)
                if (dustOpenState != 2)
                {
                    SoundMaixerManager.instance.OpenDoorSound();
                    dustOpenState = 1; //오픈 상태로 변경(1로)
                }
                gatewayDoor[2].transform.position = Vector3.MoveTowards(gatewayDoor[2].transform.position,
                    targetPos[2].position, 0.078f);
            }
            else
            {
                doorNearby = true;
            }

            //먼지 발생 상태일 때
            if(dustOpenState.Equals(1))
            {
                StartCoroutine(SuccessText());  //결과 텍스트
                gatewayDust[2].SetActive(true);
                dustOpenState = 2;
                StartCoroutine(FireSoundPlay(1.5f));
                StartCoroutine(WalkMovingSound(3.5f, 4f));
            }

            if (dustOpenState.Equals(2))
                StartCoroutine(ThreeGatewayCameraStraight());
        }
    }

    IEnumerator SuccessText()
    {
        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("유물을 찾으니 문이 열렸어..! 얼른 여기를 빠져나가야겠어.", 3.0f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(4f);
        textBack.SetActive(false);
    }

    void RayCastHit()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //미션 선택 팝업이 나오지 않았을 경우에만 레이케스트를 이용해서 문 선택
        if(missionPopup.activeSelf.Equals(false) && pausePopup.activeSelf.Equals(false))
        {
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(ray, out hit, 1000f) && textShowState.Equals(true))
                {
                    Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);

                    Debug.Log(hit.transform.gameObject.name);
                    if (hit.transform.gameObject.name.Equals("Mission_1"))
                    {
                        SoundMaixerManager.instance.ClickSoundPlay();
                        //UpButtonsCtrl.instance.MissionPopup(0);
                        //UpButtonsCtrl.instance.missionNum = 1;
                        missionNum = 1;
                        if (fade_sripts.fadeStartState == false)
                        {
                            fadeEffect.SetActive(true);
                            StartCoroutine(FadeStart());
                        }
                    }
                    else if (hit.transform.gameObject.name.Equals("Mission_2"))
                    {
                        SoundMaixerManager.instance.ClickSoundPlay();
                        //UpButtonsCtrl.instance.MissionPopup(1);
                        //UpButtonsCtrl.instance.missionNum = 2;
                        missionNum = 2;
                        if (fade_sripts.fadeStartState == false)
                        {
                            fadeEffect.SetActive(true);
                            StartCoroutine(FadeStart());
                        }
                    }
                    else if (hit.transform.gameObject.name.Equals("Mission_3"))
                    {
                        SoundMaixerManager.instance.ClickSoundPlay();
                        //UpButtonsCtrl.instance.MissionPopup(2);
                        //UpButtonsCtrl.instance.missionNum = 3;
                        missionNum = 3;
                        if (fade_sripts.fadeStartState == false)
                        {
                            fadeEffect.SetActive(true);
                            StartCoroutine(FadeStart());
                        }
                    }

                    StartCoroutine(ARSceneMoving());    //팝업 띄울거면 이거 주석
                }
            }
        }
    }

    IEnumerator ARSceneMoving()
    {
        yield return new WaitForSeconds(3f);

        if (missionNum.Equals(1))
        {
            PlayerPrefs.SetString("HPG_FirstGateway", "One");
            SceneManager.LoadScene("Game_1");
        }
        else if (missionNum.Equals(2))
        {
            PlayerPrefs.SetString("HPG_FirstGateway", "Two");
            SceneManager.LoadScene("Game_1");
        }
        else if (missionNum.Equals(3))
        {
            PlayerPrefs.SetString("HPG_FirstGateway", "Three");
            SceneManager.LoadScene("Game_1");
        }
    }


    //1. 첫번째 관문 통과 시 카메라 무빙
    IEnumerator OneGatewayCameraMoving()
    {
        virtualCam.Follow = followObj[1].transform;

        yield return new WaitForSeconds(1.5f);
        followObj[1].m_Speed = 3.5f;
    }

    IEnumerator FadeStart()
    {
        yield return new WaitForSeconds(0.5f);
        fade_sripts.Fade(); //점점 어둡게 실행
    }

    //1. 철문이 열리면 안으로 카메라 무빙
    IEnumerator OneGatewayCameraStraight()
    {
        yield return new WaitForSeconds(3f);
        //Debug.Log("들어왔는데");
        virtualCam.Follow = null;   //가상카메라 팔로우 비우기

        virtualCam.transform.position = Vector3.MoveTowards(virtualCam.transform.position,
            movingTarget[0].position, cameraSpeed * Time.deltaTime);   //타켓이 있는데 까지 진진 무빙

        //yield return new WaitForSeconds(1f);

        if(fade_sripts.fadeStartState == false)
        {
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());
        }

        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("PuzzleGame");
    }

    //2. 두번째 관문 통과 시 카메라 무빙
    IEnumerator TwoGatewayCameraMoving()
    {
        virtualCam.Follow = followObj[2].transform;

        yield return new WaitForSeconds(1.5f);
        followObj[2].m_Speed = 3.5f;
    }
    //2. 철문이 열리면 안으로 카메라 무빙
    IEnumerator TwoGatewayCameraStraight()
    {
        yield return new WaitForSeconds(3f);

        virtualCam.Follow = null;
        virtualCam.transform.position = Vector3.MoveTowards(virtualCam.transform.position,
            movingTarget[1].position, cameraSpeed * Time.deltaTime);

        //yield return new WaitForSeconds(1f);

        if (fade_sripts.fadeStartState == false)
        {
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());
        }

        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("PuzzleGame");
    }

    //3. 세번째 관문 통과 시 카메라 무빙
    IEnumerator ThreeGatewayCameraMoving()
    {
        virtualCam.Follow = followObj[3].transform;

        yield return new WaitForSeconds(1.5f);
        followObj[3].m_Speed = 3.5f;
    }
    //3. 철문이 열리면 안으로 카메라 무빙
    IEnumerator ThreeGatewayCameraStraight()
    {
        yield return new WaitForSeconds(3f);

        virtualCam.Follow = null;
        virtualCam.transform.position = Vector3.MoveTowards(virtualCam.transform.position,
            movingTarget[2].position, cameraSpeed * Time.deltaTime);

        //yield return new WaitForSeconds(1f);

        if (fade_sripts.fadeStartState == false)
        {
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());
        }
        //Debug.Log("뭔가 이상하다 ..HPG_SendGame : " + PlayerPrefs.GetString("HPG_SendGame"));
        //Debug.Log("뭔가 이상하다 ..HPG_SendMission : " + PlayerPrefs.GetString("HPG_SendMission"));

        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("PuzzleGame");
    }


}
