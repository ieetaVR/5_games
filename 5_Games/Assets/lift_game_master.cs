using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class lift_game_master : MonoBehaviour
{

    public Controller controller;
    public lift_game_dumbbell dumbbell;
    public text_master bestText;
    public text_master logText;
    public text_master timeText;

    //to set
    public bool workFlag = false;
    public bool leftHand = true;
    public int waitBetweenIterations = 0;


    //to check
    public bool waiting = false;
    public int secsToWait;
    public int secsWaited;
    public float startWaitingTime;
    public int gameState = 0;
    public int CountedHands = 0;
    public bool serverIsAlive = false;

    void Start()
    {

        bestText.setCurrentText("Welcome to Lift!");
        gameState = 1;
        waitFor(3);

        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);

    }

    public void SetRestParameters(JSONObject liftGameParameters)
    {
        leftHand = liftGameParameters.GetField("left_hand");
        waitBetweenIterations = (int) liftGameParameters.GetField("time_between_interactions").n;
        dumbbell.maxDistance = liftGameParameters.GetField("distance").n;
        dumbbell.totalIterations = (int) liftGameParameters.GetField("total_interactions").n;
        dumbbell.timeToHold = (int) liftGameParameters.GetField("time_to_hold").n;
        serverIsAlive = true;

    }

    void waitFor(int seconds)
    {
        secsToWait = seconds;
        waiting = true;
        startWaitingTime = Time.time;
    }

    bool HandsAreCorrect()
    {
        bool result = true;

        Frame frame = controller.Frame();
        CountedHands = frame.Hands.Count;
        if (frame.Hands.Count == 1)
        {
            List<Hand> hands = frame.Hands;
            Hand firstHand = hands[0];

            if ((leftHand == false && firstHand.IsLeft) || (leftHand == true && firstHand.IsLeft == false))
            {
                logText.setCurrentText("ERROR: wrong hand detected");
                result = false;
            }
        }
        else
        {
            result = false;
            logText.setCurrentText("ERROR: wrong number of hands detected");
        }

        if (result == true)
        {
            logText.setCurrentText("");
        }
        return result;
    }


    // Use this for initialization


    // Update is called once per frame
    void Update()
    {
        if (workFlag == true)
        {
            if (gameState != 99)
            {

                if (waiting)
                {
                    float currentTime = Time.time;
                    secsWaited = (int)(currentTime - startWaitingTime);
                    if (secsWaited >= secsToWait)
                    {
                        waiting = false;
                    }
                }
                else
                {
                    if (gameState == 1)
                    {
                        bestText.setCurrentText("lift the dumbbell.");
                        gameState = 2;
                        dumbbell.worFlag = true;
                        //waitFor(4);
                    }
                    else if (gameState == 3)
                    {
                        bestText.setCurrentText("let the dumbbell go down.");
                        dumbbell.worFlag = true;
                        if (dumbbell.backTo_startPos == false)
                        {
                            gameState = 1;
                        }
                    }
                    else
                    {
                        if (HandsAreCorrect())
                        {

                            if (gameState == 2)
                            {
                                timeText.setCurrentText("time to hold: " + dumbbell.secsWaited + "/" + dumbbell.timeToHold);

                                if (dumbbell.backTo_startPos == true)
                                {
                                    bestText.setCurrentText("Well done!\nIterations: " + dumbbell.iterations + "/" + dumbbell.totalIterations);
                                    if (dumbbell.exerciseDone == true)
                                    {
                                        gameState = 99;
                                    }
                                    else
                                    {
                                        gameState = 3;
                                    }

                                    waitFor(waitBetweenIterations);
                                }
                            }

                        }
                        else
                        {
                            if (gameState != 1)
                                dumbbell.resetState();
                        }

                    }
                }



            }
            else
            {
                bestText.setCurrentText("Well Done!\nThe Exercise is complete!");
                dumbbell.worFlag = false;
                if (serverIsAlive == true)
                {
                    //sendToServer
                }

            }
        }

    }
}
