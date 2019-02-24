using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeClusterName : MonoBehaviour
{
    public TwoController Controller;
    public int ClusterIndex;
    void Start()
    {
        Controller = GameObject.Find("Controller").GetComponent<TwoController>();
        var input = transform.GetChild(0).GetComponent<InputField>();
        input.onEndEdit.AddListener(delegate
        {
            Controller.RenameCluster(ClusterIndex,input.text);
        });
    }

   
}
