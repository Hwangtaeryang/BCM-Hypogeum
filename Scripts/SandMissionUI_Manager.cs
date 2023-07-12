using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SandMissionUI_Manager : MonoBehaviour
{
    public static SandMissionUI_Manager instance { get; private set; }

    public CinemachineVirtualCamera virtualCam; //가상카메라
    CinemachineBasicMultiChannelPerlin _virtalCam;
    public NoiseSettings shake6D;
    public CinemachineDollyCart followObj;    //따라갈 오브젝트

    public GameObject missionBar;   //미션바
    public GameObject potObj;   //항아리
    public GameObject potParticel;  //항아리파티클
    public GameObject[] sendObj;    //천장 모래
    public Rigidbody ceiling;   //동굴천장
    public GameObject lightObj; //천장 빛    

    public GameObject missionPopup2; //미션 선택 팝업
    public GameObject textBack; //텍스트 나오는 박스
    public Text storyText;  //나레이션 텍스트

    public GameObject fadeEffect;   //페이드 효과 스크립트
    FadeEffect fade_sripts;

    public GameObject sendGrave;    //모래무덤
    public GameObject rainSend; //쏟아지는 모래
    public GameObject dustGroup;    //먼지그룹
    public GameObject sandGroup;    //모래그룹

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
        SoundMaixerManager.instance.SendBGMPlay();
        SoundMaixerManager.instance.FireSoundPlay();
        beforeText = storyText.text;

        _virtalCam = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        fade_sripts = fadeEffect.GetComponent<FadeEffect>();

        Debug.Log(" =====  " + PlayerPrefs.GetString("HPG_SendGame"));
        Debug.Log(" =====  " + PlayerPrefs.GetString("HPG_SendMission"));
        //PlayerPrefs.SetString("HPG_SendMission","Success");

        if ((PlayerPrefs.GetString("HPG_SendGame").Equals("No") &&
            PlayerPrefs.GetString("HPG_SendMission").Equals("Fail"))||
            PlayerPrefs.GetString("HPG_SendGame").Equals(""))
        {
            StartCoroutine(TestShowBox_Before());
        }
        else if(PlayerPrefs.GetString("HPG_SendMission").Equals("Success")) //Success
        {
            StartCoroutine(MissionTextBoxShow());
        }
    }

    private void Update()
    {
        if(potObj.activeSelf.Equals(true))
        {
            potParticel.transform.position = new Vector3(potObj.transform.position.x,
                potParticel.transform.position.y, potParticel.transform.position.z);
        }
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

    IEnumerator TestShowBox_Before()
    {
        potObj.SetActive(false);
        yield return new WaitForSeconds(1f);

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("여기는 뭔가 으스스한데...", 2f).SetEase(Ease.Linear).
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
        _virtalCam.m_AmplitudeGain = 1f;
        dustGroup.SetActive(true);
        SoundMaixerManager.instance.DownSendSoundPlay();    //모래떨어지는 소리

        yield return new WaitForSeconds(1f);
        storyText.text = "";
        storyText.DOText("으아앗!갑자기 모래 천장이 무너질 것 같아..!!!", 3f).SetEase(Ease.Linear).
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
        sandGroup.SetActive(true);

        yield return new WaitForSeconds(1f);
        storyText.text = "";
        storyText.DOText("떨어지는 모래가 많아지면, 여길 빠져나갈 수 없을거야!\n" +
            "쏟아지는 모래를 담을 항아리를 찾아야해!", 6f).SetEase(Ease.Linear).
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
        //UpButtonsCtrl.instance.MissionPopup(0);
        if (fade_sripts.fadeStartState == false)
        {
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());    //점점 하얗게
        }

        yield return new WaitForSeconds(3.5f);
        UpButtonsCtrl.instance.SandGamePlay();
    }

    //중간 미션 - 모래를 담아라
    IEnumerator MissionTextBoxShow()
    {
        potObj.SetActive(true); //항아리 활성화
        SoundMaixerManager.instance.DownSendSoundPlay();    //모래떨어지는 소리
        _virtalCam.m_AmplitudeGain = 1f;
        dustGroup.SetActive(true);
        sandGroup.SetActive(true);

        yield return new WaitForSeconds(1f);
        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("얼른 큰 항아리를 이용해서 쏟아지는 모래를 담자!", 4f).SetEase(Ease.Linear).
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

        yield return new WaitForSeconds(1f);
        SoundMaixerManager.instance.MissionSoundPlay();
        missionPopup2.SetActive(true);
    }

    //모래담기 성공 - 모래 멈춤
    public void MissionSuccess_SendStop()
    {
        StartCoroutine(_MissionSuccess_SendStop());
    }

    IEnumerator RainSendShow()
    {
        rainSend.SetActive(true);
        yield return new WaitForSeconds(3f);
        rainSend.SetActive(false);
        sendGrave.SetActive(true);
    }

    IEnumerator _MissionSuccess_SendStop()
    {
        yield return new WaitForSeconds(1f);
        SoundMaixerManager.instance.AppearMagicSoundPlay();
        potObj.SetActive(false);
        potParticel.SetActive(true);
        sendObj[0].SetActive(false);
        sendObj[1].SetActive(false);

        yield return new WaitForSeconds(0.5f);
        sendObj[2].SetActive(false);
        sendObj[3].SetActive(false);

        yield return new WaitForSeconds(0.5f);
        sendObj[4].SetActive(false);
        sendObj[5].SetActive(false);
        SoundMaixerManager.instance.DownSendSoundStop();


        yield return new WaitForSeconds(1f);
        ceiling.useGravity = true;
        ceiling.GetComponent<MeshCollider>().enabled = true;
        SoundMaixerManager.instance.ExplosionSoundPlay();
        lightObj.SetActive(true);
        Debug.Log("여기까지!");
        StartCoroutine(RainSendShow()); //쏟아지는 모래
        
        //yield return new WaitForSeconds(1f);
        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("앗! 모래가 멎었어...!\n" +
            "천장의 저 구멍에서 빛이 새어나오고 있어...!", 3f).SetEase(Ease.Linear).
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
        SoundMaixerManager.instance.WalkSoundPlay();
        followObj.m_Speed = 5;
        yield return new WaitForSecondsRealtime(2.5f);
        SoundMaixerManager.instance.WalkSoundStop();

        yield return new WaitForSeconds(6f);
        SoundMaixerManager.instance.LightSoundPlay();
        textBack.SetActive(true);
        storyText.text = "";
        storyText.DOText("너무 눈부셔..으아아아앗!!!!!", 1.5f).SetEase(Ease.Linear).
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
        if (fade_sripts.fadeStartState == false)
        {
            fadeEffect.SetActive(true);
            StartCoroutine(FadeStart());    //점점 하얗게
        }

        yield return new WaitForSeconds(2f);
        PlayerPrefs.SetString("HPG_SendMission", "End");
        SceneManager.LoadScene("EndingScene");
    }

    IEnumerator FadeStart()
    {
        yield return new WaitForSeconds(0.5f);
        fade_sripts.Fade(); //점점 어둡게 실행
    }
}
