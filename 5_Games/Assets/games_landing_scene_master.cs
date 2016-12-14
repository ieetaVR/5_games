using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class games_landing_scene_master : MonoBehaviour
{

    public text_master bestText;
    public text_master logText;
    public text_master timeText;

    public bool workFlag = false;

    string greet = "Welcome\n";
    string prepare = "You will play\nLift";
    string next_scene = "1_lifting";

    public bool waiting = false;
    public int secsToWait;
    public int secsWaited;
    public float startWaitingTime;
    public int gameState = 0;

    // Use this for initialization
    void Start()
    {

    }

    public void SetRestParameters(JSONObject parameters)
    {
        greet = "Welcome\n" + parameters.GetField("custom_name").str;
        prepare = "You will Play\n" + parameters.GetField("name").str;
        switch((int) parameters.GetField("type").n)
        {
            case 0:
                next_scene = "1_lifting";
                break;
            case 1:
                next_scene = "2_bowl_of_cereal";
                break;
            case 2:
                next_scene = "3_water_plants";
                break;
            case 3:
                next_scene = "4_pinch_puzzle";
                break;
            case 4:
                next_scene = "5_pinch_objects";
                break;
            default:
                break;
        }
        
    }

    void waitFor(int seconds)
    {
        secsToWait = seconds;
        waiting = true;
        startWaitingTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        if (workFlag == true)
        {
            if (waiting)
            {
                float currentTime = Time.time;
                secsWaited = (int)(currentTime - startWaitingTime);
                if (secsWaited >= secsToWait)
                {
                    waiting = false;
                }
            }
            else
            {
                if (gameState == 0)
                {
                    bestText.setCurrentText(greet);
                    gameState = 1;
                    waitFor(4);
                }
                else if(gameState == 1)
                {
                    bestText.setCurrentText(prepare);
                    gameState = 2;
                    waitFor(4);
                }
                else if (gameState == 2)
                {
                    Debug.Log("moviing to next Scene");
                    SceneManager.LoadScene(next_scene);

                }
            }
        }

    }
}
