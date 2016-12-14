using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;



public class drink_game_master : MonoBehaviour
{

    [System.Serializable]
    public class fruit
    {
        public GameObject fruitObject;
        public int fruitQuantity;
    }

    public Controller controller;
    public fruit[] fruits;

    public drink_game_cup GreenApple;
    public drink_game_cup redApple;

    public point_position leftHandPalm;
    public point_position rightHandPalm;

    point_position handPalm;
    public text_master bestText;
    public text_master logText;
    public text_master timeText;

    public AudioSource bite;

    //to set
    public bool workFlag = false;
    public bool leftHand = true;
    public int waitBetweenIterations = 0;
    public float appleMargin = 0.25f;

    //to check
    public bool waiting = false;
    public int secsToWait;
    public int secsWaited;
    public float startWaitingTime;
    public int gameState = 0;
    public int CountedHands = 0;
    public bool serverIsAlive = false;
    public Vector3 goalPosition;
    public bool hasFruit = false;
    public float distanceToGreenApple = 0f;
    public float distanceToRedApple = 0f;

    public Hand firstHand;

    void Start()
    {
        if (leftHand)
        {
            handPalm = leftHandPalm;
        }
        else
        {
            handPalm = rightHandPalm;
        }

        bestText.setCurrentText("Welcome to Apple!");
        gameState = 2;
        waitFor(2);

        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);

    }


    public void SetRestParameters(JSONObject liftGameParameters)
    {
        leftHand = liftGameParameters.GetField("left_hand");
        if(leftHand)
        {
            handPalm = leftHandPalm;
        }
        else
        {
            handPalm = rightHandPalm;
        }

        fruits[0].fruitQuantity = (int) liftGameParameters.GetField("green_apple_quantity").n;
        fruits[1].fruitQuantity = (int) liftGameParameters.GetField("red_apple_quantity").n;
        appleMargin = liftGameParameters.GetField("apple_margin").n;

        waitBetweenIterations = (int) liftGameParameters.GetField("time_between_interactions").n;
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
            logText.setCurrentText("");
        }
        return result;
    }

    bool gameOver()
    {
        bool result = true;

        for(int i=0; i<fruits.Length; i++)
        {
            if (fruits[i].fruitQuantity >0)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    public int i = -1;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (i==0)
            {
                i = 1;
            }
        }

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
                    if (HandsAreCorrect())
                    {
                        if (gameState == 2)
                        {
                            if (i == 1)
                            {
                                i = -1;
                                bestText.setCurrentText("Goal Position Set.");
                                goalPosition = handPalm.transform.position;
                                gameState = 3;
                                waitFor(2);
                            }
                            else
                            {
                                i = 0;
                                bestText.setCurrentText("Set goal position.");
                            }
                            
                            
                        }
                        else if(gameState == 3)
                        {

                            timeText.setCurrentText("Apples left\nGreen: " + fruits[0].fruitQuantity + "\nRed: " + fruits[1].fruitQuantity);
                            if (gameOver() == false)
                            {
                                bestText.setCurrentText("reach apple to catch it");

                                distanceToGreenApple = Vector3.Distance(handPalm.transform.position, GreenApple.transform.position);
                                distanceToRedApple = Vector3.Distance(handPalm.transform.position, redApple.transform.position);

                                if (distanceToGreenApple < appleMargin)
                                {
                                    gameState = 4;
                                }
                                else if (distanceToRedApple < appleMargin)
                                {
                                    gameState = 5;
                                }
                            }
                            else
                            {
                                gameState = 99;
                            }
                        }
                        else if(gameState==4)
                        {
                            bestText.setCurrentText("Eat The Apple.");
                            GreenApple.moveToMe(handPalm.transform.position, handPalm.transform.rotation);
                            if(Vector3.Distance(handPalm.transform.position, goalPosition)<0.1)
                            {
                                bite.PlayOneShot(bite.clip);
                                fruits[0].fruitQuantity--;
                                gameState = 3;
                                if(fruits[0].fruitQuantity!=0)
                                    GreenApple.putBack();
                                else
                                {
                                    GreenApple.ThrowAway();
                                }
                            }
                        }

                        else if (gameState == 5)
                        {
                            bestText.setCurrentText("Eat The Apple.");
                            redApple.moveToMe(handPalm.transform.position, handPalm.transform.rotation);
                            if (Vector3.Distance(handPalm.transform.position, goalPosition) < 0.1)
                            {
                                bite.PlayOneShot(bite.clip);
                                fruits[1].fruitQuantity--;
                                gameState = 3;
                                if (fruits[1].fruitQuantity != 0)
                                    redApple.putBack();
                                else
                                {
                                    redApple.ThrowAway();
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                bestText.setCurrentText("Well Done!\nThe Exercise is complete!");
                if (serverIsAlive == true)
                {
                    //sendToServer
                }
            }
        }






    }
}
