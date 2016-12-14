using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class leap_enjalbert_lift_arm : MonoBehaviour {
    
    public float range = 3;

    public int secondsToCount = 5;
    public Color altColor = Color.green;
    float green = 0.1f;

    //public text_change text_changeReference;
    //public TypeOutScript bestText;
    public text_master bestText;
    public change_color phantomHandColor;
    //public text_change bestTextQuick;
    public Level_master masterLVL;
    public enjalbert_rest_client restClient;

    float t, newT, lastCollisionEnter;
    int secsPassed;
    bool firstCol = true, taskComplete = false;
    string currentCollider = "";
    public string hand = "RigidRoundHand_L";
    public int workFlag = 0;
    public float vertical_distance_variable = 0f;

    public Vector3 startPosition;
    public Vector3 updatedPosition;

    // Use this for initialization
    void Start()
    {
        //GameObject text3d = GameObject.Find("info_text");
        //text_changeReference = text3d.GetComponent<text_change>();
        GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
        startPosition = this.transform.position;
        updatedPosition = startPosition;
        workFlag = 0;

        
    }

    public void setRESTParameters()
    {
        Debug.Log("setting test params");
        Debug.Log("restClient hand: " + restClient.testToDo.GetField("hand").str);

        if (restClient.testToDo.GetField("hand").str.Equals("right"))
        {
            Debug.Log("changing to right hand");
            hand = "RigidRoundHand_R";
        }

        switch (masterLVL.currentLevel)
        {
            case 1:
                //transform.position = new Vector3(transform.position.y, 0.113f, transform.position.z);
                secondsToCount = int.Parse(restClient.testToDo.GetField("lvl1").GetField("hold_time").str);
                vertical_distance_variable = float.Parse(restClient.testToDo.GetField("lvl1").GetField("vertical_distance").str);
                
                //updatedPosition.y = updatedPosition.y + (vertical_distance_variable / 10);
                Debug.Log("prev y: " + updatedPosition.y);
                Debug.Log("calc : " + updatedPosition.y + " + " + ((float)vertical_distance_variable / 10));
                updatedPosition = new Vector3(updatedPosition.x, updatedPosition.y + ((float)vertical_distance_variable / 10), updatedPosition.z);
                
                Debug.Log("topBar new pos: " + updatedPosition.ToString());
                
                this.transform.position = updatedPosition;
                break;
            case 2:
                secondsToCount = int.Parse(restClient.testToDo.GetField("lvl2").GetField("hold_time").str);
                break;
        }

        

    }


    public void setWorkFlag(int new_val)
    {
        workFlag = new_val;
        //bestText.TotalTypeTime = 0f;

    }


    void OnCollisionStay(Collision collision)
    {
        if(workFlag == 1 && (collision.gameObject.transform.parent.transform.parent.name.Equals(hand) || collision.gameObject.transform.parent.name.Equals(hand)))
        {
            if (taskComplete == false)
            {
                newT = Time.time;
                int secsPassed_now = (int)(newT - t);
                if (secsPassed_now > secsPassed)
                {
                    secsPassed = secsPassed_now;
                    //bestTextQuick.setCurrentText("secs left: " + (secondsToCount - secsPassed).ToString());
                    /*bestText.reset = true;
                    bestText.FinalText = "secs left: " + (secondsToCount - secsPassed).ToString();
                    bestText.On = true;*/
                    bestText.setCurrentText("secs left: " + (secondsToCount - secsPassed).ToString(), 0f);


                    green += 0.1f;
                    GetComponent<Renderer>().material.color = new Color(0, green, 0, 1);
                    //Debug.Log((float)secsPassed / secondsToCount);
                    phantomHandColor.setColor((float)secsPassed /secondsToCount);

                    if (secsPassed == secondsToCount)
                    {
                        //text_changeReference.setCurrentText("task completed");
                        /*bestText.reset = true;
                        bestText.FinalText = "task completed";
                        bestText.On = true;*/
                        bestText.setCurrentText("Task Completed.", 2f);
                        Debug.Log("Task Complete!");
                        taskComplete = true;
                        workFlag = 0;
                        green = 0.1f;

                        masterLVL.taskFinalStage();

                    }
                }
            }
        }
        
        

       
    }


    void OnCollisionEnter(Collision col)
    {

        if (workFlag == 1 && (col.gameObject.transform.parent.transform.parent.name.Equals(hand) || col.gameObject.transform.parent.name.Equals(hand)))
        {

            if (!(col.gameObject.transform.parent.transform.parent.name.Equals(currentCollider) || col.gameObject.transform.parent.name.Equals(currentCollider)))
            {
                //Debug.Log("new col");
                lastCollisionEnter = Time.time;
                currentCollider = col.gameObject.transform.parent.transform.parent.name;
                t = Time.time;
                newT = 0;
                secsPassed = 0;
                GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
                taskComplete = false;
            }
            else
            {
                float extra_T = Time.time;
                if (extra_T - newT > 1 && taskComplete == false)
                {
                    //Debug.Log("new col");
                    lastCollisionEnter = Time.time;
                    currentCollider = col.gameObject.transform.parent.transform.parent.name;
                    t = Time.time;
                    newT = 0;
                    secsPassed = 0;
                    GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);

                }
            }

        }


    }


    void OnCollisionExit(Collision collisionInfo)
    {
        //print("No longer in contact with " + collisionInfo.transform.name);

    }

    void goBack()
    {
        
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("z"))
        {
            Debug.Log("input string: " + restClient.testToDo.GetField("lvl1").GetField("hold_time").ToString());

            secondsToCount = int.Parse(restClient.testToDo.GetField("lvl1").GetField("hold_time").str);

            Debug.Log("secs: " + secondsToCount);
            
            


        }

    }
}
