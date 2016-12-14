using UnityEngine;
using System.Collections;

public class pinch_counter : MonoBehaviour {


    public Level_master masterLVL;
    public text_master bestText;
    public enjalbert_rest_client rest_client;

    public text_master scoreText;
    //index, middle, ring, pinky
    public int[] totalCounts = { 0, 0, 0, 0 };
    public int[] currentCounts = { 0, 0, 0, 0 };
    public int workFlag = 0;
    public bool done = false;

    // Use this for initialization

    void Start () {

        printCurrentScore();
        if(masterLVL.currentLevel==4)
        {
            totalCounts[0] = 2;
            totalCounts[1] = 2;
        }
        else
        {
            totalCounts[2] = 2;
            totalCounts[3] = 2;
        }

    }

    public void setRESTParameters()
    {
        switch (masterLVL.currentLevel)
        {
            case 4:
                totalCounts[0] = int.Parse(rest_client.testToDo.GetField("lvl4").GetField("total_index_counts").str);
                totalCounts[1] = int.Parse(rest_client.testToDo.GetField("lvl4").GetField("total_middle_counts").str);
                break;
            case 5:
                totalCounts[2] = int.Parse(rest_client.testToDo.GetField("lvl5").GetField("total_ring_counts").str);
                totalCounts[3] = int.Parse(rest_client.testToDo.GetField("lvl5").GetField("total_pinky_counts").str);
                break;
        }
        workFlag = 1;
    }


    void printCurrentScore()
    {
        scoreText.setCurrentText("index: " + currentCounts[0] + "/" + totalCounts[0] + "\n" +
                                 "middle: " + currentCounts[1] + "/" + totalCounts[1] + "\n" +
                                 "ring: " + currentCounts[2] + "/" + totalCounts[2] + "\n" +
                                 "pinky: " + currentCounts[3] + "/" + totalCounts[3] + "\n", 0f);
    }

    public void indexCount()
    {
        currentCounts[0]++;
        printCurrentScore();
    }

    public void middleCount()
    {
        currentCounts[1]++;
        printCurrentScore();
    }

    public void ringCount()
    {
        currentCounts[2]++;
        printCurrentScore();
    }

    public void pinkyCount()
    {
        currentCounts[3]++;
        printCurrentScore();
    }

    // Update is called once per frame
    void Update () {

        printCurrentScore();
        if (workFlag == 1)
        {
            bool taskComplete = true;

            //Debug.Log("state: " + currentCounts[0] + " " + totalCounts[0] + "      " + currentCounts[1] + " " + totalCounts[1] + "      " + currentCounts[2] + " " + totalCounts[2] + "      " + currentCounts[3] + " " + totalCounts[3]);

            for (int i = 0; i < totalCounts.Length; i++)
            {
                //Debug.Log("Comparing: " + currentCounts[i] + " " + totalCounts[i]);
                if (currentCounts[i] < totalCounts[i])
                {
                    taskComplete = false;
                    //Debug.Log("breaking on " + i.ToString());
                    break;
                }

                
            }

            if (taskComplete == true)
            {
                Debug.Log("Task done");
                bestText.setCurrentText("Task Completed!");
                masterLVL.taskFinalStage();
                done = true;
                workFlag = 0;
            }
        }

        
	}
}
