using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class pinch_hand_test_right : MonoBehaviour {


    public Controller controller;
    public text_change_right info_text;

    public int min_dist = 5;

    // Use this for initialization
    void Start()
    {

        Debug.Log("thumb index distance");

        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);
    }

    // Update is called once per frame
    void Update()
    {

        Frame frame = controller.Frame();
        if (frame.Hands.Count > 0)
        {

            //Debug.Log("num of hands: " + frame.Hands.Count);
            List<Hand> hands = frame.Hands;
            Hand firstHand = hands[0];
            Finger thumb = firstHand.Fingers[0];
            Finger index = firstHand.Fingers[1];
            Finger middle = firstHand.Fingers[2];
            Finger ring = firstHand.Fingers[3];
            Finger pinky = firstHand.Fingers[4];

            float index_distance = thumb.TipPosition.DistanceTo(index.TipPosition);
            float middle_distance = thumb.TipPosition.DistanceTo(middle.TipPosition);
            float ring_distance = thumb.TipPosition.DistanceTo(ring.TipPosition);
            float pinky_distance = thumb.TipPosition.DistanceTo(pinky.TipPosition);
            //Debug.Log("index_distance: " + index_distance);
            //Debug.Log("grab_angle: " + firstHand.GrabAngle + "   pinch distance: " + firstHand.PinchDistance + " fingers extended: " + firstHand.Fingers[4].IsExtended + " " + firstHand.Fingers[3].IsExtended + " " + firstHand.Fingers[2].IsExtended + " " + firstHand.Fingers[1].IsExtended + " " + firstHand.Fingers[0].IsExtended + " ");
            //Debug.Log("pinky: " + pinky.IsExtended + " ring: " + ring.IsExtended + " middle: " + middle.IsExtended + " index: " + index.IsExtended + " thumb: " + thumb.IsExtended);

            if (!thumb.IsExtended)
            {
                //index pinch
                if (!index.IsExtended && pinky.IsExtended && ring.IsExtended && middle.IsExtended)
                {
                    Debug.Log("index dist: " + index_distance);

                    if (index_distance < min_dist)
                    {
                        //Debug.Log("index pinch");
                        info_text.setCurrentText("index pinch");
                    }
                }
                //middle pinch
                else if (index.IsExtended && pinky.IsExtended && ring.IsExtended && !middle.IsExtended)
                {
                    Debug.Log("middle dist: " + middle_distance);

                    if (middle_distance < min_dist)
                    {
                        //Debug.Log("middle pinch");
                        info_text.setCurrentText("middle pinch");
                    }
                }
                //ring pinch
                else if (index.IsExtended && pinky.IsExtended && !ring.IsExtended && middle.IsExtended)
                {
                    Debug.Log("ring dist: " + ring_distance);

                    if (ring_distance < min_dist)
                    {
                        //Debug.Log("ring pinch");
                        info_text.setCurrentText("ring pinch");
                    }
                }
                //pinky pinch
                else if (index.IsExtended && !pinky.IsExtended && ring.IsExtended && middle.IsExtended)
                {
                    Debug.Log("pinky dist: " + pinky_distance);

                    if (pinky_distance < min_dist)
                    {
                        //Debug.Log("pinky pinch");
                        info_text.setCurrentText("pinky pinch");
                    }
                }
                else
                {
                    //no pinch
                    //Debug.Log("no pinch");
                    info_text.setCurrentText("no pinch");
                }
            }
            else
            {
                //can't be pinch (thumb is extended)
                //Debug.Log("no pinch");
                info_text.setCurrentText("no pinch");
            }
        }
    }
}
