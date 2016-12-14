using UnityEngine;
using System.Collections;

public class lift_arm_start_pos : MonoBehaviour {

    public leap_enjalbert_lift_arm top_bar_ref;
    public Level_master masterLVL;
    //public text_change info_text_ref;
    public phantom_hand phantomHand;
    //public TypeOutScript bestText;
    public lift_arm_side_wall sideWall_left;
    public lift_arm_side_wall sideWall_right;

    public text_master bestText;

    public int workFlag = 1;
    

    // Use this for initialization
    void Start () {
        top_bar_ref.setWorkFlag(0);
        //info_text_ref.setCurrentText("place arm on start");

    }


    void OnCollisionStay(Collision collision)
    {
        if(masterLVL.getFinalStage()==false && (collision.gameObject.transform.parent.transform.parent.name.Equals(top_bar_ref.hand) || collision.gameObject.transform.parent.name.Equals(top_bar_ref.hand)) && (sideWall_left.workFlag == 0 || sideWall_right.workFlag==0))
        {
            Debug.Log("col on start");
            top_bar_ref.setWorkFlag(1);
            phantomHand.moveToStart();
            phantomHand.mustMove = true;


            //bestText.reset = true;
            bestText.setCurrentText("Follow the hand.");
            //bestText.clearText();

            sideWall_left.workFlag = 1;
            sideWall_right.workFlag = 1;
            //info_text_ref.setCurrentText("lift arm to top bar");

            if(top_bar_ref.hand.Equals("RigidRoundHand_L"))
            {
                phantomHand.direction = 0;
            }
            else
            {
                phantomHand.direction = 1;
            }

            workFlag = 0;
        }
    }


    // Update is called once per frame
    void Update () {
	
	}
}
