using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToMove : MonoBehaviour, IPointerClickHandler
{
    public SingleEmojiInfo Self;
    public Transform EmojiFather;
    public float[] Coors;

    public TwoController Controller;

    void Start()
    {
        Controller = GameObject.Find("Controller").GetComponent<TwoController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Controller.ResetTestState();

        gameObject.transform.SetParent(EmojiFather);
        float xP = Coors[0] + Coors[2] * Self.x;
        float yP = Coors[1] + Coors[3] * (Self.y - 10);
       
        gameObject.transform.DOLocalMove(new Vector3(xP, yP, 0), 1f);
        gameObject.transform.localScale = new Vector3(1f,1f,1f);

        Controller.OnEmojiTest(Self.x,Self.y);
    }
}
