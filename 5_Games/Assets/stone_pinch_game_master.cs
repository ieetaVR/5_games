using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class stone_pinch_game_master : MonoBehaviour
{

    public Controller controller;

    public text_master bestText;
    public text_master logText;
    public text_master timeText;


    //to set
    public bool workFlag = false;
    public bool leftHand = true;
    public int pinchMargin = 20;
    public int totalIterations = 2;

    public GameObject[] goodItems;
    public GameObject[] badItems;
    public Vector3 leftItemPosition;
    public Vector3 rightItemPosition;


    //to check
    public bool waiting = false;
    public int secsToWait;
    public int secsWaited;
    public float startWaitingTime;
    public int gameState = 0;
    public int CountedHands = 0;
    public bool serverIsAlive = false;

    public GameObject leftItem;
    public GameObject rightItem;

    public Hand firstHand;
    public int pinchState = 0;

    int iterations = 0;
    int score = 0;
    bool rightIsCorrect = true;

    // Use this for initialization
    void Start()
    {

        bestText.setCurrentText("Welcome to Pinch Picker!");
        gameState = 0;
        waitFor(2);


        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);
    }

    public void SetRestParameters(JSONObject liftGameParameters)
    {
        leftHand = liftGameParameters.GetField("left_hand");

        pinchMargin = (int) liftGameParameters.GetField("pinch_margin").n;
        totalIterations = (int) liftGameParameters.GetField("total_interactions").n;

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
            firstHand = hands[0];

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
            //logText.setCurrentText("");
        }
        return result;
    }

    void updatePinchState()
    {
        Finger thumb = firstHand.Fingers[0];
        Finger index = firstHand.Fingers[1];
        Finger middle = firstHand.Fingers[2];
        Finger ring = firstHand.Fingers[3];
        Finger pinky = firstHand.Fingers[4];

        float index_distance = thumb.TipPosition.DistanceTo(index.TipPosition);
        float middle_distance = thumb.TipPosition.DistanceTo(middle.TipPosition);
        float ring_distance = thumb.TipPosition.DistanceTo(ring.TipPosition);
        float pinky_distance = thumb.TipPosition.DistanceTo(pinky.TipPosition);


        if (!thumb.IsExtended)
        {
            //index pinch
            if (!index.IsExtended && pinky.IsExtended && ring.IsExtended && middle.IsExtended)
            {
                //Debug.Log("index dist: " + index_distance);

                if (index_distance < pinchMargin)
                {
                    pinchState = 1;
                }
            }
            //middle pinch
            else if (index.IsExtended && pinky.IsExtended && ring.IsExtended && !middle.IsExtended)
            {
                //Debug.Log("middle dist: " + middle_distance);

                if (middle_distance < pinchMargin)
                {
                    pinchState = 2;
                }
            }
            //ring pinch
            else if (index.IsExtended && pinky.IsExtended && !ring.IsExtended && middle.IsExtended)
            {
                //Debug.Log("ring dist: " + ring_distance);

                if (ring_distance < pinchMargin)
                {
                    pinchState = 3;
                }
            }
            //pinky pinch
            else if (index.IsExtended && !pinky.IsExtended && ring.IsExtended && middle.IsExtended)
            {
                //Debug.Log("pinky dist: " + pinky_distance);

                if (pinky_distance < pinchMargin)
                {
                    pinchState = 4;
                }

            }
            else
            {
                //no pinch
                pinchState = 0;
            }
        }
        else
        {
            //can't be pinch (thumb is extended)
            pinchState = 0;
        }

    }


    void populateMeshes()
    {

        leftItem.GetComponent<stone_pinch_item>().putBack();
        rightItem.GetComponent<stone_pinch_item>().putBack();

        //50-50
        float random = Random.Range(0.0f, 10.0f);

        //good item on left
        if(random<5f)
        {
            leftItem= goodItems[Random.Range(0, goodItems.Length)];
            rightItem = badItems[Random.Range(0, badItems.Length)];
            
            rightIsCorrect = false;
        }

        //good item on right
        else
        {
            leftItem = badItems[Random.Range(0, badItems.Length)];
            rightItem = goodItems[Random.Range(0, goodItems.Length)];
            rightIsCorrect = true;
        }

        leftItem.transform.position = leftItemPosition;
        leftItem.GetComponent<stone_pinch_item>().setRotate(true);
        rightItem.transform.position = rightItemPosition;
        rightItem.GetComponent<stone_pinch_item>().setRotate(true);
    }

    void highlightRightItem()
    {
        if (rightIsCorrect == true)
        {
            rightItem.GetComponent<stone_pinch_item>().highlight();
        }
        else
        {
            leftItem.GetComponent<stone_pinch_item>().highlight();
        }
    }


    bool gameOver()
    {
        bool result = false;

        if(totalIterations == 0)
        {
            result = true;
        }

        return result;
    }

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

                    if(gameState == 0)
                    {
                        bestText.setCurrentText("For each Pair\nPick the Right Item.");
                        gameState = 1;
                        waitFor(2);
                    }
                    else if (gameState == 1)
                    {
                        bestText.setCurrentText("Right -> Ring Finger\nLeft -> Pinky Finger");
                        gameState = 2;
                        waitFor(2);
                    }
                    else if (gameState == 2)
                    {
                        bestText.setCurrentText("");
                        populateMeshes();
                        gameState = 3;
                        //waitFor(2);
                    }
                    else if (gameState == 4)
                    {
                        bestText.setCurrentText("Well Done!");
                        score++;
                        if(rightIsCorrect == true)
                        {
                            rightItem.GetComponent<stone_pinch_item>().highlight();
                        }
                        gameState = 6;
                        waitFor(2);
                    }
                    else if (gameState == 5)
                    {
                        bestText.setCurrentText("Wrong!");

                        highlightRightItem();

                        gameState = 6;
                        waitFor(2);
                    }
                    else if (gameState == 6)
                    {

                        totalIterations--;
                        timeText.setCurrentText("score: " + score + "\nleft: " + totalIterations);

                        if(gameOver() == true)
                        {
                            gameState = 99;
                        }
                        else
                        {
                            bestText.setCurrentText("Picking next trial.");
                            gameState = 2;
                            waitFor(2);
                        }
                    }

                    else if (HandsAreCorrect())
                    {
                        if(gameState == 3)
                        {
                            updatePinchState();
                            logText.setCurrentText("Right -> Ring Finger\nLeft -> Pinky Finger");
                            //right choice
                            if (pinchState == 3 && rightIsCorrect == true || pinchState == 4 && rightIsCorrect == false)
                            {
                                gameState = 4;
                            }
                            //wrong choice
                            else if(pinchState == 4 && rightIsCorrect == true || pinchState == 3 && rightIsCorrect == false)
                            {
                                gameState = 5;
                            }
                        }
                    }
                    else
                    {
                        secsWaited = 0;
                    }
                }
            }
            else
            {
                bestText.setCurrentText("Well Done!\nThe Exercise is complete!");
                logText.setCurrentText("");
                if (serverIsAlive == true)
                {
                    //sendToServer
                }
            }
        }
    }
}
