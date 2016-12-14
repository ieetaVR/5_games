using UnityEngine;
using System.Collections;

public class lift_arm_side_wall : MonoBehaviour {


    public int workFlag;

    public leap_enjalbert_lift_arm top_bar_ref;
    //public text_change info_text_ref;
    //public TypeOutScript bestText;
    public text_master bestText;
    public phantom_hand phantomHand;

    // Use this for initialization
    void Start () {
	
	}

    void OnCollisionStay(Collision collision)
    {
        if(workFlag == 1 && top_bar_ref.workFlag == 1 && (collision.gameObject.transform.parent.transform.parent.name.Equals(top_bar_ref.hand) || collision.gameObject.transform.parent.name.Equals(top_bar_ref.hand)))
        {
            Debug.Log("col on wall");
            top_bar_ref.setWorkFlag(0);
            //info_text_ref.setCurrentText("Wall bump - press start");
            /*bestText.reset = true;
            bestText.FinalText = "Wall bump - back to the start";
            bestText.On = true;*/
            bestText.setCurrentText("Wall bump - back to the start", 2f);
            phantomHand.mustMove = false;
            phantomHand.moveToStart();
            workFlag = 0;
        }
    }


    // Update is called once per frame
    void Update () {
	
	}
}
