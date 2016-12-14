using UnityEngine;
using System.Collections;

public class bubble_controller : MonoBehaviour
{

    public int workFlag = 0;
    public float margin = 0f;
    public bubble_grab bubGrab;
    public point_position stickPoint;
    public point_position boxCenter;
    public Vector3 startPos;
    public float distance;
    public string hand = "RigidRoundHand_R";
    public bool canMove=false;

    // Use this for initialization
    void Start()
    {
        startPos = transform.position;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            transform.position = startPos;
        }
        if (bubGrab.hand_state == 1)
        {
            if (canMove)
            {
                //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                Vector3 toGo = new Vector3(bubGrab.my_Hand.PalmPosition.x, bubGrab.my_Hand.PalmPosition.y, bubGrab.my_Hand.PalmPosition.z);
                transform.position = Vector3.MoveTowards(transform.position, stickPoint.gameObject.transform.position, 1);


                Vector3 my2dPos = new Vector3(transform.position.x, transform.position.y, 0);
                Vector3 boxCenterPos = new Vector3(boxCenter.transform.position.x, boxCenter.transform.position.y, 0);
                distance = Vector3.Distance(my2dPos, boxCenterPos);
                //Debug.Log("distance: " + distance);
            }

         }
        else if(transform.position != startPos)
        {
            Vector3 my2dPos = new Vector3(transform.position.x, transform.position.y, 0);
            Vector3 boxCenterPos = new Vector3(boxCenter.transform.position.x, boxCenter.transform.position.y, 0);
            distance = Vector3.Distance(my2dPos, boxCenterPos);
            if (distance > margin)
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                Debug.Log("will go back");
                transform.position = startPos;
                canMove = false;
            }
            else
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                Debug.Log("right spot");
                canMove = false;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (workFlag == 1 && (collision.gameObject.transform.parent.transform.parent.name.Equals(hand) || collision.gameObject.transform.parent.name.Equals(hand)))
        {
            canMove = true;
        }
    }

}