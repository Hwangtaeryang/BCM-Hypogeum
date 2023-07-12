using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotTiggerCtrl : MonoBehaviour
{
    public GameObject[] sendParticel; //모래 흘러 나오기

    float sendTime_1, sendTime_2, sendTime_3;


    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Send"))
        {
            if(other.gameObject.name.Equals("Send1"))
            {
                sendTime_1 += Time.deltaTime;

                if(sendTime_1 >= 3f)
                {
                    SoundMaixerManager.instance.SendPutSuccessSoundPlay();
                    sendParticel[0].SetActive(false);
                    sendParticel[1].SetActive(true);
                }
            }
            else if (other.gameObject.name.Equals("Send2"))
            {
                sendTime_2 += Time.deltaTime;

                if (sendTime_2 >= 3f)
                {
                    SoundMaixerManager.instance.SendPutSuccessSoundPlay();
                    sendParticel[1].SetActive(false);
                    sendParticel[2].SetActive(true);
                }
            }
            else if (other.gameObject.name.Equals("Send3"))
            {
                sendTime_3 += Time.deltaTime;

                if (sendTime_3 >= 3f)
                {
                    SoundMaixerManager.instance.SendPutSuccessSoundPlay();
                    sendParticel[2].SetActive(false);
                    SandMissionUI_Manager.instance.MissionSuccess_SendStop();
                    SoundMaixerManager.instance.SendPutSoundStop();
                }
            }
        }
    }


}
