using UnityEngine;
using System.Collections;

public class leap_enjalbert_score_keeper : MonoBehaviour
{


    public Level_master masterLVL;
    public enjalbert_rest_client restClient;
    public enjalbert_time_keeper timer;

    public leap_enjalbert_lift_arm lift_arm_script;
    public grab grab_script_left;
    public grab grab_script_right;
    public pinch_counter pinchCounter;

    public bool canGotoNextScene = true;
    bool waitingForResponse = false;

    static JSONObject scores;// = new JSONObject("{\"lvl1\":{\"success\":0,\"time_taken\":10,\"total_time\":60,\"hold_time\":4},\"lvl2\":{\"success\":0,\"time_taken\":10,\"total_time\":60,\"hold_time\":4},\"lvl3\":{\"success\":0,\"time_taken\":10,\"total_time\":60,\"iterations\":4,\"total_iterations\":4,\"margin\":0.1,\"hold_time\":3},\"lvl4\":{\"success\":0,\"time_taken\":10,\"total_time\":60,\"index_counts\":2,\"total_index_counts\":2,\"middle_counts\":2,\"total_middle_counts\":2},\"lvl5\":{\"success\":0,\"time_taken\":10,\"total_time\":60,\"ring_counts\":2,\"total_ring_counts\":2,\"pinky_counts\":2,\"total_pinky_counts\":2}}");

    // Use this for initialization
    void Start()
    {
        if (masterLVL.currentLevel == 0)
        {
           // scores = new JSONObject("{\"lvl1\":{\"success\":0,\"time_taken\":10,\"total_time\":60,\"hold_time\":4},\"lvl2\":{\"success\":0,\"time_taken\":10,\"total_time\":60,\"hold_time\":4},\"lvl3\":{\"success\":0,\"time_taken\":10,\"total_time\":60,\"iterations\":4,\"total_iterations\":4,\"margin\":0.1,\"hold_time\":3},\"lvl4\":{\"success\":0,\"time_taken\":10,\"total_time\":60,\"index_counts\":2,\"total_index_counts\":2,\"middle_counts\":2,\"total_middle_counts\":2},\"lvl5\":{\"success\":0,\"time_taken\":10,\"total_time\":60,\"ring_counts\":2,\"total_ring_counts\":2,\"pinky_counts\":2,\"total_pinky_counts\":2}}");
            scores = new JSONObject("{}");
        }
    }

    public void collectLevelInfo()
    {
        scores.AddField("lvl" + masterLVL.currentLevel, new JSONObject("{}"));

        if (masterLVL.finalStage)
        {
            scores.GetField("lvl" + masterLVL.currentLevel).AddField("success", 1);
            //Debug.Log("current: " + scores.Print());
        }
        else
        {
            scores.GetField("lvl" + masterLVL.currentLevel).AddField("success", 0);
            //Debug.Log("current errp«or: " + scores.Print());
        }
        //Debug.Log("secs: " + timer.secsPassed);
        scores.GetField("lvl" + masterLVL.currentLevel).AddField("time_taken", timer.secsPassed);
        scores.GetField("lvl" + masterLVL.currentLevel).AddField("total_time", timer.secsToWait);

        switch (masterLVL.currentLevel)
        {
            case 1:
                scores.GetField("lvl1").AddField("hold_time", lift_arm_script.secondsToCount);
                break;
            case 2:
                scores.GetField("lvl2").AddField("hold_time", lift_arm_script.secondsToCount);
                break;
            case 3:
                if (grab_script_left.workflag == 1)
                {
                    scores.GetField("lvl3").AddField("iterations", grab_script_left.iterations);
                    scores.GetField("lvl3").AddField("total_iterations", grab_script_left.TotalIterations);
                    scores.GetField("lvl3").AddField("margin", grab_script_left.margin);
                    scores.GetField("lvl3").AddField("total_iterations", grab_script_left.secsToWait);
                }
                else
                {
                    scores.GetField("lvl3").AddField("iterations", grab_script_right.iterations);
                    scores.GetField("lvl3").AddField("total_iterations", grab_script_right.TotalIterations);
                    scores.GetField("lvl3").AddField("margin", grab_script_right.margin);
                    scores.GetField("lvl3").AddField("total_iterations", grab_script_right.secsToWait);
                }
                break;
            case 4:
                scores.GetField("lvl4").AddField("index_counts", pinchCounter.currentCounts[0]);
                scores.GetField("lvl4").AddField("total_index_counts", pinchCounter.totalCounts[0]);
                scores.GetField("lvl4").AddField("middle_counts", pinchCounter.currentCounts[1]);
                scores.GetField("lvl4").AddField("total_middle_counts", pinchCounter.totalCounts[1]);
                break;
            case 5:
                scores.GetField("lvl5").AddField("ring_counts", pinchCounter.currentCounts[2]);
                scores.GetField("lvl5").AddField("total_ring_counts", pinchCounter.totalCounts[2]);
                scores.GetField("lvl5").AddField("pinky_counts", pinchCounter.currentCounts[3]);
                scores.GetField("lvl5").AddField("total_pinky_counts", pinchCounter.totalCounts[3]);
                break;

            default:
                break;
        }
    }

    public void sendToServer(bool itsTheEnd)
    {
        Debug.Log("sending to server: " + scores.Print());
        restClient.sendTestResults(scores);
        canGotoNextScene = false;
        if (itsTheEnd)
        {
            waitingForResponse = true;
        }

    }


    // Update is called once per frame
    void Update()
    {
        if(waitingForResponse)
        {
            if (restClient.sentToServer)
            {
                canGotoNextScene = true;
            }
        }
    }
}
