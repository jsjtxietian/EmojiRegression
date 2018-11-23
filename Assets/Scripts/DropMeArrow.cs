using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropMeArrow : MonoBehaviour, IDropHandler
{
    public Controller Controller;

    void Start()
    {
        Controller = GameObject.Find("Controller").GetComponent<Controller>();
    }

    public void OnDrop(PointerEventData data)
    {
        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite != null)
        {
            var originalObj = data.pointerDrag;
            int index = Int32.Parse(originalObj.name);
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(originalObj.GetComponent<RectTransform>(),
                data.position, data.pressEventCamera, out globalMousePos))
            {
                Controller.EmojiAxisDatas.Add(new EmojiAxisData(
                    dropSprite, globalMousePos.x, index,GetX(globalMousePos.x)));
                Controller.UpdateAxisView();
            }
        }
    }

    private Sprite GetDropSprite(PointerEventData data)
    {
        var originalObj = data.pointerDrag;

        if (originalObj == null)
            return null;

        var dragMe = originalObj.GetComponent<DragMe>();
        if (dragMe == null)
            return null;

        var srcImage = originalObj.GetComponent<Image>();
        if (srcImage == null)
            return null;

        return srcImage.sprite;
    }


    public float GetX(float worldX)
    {
        var pos = gameObject.transform.position;
        var width = gameObject.GetComponent<RectTransform>().rect.width;
        var minX = pos.x - width / 2;
        var maxX = pos.x + width / 2;

        float x = (worldX - minX) / (maxX - minX);
        return x;
    }

    public float GetWorldX(float x)
    {
        var pos = gameObject.transform.position;
        var width = gameObject.GetComponent<RectTransform>().rect.width;
        var minX = pos.x - width / 2;
        var maxX = pos.x + width / 2;

        float result = x * (maxX - minX) + minX;
        return result;
    }
}

public class EmojiAxisData
{
    public Sprite sprite;
    public int label;
    public float posX;
    public int index;
    public float floatLabel;

    public EmojiAxisData(
        Sprite _sprite, float _x, int _index , float _floatLabel)
    {
        sprite = _sprite;
        posX = _x;
        label = Int32.Parse(sprite.name);
        index = _index;
        floatLabel = _floatLabel;
    }
}