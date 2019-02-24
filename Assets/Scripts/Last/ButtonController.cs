using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Controller Controller;

	// Use this for initialization
	void Start ()
	{
	    var undo = transform.GetChild(0).gameObject.GetComponent<Button>();
	    var reset = transform.GetChild(1).gameObject.GetComponent<Button>();
	    var train = transform.GetChild(2).gameObject.GetComponent<Button>();

        undo.onClick.AddListener(Controller.Redo);
        reset.onClick.AddListener(Controller.Reset);
        train.onClick.AddListener(Controller.Train);
    }
	
}
