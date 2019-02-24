using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoController : MonoBehaviour
{
    //Config Info
    public List<String> Paths;

    public List<Transform> Transforms;
    public Sprite[] Sprites;
    public GameObject EmojiPrefab;
    public GameObject SelectedMask;

    public float[] FirstCoor;

    //self Info
    private List<SingleEmojiInfo> EmojiInfos = new List<SingleEmojiInfo>();

    // Use this for initialization
    void Start()
    {
        //add First Screen Emoji
        Sprites = Resources.LoadAll<Sprite>(Paths[0]);
        for (int i = 0; i < Sprites.Length; i++)
        {
            var s = Sprites[i];
            var newObject = Instantiate(EmojiPrefab);
            newObject.GetComponent<Image>().sprite = s;
            newObject.transform.SetParent(Transforms[0]);
            newObject.transform.localScale = new Vector3(1, 1, 1);

            var name = s.name;
            string[] coors = name.Split(' ');
            float xc = float.Parse(coors[0]);
            float yc = float.Parse(coors[1]);

            EmojiInfos.Add(new SingleEmojiInfo(xc, yc));

            newObject.transform.localPosition =
                new Vector3(
                    FirstCoor[0] + FirstCoor[2] * xc,
                    FirstCoor[1] + FirstCoor[3] * (yc - 10),
                    0);
        }
    }

    private bool drawRectangle;
    private Vector3 start;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            drawRectangle = true; //如果鼠标左键按下 设置开始画线标志
            start = Input.mousePosition; //记录按下位置
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //todo 加入判断条件
            drawRectangle = false; //如果鼠标左键放开 结束画线
            Vector3 end = Input.mousePosition;

            var newMask = Instantiate(SelectedMask);
            newMask.transform.SetParent(Transforms[1]);

            float height = Math.Abs(end.y - start.y);
            float width = Math.Abs(end.x - start.x);
            Vector2 pos = new Vector2(
                start.x + (end.x - start.x) / 2,
                start.y + (end.y - start.y) / 2);

            newMask.transform.position = new Vector3(pos.x, pos.y, 0);
            newMask.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
    }

    void OnGUI()
    {
        //画线这种操作推荐在OnPostRender（）里进行 而不是直接放在Update，所以需要标志来开启
        if (drawRectangle)
        {
            Vector3 end = Input.mousePosition; //鼠标当前位置
            GL.PushMatrix(); //保存摄像机变换矩阵
            GL.LoadPixelMatrix(); //设置用屏幕坐标绘图

            GL.Begin(GL.QUADS);
            GL.Color(new Color(255, 1, 1, 0.3f)); //设置颜色和透明度，方框内部透明
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.Vertex3(start.x, end.y, 0);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Color(new Color(255, 255, 255, 1f)); //设置方框的边框颜色 边框不透明
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, start.y, 0);
            GL.Vertex3(end.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.Vertex3(start.x, end.y, 0);
            GL.Vertex3(start.x, end.y, 0);
            GL.Vertex3(start.x, start.y, 0);
            GL.End();

            GL.PopMatrix(); //恢复摄像机投影矩阵
        }
    }
}

public struct SingleEmojiInfo
{
    public float x;
    public float y;

    public SingleEmojiInfo(float _x, float _y)
    {
        x = _x;
        y = _y;
    }
}