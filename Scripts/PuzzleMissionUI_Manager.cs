using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class PuzzleMissionUI_Manager : MonoBehaviour
{
    public GameObject textBack; //텍스트 나오는 박스
    public Text storyText;  //나레이션 텍스트

    public GameObject stonObj;  //돌판
    public Transform mainCamera;
    public Transform[] cameraPos;   //카메라 전진 타겟
    public Rigidbody[] treePanel;
    public GameObject[] modelObj; //공중 회전 모델링
    public GameObject partical; //사라지는 파티클

    public Image puzzlePanelImg;    //퍼즐판넬 이미지
    public Sprite[] puzzleSprtie;   //퍼즐이미지

    public GameObject fadeEffect;   //페이드 효과 스크립트
    FadeEffect fade_sripts;

    bool cameraMovingState; //카메라 무빙
    bool cameraMovingState2;    //두번째 카메라 무빙

    float cameraSpeed = 6f;


    string beforeText;
    private static readonly string[] INVALID_CHARS = {
  " ", "　", "!", "?", "！", "？", "\"", "\'", "\\",
  ".", ",", "、", "。", "…", "・"
};

    void Start()
    {
        SoundMaixerManager.instance.PuzzleBGMPlay();
        SoundMaixerManager.instance.FireSoundPlay();
        beforeText = storyText.text;
        fade_sripts = fadeEffect.GetComponent<FadeEffect>();

        //PlayerPrefs.SetString("HPG_SandGame", "No");
        //Debug.Log(PlayerPrefs.GetString("HPG_FirstGateway"));
        //Debug.Log(" !!!++++++++  " + PlayerPrefs.GetString("HPG_SandGame"));

        //게임 관문에 따른 해당 퍼증 이미지 생성
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One"))
            puzzlePanelImg.sprite = puzzleSprtie[0];    //투구
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two"))
            puzzlePanelImg.sprite = puzzleSprtie[1];    //목걸이
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three"))
            puzzlePanelImg.sprite = puzzleSprtie[2];    //화로모양그릇받침

        //PlayerPrefs.SetString("HPG_PuzzleGame", "No");
        if (PlayerPrefs.GetString("HPG_PuzzleGame").Equals("No") ||
            PlayerPrefs.GetString("HPG_PuzzleGame").Equals(""))
            StartCoroutine(TextShowBox_Before());
        else
            StartCoroutine(TextShowBox_After());
    }


    
    void Update()
    {
        if (PlayerPrefs.GetString("HPG_PuzzleGame").Equals("No") ||
            PlayerPrefs.GetString("HPG_PuzzleGame").Equals(""))

        {
            if (cameraMovingState.Equals(true))
                mainCamera.position = Vector3.MoveTowards(mainCamera.position, cameraPos[0].position, (cameraSpeed -1.5f) * Time.deltaTime);
        }
        else
        {
            if (cameraMovingState.Equals(true))
                mainCamera.position = Vector3.MoveTowards(mainCamera.position, cameraPos[1].position, cameraSpeed * Time.deltaTime);
            else if(cameraMovingState2.Equals(true))
                mainCamera.position = Vector3.MoveTowards(mainCamera.position, cameraPos[2].position, cameraSpeed * Time.deltaTime);
        }
        
    }

    IEnumerator WalkSoundPlay(float _standbySecond, float _second)
    {
        yield return new WaitForSecondsRealtime(_standbySecond);
        SoundMaixerManager.instance.WalkSoundPlay();
        yield return new WaitForSecondsRealtime(_second);
        SoundMaixerManager.instance.WalkSoundStop();
    }

    //미션에 관한 스토리 진행텍스트
    IEnumerator TextShowBox_Before()
    {
        yield return new WaitForSeconds(1f);

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("문을 가로막고 있는 이건 뭐지...?\n" +
            "아, 추억의 퍼즐이군. 이런 음침한 곳에 어울리지 않는 것 같은데...", 5f).SetEase(Ease.Linear).
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

        textBack.SetActive(false);
        cameraMovingState = true;
        StartCoroutine(WalkSoundPlay(0f, 2.5f));

        yield return new WaitForSeconds(2f);

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("일단 저 퍼즐 조각을 맞추면 문이 열릴 것 같군.", 3f).SetEase(Ease.Linear).
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

        textBack.SetActive(false);

        //yield return new WaitForSeconds(1f);

        if (fade_sripts.fadeStartState == false)
        {
            fadeEffect.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());
        }
        yield return new WaitForSeconds(2f);
        UpButtonsCtrl.instance.PuzzleGamePlay();
    }

    IEnumerator FadeStart()
    {
        yield return new WaitForSeconds(0.5f);
        fade_sripts.Fade(); //점점 어둡게 실행
    }

    //미션 성공 후 스토리 진행
    IEnumerator TextShowBox_After()
    {
        stonObj.SetActive(false);   //돌 오프젝트 비활성화
        mainCamera.position = cameraPos[0].position;

        yield return new WaitForSeconds(1f);
        
        SoundMaixerManager.instance.TreeFallSoundPlay();
        //나무판넬이 떨어지기 위해서 그레비티 활성화
        treePanel[0].useGravity = true; treePanel[1].useGravity = true;
        treePanel[0].GetComponent<MeshCollider>().enabled = true;
        treePanel[1].GetComponent<MeshCollider>().enabled = true;
        SoundMaixerManager.instance.TreeFallSoundPlay();
        yield return new WaitForSeconds(0.2f);
        SoundMaixerManager.instance.TreeFallSoundPlay();
        yield return new WaitForSeconds(0.2f);

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("역시 퍼즐을 맞추니 문이 열리는군!", 2f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(3.5f);

        textBack.SetActive(false);
        cameraMovingState = true;
        StartCoroutine(WalkSoundPlay(0f, 3f));  //걷기 사운드

        yield return new WaitForSeconds(1f);
        SoundMaixerManager.instance.FireSoundStop();
        yield return new WaitForSeconds(2f);
        SoundMaixerManager.instance.AppearSoundPlay();
        //회전 오브젝트 활성화
        PuzzieStateModelShow(true);

        yield return new WaitForSeconds(1f);
        PuzzleStateTextShow();

        yield return new WaitForSeconds(10f);
        textBack.SetActive(false);
        //오브젝트 사라짐
        PuzzieStateModelShow(false);
        SoundMaixerManager.instance.BoomSoundPlay();
        partical.SetActive(true);   //파티클 생성

        yield return new WaitForSeconds(1.5f);
        StartCoroutine(WalkSoundPlay(0f, 3f));  //걷기 사운드
        cameraMovingState = false;
        cameraMovingState2 = true;

        //yield return new WaitForSeconds(1f);
        if (fade_sripts.fadeStartState == false)
        {
            fadeEffect.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());
        }
        StartCoroutine(SceneMoving_ThreeGame());    //3번째 게임 씬으로 이동
    }

    //퍼즐 선택 관문에 따른 결과 화면 달라지기
    void PuzzleStateTextShow()
    {
        if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("One"))
        {
            textBack.SetActive(true);
            storyText.DOKill();
            storyText.text = "";
            storyText.DOText("어..! 이건 퍼즐 속에 있던 54호분 '투구'야.\n" +
                "크게 투구 몸통과 볼가리개로 구성되어 있지. 투구 몸통의 세로판은 \n" +
                "가죽끈으로 연결했겠군. 이걸 실제로 보다니!", 9f).SetEase(Ease.Linear).
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
        else if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two"))
        {
            textBack.SetActive(true);
            storyText.DOKill();
            storyText.text = "";
            storyText.DOText("어..! 이건 퍼즐 속에 있던 53호분 '목걸이'야.\n" +
                "담갈색과 담청색 경옥제와 곱은 옥 2점과 암청색의 유리제옥 115점으로 만들었군!\n" +
                "이걸 실제로 보다니!", 9f).SetEase(Ease.Linear).
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
        else if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three"))
        {
            textBack.SetActive(true);
            storyText.DOKill();
            storyText.text = "";
            storyText.DOText("어..! 이건 퍼즐 속에 있던 54호분 '화로모양 그릇받침'이야.\n" +
                "토기에 손잡이 점토만큼 구멍을 뚫은 후 그 자리에 손잡이 점토를 끼워 넣어 만들었지.\n" +
                "이걸 실제로 보다니!", 9f).SetEase(Ease.Linear).
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
    }

    //퍼즐 선택 관문에 따른 모델링 달라지기
    void PuzzieStateModelShow(bool _modelState)
    {
        if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("One"))
        {
            modelObj[0].SetActive(_modelState);  
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two"))
        {
            modelObj[1].SetActive(_modelState);
        }
        else if (PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three"))
        {
            modelObj[2].SetActive(_modelState);
        }
    }

    IEnumerator SceneMoving_ThreeGame()
    {
        yield return new WaitForSeconds(3f);

        //게임을 선택한 경로로 이동.
        if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("One"))
            SceneManager.LoadScene("TombstoneGame"); //환두대두 칼뽑기
        else if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("Two"))
            SceneManager.LoadScene("DustGame"); //등잔불 찾기, 터지
        else if(PlayerPrefs.GetString("HPG_FirstGateway").Equals("Three"))
            SceneManager.LoadScene("SendGame"); //모래 담기
    }
}
