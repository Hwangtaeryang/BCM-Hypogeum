using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;



public class VideoHandler : MonoBehaviour
{
    public static VideoHandler instance { get; private set; }

    public RawImage mScreen;
    public VideoPlayer mVideoPlayer;
    public VideoClip mVideoClip;  //비디오칩
    public GameObject mainPanel;    //메인화면
    public RenderTexture mVideoRawImage;
    bool pauseState;

    double time;
    double currentTime;


    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else instance = this;
    }

    private void Start()
    {
        mVideoRawImage.Release();
        time = mVideoPlayer.clip.length;
        mVideoPlayer.clip = mVideoClip;
        mVideoPlayer.SetDirectAudioVolume(0, 1);

        if (SceneManager.GetActiveScene().name.Equals("IntroScene"))
        {
            Initialized();
            SoundMaixerManager.instance.IntroBGMPlay();
        }
            
    }

    //메인인트로에서 무조건 초기화
    void Initialized()
    {
        Debug.Log("초기화!!!");
        //첫번째 관문 프리팹
        PlayerPrefs.SetString("HPG_FirstGateway", "No");   //No, One, Two, Three
        //처음 AR게임 1-1, 1-2, 1-3
        PlayerPrefs.SetString("HPG_OneGame", "Fail"); //Fail, Success
        PlayerPrefs.SetString("HPG_TwoGame", "Fail");   //Fail, Success
        PlayerPrefs.SetString("HPG_ThreeGame", "Fail"); //Fail, Success

        //3차 게임 시작 여부
        PlayerPrefs.SetString("HPG_TombstoneGame", "No");   //Start
        PlayerPrefs.SetString("HPG_DustGame", "No");    //Start
        PlayerPrefs.SetString("HPG_SendGame", "No");    //Start

        //유물 뽑고, 털기 미션 성공 여부
        PlayerPrefs.SetString("HPG_TombstoneMission", "Fail"); //Fail, Success, End
        PlayerPrefs.SetString("HPG_DustMisson", "Fail");    //Fail, Success, End
        PlayerPrefs.SetString("HPG_SendMission", "Fail");  //Fail, Success, End

        //퍼즐 관문 프리팹 - 성공여부
        PlayerPrefs.SetString("HPG_PuzzleGame", "No");  //No, Yes

        //엔딩씬
        PlayerPrefs.SetString("HPG_Ending", "No"); //No, TheEND
        
    }


    private void Update()
    {
        if(SceneManager.GetActiveScene().name.Equals("IntroScene"))
        {
            //영상이 끝나면 이벤트함수를 불러온다.
            mVideoPlayer.loopPointReached += MainPanelShow;
        }
        else
        {
            //Debug.Log("currentTime " + currentTime);
            //EndingScenePlay();
            //영상이 끝나면 이벤트함수를 불러온다.
            mVideoPlayer.loopPointReached += MainPanelShow;
        }
            
    }

    //메인판넬를 불러오는 함수
    void MainPanelShow(VideoPlayer _thisCam)
    {
        mainPanel.SetActive(true);
    }

    void EndingScenePlay()
    {
        if (UpButtonsCtrl.instance.pauseState.Equals(true))
            PauseVideo();
        else
        {
            currentTime = mVideoPlayer.time;
            if (currentTime >= time)
            {
                PlayerPrefs.SetString("HPG_Ending", "TheEND");
                mVideoPlayer.SetDirectAudioVolume(0, 0);
                StopVideo();
            } 
            else if(currentTime < time && PlayerPrefs.GetString("HPG_Ending").Equals("No"))
                PausePlayVideo();   //비디오 재생
        }
    }

    public void IntroSkipButton()
    {
        if(mVideoPlayer.isPlaying)
        {
            mVideoPlayer.Pause();
        }
        else
        {
            mVideoPlayer.Play();
        }
    }


    public void SkipButton()
    {
        pauseState = true;
    }


    public void PlayVideo()
    {
        mVideoPlayer.clip = mVideoClip;
        mVideoPlayer.Play();
        float timeVideo = (float)mVideoPlayer.clip.length;
        Debug.Log(timeVideo);
    }

    public void StopVideo()
    {
        mVideoPlayer.Stop();
    }

    public void PauseVideo()
    {
        Debug.Log("중지");
        mVideoPlayer.Pause();
    }

    public void PausePlayVideo()
    {
        //Debug.Log("왜 안들어오지?????" + mainPanel.activeSelf);
        if (mainPanel.activeSelf.Equals(false))
        {
            mVideoPlayer.Play();
            //Debug.Log("왜 안들어오지");
        }
            
        //currentTime = mVideoPlayer.time;
        //if (currentTime >= time)
        //{
        //    mVideoPlayer.Stop();
        //}
        //else
        //    mVideoPlayer.Play();
    }


    public void SceneMove()
    {
        SceneManager.LoadScene("GameChoiceScene");
    }

    
}
