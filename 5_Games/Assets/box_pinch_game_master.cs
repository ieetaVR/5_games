using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class box_pinch_game_master : MonoBehaviour
{

    public Controller controller;

    public text_master bestText;
    public text_master logText;
    public text_master timeText;

    public point_position leftHandPalm;
    public point_position rightHandPalm;

    public proximity_color_changer leftProximityBall;
    public proximity_color_changer rightProximityBall;

    point_position handPalm;

    public GameObject[] pickables;
    public int pickablesQuantity;
    public Vector3 pickablesAppearPosition;
    public Vector3 pickablesReappearPosition;
    public Vector3[] CollectedPickablesDropZone;

    //to set
    public bool workFlag = false;
    public bool leftHand = true;
    public float margin = 0.1f;
    public int pinchMargin = 20;

    //to check
    public bool waiting = false;
    public int secsToWait;
    public int secsWaited;
    public float startWaitingTime;
    public int gameState = 0;
    public int CountedHands = 0;
    public bool serverIsAlive = false;


    public Hand firstHand;
    //public GameObject[] realPickables;
    public List<GameObject> realPickables;
    public List<GameObject> donePickables;
    public int closestPickable_index;
    public float closestPickableDistance;
    public int pinchState = 0; // 0-no;1-index;2-middle;3-ring;4-pinky

    // Use this for initialization
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

        bestText.setCurrentText("Welcome to Box Pinch!");
        gameState = 0;
        waitFor(2);

        //realPickables = new GameObject[pickables.Length * pickablesQuantity];


        /*for(int i=0; i<pickables.Length; i++)
        {
            for(int j=0;j<pickablesQuantity; j++)
            {
                realPickables[realPickablesCount] =(GameObject) GameObject.Instantiate(pickables[i], pickablesAppearPosition, new Quaternion(0, 0, 0, 0));
                realPickablesCount++;
            }
        }*/

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

        margin = liftGameParameters.GetField("item_margin").n;
        pinchMargin = (int) liftGameParameters.GetField("pinch_margin").n;
        pickablesQuantity = (int) liftGameParameters.GetField("iterations_per_finger").n;

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

    void getClosestPickable()
    {
        float minDist = 99f;
        int pick = 0;
        float currentDistance = 0;

        for (int i = 0; i < realPickables.Count; i++)
        {
            currentDistance = Vector3.Distance(realPickables[i].transform.position, handPalm.transform.position);
            if (currentDistance < minDist)
            {
                pick = i;
                minDist = currentDistance;
            }
        }

        closestPickable_index = pick;
        closestPickableDistance = minDist;
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

    void stickToThumb()
    {
        realPickables[closestPickable_index].transform.position = Vector3.MoveTowards(realPickables[closestPickable_index].transform.position, handPalm.transform.position, 1);
    }

    void removeFromPickables()
    {
        Vector3 dropZone;

        Debug.Log("comparing: " + pickables[0].name + "(Clone)" + '/' + realPickables[closestPickable_index].name);

        if(realPickables[closestPickable_index].name.Equals(pickables[0].name + "(Clone)"))
        {
            Debug.Log("right drop");
            dropZone = CollectedPickablesDropZone[0];
        }
        else
        {
            Debug.Log("left drop");
            dropZone = CollectedPickablesDropZone[1];
        }

        donePickables.Add( (GameObject) GameObject.Instantiate(realPickables[closestPickable_index], dropZone, new Quaternion()));
        Destroy(realPickables[closestPickable_index]);
        realPickables.RemoveAt(closestPickable_index);
    }

    bool pinchIsCorrect()
    {
        bool result = false;

        if((pinchState == 1 && realPickables[closestPickable_index].name.Equals(pickables[0].name + "(Clone)")) || (pinchState == 2 && realPickables[closestPickable_index].name.Equals(pickables[1].name + "(Clone)")))
        {
            result = true;
        }

        return result;
    }

    bool gameOver()
    {
        bool result = false;

        if (realPickables.Count == 0)
        {
            result = true;
        }

        return result;
    }

    int i = 0, j = 0;
    int realPickablesCount = 0;

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
                    if (gameState == 0)
                    {
                        if (i < pickables.Length)
                        {
                            if (j < pickablesQuantity)
                            {
                                //realPickables[realPickablesCount] = (GameObject)GameObject.Instantiate(pickables[i], pickablesAppearPosition, new Quaternion(0, 0, 0, 0));
                                //realPickablesCount++;
                                realPickables.Add( (GameObject)GameObject.Instantiate(pickables[i], pickablesAppearPosition, new Quaternion(0, 0, 0, 0)));
                                j++;
                            }
                            else
                            {
                                j = 0;
                                i++;
                            }
                        }
                        else
                        {
                            gameState = 1;
                        }
                        waitFor(1);

                    }
                    else if (gameState == 1)
                    {
                        bestText.setCurrentText("Red items -> index pinch\nGreen items -> middle pinch");
                        gameState = 2;
                        waitFor(2);
                    }

                    else if (HandsAreCorrect())
                    {
                        if (gameState == 2)
                        {
                            bestText.setCurrentText("");
                            logText.setCurrentText("pick a item.\n(with pinch)");
                            leftProximityBall.setOpacity(0.1f);
                            leftProximityBall.updateColor(new Vector3(0, 0, 0));
                            getClosestPickable();
                            updatePinchState();

                            if(closestPickableDistance< margin)
                            {
                                if (pinchIsCorrect() == true)
                                {
                                    //Debug.Log("pickable name: " + realPickables[closestPickable_index].name);
                                    gameState = 3;
                                }
                            }
                            
                            //waitFor(2);
                        }
                        else if (gameState ==3)
                        {
                            updatePinchState();
                            if(pinchIsCorrect() == true)
                            {
                                if (leftProximityBall.distance <= margin)
                                {
                                    leftProximityBall.setOpacity(1f);
                                }

                                realPickables[closestPickable_index].GetComponent<Rigidbody>().detectCollisions = false;
                                logText.setCurrentText("bring item to green sphere");
                                stickToThumb();
                                leftProximityBall.updateColor(handPalm.transform.position);
                            }
                            else
                            {
                                gameState = 2;

                                if (leftProximityBall.distance > margin)
                                {
                                    realPickables[closestPickable_index].transform.position = pickablesReappearPosition;
                                    realPickables[closestPickable_index].GetComponent<Rigidbody>().detectCollisions = true;
                                }
                                else
                                {
                                    realPickables[closestPickable_index].GetComponent<Rigidbody>().detectCollisions = true;
                                    removeFromPickables();
                                    timeText.setCurrentText("items left: " + realPickables.Count + "\nRed: index\nGreen: middle");

                                    if(gameOver() == true)
                                    {
                                        gameState = 99;
                                    }

                                }
                                
                                
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
