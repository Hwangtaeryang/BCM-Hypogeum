using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleTiggerCtrl : MonoBehaviour
{
    public static CandleTiggerCtrl instance { get; private set; }

    public GameObject touchBtn; //터치 버튼
    public ParticleSystem DustObj;  //먼지 오브젝
    public int touchCount;  //촛대 터치 수
    public GameObject[] dustObj;    //촛대에 붙은 흙
    public bool dustMission;    //미션 성공 여부


    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CandleTouch()
    {
        touchCount += 1;
        SoundMaixerManager.instance.SendTouchSoundPlay();
        if (touchCount.Equals(3))
        {
            dustObj[0].SetActive(false);
            DustObj.gameObject.SetActive(true);
            DustObj.Play();
        } 
        else if (touchCount.Equals(6))
            dustObj[1].SetActive(false);
        else if (touchCount.Equals(9))
        {
            dustObj[2].SetActive(false);
            DustObj.Play();
        }
        else if (touchCount.Equals(12))
            dustObj[3].SetActive(false);
        else if (touchCount.Equals(15))
        {
            dustObj[4].SetActive(false);
            DustObj.Play();
        } 
        else if (touchCount.Equals(18))
            dustObj[5].SetActive(false);
        else if (touchCount.Equals(21))
        {
            dustObj[6].SetActive(false);
            DustObj.Play();
            PlayerPrefs.SetString("HPG_DustMisson", "Success");
            dustMission = true;
            touchBtn.SetActive(false);
        }
            
    }
}
