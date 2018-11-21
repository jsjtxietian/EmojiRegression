using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropMe : MonoBehaviour, IDropHandler
{
    public Dictionary<int,float> CurrentEmojis = new Dictionary<int, float>();

    public void OnDrop(PointerEventData data)
    {
        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite != null)
        {
            var originalObj = data.pointerDrag;

            GameObject newOne = new GameObject("EmojiDropped");
            var img = newOne.AddComponent<Image>();
            img.sprite = dropSprite;
            img.SetNativeSize();
            newOne.transform.SetParent(gameObject.transform);

            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(originalObj.GetComponent<RectTransform>(),
                data.position, data.pressEventCamera, out globalMousePos))
            {
                CurrentEmojis.Add(Int32.Parse(img.sprite.name),GetX(globalMousePos.x));
                newOne.transform.position = new Vector3(globalMousePos.x, 540, 0);
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

        float x = (worldX - minX) / (maxX - minX) ;
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