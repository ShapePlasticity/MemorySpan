using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChoiceHandler : MonoBehaviour
{
    public int ItemIndex { get; set; }
    public List<int> ResponseHandler { get; set; }

    // Use this for initialization
	void Start () {
        Input.simulateMouseWithTouches = true;
    }
	
    void OnMouseDown()
    {
        Debug.Log("Item Chosen: " + ItemIndex);
        ResponseHandler.Add(ItemIndex);  
    }

}
