using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class ObjectRotate : MonoBehaviour
{
    float turnSpeed = 23;



    void Update()
    {
        if(SceneManager.GetActiveScene().name.Equals("GameChoiceScene"))
        {
           transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }
        else if(SceneManager.GetActiveScene().name.Equals("Game_1"))
        {
            if (gameObject.name.Equals("MissionModel3"))
            {
                transform.Rotate(Vector3.forward, turnSpeed * Time.deltaTime);
            }
            else if(gameObject.name.Equals("MissionModel3_1"))
            {
                //transform.Rotate(Vector3.left, turnSpeed * Time.deltaTime);
            }
            else if(gameObject.name.Equals("MissionModel2_2"))
                transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
            else
                transform.Rotate(Vector3.forward, turnSpeed * Time.deltaTime);
        }
        else
        {
            if(gameObject.name != "Necklace")
                transform.Rotate(Vector3.forward, turnSpeed * Time.deltaTime);
            else
                transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }
            
    }
}
