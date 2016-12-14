using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;

public class enjalbert_rest_client : MonoBehaviour
{


    public Level_master masterLVL;
    public leap_enjalbert_lift_arm lift_arm_script;
    public grab grab_script_left;
    public grab grab_script_right;
    public pinch_counter pinch_counter_script;
    public pinch_hand_test pinchHandTestLeft;
    public pinch_hand_test pinchHandTestRight;

    public enjalbert_time_keeper timer;


    public bool sentToServer = false;
    public JSONObject testToDo = new JSONObject();
    string server_url = "http://localhost:8090/";

    IEnumerator Start()
    {
        string url = server_url + "getTestToDo";

        WWW www = new WWW(url);
        yield return www;

        if (www.text != null && !www.text.Equals(""))
        {
            //Debug.Log("REST testToDo: " + www.text);
            testToDo = new JSONObject(www.text);

            switch (masterLVL.currentLevel)
            {
                case 1:
                case 2:
                    lift_arm_script.setRESTParameters();
                    break;
                case 3:
                    grab_script_left.setRESTParameters();
                    grab_script_right.setRESTParameters();
                    break;
                case 4:
                case 5:
                    pinch_counter_script.setRESTParameters();
                    if (testToDo.GetField("hand").str.Equals("left"))
                    {
                        pinchHandTestLeft.workFlag = 1;
                    }
                    else
                    {
                        pinchHandTestRight.workFlag = 1;
                    }
                    break;

                default:
                    break;
            }

            if (masterLVL.currentLevel != 0)
                timer.setRESTParameters();
        }
        else
        {
            Debug.Log("REST server not accessible!");

            switch (masterLVL.currentLevel)
            {
                case 1:
                case 2:
                    //lift_arm_script.wakeUp();
                    lift_arm_script.workFlag = 1;
                    break;
                case 3:
                    //grab_script_left.wakeUp();
                    grab_script_left.workflag = 1;
                    //grab_script_right.wakeUp();
                    break;
                case 4:
                case 5:
                    //pinch_counter_script.wakeUp();
                    pinch_counter_script.workFlag = 1;
                    pinchHandTestLeft.workFlag = 1;
                    break;

                default:
                    break;
            }

            if (masterLVL.currentLevel != 0)
                timer.workFlag = 1;
        }
    }


    public void sendTestResults(JSONObject scores)
    {
        StartCoroutine(sendTestResults_IEnumerator(scores));
    }

    IEnumerator sendTestResults_IEnumerator(JSONObject scores)
    {
        //Debug.Log("sendTestResults: " + System.Text.Encoding.UTF8.GetBytes(scores.ToString()).ToString());

        scores.SetField("test_id", testToDo.GetField("id"));
        scores.SetField("timestamp", System.DateTime.Now.ToString());

        string url = server_url + "sendTestResults";

        HttpWebRequest http =(HttpWebRequest) WebRequest.Create(url);
        http.Accept = "application/json";
        http.ContentType = "application/json";
        http.Method = "POST";

        string parsedContent = scores.ToString();
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        byte[] bytes = encoding.GetBytes(parsedContent);
        
        Stream newStream = http.GetRequestStream();
        newStream.Write(bytes, 0, bytes.Length);
        newStream.Close();

        var response = http.GetResponse();

        var stream = response.GetResponseStream();
        var sr = new StreamReader(stream);
        var content = sr.ReadToEnd();


        yield return response;

        /*

        WWW www;

        Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");

        WWWForm lvl1 = new WWWForm();
        lvl1.AddField("success", (int) scores.GetField("lvl1").GetField("success").n);
        WWWForm form = new WWWForm();//(WWWForm) scores;//new WWWForm();
       
        //www = new WWW(url, System.Text.Encoding.UTF8.GetBytes(scores.ToString()));
        www = new WWW(url, form);
        yield return www;

        Debug.Log("REST sendTestResults: " + www.text);
        */

        sentToServer = true;
        //testToDo = new JSONObject(www.text);

    }

    // Use this for initialization
    /*void Start () {
        

        Debug.Log(testToDo.ToString());
	
	}*/

    bool paramSet = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            Debug.Log("REST testToDo: " + testToDo.ToString());
        }
        /*
            if(paramSet==false)
            {
                if(testToDo != null)
                {
                    lift_arm_script.setRESTParameters();
                    paramSet = true;
                }

            }*/
    }
}
