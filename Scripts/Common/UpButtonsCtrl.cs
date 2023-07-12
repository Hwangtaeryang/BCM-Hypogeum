using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class UpButtonsCtrl : MonoBehaviour
{
    public static UpButtonsCtrl instance { get; private set; }

    public GameObject endPopup; //종료팝업
    public GameObject missionPopup; //미션 선택 팝업
    public Image missionImage;  //미션이미지
    public Sprite[] missionSprite;
    public Image ahandLookImage;    //미션보기이미지

    public Image pauseImg;  //일시정지버튼
    public Sprite[] pauseSprite;    //재생이미지, 일시정지

    //수첩 열었을 때 사용되는 것들
    public Button noticButton;  //노트버튼
    public Image noticImage;    //수첩 이미지
    //0.신발모기, 1. 굽다리접시, 2.재갈, 3.투구, 4.목걸이, 5.화로, 6.칼, 7.등잔, 8.항아리
    public Sprite[] relicsSprite;   //유물이미지 
    public Text noticTitleText; //수첩 유물 제목
    public Text noticPageText;  //수첩 페이지 
    public Text contentText;    //유물 상세 설명
    public Button leftPageBtn;  //왼쪽페잊 버튼
    public Button rightPageBtn; //오른쪽페이지 버튼

    public int missionNum;  //게임 선택 번호

    public bool pauseState; //정지상태
    public bool tombstoneMission;   //비석뽑기미션
    public bool sendMission;    //모래담기미션

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else instance = this;
    }

    void Start()
    {
        //PlayerPrefs.SetString("HPG_FirstGateway", "One");
        //PlayerPrefs.SetString("HPG_OneGame", "Success");
        //PlayerPrefs.SetString("HPG_PuzzleGame", "Yes");
        //PlayerPrefs.SetString("HPG_TombstoneMission", "End");
        //Debug.Log("_-_ :  " + PlayerPrefs.GetString("HPG_OneGame"));
        //Debug.Log("_-_ :  " + PlayerPrefs.GetString("HPG_TwoGame"));
        //Debug.Log("_-_ :  " + PlayerPrefs.GetString("HPG_ThreeGame"));

        if (SceneManager.GetActiveScene().name.Equals("EndingScene") || 
            SceneManager.GetActiveScene().name.Equals("Game_1") || 
            SceneManager.GetActiveScene().name.Equals("GameChoiceScene"))
        {
            //Debug.Log("들어옴");
            if ((PlayerPrefs.GetString("HPG_OneGame").Equals("Fail") && 
                PlayerPrefs.GetString("HPG_TwoGame").Equals("Fail") &&
                PlayerPrefs.GetString("HPG_ThreeGame").Equals("Fail")))
                noticButton.interactable = false;
        }
    }



    /// <summary>
    /// 상단 바에 있는 버튼 눌렀을 이벤트
    /// </summary>

    //일시정지 버튼 클릭
    public void PauseButton()
    {
        pauseState = true;
        Time.timeScale = 0; //멈춤
        pauseImg.sprite = pauseSprite[1];
        SoundMaixerManager.instance.SourcePause();

        if(SceneManager.GetActiveScene().name.Equals("EndingScene"))
        {
            VideoHandler.instance.PauseVideo();
        }
    }

    //게임으로 돌아가기
    public void InGameButton()
    {
        pauseState = false;
        Time.timeScale = 1; //재생
        pauseImg.sprite = pauseSprite[0];
        SoundMaixerManager.instance.SourcePlay();

        if (SceneManager.GetActiveScene().name.Equals("EndingScene"))
        {
            VideoHandler.instance.PausePlayVideo();
        }
    }

    //수첩나가기 백버튼
    public void NoticBackButton()
    {
        pauseState = false;
        Time.timeScale = 1; //재생
        SoundMaixerManager.instance.SourcePlay();

        if (SceneManager.GetActiveScene().name.Equals("EndingScene"))
        {
            VideoHandler.instance.PausePlayVideo();
        }
    }

    //노트 클릭 
    public void NoticButton()
    {
        pauseState = true;
        Time.timeScale = 0; //멈춤

        if (SceneManager.GetActiveScene().name.Equals("EndingScene"))
        {
            VideoHandler.instance.PauseVideo();
        }
    }

    //홈버튼 눌렀을 때 인트로 씬으로 이동
    public void MainIntroSceneMove()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void GameEndButton()
    {
        Application.Quit();
    }

    public void EndPopupOpen()
    {
        if (SceneManager.GetActiveScene().name.Equals("EndingScene"))
        {
            VideoHandler.instance.PauseVideo();
        }

        endPopup.SetActive(true);
        Time.timeScale = 0; //멈춤
    }

    public void EndPopupClose()
    {
        Time.timeScale = 1; //재생

        if (SceneManager.GetActiveScene().name.Equals("EndingScene"))
        {
            VideoHandler.instance.PausePlayVideo();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            //endPopup.SetActive(true);
            EndPopupOpen();
        }
    }


    /// <summary>
    /// 수첩 버튼 눌렀을 때 이벤트
    /// </summary>

    //수첩보기 버튼 클릭 시 이벤트
    public void NoticLookButtonClick()
    {
        Debug.Log("이상하네");
        if (SceneManager.GetActiveScene().name.Equals("EndingScene"))
        {
            VideoHandler.instance.PauseVideo();
        }

        SoundMaixerManager.instance.SourcePause();
        StartCoroutine(_NoticLookButtonClick());
    }

    //수첩보기 버튼 클릭 시 이벤트
    IEnumerator _NoticLookButtonClick()
    {
        yield return null;// new WaitForSeconds(0.05f);
        Debug.Log("안들어왔어 ?");
        pauseState = true;  //일시정지
        Time.timeScale = 0; //멈춤

        Debug.Log("pauseState " + pauseState);


        //첫번째 페이지가 1이고 퍼즐게임이 통과되지 않앗으면 오른쪽 버튼 비활성화
        if (PlayerPrefs.GetString("HPG_PuzzleGame").Equals("No"))
        {
            leftPageBtn.interactable = false;    //비활성화
            rightPageBtn.interactable = false;
        }
        else
        {
            leftPageBtn.interactable = false;    //비활성화
            rightPageBtn.interactable = true;
        }


        //첫 게임 AR 3개의 게임 중에 첫번째 신발모양토기찾기 성공 시
        if (PlayerPrefs.GetString("HPG_OneGame").Equals("Success"))
        {
            Debug.Log("왜");
            RelicInfoSetting(0, "신발 모양 토기", "1");
            RelicContent(0);
        }
        else if (PlayerPrefs.GetString("HPG_TwoGame").Equals("Success"))
        {
            RelicInfoSetting(1, "굽다리 접시", "1");
            RelicContent(1);
        }
        else if (PlayerPrefs.GetString("HPG_ThreeGame").Equals("Success"))
        {
            RelicInfoSetting(2, "재갈", "1");
            RelicContent(2);
        }
    }

    //유물 기본 정보
    void RelicInfoSetting(int _index, string _title, string _page)
    {
        noticImage.sprite = relicsSprite[_index];
        noticTitleText.text = _title;
        noticPageText.text = _page;
    }

    void RelicContent(int _index)
    {
        if (_index.Equals(0))
        {
            contentText.text = " 53호분의 신발모양토기는 굽다리접시의 다리\n" +
                "에 요즘 신는 샌들(Sandal) 모양과 유사한 신발\n" +
                "형 토기가 부착된 형태입니다.\n" +
                " 굽다리와 신발은 각기 제작되어 접합되었습니\n" +
                "다. 신발에는 잔이 부착되어 있으나 한 점은 토\n" +
                "기를 다 빚은 후 제거한 상태로 불에 구워 소성\n" +
                "하였습니다.\n" +
                " 신발은 전체적으로 유선형입니다. 신발의 상부 \n" +
                "끈에는 격자를 부분적으로 그려 넣어 사실감을 \n" +
                "높였습니다. 짚으로 엮은 짚신인지, 둥근 가죽\n" +
                "끈으로 만든 가죽신인지는 확실히 알 수 없습니\n" +
                "다.";
        }
        else if (_index.Equals(1))
        {
            contentText.text = " 54호분의 원통모양 굽다리 접시는 대체로 그\n" +
                "릇부분은 얕고, 다리는 원통형으로 긴 편 입니\n" +
                "다. 다리의 하단은 나팔모양으로 외반합니다.\n" +
                "매우 정선된 흙을 태토로 사용하였고 높은 온\n" +
                "도에서 소성되어 표면에 자연유로 인한 유리막\n" +
                "이 고르게 발려졌습니다. 다리에는 장방형 혹은 \n" +
                "삼각형으로 3단에 걸쳐 투창이 뚫려 있습니다.\n" +
                "이러한 형태의 원통모양 굽다리 접시는 당시 \n" +
                "함안에서 유행한 형식입니다.";
        }
        else if (_index.Equals(2))
        {
            contentText.text = " 54호의 재갈입니다. 재갈멈치가 사다리꼴인 \n" +
                "판비입니다. 오른쪽의 재갈멈치에는 굴레연결\n" +
                "부와 재갈멈치복판구멍이 있습니다. \n" +
                "재갈멈치복판구멍에는 재갈쇠멈춤띠가 가로질\n" +
                "러있고 재갈과 고삐가 걸려있습니다. 재갈은 꼬\n" +
                "우지 않은 철봉 2마디로 구성되며, 고삐는 철봉\n" +
                "을 꼬아서 만들었습니다. ";
        }
        else if (_index.Equals(3))
        {
            contentText.text = " 54호분의 세로판 투구입니다. 투구 몸통과 볼\n" +
                "가리개로 구성되며 복발은 남아있지 않았습니\n" +
                "다만 납작한 반구형의 유기질제 복발이 있었을 \n" +
                "것으로 추정됩니다. \n" +
                " 잔존 상태가 나빠 몸통의 구체적인 형태는 알 \n" +
                "수 없습니다. 몸통 세로판은 가죽끈을 이용해 \n" +
                "연결하였으며, 최하단은 가죽띠를 덧대고 가죽\n" +
                "끈으로 박음질하여 날카로운 철판의 끝이 몸에 \n" +
                "닿지 않도록 했습니다.\n" +
                " 볼가리개는 오른쪽과 왼쪽 각각 1개의 판입니\n" +
                "다. 볼가리개의 철판 테두리는 가죽끈으로 시침\n" +
                "질하여 마감하였으며, 안쪽에는 가죽을 한 겹 \n" +
                "대었던 흔적이 있습니다.";
        }
        else if (_index.Equals(4))
        {
            contentText.text = " 53호분의 목걸이는 곱은 옥 2점과 유리제옥 \n" +
                "115점으로 구성되었습니다. \n" +
                " 곱은 옥은 한쪽 면의 구멍이 반대편의 구멍보\n" +
                "다 큽니다. 이것은 상대적으로 지름이 넓은 구\n" +
                "멍쪽에서 반대쪽을 향해 뚫었기 때문으로 \n" +
                "추정됩니다.\n" +
                " 유리제 옥은 대부분 암청색으로, 지름은 가장 \n" +
                "작은 것이 0.3cm, 가장 큰 것은 0.6cm정도 \n" +
                "됩니다.";
        }
        else if (_index.Equals(5))
        {
            contentText.text = " 54호분에서는 화로모양 그릇받침이 19점 \n" +
                "출토되었습니다. \n" +
                " 화로모양 그릇받침은 거의 동일한 형태이며, \n" +
                "손잡이의 점토 단면이 장방형인 것이 많습니다. \n" +
                "손잡이는 토기에 손잡이 점토 두께로 구멍을 \n" +
                "뚫은 후 그 자리에 손잡이 점토를 끼우고 \n" +
                "안쪽 면은 점토로 보강하여 마무리하였습니다.";
        }
        else if (_index.Equals(6))
        {
            contentText.text = " 53호분에서 고리자루 큰 칼 2점이 출토되었\n" +
                "습니다. 한 점은 칼의 몸체와 고리 부분이 하나\n" +
                "의 철판으로 구성되었으며, 다른 한 점은 칼의 \n" +
                "몸체와 고리부분을 별도로 제작하여 못으로 \n" +
                "고정시켰습니다. 두 점 모두 옻칠을 한 목제의 \n" +
                "칼집에 넣은 채로 매납하였던 것으로 추정됩\n" +
                "니다. 또한 손잡이 부분 일부에 부착된 목질로 \n" +
                "보아 목제 손잡이가 있었던 것으로 추정됩니다.";
        }
        else if (_index.Equals(7))
        {
            contentText.text = " 53호분의 등잔모양토기는 투창이 여러 개 뚫린 \n" +
                "굽다리 토기 모양 위에 4개의 잔이 부착된 형태\n" +
                "입니다. 높은 온도에서 소성하여 토기 겉면과 \n" +
                "안쪽면에 자연유가 형성되어 있습니다. \n" +
                " 잔을 받치고 있는 굽다리토기는 다리에는 5개\n" +
                "의 장방형투창이, 몸통에는 7개의 장방형투창\n" +
                "이 뚫어져 있습니다.";
        }
        else if (_index.Equals(8))
        {
            contentText.text = " 54호분에서 큰 항아리 3점이 출토되었습니다.\n" +
                "복천동고분군에서 큰 항아리는 주로 딸린덧널\n" +
                "의 바닥에 구덩이를 항아리 몸체 절반정도가 \n" +
                "묻힐 깊이로 판 후 그 안에 항아리를 놓는 특징\n" +
                "이 있는데, 54호의 큰항아리도 그러한 상태로 \n" +
                "조사되었습니다.";
        }
    }

    //페이지 왼쪽 버튼 클릭 시
    public void LeftPageButtonClick()
    {
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            (PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Yes") ||
            PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Success")))
        {
            if (PlayerPrefs.GetString("HPG_OneGame").Equals("Success") &&
                PlayerPrefs.GetString("HPG_TombstoneMission").Equals("Fail"))
            {
                //페이지가 2이면
                if (noticPageText.text.Equals("2"))
                {
                    leftPageBtn.interactable = false;    //비활성화
                    rightPageBtn.interactable = true;   //활성화
                    //첫번째 유물의 정보를 넣어주고, 페이지 1로 바꾼다.
                    RelicInfoSetting(0, "신발 모양 토기", "1");
                    RelicContent(0);
                }
            }
            else if (PlayerPrefs.GetString("HPG_OneGame").Equals("Success") &&
                (PlayerPrefs.GetString("HPG_TombstoneMission").Equals("Success") ||
                PlayerPrefs.GetString("HPG_TombstoneMission").Equals("End")))
            {
                if (noticPageText.text.Equals("2"))
                {
                    leftPageBtn.interactable = false;    //비활성화
                    rightPageBtn.interactable = true;   //활성화
                    RelicInfoSetting(0, "신발 모양 토기", "1");
                    RelicContent(0);
                }
                else if (noticPageText.text.Equals("3"))
                {
                    RelicInfoSetting(3, "투구", "2");
                    RelicContent(3);
                }
            }
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
            (PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Yes") ||
            PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Success")))
        {
            if (PlayerPrefs.GetString("HPG_TwoGame").Equals("Success") &&
                PlayerPrefs.GetString("HPG_DustMisson").Equals("Fail"))
            {
                if (noticPageText.text.Equals("2"))
                {
                    leftPageBtn.interactable = false;    //비활성화
                    rightPageBtn.interactable = true;   //활성화
                    RelicInfoSetting(1, "굽다리 접시", "1");
                    RelicContent(1);
                }
            }
            else if (PlayerPrefs.GetString("HPG_TwoGame").Equals("Success") &&
                (PlayerPrefs.GetString("HPG_DustMisson").Equals("Success") ||
                PlayerPrefs.GetString("HPG_DustMisson").Equals("End")))
            {
                if (noticPageText.text.Equals("2"))
                {
                    leftPageBtn.interactable = false;    //비활성화
                    rightPageBtn.interactable = true;   //활성화
                    RelicInfoSetting(1, "굽다리 접시", "1");
                    RelicContent(1);
                }
                else if (noticPageText.text.Equals("3"))
                {
                    RelicInfoSetting(4, "목걸이", "2");
                    RelicContent(4);
                }
            }
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            (PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Yes") ||
            PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Success")))
        {
            if (PlayerPrefs.GetString("HPG_ThreeGame").Equals("Success") &&
                PlayerPrefs.GetString("HPG_SendMission").Equals("Fail"))
            {
                if (noticPageText.text.Equals("2"))
                {
                    leftPageBtn.interactable = false;    //비활성화
                    rightPageBtn.interactable = true;   //활성화
                    RelicInfoSetting(2, "재갈", "1");
                    RelicContent(2);
                }
            }
            else if (PlayerPrefs.GetString("HPG_ThreeGame").Equals("Success") &&
                (PlayerPrefs.GetString("HPG_SendMission").Equals("Success") ||
                PlayerPrefs.GetString("HPG_SendMission").Equals("End")))
            {
                if (noticPageText.text.Equals("2"))
                {
                    leftPageBtn.interactable = false;    //비활성화
                    rightPageBtn.interactable = true;   //활성화
                    RelicInfoSetting(2, "재갈", "1");
                    RelicContent(2);
                }
                else if (noticPageText.text.Equals("3"))
                {
                    RelicInfoSetting(5, "화로모양 그릇받침", "2");
                    RelicContent(5);
                }
            }
        }
    }

    //페이지 오른쪽 버튼 클릭 시
    public void RightPageButtonClick()
    {
        Debug.Log("들어왓지 : " + PlayerPrefs.GetString("HPG_TombstoneMission"));
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One") &&
            (PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Yes") ||
            PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Success")))
        {
            Debug.Log("HPG_TombstoneMission " + PlayerPrefs.GetString("HPG_TombstoneMission"));
            if (PlayerPrefs.GetString("HPG_OneGame").Equals("Success") &&
                PlayerPrefs.GetString("HPG_TombstoneMission").Equals("Fail"))
            {
                Debug.Log("------------");
                //페이지가 2이면
                if (noticPageText.text.Equals("1"))
                {
                    Debug.Log("헐");
                    rightPageBtn.interactable = false; //비활성화
                    leftPageBtn.interactable = true;    //활성화
                    //첫번째 유물의 정보를 넣어주고, 페이지 1로 바꾼다.
                    RelicInfoSetting(3, "투구", "2");
                    RelicContent(3);
                }
            }
            else if (PlayerPrefs.GetString("HPG_OneGame").Equals("Success") &&
                (PlayerPrefs.GetString("HPG_TombstoneMission").Equals("Success") ||
                PlayerPrefs.GetString("HPG_TombstoneMission").Equals("End")))
            {
                if (noticPageText.text.Equals("1"))
                {
                    leftPageBtn.interactable = true;    //활성화
                    RelicInfoSetting(3, "투구", "2");
                    RelicContent(3);
                }
                else if (noticPageText.text.Equals("2"))
                {
                    rightPageBtn.interactable = false; //비활성화
                    RelicInfoSetting(6, "고리자루 큰 칼", "3");
                    RelicContent(6);
                }
            }
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two") &&
           (PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Yes") ||
           PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Success")))
        {
            if (PlayerPrefs.GetString("HPG_TwoGame").Equals("Success") &&
                PlayerPrefs.GetString("HPG_DustMisson").Equals("Fail"))
            {
                if (noticPageText.text.Equals("1"))
                {
                    rightPageBtn.interactable = false; //비활성화
                    leftPageBtn.interactable = true;    //활성화
                    RelicInfoSetting(4, "목걸이", "2");
                    RelicContent(4);
                }
            }
            else if (PlayerPrefs.GetString("HPG_TwoGame").Equals("Success") &&
                (PlayerPrefs.GetString("HPG_DustMisson").Equals("Success") ||
                PlayerPrefs.GetString("HPG_DustMisson").Equals("End")))
            {
                if (noticPageText.text.Equals("1"))
                {
                    leftPageBtn.interactable = true;    //활성화
                    RelicInfoSetting(4, "목걸이", "2");
                    RelicContent(4);
                }
                else if (noticPageText.text.Equals("2"))
                {
                    rightPageBtn.interactable = false; //비활성화
                    RelicInfoSetting(7, "등잔 모양 토기", "3");
                    RelicContent(7);
                }
            }
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three") &&
            (PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Yes") ||
            PlayerPrefs.GetString("HPG_PuzzleGame").Equals("Success")))
        {
            if (PlayerPrefs.GetString("HPG_ThreeGame").Equals("Success") &&
                PlayerPrefs.GetString("HPG_SendMission").Equals("Fail"))
            {
                if (noticPageText.text.Equals("1"))
                {
                    rightPageBtn.interactable = false; //비활성화
                    leftPageBtn.interactable = true;    //활성화
                    RelicInfoSetting(5, "화로모양 그릇받침", "2");
                    RelicContent(5);
                }
            }
            else if (PlayerPrefs.GetString("HPG_ThreeGame").Equals("Success") &&
                (PlayerPrefs.GetString("HPG_SendMission").Equals("Success") ||
                PlayerPrefs.GetString("HPG_SendMission").Equals("End")))
            {
                if (noticPageText.text.Equals("1"))
                {
                    leftPageBtn.interactable = true;    //활성화
                    RelicInfoSetting(5, "화로모양 그릇받침", "2");
                    RelicContent(5);
                }
                else if (noticPageText.text.Equals("2"))
                {
                    rightPageBtn.interactable = false; //비활성화
                    RelicInfoSetting(8, "큰 항아리", "3");
                    RelicContent(8);
                }
            }
        }
    }








    ///AR찾는 씬에서 유물 이미지 보기 세팅
    public void AhandofTimeRelic(int _index)
    {
        ahandLookImage.sprite = missionSprite[_index];
        RectTransform rect = ahandLookImage.GetComponent<RectTransform>();

        if (_index.Equals(0))
            rect.sizeDelta = new Vector2(327f, 195f);
        else if (_index.Equals(1))
            rect.sizeDelta = new Vector2(321f, 157f);
        else if (_index.Equals(2))
            rect.sizeDelta = new Vector2(272f, 281f);
        else if (_index.Equals(3))
            rect.sizeDelta = new Vector2(306f, 230f);
        else if (_index.Equals(4))
            rect.sizeDelta = new Vector2(261f, 269f);
        else if (_index.Equals(5))
            rect.sizeDelta = new Vector2(234f, 251f);
    }


    //레이캐스트로 선택한 화면에 맞는 미션 이미지가 뜨는 함수
    public void MissionPopup(int _index)
    {
        SoundMaixerManager.instance.MissionSoundPlay();
        missionPopup.SetActive(true);

        RectTransform rect = (RectTransform)missionImage.transform;
        if (_index.Equals(0))
            rect.sizeDelta = new Vector2(327f, 195f);
        else if (_index.Equals(1))
            rect.sizeDelta = new Vector2(321f, 157f);
        else if (_index.Equals(2))
            rect.sizeDelta = new Vector2(272f, 281f);
        else if (_index.Equals(3))
            rect.sizeDelta = new Vector2(306f, 230f);
        else if (_index.Equals(4))
            rect.sizeDelta = new Vector2(261f, 269f);
        else if (_index.Equals(5))
            rect.sizeDelta = new Vector2(234f, 251f);

        missionImage.sprite = missionSprite[_index];
    }

    // 1. 미션 팝업에서 게임 선택했을때 해당 씬으로 이동
    public void MissionStartButtonClickOn()
    {
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

    //1. 퍼즐 동굴 - 퍼즐게임씬으로
    public void PuzzleGamePlay()
    {
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One"))
            SceneManager.LoadScene("PuzzleGamePlay_2_1");   //투구
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two"))
            SceneManager.LoadScene("PuzzleGamePlay_2_2");   //목걸이
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three"))
            SceneManager.LoadScene("PuzzleGamePlay_2_3");   //화로모양그릇받침
    }

    //3-1. 비석 동굴
    public void TombStoneGamePlay()
    {
        PlayerPrefs.SetString("HPG_TombstoneGame", "Start");
        SceneManager.LoadScene("Game_1");
    }
    //3-1.비석동굴 환두대도 뽑기 미션
    public void TombStoneMissionPlay()
    {
        tombstoneMission = true;
        //Debug.Log(tombstoneMission);
    }
    //3-1. 비석동굴 환두대도 뽑기 미션 성공
    public void TombStoneMissionSuccess()
    {
        PlayerPrefs.SetString("HPG_TombstoneMission", "Success");
    }


    //3-2. 흙 털기 게임
    public void DustGamePlay()
    {
        PlayerPrefs.SetString("HPG_DustGame", "Start");
        SceneManager.LoadScene("Game_1");
    }

    //3-3. 모래 미션
    public void SandGamePlay()
    {
        PlayerPrefs.SetString("HPG_SendGame", "Start");
        SceneManager.LoadScene("Game_1");
    }

    //3-3. 모래미션 담기 
    public void SendMissionPlay()
    {
        sendMission = true;
        SoundMaixerManager.instance.SendPutSoundPlay();
    }

}
