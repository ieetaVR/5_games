using UnityEngine;
using System.Collections;

public class change_color : MonoBehaviour {


    public Color good = Color.green;
    public Color bad = Color.red;


	// Use this for initialization
	void Start () {
	
	}
	

    public void setColor (float done)
    {
        this.GetComponent<Renderer>().material.color = new Color(1 - done, done, 0, 1);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
