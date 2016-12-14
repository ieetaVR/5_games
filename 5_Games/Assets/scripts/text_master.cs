using UnityEngine;
using System.Collections;

public class text_master : MonoBehaviour {

    public TypeOutScript bestText;

    string currentText;

	// Use this for initialization
	void Start () {
	
	}
	
    public void setCurrentText(string newText, float typeTime=0f)
    {
        if(!newText.Equals(currentText))
        {
            bestText.TotalTypeTime = typeTime;
            bestText.reset = true;
            bestText.FinalText = newText;
            bestText.On = true;
            //Debug.Log("text Change. last: " + currentText + "    new: " + newText);
            currentText = newText;

        }
    }
    public void clearText()
    {
        if (!bestText.reset)
        {
            bestText.reset = true;
        }
    }

    

    // Update is called once per frame
    void Update () {

        bestText.On = true;

    }
}
