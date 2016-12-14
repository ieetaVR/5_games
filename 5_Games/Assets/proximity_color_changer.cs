using UnityEngine;
using System.Collections;

public class proximity_color_changer : MonoBehaviour {

    public float distance;
    public float transparency = 0.1f;
	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().materials[0].color = new Color(0, 0, 0, transparency);
    }
	
    public void updateColor(Vector3 otherObject)
    {
        Vector3 me = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        Vector3 other = new Vector3(otherObject.x, 0, otherObject.z);
        //distance = Vector3.Distance(this.transform.position, otherObject);
        distance = Vector3.Distance(me, other);
        GetComponent<Renderer>().materials[0].color = new Color(distance, 1-distance, 0, transparency);
    }

    public void setOpacity(float opacity)
    {
        /*Color myColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = new Color(myColor.r, myColor.g, myColor.b, opacity);*/
        transparency = opacity;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
