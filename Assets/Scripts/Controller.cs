using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public DropMeArrow DropMeArrow;
    public GameObject EmojiAxisPrefabs;
    public EmojiAdder EmojiAdder;
    public List<EmojiAxisData> EmojiAxisDatas = new List<EmojiAxisData>();

    public void UpdateAxisView()
    {
        DestoryAllChild(TestImageFather);

        //destory all axis emoji
        for (int i = 0; i < DropMeArrow.transform.childCount; i++)
        {
            Destroy(DropMeArrow.transform.GetChild(i).gameObject);
        }

        HashSet<int> trainInts = new HashSet<int>();

        //add emoji to axis
        foreach (var data in EmojiAxisDatas)
        {
            GameObject newOne = Instantiate(EmojiAxisPrefabs);
            var childImage = newOne.transform.GetChild(1).gameObject.GetComponent<Image>();
            childImage.sprite = data.sprite;
            newOne.transform.position = new Vector3(data.posX, DropMeArrow.transform.position.y, 0);
            newOne.transform.SetParent(DropMeArrow.transform);
            trainInts.Add(data.index);
        }

        //set rect left state
        for (int i = 0; i < EmojiAdder.DraggableEmojis.Count; i++)
        {
            if (trainInts.Add(i) == false)
            {
                EmojiAdder.DraggableEmojis[i].State = State.Train;
            }
            else
            {
                EmojiAdder.DraggableEmojis[i].State = State.Unused;
            }
        }

        EmojiAdder.UpdateLeftArea();

        TestMask.SetActive(true);
    }

    private void DestoryAllChild(Transform father)
    {
        for (int i = 0; i < father.childCount; i++)
        {
            Destroy(father.GetChild(i).gameObject); 
        }
    }

    #region UpButton

    public void Redo()
    {
        if (EmojiAxisDatas.Count > 0)
        {
            EmojiAxisDatas.RemoveAt(EmojiAxisDatas.Count - 1);
            UpdateAxisView();
        }
    }

    public void Reset()
    {
        EmojiAxisDatas.Clear();
        UpdateAxisView();
    }

    public GameObject TestMask;
    public GameObject TrainingUI;
    public float TrainingTime;

    public void Train()
    {
        if (EmojiAxisDatas.Count >= 2)
        {
            TestMask.SetActive(false);
            TrainingUI.SetActive(true);
            var text = TrainingUI.transform.GetChild(2).gameObject.GetComponent<Text>();
            text.text = "";
            text.DOText("...", TrainingTime);
            StartCoroutine(EndTrain());
        }
    }

    public IEnumerator EndTrain()
    {
        yield return new WaitForSeconds(TrainingTime);
        TrainingUI.SetActive(false);
    }

    #endregion


    #region Test

    public Button TestButton;
    public Image TestImage;
    public GameObject EmojiTest;
    public Transform TestImageFather;

    public void Test()
    {
        if (TestImage.sprite != null && EmojiAxisDatas.Count > 1)
        {
            int TestLabel = Int32.Parse(TestImage.sprite.name);

            GameObject FlyObject = Instantiate(EmojiTest);
            FlyObject.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = TestImage.sprite;
            FlyObject.transform.SetParent(TestImageFather);

            float finalX = CalcTestX(TestLabel);
            FlyObject.transform.position = TestImage.transform.position;
            Vector3 newPos = new Vector3(finalX, 770, 0);
            var handle = FlyObject.transform.DOMove(newPos, TrainingTime);
            handle.onComplete = () =>
            {
                FlyObject.transform.GetChild(0).gameObject.SetActive(true);
                FlyObject.transform.GetChild(2).gameObject.SetActive(true);
                int index = TestImage.gameObject.GetComponent<DropMEBySprite>().CurrentIndex;
                EmojiAdder.DraggableEmojis[index].State = State.Test;
                EmojiAdder.UpdateLeftArea();
            };

            TestImage.color = new Color(1, 1, 1, 0);
        }
    }

    public float CalcTestX(int label)
    {
        double[] arrX = new double[20];
        double[] arrY = new double[20];
        int len = 0;

        foreach (var v in EmojiAxisDatas)
        {
            arrX[len] = v.label;
            arrY[len] = v.floatLabel;
            len++;
        }

        //double[] Para = MultiLine(arrX, arrY, len, len - 1);
        //float result = (float) GetResultFloatLabel(Para, label, len - 1);

        double[] Para = MultiLine(arrX, arrY, len, 2);
        float result = (float) GetResultFloatLabel(Para, label, 2);

        result = DropMeArrow.GetWorldX(result);

        return (float) result;
    }

    private double GetResultFloatLabel(double[] Para, int label, int len)
    {
        double result = 0.0;

        for (; len >= 0; len--)
        {
            result += Para[len] * Math.Pow(label, len);
        }

        if (result > 1.0)
            result = 1.0;
        if (result < 0.0)
            result = 0.0;
        return result;
    }

    #endregion

    #region Gaussion

    public static double[] MultiLine(double[] arrX, double[] arrY, int length, int dimension)
    {
        int n = dimension + 1; //dimension次方程需要求 dimension+1个 系数
        double[,] Guass = new double[n, n + 1]; //高斯矩阵 例如：y=a0+a1*x+a2*x*x
        for (int i = 0; i < n; i++)
        {
            int j;
            for (j = 0; j < n; j++)
            {
                Guass[i, j] = SumArr(arrX, j + i, length);
            }
            Guass[i, j] = SumArr(arrX, i, arrY, 1, length);
        }
        return ComputGauss(Guass, n);
    }

    public static double SumArr(double[] arr, int n, int length) //求数组的元素的n次方的和
    {
        double s = 0;
        for (int i = 0; i < length; i++)
        {
            if (arr[i] != 0 || n != 0)
                s = s + Math.Pow(arr[i], n);
            else
                s = s + 1;
        }
        return s;
    }

    public static double SumArr(double[] arr1, int n1, double[] arr2, int n2, int length)
    {
        double s = 0;
        for (int i = 0; i < length; i++)
        {
            if ((arr1[i] != 0 || n1 != 0) && (arr2[i] != 0 || n2 != 0))
                s = s + Math.Pow(arr1[i], n1) * Math.Pow(arr2[i], n2);
            else
                s = s + 1;
        }
        return s;
    }

    public static double[] ComputGauss(double[,] Guass, int n)
    {
        int i, j;
        int k, m;
        double temp;
        double max;
        double s;
        double[] x = new double[n];
        for (i = 0; i < n; i++) x[i] = 0.0; //初始化

        for (j = 0; j < n; j++)
        {
            max = 0;
            k = j;
            for (i = j; i < n; i++)
            {
                if (Math.Abs(Guass[i, j]) > max)
                {
                    max = Guass[i, j];
                    k = i;
                }
            }


            if (k != j)
            {
                for (m = j; m < n + 1; m++)
                {
                    temp = Guass[j, m];
                    Guass[j, m] = Guass[k, m];
                    Guass[k, m] = temp;
                }
            }
            if (0 == max)
            {
                // "此线性方程为奇异线性方程" 
                return x;
            }

            for (i = j + 1; i < n; i++)
            {
                s = Guass[i, j];
                for (m = j; m < n + 1; m++)
                {
                    Guass[i, m] = Guass[i, m] - Guass[j, m] * s / (Guass[j, j]);
                }
            }
        } //结束for (j=0;j<n;j++)

        for (i = n - 1; i >= 0; i--)
        {
            s = 0;
            for (j = i + 1; j < n; j++)
            {
                s = s + Guass[i, j] * x[j];
            }
            x[i] = (Guass[i, n] - s) / Guass[i, i];
        }
        return x;
    } //返回值是函数的系数

    #endregion
}