using UnityEngine;
using System.Collections;

public class lift_game_dumbbell : MonoBehaviour
{


    //variables to set
    public float maxDistance = 0.5f;
    public int totalIterations = 3;
    public int timeToHold = 3;

    //to check
    public bool worFlag = false;
    public bool backTo_startPos = false;
    public bool startPos_Defined = false;
    public float current_height = 0f;
    public float distance;
    public Vector3 startPos;
    public int iterations = 0;
    public bool holding = false;
    public bool exerciseDone = false;

    public int secsWaited = 0;
    public float startTime = 0;


    // Use this for initialization
    void Start()
    {



    }

    public void resetState()
    {
        secsWaited = 0;
        holding = false;
        transform.position = startPos;
    }

    void changeColor()
    {
        GetComponent<Renderer>().materials[0].color = new Color(1 - distance / maxDistance, distance / maxDistance, 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (worFlag == true)
        {
            if (startPos_Defined == false)
            {
                startPos = this.transform.position;
                startPos_Defined = true;
                //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }
            else
            {

                changeColor();

                if (backTo_startPos == true)
                {
                    this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                    distance = Vector3.Distance(startPos, transform.position);
                    if (distance < 9.394719e-07)
                    {
                        backTo_startPos = false;
                    }
                }
                else
                {

                    current_height = this.transform.position.y;

                    distance = Vector3.Distance(startPos, transform.position);


                    if (distance >= maxDistance)
                    {
                        if(holding==true)
                        {
                            float currentTime = Time.time;
                            secsWaited = (int) (currentTime - startTime);
                            if(secsWaited >= timeToHold)
                            {
                                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                                worFlag = false;
                                backTo_startPos = true;
                                iterations++;
                                Debug.Log("peak reached! Iterations: " + iterations);

                                if(iterations == totalIterations)
                                {
                                    Debug.Log("Exercise ended");
                                    exerciseDone = true;
                                }
                            }
                        }
                        else
                        {
                            holding = true;
                            secsWaited = 0;
                            startTime = Time.time;
                            
                        }
                        
                    }
                    else
                    {
                        holding = false;
                    }
                }

            }
        }


    }
}
