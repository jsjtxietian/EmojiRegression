using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiAdder : MonoBehaviour
{
    public List<String> Paths;
    public Sprite[] Sprites;
    public GameObject EmojiPrefab;
    public Transform EmojiFather;

    // Use this for initialization
    void Start () {
        foreach (var x in Paths)
        {
            Sprites = Resources.LoadAll<Sprite>(x);
            foreach (var s in Sprites)
            {
                var newObject = Instantiate(EmojiPrefab);
                EmojiPrefab.transform.GetChild(2).GetComponent<Image>().sprite = s;
                newObject.transform.SetParent(EmojiFather);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
