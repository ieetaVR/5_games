using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

[Serializable]

public class scene_jumper : MonoBehaviour {
    
    public string nextSceneName;
    

    // Use this for initialization
    void Start () {
	
	}

    void OnMouseDown()
    {
        // this object was clicked - do something
        //Destroy(this.gameObject);
        SceneManager.LoadScene(nextSceneName);

    }

    // Update is called once per frame
    void Update () {
	
	}
}
