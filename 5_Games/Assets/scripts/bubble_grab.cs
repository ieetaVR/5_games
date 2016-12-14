using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class bubble_grab : MonoBehaviour
{


    public Controller controller;
    //public text_change text_changeReference;
    //public TypeOutScript bestText;
    //public text_master bestText;
    //public change_color change_color_reference;
    //public Level_master masterLVL;
    //public enjalbert_rest_client restClient;
    
    //public text_master scoreText;
    
    public float margin;
    public int workflag = 0;
    public Hand my_Hand;

    public int hand_state = 0; //1-> closed; 2-> fully->open; 0->neither

    // Use this for initialization
    void Start()
    {
        
        //GameObject text3d = GameObject.Find("info_text");
        //text_changeReference = text3d.GetComponent<text_change>();
        //text_changeReference.setCurrentText("maybe!");
        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);

    }

    // Update is called once per frame
    void Update()
    {

        if (workflag == 1)
        {
            Frame frame = controller.Frame();
            if (frame.Hands.Count > 0)
            {

                //Debug.Log("num of hands: " + frame.Hands.Count);
                List<Hand> hands = frame.Hands;
                Hand firstHand = hands[0];
                //Debug.Log("Grab Angle: " + firstHand.GrabAngle);
                my_Hand  = firstHand;

                if (firstHand.GrabAngle <= 0 + margin)
                {
                    hand_state = 2;
                }
                else if (firstHand.GrabAngle > 3.14 - margin)
                {
                    hand_state = 1;
                }
                else
                {
                    hand_state = 0;
                }



            }

            else
            {
                Debug.Log("no hands!");
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
