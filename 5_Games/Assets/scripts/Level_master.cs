using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Level_master : MonoBehaviour
{

    public int currentLevel = 0;
    public int totalLevels = 5;

    //public TypeOutScript TypeOutScript;
    public text_master TypeOutScript;
    public TypeOutScript typeOutScript_numbers;
    public phantom_hand phantomHand;
    public leap_enjalbert_score_keeper scoreKeeper;

    //public text_change textChange;
    public bool finalStage = false;
    public bool gameOver = false;

    bool scoreCollected = false;
    bool scoreSent = false;

    // Use this for initialization
    void Start()
    {

        switch (currentLevel)
        {
            case 0:
                //textChange.setCurrentText("Welcome!");
                //TypeOutScript.TotalTypeTime = 2f;
                typeOutScript_numbers.TotalTypeTime = 4f;

                TypeOutScript.setCurrentText("Welcome to Enjalbert!\nPlease select a level.", 2f);

                //TypeOutScript.FinalText = "Welcome to Enjalbert!\nPlease select a level.";
                //TypeOutScript.On = true;

                //typeOutScript_numbers.On = true;
                break;

            case 1:
                //textChange.setCurrentText("Welcome!");
                /*TypeOutScript.TotalTypeTime = 2f;
                TypeOutScript.FinalText = "Level 1: follow the hand.";
                TypeOutScript.On = true;*/
                TypeOutScript.setCurrentText("Level 1: follow the hand.", 2f);

                break;

            case 2:
                //textChange.setCurrentText("Welcome!");
                /*TypeOutScript.TotalTypeTime = 2f;
                TypeOutScript.FinalText = "Level 2: follow the hand.";
                TypeOutScript.On = true;*/
                TypeOutScript.setCurrentText("Level 2: follow the hand.", 2f);
                break;

            case 3:
                //textChange.setCurrentText("Welcome!");
                /*TypeOutScript.TotalTypeTime = 2f;
                TypeOutScript.FinalText = "Level 3: Open & Close.";
                TypeOutScript.On = true;*/
                TypeOutScript.setCurrentText("Level 3: Open & Close.", 2f);
                break;

            case 4:
                //textChange.setCurrentText("Welcome!");
                /*TypeOutScript.TotalTypeTime = 2f;
                TypeOutScript.FinalText = "Level 3: Open & Close.";
                TypeOutScript.On = true;*/
                TypeOutScript.setCurrentText("Level 4: Pinch Index & Middle.", 2f);
                break;

            case 5:
                //textChange.setCurrentText("Welcome!");
                /*TypeOutScript.TotalTypeTime = 2f;
                TypeOutScript.FinalText = "Level 3: Open & Close.";
                TypeOutScript.On = true;*/
                TypeOutScript.setCurrentText("Level 5: Pinch Ring & Pinky.", 2f);
                break;

            default:
                break;
        }

    }



    float t;
    int secsPassed, stage = 0;


    public bool getFinalStage()
    {
        return finalStage;
    }


    public void gameOverCall()
    {
        t = Time.time;
        secsPassed = 0;
        gameOver = true;
    }
    public void taskFinalStage()
    {

        t = Time.time;
        secsPassed = 0;
        finalStage = true;
        /*
        while(secsPassed < 3)
        {
            newT = Time.time;
            secsPassed = (int)(newT - t);
        }


        TypeOutScript.reset = true;
        TypeOutScript.FinalText = "Moving to level 2.";
        TypeOutScript.On = true;

        secsPassed = 0;
        t = Time.time;

        while (secsPassed < 3)
        {
            newT = Time.time;
            secsPassed = (int)(newT - t);
        }

        Debug.Log("next scene: " + "Test_lvl" + (currentLevel + 1).ToString());
        //SceneManager.LoadScene("Test_lvl" + (currentLevel + 1).ToString());*/

    }


    // Update is called once per frame
    void Update()
    {



        if (finalStage == true)
        {
            if (!scoreCollected)
            {
                //Debug.Log("collecting data");
                scoreCollected = true;
                scoreKeeper.collectLevelInfo();
            }

            float newT;
            if (secsPassed < 3)
            {
                newT = Time.time;
                secsPassed = (int)(newT - t);
            }
            else
            {
                if (currentLevel < totalLevels)
                {
                    if (stage == 0)
                    {
                        /*TypeOutScript.reset = true;
                        TypeOutScript.FinalText = "Moving to level " + (currentLevel+1).ToString() + ".";
                        TypeOutScript.On = true;*/
                        TypeOutScript.setCurrentText("Moving to level " + (currentLevel + 1).ToString() + ".", 2f);

                        secsPassed = 0;
                        t = Time.time;
                        stage = 1;
                    }
                    else
                    {
                        //Debug.Log("next scene: " + "Test_lvl" + (currentLevel + 1).ToString());
                        SceneManager.LoadScene("Test_lvl" + (currentLevel + 1).ToString());
                    }
                }
                else
                {
                    TypeOutScript.setCurrentText("Well Done!\n The test is finished!");
                    if (!scoreSent)
                    {
                        scoreKeeper.sendToServer(true);
                        scoreSent = true;
                    }
                }

            }
        }
        else if (gameOver == true)
        {
            if (!scoreCollected)
            {
                scoreCollected = true;
                scoreKeeper.collectLevelInfo();
            }

            float newT;
            if (secsPassed < 3)
            {
                newT = Time.time;
                secsPassed = (int)(newT - t);
            }
            else
            {
                if (stage == 0)
                {
                    /*TypeOutScript.reset = true;
                    TypeOutScript.FinalText = "Moving to level " + (currentLevel+1).ToString() + ".";
                    TypeOutScript.On = true;*/
                    TypeOutScript.setCurrentText("Game Over!", 2f);

                    secsPassed = 0;
                    t = Time.time;
                    stage = 1;
                }
                else if (stage == 1)
                {
                    /*TypeOutScript.reset = true;
                    TypeOutScript.FinalText = "Moving to level " + (currentLevel+1).ToString() + ".";
                    TypeOutScript.On = true;*/
                    TypeOutScript.setCurrentText("Returning to Start.", 2f);

                    secsPassed = 0;
                    t = Time.time;
                    stage = 2;
                }
                else if (stage == 2)
                {
                    //Debug.Log("next scene: " + "Test_lvl" + (currentLevel + 1).ToString());
                    if (!scoreSent)
                    {
                        scoreKeeper.sendToServer(true);
                        scoreSent = true;
                    }
                    if (scoreKeeper.canGotoNextScene)
                    {
                        SceneManager.LoadScene("Full_test");
                    }
                }
            }
        }
    }
}
