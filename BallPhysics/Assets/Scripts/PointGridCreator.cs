using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Вспомогательный класс, создающий "координатную сетку".
/// Используется для визуализации процесса полета шарика.
/// Похоже, довольно тяжелая для рендера штука
/// </summary>
public class PointGridCreator : MonoBehaviour
{
    public Vector3Int repeatRate = new Vector3Int(2,2,2);
    public Vector3Int pointsPerAxis = new Vector3Int(20, 20, 20); //TODO - подпавить, чтобы было полное соответствие (сейчас +-

    public GameObject pointPrefab;
    public GameObject every5PointPrefab;
    public Transform parentTransform;

    public static GameObject CreateInstance(GameObject prefab, Vector3 pos)
    {
        GameObject result = Instantiate(prefab);
        result.transform.localPosition = pos;
        //result.isStatic = true;
        //TextMeshPro tmp = result.GetComponentInChildren<TextMeshPro>();
        //if (tmp)
        //{
        //    Vector3Int intPos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
        //    tmp.text = intPos.ToString();
        //    //tmp.gameObject.SetActive(false);
        //}

        return result;
    }

    private void Awake()
    {
        if (parentTransform == null) parentTransform = new GameObject("CylinderHolder").transform;
        GameObject tmp;
        for (int xi = -pointsPerAxis.x/2; xi < pointsPerAxis.x/2; xi+= repeatRate.x)
            for (int yi = -pointsPerAxis.y / 2; yi < pointsPerAxis.y / 2; yi += repeatRate.y)
                for (int zi = -pointsPerAxis.z / 2; zi < pointsPerAxis.z/2; zi+= repeatRate.z)
                {
                    if ((xi % 5 == 0) && (yi % 5 == 0) && (zi % 5 == 0))
                    {
                        tmp = CreateInstance(every5PointPrefab, new Vector3(xi, yi, zi));
                    }
                    else
                    {
                        tmp = CreateInstance(pointPrefab, new Vector3(xi, yi, zi));
                    }
                    tmp.transform.SetParent(parentTransform);
                }
    }
}
