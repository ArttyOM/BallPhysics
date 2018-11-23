using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Структура, описывающая возможное положение шарика.
/// Нужна для считывания данных с Json-файла
/// </summary>
[Serializable]
public struct BallPath
{
    public double[] x;
    public double[] y;
    public double[] z;

    public static BallPath DeserializeFromFile(string path)
    {
        TextAsset json = Resources.Load<TextAsset>(path);
        return JsonUtility.FromJson<BallPath>(json.text);
    }
}
