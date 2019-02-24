using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropMEBySprite : MonoBehaviour,IDropHandler
{
    public int CurrentIndex = -1;

    public void OnDrop(PointerEventData data)
    {
        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite != null)
        {
            var originalObj = data.pointerDrag;
            int index = Int32.Parse(originalObj.name);
            CurrentIndex = index;

            gameObject.GetComponent<Image>().sprite = dropSprite;
            gameObject.GetComponent<Image>().color = new Color(1,1,1,1);
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

}
