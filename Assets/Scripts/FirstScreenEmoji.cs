using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstScreenEmoji : MonoBehaviour
{
    public List<String> Paths;
    public List<Transform> Transforms;
    public Sprite[] Sprites;
    public GameObject EmojiPrefab;

    public float[] FirstCoor;

    // Use this for initialization
    void Start()
    {
        int index = 0;

        foreach (var x in Paths)
        {
            Sprites = Resources.LoadAll<Sprite>(x);
            for (int i = 0; i < Sprites.Length; i++)
            {
                var s = Sprites[i];
                var newObject = Instantiate(EmojiPrefab);
                newObject.GetComponent<Image>().sprite = s;
                newObject.transform.SetParent(Transforms[0]);
                newObject.transform.localScale= new Vector3(1,1,1);

                var name = s.name;
                string[] coors = name.Split(' ');
                float xc = float.Parse(coors[0]); 
                float yc = float.Parse(coors[1]);
                newObject.transform.localPosition = 
                    new Vector3(
                        FirstCoor[0] + FirstCoor[2] * xc,
                        FirstCoor[1] + FirstCoor[3] * (yc-10),
                        0);
            }   
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
