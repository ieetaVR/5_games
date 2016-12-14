using UnityEngine;
using System.Collections;

public class stone_pinch_item : MonoBehaviour
{

    public Color whenTouchedColor;
    public float rotateSpeed = 1;
    public bool rotate = false;

    Color originalColor;
    public float timeSinceCollisionExit = 0;

    static int direction = 1;

    Vector3 originalPosition;

    // Use this for initialization
    void Start()
    {
        originalPosition = transform.position;
        rotateSpeed = rotateSpeed * direction;

        direction = direction * -1;

        originalColor = GetComponent<Renderer>().material.color;
    }


    public void putBack()
    {
        transform.position = originalPosition;
        GetComponent<Renderer>().material.color = originalColor;
        setRotate(false);
    }

    public void setRotate(bool r)
    {
        rotate = r;
    }

    public void highlight()
    {
        GetComponent<Renderer>().material.color = whenTouchedColor;
    }

    void OnCollisionStay(Collision col)
    {
        GetComponent<Renderer>().material.color = whenTouchedColor;
    }

    void OnCollisionExit(Collision col)
    {
        GetComponent<Renderer>().material.color = originalColor;
    }

    // Update is called once per frame
    void Update()
    {
        if(rotate == true)
            transform.Rotate(new Vector3(0, rotateSpeed, 0));

    }
}
