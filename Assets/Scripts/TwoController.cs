using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TwoController : MonoBehaviour
{
    //Config Info
    public List<String> Paths;

    public RectTransform BoardBG;
    public List<Transform> Transforms;
    public Sprite[] Sprites;
    public GameObject EmojiPrefab;
    public GameObject TestEmojiPrefab;
    public GameObject SelectedMask;

    public float[] FirstCoor;

    public Transform TestEmojiFather;

    //self Info
    private List<SingleEmojiInfo> EmojiInfos = new List<SingleEmojiInfo>();

    private List<ClusterInfo> Clusters = new List<ClusterInfo>();
    private List<GameObject> Emojis = new List<GameObject>();
    private List<GameObject> Masks = new List<GameObject>();
    private List<GameObject> TestEmojis = new List<GameObject>();

    private List<int> NearestEmoji = new List<int>();

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

            //relative  pos
            float xP = FirstCoor[0] + FirstCoor[2] * xc;
            float yP = FirstCoor[1] + FirstCoor[3] * (yc - 10);

            newObject.transform.localPosition =
                new Vector3(xP, yP, 0);

            xP = newObject.transform.position.x;
            yP = newObject.transform.position.y;
            EmojiInfos.Add(new SingleEmojiInfo(i, xc, yc, xP, yP));
            Emojis.Add(newObject);
        }

        //add test emoji
        Sprites = Resources.LoadAll<Sprite>(Paths[1]);
        for (int i = 0; i < Sprites.Length; i++)
        {
            var s = Sprites[i];
            var newObject = Instantiate(TestEmojiPrefab);
            newObject.GetComponent<Image>().sprite = s;
            newObject.transform.SetParent(TestEmojiFather);
            newObject.transform.localScale = new Vector3(1, 1, 1);

            var name = s.name;
            string[] coors = name.Split(' ');
            float xc = float.Parse(coors[0]);
            float yc = float.Parse(coors[1]);

            newObject.GetComponent<ClickToMove>().Coors = FirstCoor;
            newObject.GetComponent<ClickToMove>().EmojiFather = Transforms[0];
            newObject.GetComponent<ClickToMove>().Self = new SingleEmojiInfo(i, xc, yc, 0, 0);

            TestEmojis.Add(newObject);
        }
    }

    private bool drawRectangle = false;
    private Vector3 start;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (WithinBG(Input.mousePosition))
            {
                drawRectangle = true;
                start = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (drawRectangle)
            {
                drawRectangle = false;
                Vector3 end = Input.mousePosition;

                int emojiCount = CountInBox(start, end);

                if (emojiCount > 0)
                {
                    var newMask = Instantiate(SelectedMask);
                    newMask.transform.SetParent(Transforms[1]);

                    float height = Math.Abs(end.y - start.y);
                    float width = Math.Abs(end.x - start.x);
                    Vector2 pos = new Vector2(
                        start.x + (end.x - start.x) / 2,
                        start.y + (end.y - start.y) / 2);

                    newMask.transform.position = new Vector3(pos.x, pos.y, 0);
                    newMask.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
                    newMask.GetComponent<ChangeClusterName>().ClusterIndex = Clusters.Count - 1;
                    Masks.Add(newMask);
                }
            }
        }
    }

    public void ResetTestState()
    {
        foreach (var index in NearestEmoji)
        {
            Emojis[index].transform.localScale = new Vector3(1, 1, 1);
        }

        NearestEmoji.Clear();
    }

    public void OnEmojiTest(int index ,float x, float y)
    {
        List<Distance> distances = new List<Distance>();
        for (int i = 0; i < EmojiInfos.Count; i++)
        {
            distances.Add(new Distance(
                (float) Math.Pow(x - EmojiInfos[i].x, 2) + (float) Math.Pow(y - EmojiInfos[i].y, 2), i));
        }

        distances = distances.OrderBy(e => e.distance).ToList();
        Dictionary<int, int> Belongs = new Dictionary<int, int>();

        for (int i = 0; i < 3; i++)
        {
            NearestEmoji.Add(distances[i].index);
            Emojis[NearestEmoji[i]].transform.localScale = new Vector3(1.5f, 1.5f, 1);
            Belongs[NearestEmoji[i]] = EmojiInfos[NearestEmoji[i]].belong;
        }

        //calculate belongs to which one
        //todo
        int belong = Belongs.OrderByDescending(e => e.Value).FirstOrDefault().Value;
        if (belong != -1)
        {
            TestEmojis[index].transform.GetChild(0).gameObject.SetActive(true);
            TestEmojis[index].GetComponentInChildren<Text>().text = Clusters[belong].Name;
        }
    }

    public void RenameCluster(int index, string name)
    {
        Clusters[index].ChangeName(name);
    }

    private int CountInBox(Vector3 start, Vector3 end)
    {
        int count = 0;
        Clusters.Add(new ClusterInfo("temp"));

        foreach (var e in EmojiInfos)
        {
            if (e.xCoor > start.x && e.xCoor < end.x
                && e.yCoor < start.y && e.yCoor > end.y)
            {
                count++;
                e.belong = Clusters.Count - 1;
                Clusters[Clusters.Count - 1].Contains.Add(e);
            }
        }

        if (count == 0)
        {
            Clusters.RemoveAt(Clusters.Count - 1);
        }
        return count;
    }

    private bool WithinBG(Vector2 pos)
    {
        var bgPos = BoardBG.position;
        var bgRect = BoardBG.rect;
        var scale = BoardBG.localScale.x;
        //Debug.Log(pos);
        //Debug.Log(bgPos);
        //Debug.Log(bgRect);

        return (pos.x > bgPos.x - bgRect.width * scale / 2)
               && (pos.x < bgPos.x + bgRect.width * scale / 2)
               && (pos.y > bgPos.y - bgRect.height * scale / 2)
               && (pos.y < bgPos.x + bgRect.height * scale / 2);
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

public class SingleEmojiInfo
{
    public int index;
    public float x;
    public float y;

    public float xCoor;
    public float yCoor;

    public int belong;

    public SingleEmojiInfo(int i, float _x, float _y, float _xC, float _yC)
    {
        index = i;
        x = _x;
        y = _y;
        xCoor = _xC;
        yCoor = _yC;
        belong = -1;
    }

    public void Reset()
    {
        belong = -1;
    }
}

public class ClusterInfo
{
    public List<SingleEmojiInfo> Contains;
    public string Name;

    public ClusterInfo(string name)
    {
        Name = name;
        Contains = new List<SingleEmojiInfo>();
    }

    public void ChangeName(string name)
    {
        Name = name;
    }
}

public struct Distance
{
    public float distance;
    public int index;

    public Distance(float _d, int _i)
    {
        distance = _d;
        index = _i;
    }
}