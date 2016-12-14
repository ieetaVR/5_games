using UnityEngine;
using System.Collections;

public class text_change_right : MonoBehaviour {
    public static string currentText = "text";
    public static float playerThirst = 100;
    public int workFlag = 1;


    public void setWorkFlag(int state)
    {
        if (state == 1 || state == 0)
            workFlag = state;
    }


    // Use this for initialization
    void Start()
    {
        //workFlag = 0;
        //setCurrentText("abc");
    }


    public void setCurrentText(string text)
    {
        currentText = text;
    }

    public void changeText(string text)
    {
        GetComponent<TextMesh>().text = text;
    }

    // Update is called once per frame
    void Update()
    {
        if (workFlag == 1)
            changeText(currentText);
    }
}
