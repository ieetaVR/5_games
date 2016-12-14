using UnityEngine;
using System.Collections;

public class box_pinch_game_rest_client : MonoBehaviour {

    public box_pinch_game_master masterLVL;

    public bool sentToServer = false;
    public JSONObject gameToDo = new JSONObject();
    string server_url = "http://localhost:8090/";

    // Use this for initialization
    IEnumerator Start()
    {
        string url = server_url + "getGameToDo";

        WWW www = new WWW(url);
        yield return www;

        if (www.text != null && !www.text.Equals(""))
        {
            //Debug.Log("REST testToDo: " + www.text);
            gameToDo = new JSONObject(www.text);

            Debug.Log("game type: " + gameToDo.GetField("type").n);

            if (gameToDo.GetField("type").n == 3)
            {
                Debug.Log("game type Correct");
                masterLVL.SetRestParameters(gameToDo);
            }
            else
            {
                Debug.Log("Wrong game type");
            }
        }
        else
        {
            Debug.Log("REST server not accessible!");
        }
        masterLVL.workFlag = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
