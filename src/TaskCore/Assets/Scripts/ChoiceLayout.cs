using UnityEngine;
using System.Collections;

public class ChoiceLayout : MonoBehaviour {

	// Use this for initialization
	void Start () {

        var width = Camera.main.orthographicSize;

	}
	
	// Update is called once per frame
	void Update () {

        var width = Camera.main.orthographicSize * 2.0 * Screen.width / Screen.height;

        //transform.localScale = new Vector3((float)width * 0.9f, (float)width * 0.9f, (float)width * 0.9f);
    }
}
