using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AccelerationCtrl : MonoBehaviour
{
    //public Text text;
    //public Text text2;
    //public Text text3;
    public GameObject knifeObj; //칼 오브젝트
    public GameObject potObj;   //항아리 오브젝트

    public GameObject partical; //빛나는 효과

    bool leftState;
    bool rightState;
    bool pullOffState;  //뽑았는지 상태
    bool putSendState;  //모래담은상태

    void Start()
    {
    }

    
    void Update()
    {
        //transform.Translate(Input.acceleration.x, 0, 0);
        //text.text = (Input.acceleration.x).ToString();
        //Debug.Log(UpButtonsCtrl.instance.tombstoneMission);
        if (SceneManager.GetActiveScene().name.Equals("TombstoneGame"))
        {
            //환두대도 뽑기 미션 팝업에서 시작버튼 눌렀고.
            if (pullOffState.Equals(false) && UpButtonsCtrl.instance.tombstoneMission.Equals(true))
            {
                //왼쪽기울기 - 음수
                if (Input.acceleration.x < 0)
                {
                    leftState = true;
                    if (rightState.Equals(true))
                    {
                        //SoundMaixerManager.instance.KnifeSoundPlay();
                        leftState = false;
                        rightState = false;
                        knifeObj.transform.position =
                            new Vector3(knifeObj.transform.position.x, knifeObj.transform.position.y + 0.05f,
                            knifeObj.transform.position.z);
                    }
                }
                else if (Input.acceleration.x > 0)
                {
                    rightState = true;
                    if (leftState.Equals(true))
                    {
                        SoundMaixerManager.instance.KnifeSoundPlay();
                        leftState = false;
                        rightState = false;
                        knifeObj.transform.position =
                            new Vector3(knifeObj.transform.position.x, knifeObj.transform.position.y + 0.05f,
                            knifeObj.transform.position.z);
                    }
                }
            }
            if (knifeObj.transform.position.y >= 5.6f)
            {
                if (UpButtonsCtrl.instance.tombstoneMission.Equals(true))
                {
                    pullOffState = true; UpButtonsCtrl.instance.tombstoneMission = false;
                    StartCoroutine(MissionSuccess());
                }
            }
        }
        else if (SceneManager.GetActiveScene().name.Equals("SendGame"))
        {
            if (putSendState.Equals(false) && UpButtonsCtrl.instance.sendMission.Equals(true))
            {
                //왼쪽기울기 - 음수
                if (Input.acceleration.x < 0)
                {
                    if (potObj.transform.position.x > -5.5f)
                    {
                        potObj.transform.position =
                        new Vector3(potObj.transform.position.x - 0.15f, potObj.transform.position.y,
                        potObj.transform.position.z);
                    }

                }
                else if (Input.acceleration.x > 0)
                {
                    if (potObj.transform.position.x < 5.7f)
                    {
                        potObj.transform.position =
                        new Vector3(potObj.transform.position.x + 0.15f, potObj.transform.position.y,
                        potObj.transform.position.z);
                    }
                }
            }
        }
    }

    IEnumerator MissionSuccess()
    {
        UpButtonsCtrl.instance.tombstoneMission = false;
        PlayerPrefs.SetString("HPG_TombstoneMission", "End");
        SoundMaixerManager.instance.LightSoundPlay();
        partical.SetActive(true);

        yield return new WaitForSeconds(3f);
        partical.SetActive(false);
        //TombstoneMissionUI_Manager.instance.MissionSuccessPopup();
    }
}
