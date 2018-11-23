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
    public Image OrangeCircle;

    public List<EmojiInfo> DraggableEmojis = new List<EmojiInfo>();

    // Use this for initialization
    void Start ()
    {
        int index = 0;

        foreach (var x in Paths)
        {
            Sprites = Resources.LoadAll<Sprite>(x);
            for(int i = 0 ; i < Sprites.Length ; i++)
            {
                var s = Sprites[i];
                var newObject = Instantiate(EmojiPrefab);
                newObject.transform.GetChild(2).GetComponent<Image>().sprite = s;
                newObject.transform.GetChild(2).gameObject.name = index.ToString();
                newObject.transform.SetParent(EmojiFather);
                DraggableEmojis.Add(new EmojiInfo(newObject,index));
                newObject.GetComponent<EmojiController>().Index = index++;
            }
        }
	}

    public void UpdateLeftArea()
    {
        for (int i = 0; i < DraggableEmojis.Count; i++)
        {
            var info = DraggableEmojis[i];

            var bg = info.Emoji.gameObject.transform.GetChild(0).gameObject;
            var circle = info.Emoji.gameObject.transform.GetChild(1).gameObject;
            var emojiSelf = info.Emoji.gameObject.transform.GetChild(2).gameObject;

            switch (info.State)
            {
                case State.Test:
                    bg.SetActive(true);
                    bg.GetComponent<Image>().sprite = OrangeCircle.sprite;
                    circle.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    emojiSelf.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    emojiSelf.GetComponent<DragMe>().enabled = false;
                    break;
                case State.Train:
                    bg.SetActive(true);
                    circle.GetComponent<Image>().color = new Color(1,1,1,1);
                    emojiSelf.GetComponent<Image>().color = new Color(1,1,1,1);
                    StartCoroutine(EndDrag(emojiSelf));
                    break;
                case State.Unused:
                    bg.SetActive(false);
                    circle.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
                    emojiSelf.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
                    emojiSelf.GetComponent<DragMe>().enabled = true;
                    break;
            }
        }
    }

    private IEnumerator EndDrag(GameObject e)
    {
        yield return new WaitForSeconds(0.1f);
        e.GetComponent<DragMe>().enabled = false;
    }

}

public class EmojiInfo
{
    public GameObject Emoji;
    public int Index;
    public State State;
    public EmojiInfo(GameObject _Emoji , int _Index)
    {
        Emoji = _Emoji;
        Index = _Index;
        State = State.Unused;
    }
}

public enum State
{
    Unused,
    Train,
    Test
}
