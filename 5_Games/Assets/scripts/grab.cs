using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class grab : MonoBehaviour {


    public Controller controller;
    //public text_change text_changeReference;
    //public TypeOutScript bestText;
    public text_master bestText;
    public change_color change_color_reference;
    public Level_master masterLVL;
    public enjalbert_rest_client restClient;

    public text_master scoreText;

    public bool opening_hand;
    public bool holding_hand;

    float t;
    public float margin;
    int secsPassed;
    public int secsToWait;
    public int TotalIterations;
    public int workflag = 0;
    public int iterations;

    // Use this for initialization
    void Start () {

        opening_hand = true;

        //GameObject text3d = GameObject.Find("info_text");
        //text_changeReference = text3d.GetComponent<text_change>();
        //text_changeReference.setCurrentText("maybe!");
        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);

        //secsToWait = int.Parse(restClient.testToDo.GetField("lvl3").GetField("hold_time").ToString());
    }


    public void setRESTParameters()
    {
        secsToWait = int.Parse(restClient.testToDo.GetField("lvl3").GetField("hold_time").str);
        TotalIterations = int.Parse(restClient.testToDo.GetField("lvl3").GetField("iterations").str);
        margin = float.Parse(restClient.testToDo.GetField("lvl3").GetField("margin").str);
        
        if(this.name.Equals("RigidRoundHand_L") && restClient.testToDo.GetField("hand").str.Equals("left") || this.name.Equals("RigidRoundHand_R") && restClient.testToDo.GetField("hand").str.Equals("right"))
        {
            workflag = 1;
        }
        else
        {
            workflag = 0;
        }

    }

    void printScore()
    {
        scoreText.setCurrentText("iterations: " + iterations + "/" + TotalIterations);
    }

    // Update is called once per frame
    void Update () {
        
        if(workflag==1)
        {
            if (iterations == TotalIterations)
            {
                masterLVL.taskFinalStage();
            }

            else
            {
                Frame frame = controller.Frame();
                if (frame.Hands.Count > 0)
                {

                    //Debug.Log("num of hands: " + frame.Hands.Count);
                    List<Hand> hands = frame.Hands;
                    Hand firstHand = hands[0];
                    //Debug.Log("Grab Angle: " + firstHand.GrabAngle);

                    if (opening_hand)
                    {
                        if (holding_hand == false)
                        {
                            //text_changeReference.setCurrentText("open your hand");
                            //bestText.FinalText = "open your hand";
                            //bestText.On = true;

                            change_color_reference.setColor(1 - firstHand.GrabAngle);

                            if (firstHand.GrabAngle <= 0 + margin)
                            {

                                //text_changeReference.setCurrentText("Well done! Now hold.");
                                /*bestText.reset = true;
                                bestText.FinalText = "Well done! Now hold.";
                                bestText.On = true;*/
                                bestText.setCurrentText("Well done! Now hold.");
                                //bestText.setCurrentText(" ");

                                holding_hand = true;

                                t = Time.time;
                                secsPassed = 0;
                                //StartCoroutine(TestCoroutine());

                            }
                        }
                        else
                        {
                            if (firstHand.GrabAngle > 0 + margin)
                            {

                                /*bestText.reset = true;
                                bestText.FinalText = "Please hold a little longer.\n Open your hand.";
                                bestText.On = true;*/
                                bestText.setCurrentText("Please hold a little longer.\n Open your hand.");
                                //opening_hand = false;
                                holding_hand = false;
                            }
                            else
                            {
                                float newT = Time.time;
                                secsPassed = (int)(newT - t);
                                //bestText.setCurrentText("Well done! Now hold.");
                                if (secsPassed > 1)
                                {
                                    bestText.setCurrentText("secs left: " + (secsToWait - secsPassed).ToString(), 0f);
                                }


                                //Debug.Log("secs passed: " + secsPassed.ToString());
                                if (secsPassed >= secsToWait)
                                {


                                    opening_hand = false;
                                    holding_hand = false;
                                    iterations++;
                                    printScore();

                                    if (iterations < TotalIterations)
                                    {
                                        /*bestText.reset = true;
                                        bestText.FinalText = "Well done!\n Now close your hand";
                                        bestText.On = true;*/
                                        bestText.setCurrentText("Well done!\n Now close your hand");
                                    }
                                    else
                                    {
                                        bestText.setCurrentText("Task Completed.");
                                        //bestText.setCurrentText("Task Completed!");
                                    }

                                }
                            }
                        }

                    }
                    else
                    {
                        if (holding_hand == false)
                        {
                            //text_changeReference.setCurrentText("close your hand");

                            change_color_reference.setColor(firstHand.GrabAngle / 3.14f);

                            if (firstHand.GrabAngle > 3.14 - margin)
                            {
                                //text_changeReference.setCurrentText("Well done! now hold");
                                /*bestText.FinalText = "Well done! Now hold.";
                                bestText.On = true;*/
                                bestText.setCurrentText("Well done! Now hold.");
                                //bestText.setCurrentText(" ");


                                //StartCoroutine(TestCoroutine());
                                //opening_hand = true;
                                holding_hand = true;


                                t = Time.time;
                                secsPassed = 0;


                            }
                        }
                        else
                        {
                            if (firstHand.GrabAngle <= 3.14 - margin)
                            {
                                //opening_hand = true;
                                /*bestText.reset = true;
                                bestText.FinalText = "Please hold a little longer.\n Close your hand.";
                                bestText.On = true;*/
                                bestText.setCurrentText("Please hold a little longer.\n Close your hand.");

                                holding_hand = false;
                            }
                            else
                            {
                                float newT = Time.time;
                                secsPassed = (int)(newT - t);
                                if (secsPassed > 1)
                                {
                                    bestText.setCurrentText("secs left: " + (secsToWait - secsPassed).ToString(), 0f);
                                }

                                //Debug.Log("secs passed: " + secsPassed.ToString());
                                if (secsPassed >= secsToWait)
                                {

                                    opening_hand = true;
                                    holding_hand = false;
                                    iterations++;
                                    printScore();

                                    if (iterations < TotalIterations)
                                    {
                                        /*bestText.reset = true;
                                        bestText.FinalText = "Well done!\n Now open your hand";
                                        bestText.On = true;*/
                                        bestText.setCurrentText("Well done!\n Now open your hand");
                                    }
                                    else
                                    {
                                        bestText.setCurrentText("Task Completed.");
                                        //bestText.setCurrentText("Task Completed!");
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("no hands!");
                }
            }
        }
        
    }

    public bool running;

    IEnumerator TestCoroutine()
    {
        running = true;

        while (running)
        {
            Debug.Log("TestCoroutine()");
            yield return new WaitForSeconds(3);
        }
    }
}
