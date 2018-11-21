using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public Button TrainButton;
    public Button TestButton;
    public DropMe Axis;
    public GameObject TrainingUI;
    public float TrainTime;

    public Image TestImage;

    // Use this for initialization
    void Start()
    {
        TrainButton.onClick.AddListener(TrainMethod);
        TestButton.onClick.AddListener(TestMethod);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TrainMethod()
    {
        TrainingUI.SetActive(true);
        var handler = TrainingUI.transform.GetChild(2).gameObject.GetComponent<Text>()
            .DOText("·············", TrainTime);
        handler.OnComplete((() =>
        {
            TrainingUI.transform.GetChild(3).gameObject.SetActive(true);
            StartCoroutine(DisappearTraining());
        }));
    }

    private IEnumerator DisappearTraining()
    {
        yield return new WaitForSeconds(TrainTime);
        TrainingUI.SetActive(false);
    }

    public void TestMethod()
    {
        GameObject FlyObject = new GameObject("FlyingEmoji");
        var img = FlyObject.AddComponent<Image>();
        img.sprite = TestImage.sprite;

        img.SetNativeSize();
        FlyObject.transform.SetParent(Axis.transform);

        int index = Int32.Parse(img.sprite.name);
        FlyObject.transform.position = TestImage.transform.position;
        Vector3 newPos = new Vector3(GetXByIndex(index), 540, 0);
        FlyObject.transform.DOMove(newPos, TrainTime);
    }

    public float GetXByIndex(int i)
    {
        double[] arrX = new double[20];
        double[] arrY = new double[20];
        int len = 0;

        foreach (var v in Axis.CurrentEmojis)
        {
            arrX[len] = v.Key;
            arrY[len] = v.Value;
            len++;
        }

        double[] Para = MultiLine(arrX, arrY, len, len-1);
        float result =(float) GetResultY(Para, i,len-1);

        result = Axis.GetWorldX(result);

        return (float)result;
    }

    private double GetResultY(double[] Para , int index , int len)
    {
        double result = 0.0;

        for (; len >= 0 ; len--)
        {
            result += Para[len] * Math.Pow(index, len);
        }

        if (result > 1.1)
        {
            result = 1.1;
        }
        if (result < -0.1)
        {
            result = -0.1;
        }

        return result;
    }


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