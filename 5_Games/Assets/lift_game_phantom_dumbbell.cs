using UnityEngine;
using System.Collections;

public class lift_game_phantom_dumbbell : MonoBehaviour {

    public lift_game_dumbbell dumbbell;
    public Vector3 goalPosition;
    public bool goalPositionDefined = false;

	// Use this for initialization
	void Start () {
	

        for (int i =0; i< GetComponent<Renderer>().materials.Length;i++)
        {
            GetComponent<Renderer>().materials[i].color =new Color(0,0,0, 0f);
        }

	}
	
	// Update is called once per frame
	void Update () { 
        if(goalPositionDefined == false)
        {
            if (dumbbell.startPos_Defined == true)
            {
                goalPosition = new Vector3(dumbbell.startPos.x, dumbbell.startPos.y + dumbbell.maxDistance, dumbbell.startPos.z);
                transform.position = goalPosition;
                goalPositionDefined = true;
            }
        }

        
	}
}
