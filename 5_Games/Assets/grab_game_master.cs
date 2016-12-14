using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class grab_game_master : MonoBehaviour {

    public Controller controller;
    public point_position leftHandPalm;
    public point_position rightHandPalm;
    public ParticleSystem water;

    point_position handPalm;
    public text_master bestText;
    public text_master logText;
    public text_master timeText;

    public drink_game_cup sprayBottle;

    //to set
    public bool workFlag = false;
    public bool leftHand = true;
    public int waitBetweenIterations = 0;
    public float margin = 0.1f;
    public int holdTime = 2;
    public int totalIterations = 3;
    public GameObject[] dishTypes;
    public GameObject currentDish;

    //to check
    public int iterations=0;
    public bool waiting = false;
    public int secsToWait;
    public int secsWaited;
    public float startWaitingTime;
    public int gameState = 0;
    public int CountedHands = 0;
    public bool serverIsAlive = false;
    public int handState = 0;
    public int wastedWater = 0;

    public Vector3 dishPosition = new Vector3(0.006f, -0.539f, 0.254f);

    public Hand firstHand;

    // Use this for initialization
    void Start ()
    {
        if (leftHand)
        {
            handPalm = leftHandPalm;
        }
        else
        {
            handPalm = rightHandPalm;
        }

        bestText.setCurrentText("Welcome to Dish Washer!");
        gameState = 1;
        waitFor(2);

        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);
    }

    public void SetRestParameters(JSONObject liftGameParameters)
    {
        leftHand = liftGameParameters.GetField("left_hand");
        if (leftHand)
        {
            handPalm = leftHandPalm;
        }
        else
        {
            handPalm = rightHandPalm;
        }

        margin = liftGameParameters.GetField("grab_margin").n;
        totalIterations = (int)liftGameParameters.GetField("total_interactions").n;
        holdTime = (int) liftGameParameters.GetField("time_to_hold").n;

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


    void  updateHandState()
    {
        if (firstHand.GrabAngle <= 0 + margin)
        {
            handState = 2;
        }
        else if (firstHand.GrabAngle > 3.14 - margin)
        {
            handState = 1;
        }
        else
        {
            handState = 0;
        }
    }

    bool gameOver()
    {
        bool result = false;

        if(iterations == totalIterations)
        {
            result = true;
        }

        return result;
    }

    int getRandomDish()
    {
        int result = 0;

        result = Random.Range(0, dishTypes.Length-1);

        return result;
    }

    // Update is called once per frame
    void Update () {
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
                        bestText.setCurrentText("Close Hand to turn fossett on.");
                        timeText.setCurrentText("dishes washed: " + iterations + "/" + totalIterations + "\nUsed Water: " + wastedWater);
                        wastedWater = 0;
                        gameState = 2;
                        waitFor(2);
                    }

                    if (HandsAreCorrect())
                    {

                        updateHandState();
                        if (handState == 1)
                        {
                            water.Play();
                            wastedWater++;

                        }
                        else
                        {
                            water.Stop();
                        }

                        if (gameState == 2)
                        {
                            bestText.setCurrentText("Wash the dish.");
                            timeText.setCurrentText("dishes washed: " + iterations + "/" + totalIterations + "\nUsed Water: " + wastedWater);


                            //put dirty dish on sink
                            if(currentDish==null)
                                currentDish =(GameObject) GameObject.Instantiate(dishTypes[getRandomDish()], dishPosition, new Quaternion(0,0,0,0));
                            currentDish.GetComponent<Renderer>().material.color = new Color(0,0,0);

                            secsWaited = 0;

                            if(handState==1)
                            {
                                startWaitingTime = Time.time;
                                gameState = 3;
                            }
                        }
                        if(gameState==3)
                        {
                            if(handState==1)
                            {
                                float currentTime = Time.time;
                                secsWaited = (int)(currentTime - startWaitingTime);

                                //currentDish.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white);

                                currentDish.GetComponent<Renderer>().material.color = new Color(((currentTime - startWaitingTime) / holdTime), ((currentTime - startWaitingTime) / holdTime), ((currentTime - startWaitingTime) / holdTime));

                                if (secsWaited>=holdTime)
                                {
                                    bestText.setCurrentText("The dish is clean.\nDon't waste water");
                                    gameState = 4;

                                }
                            }
                            else
                            {
                                //Destroy(currentDish);   
                                gameState = 2;
                            }
                        }
                        if(gameState==4)
                        {
                            if(handState != 1)
                            {
                                secsWaited = 0;
                                iterations++;
                                timeText.setCurrentText("dishes washed: " + iterations + "/" + totalIterations + "\nUsed Water: " + wastedWater);
                                if (gameOver() == false)
                                {
                                    bestText.setCurrentText("More Dishes coming.");
                                    gameState = 2;
                                }
                                else
                                {
                                    gameState = 99;
                                }
                                Destroy(currentDish);
                                waitFor(waitBetweenIterations);                                
                            }
                        }
                    }
                    else
                    {
                        secsWaited = 0;
                        water.Stop();
                    }
                }
            }
            else
            {
                bestText.setCurrentText("Well Done!\nThe Exercise is complete!");
                timeText.setCurrentText("dishes washed: " + iterations + "/" + totalIterations + "\nUsed Water: " + wastedWater);
                if (serverIsAlive == true)
                {
                    //sendToServer
                }
            }
        }
        
	}
}
