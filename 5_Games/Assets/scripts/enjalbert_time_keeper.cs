using UnityEngine;
using System.Collections;

public class enjalbert_time_keeper : MonoBehaviour {

    public int workFlag = 0;
    public enjalbert_rest_client restClient;
    public text_master timerText;
    public Level_master masterLVL;
    
    public int secsPassed;
    public int secsToWait = 60;
    float startTime;

    // Use this for initialization
    void Start () {
        startTime = Time.time;
	}

    public void setRESTParameters()
    {
        secsToWait = int.Parse(restClient.testToDo.GetField("time_per_test").str);
        workFlag = 1;
    }

    // Update is called once per frame
    void Update () {
        
        if(workFlag == 1 && masterLVL.getFinalStage() == false)
        {
            float newT = Time.time;
            secsPassed = (int)(newT - startTime);
            
            if (secsPassed > secsToWait)
            {
                timerText.setCurrentText("Game Over");
                masterLVL.gameOverCall();
                workFlag = 0;
            }
            else
            {
                timerText.setCurrentText("time left: " + (secsToWait - secsPassed).ToString(), 0f);
            }
        }
        	
	}
}
