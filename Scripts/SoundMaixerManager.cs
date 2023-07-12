using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundMaixerManager : MonoBehaviour
{
    public static SoundMaixerManager instance { get; private set; }

    public AudioMixer masterMixer;

    //Slider audioSlider;
    //Slider sfxSlider;

    public AudioSource bgmSource;
    public AudioSource walkSource;   //효과음bgm
    public AudioSource uiAudioSource;
    public AudioSource effectSource;
    public AudioSource fireSource;
    public AudioSource sendSource;
    public AudioSource collapeSource;
    public AudioSource sendputSource;

    public AudioClip bgmIntro;  //인트로화면
    public AudioClip bgmFirstGame;  //첫게임-세갈랫길
    public AudioClip bgmPuzzle; //게임 퍼즐
    public AudioClip bgmTombastone;  //칼뽑기
    public AudioClip bgmDust;  //촛대
    public AudioClip bgmSend;   //항아리모래
    public AudioClip bgmEnd;    //종료화면

    
    public AudioClip click_Clip;    //클릭음
    public AudioClip touch_Clip;   //터치음
    public AudioClip mission_Clip;   //미션팝업음
    public AudioClip missionSceecss_Clip;   //미션성공팝업
    public AudioClip noticPage_Clip;    //수첩페이지음
    public AudioClip text_Clip;    //텍스트음

    public AudioClip walk_Clip;   //걷는음

    public AudioClip fire_Clip; //불음

    public AudioClip puzzleDrop_Clip;   //퍼즐음   잘못
    public AudioClip puzzleGrab_Clip;   //퍼즐음 첫 클릭
    public AudioClip puzzleAssemble_Clip;   //퍼즐음 성공
    public AudioClip doorOpen_Clip; //문 열리는 음
    public AudioClip tree_Clip; //나무떨어지는음
    public AudioClip boom_Clip; //펑 음
    public AudioClip appear_Clip;   //나타나는 음
    public AudioClip find_Clip; //찾은 음
    public AudioClip fail_Clip; //틀린 음
    public AudioClip sendTouch_Clip;    //모래털기 음
    public AudioClip fireCatches_Clip;  //불붙는 음
    public AudioClip light_Clip;    //빛 음
    public AudioClip downSend_Clip; //떨어지는모래 음
    public AudioClip collapse_Clip; //무너지는 음
    public AudioClip Explosion_Clip;    //천장구멍음
    public AudioClip AppearMagic_Clip;  //나타나는 음- 뾰로롱
    public AudioClip putSuccess_Clip;   //모래담기성공음
    public AudioClip knife_Clip;    //칼 음
    public AudioClip sendPut_Clip;  //모래담다


    float backBGMVol = 1f;



    private void OnEnable()
    {
        Initialization();
    }

    public void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else instance = this;

        //DontDestroyOnLoad(this.gameObject);
    }


    void Start()
    {
        //Initialization();
    }


    //초기화
    void Initialization()
    {
        float sound = PlayerPrefs.GetFloat("AT_BackBGMVol");
        float sfx_sound = PlayerPrefs.GetFloat("AT_SFXVol");

        if (sound == -40f) masterMixer.SetFloat("BGM", -80);
        else masterMixer.SetFloat("BGM", sound);

        if (sfx_sound.Equals(-40f)) masterMixer.SetFloat("SFX", -80);
        else masterMixer.SetFloat("SFX", sfx_sound);

    }

    public void AudioControl()
    {
        //sliderParents = GameObject.Find("UICanvas");

        //audioSlider = sliderParents.transform.Find("PopupGroup").transform.GetChild(0).transform.Find("BackMusicSlider").GetComponent<Slider>();

        //float sound = audioSlider.value;

        //if (sound == -40f) masterMixer.SetFloat("BGM", -80);
        //else masterMixer.SetFloat("BGM", sound);

        //PlayerPrefs.SetFloat("AT_BackBGMVol", sound);
    }

    public void SFXAudioControl()
    {
        //sliderParents = GameObject.Find("UICanvas");

        //sfxSlider = sliderParents.transform.Find("PopupGroup").transform.GetChild(0).transform.Find("SoundEffectSlider").GetComponent<Slider>();

        //float sfx_Sound = sfxSlider.value;

        //if (sfx_Sound.Equals(-40f)) masterMixer.SetFloat("SFX", -80);
        //else masterMixer.SetFloat("SFX", sfx_Sound);

        //PlayerPrefs.SetFloat("AT_SFXVol", sfx_Sound);
    }

    public void SourcePause()
    {
        if(SceneManager.GetActiveScene().name.Equals("SendGame"))
        {
            sendSource.Pause();
            collapeSource.Pause();
            sendputSource.Pause();
        }
        walkSource.Pause();
        effectSource.Pause();
    }
    public void SourcePlay()
    {
        if (SceneManager.GetActiveScene().name.Equals("SendGame"))
        {
            sendSource.Play();
            collapeSource.Play();
            sendputSource.Play();
        }
        walkSource.Play();
        effectSource.Play();
    }

    ////////////배경음 메서드///////////////
    //인트로화면
    public void IntroBGMPlay()
    {
        bgmSource.clip = bgmIntro;
        bgmSource.Play();
    }
    //처음 게임 세갈랫길
    public void FirstBGMPlay()
    {
        bgmSource.clip = bgmFirstGame;
        bgmSource.Play();
    }

    //퍼즐게임
    public void PuzzleBGMPlay()
    {
        bgmSource.clip = bgmPuzzle;
        bgmSource.Play();
    }

    //칼뽑기게임
    public void TombastoneBGMPlay()
    {
        bgmSource.clip = bgmTombastone;
        bgmSource.Play();
    }

    //촛대털기 게임
    public void DustBGMPlay()
    {
        bgmSource.clip = bgmDust;
        bgmSource.Play();
    }

    //항아리모래게임
    public void SendBGMPlay()
    {
        bgmSource.clip = bgmSend;
        bgmSource.Play();
    }

    //엔딩화면
    public void EndingBGMPlay()
    {
        bgmSource.clip = bgmEnd;
        bgmSource.Play();
    }


    ///////효과음 메서드/////////
    public void ClickSoundPlay()
    {
        uiAudioSource.clip = click_Clip;
        uiAudioSource.Play();
        //Debug.Log("클링");
    }

    public void TouchSoundPlay()
    {
        uiAudioSource.clip = touch_Clip;
        uiAudioSource.Play();
    }

    public void MissionSoundPlay()
    {
        uiAudioSource.clip = mission_Clip;
        uiAudioSource.Play();
    }

    public void MissionSceecssSoundPlay()
    {
        uiAudioSource.clip = missionSceecss_Clip;
        uiAudioSource.Play();
    }

    public void NoticPageSoundPlay()
    {
        uiAudioSource.clip = noticPage_Clip;
        uiAudioSource.Play();
    }

    public void TextSoundPlay()
    {
        uiAudioSource.clip = text_Clip;
        uiAudioSource.Play();
    }


    public void OpenDoorSound()
    {
        effectSource.clip = doorOpen_Clip;
        effectSource.Play();
    }

    public void TreeFallSoundPlay()
    {
        effectSource.clip = tree_Clip;
        effectSource.Play();
    }

    public void BoomSoundPlay()
    {
        effectSource.clip = boom_Clip;
        effectSource.Play();
    }

    public void AppearSoundPlay()
    {
        effectSource.clip = appear_Clip;
        effectSource.Play();
    }

    public void ExplosionSoundPlay()
    {
        effectSource.clip = Explosion_Clip;
        effectSource.Play();

    }

    public void ARFindSoundPlay()
    {
        effectSource.clip = find_Clip;
        effectSource.Play();
    }

    public void ARFailSoundPlay()
    {
        effectSource.clip = fail_Clip;
        effectSource.Play();
    }

    public void SendTouchSoundPlay()
    {
        effectSource.clip = sendTouch_Clip;
        effectSource.Play();
    }

    public void FireCatchesSoundPlay()
    {
        effectSource.clip = fireCatches_Clip;
        effectSource.Play();
    }

    public void AppearMagicSoundPlay()
    {
        effectSource.clip = AppearMagic_Clip;
        effectSource.Play();
    }

    public void SendPutSuccessSoundPlay()
    {
        effectSource.clip = putSuccess_Clip;
        effectSource.Play();
    }

    public void LightSoundPlay()
    {
        effectSource.clip = light_Clip;
        effectSource.Play();
    }

    public void KnifeSoundPlay()
    {
        effectSource.clip = knife_Clip;
        effectSource.Play();
    }

    public void PuzzleDropSoundPlay()
    {
        effectSource.clip = puzzleDrop_Clip;
        effectSource.Play();
    }

    public void PuzzleGrabSoundPlay()
    {
        effectSource.clip = puzzleGrab_Clip;
        effectSource.Play();
    }

    public void PuzzleAssembleSoundPlay()
    {
        effectSource.clip = puzzleAssemble_Clip;
        effectSource.Play();
    }




    public void WalkSoundPlay()
    {
        walkSource.clip = walk_Clip;
        walkSource.Play();
    }

    public void WalkSoundStop()
    {
        walkSource.clip = walk_Clip;
        walkSource.Stop();
    }


    public void DownSendSoundPlay()
    {
        sendSource.clip = downSend_Clip;
        collapeSource.clip = collapse_Clip;
        sendSource.Play();
        collapeSource.Play();
    }

    public void DownSendSoundStop()
    {
        sendSource.clip = downSend_Clip;
        collapeSource.clip = collapse_Clip;
        sendSource.Stop();
        collapeSource.Stop();
    }


    public void SendPutSoundPlay()
    {
        sendputSource.clip = sendPut_Clip;
        sendputSource.Play();
    }

    public void SendPutSoundStop()
    {
        sendputSource.clip = sendPut_Clip;
        sendputSource.Stop();
    }


    public void FireSoundPlay()
    {
        fireSource.clip = fire_Clip;
        fireSource.Play();
    }

    public void FireSoundStop()
    {
        fireSource.clip = fire_Clip;
        fireSource.Stop();
    }

    


    //public void BicycleChainSoundPlay()
    //{
    //    //Debug.Log("사운드 시작");
    //    chainStartState = true; //체인소리 시작
    //    bicycleSource.clip = chain_Clip;
    //    bicycleSource.playOnAwake = true;
    //    bicycleSource.Play();
    //    bicycleSource.loop = true;
    //}

    //public void BicycleChainSoundStop()
    //{
    //    chainStartState = false;    //체인소리 멈춤
    //    bicycleSource.clip = chain_Clip;
    //    bicycleSource.playOnAwake = false;
    //    bicycleSource.loop = false;
    //    bicycleSource.Stop();
    //}
}
