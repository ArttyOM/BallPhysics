using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Double - ввиду того, что в исходных JSON-строках указаны данные именно этого формата
/// И если для визуализации перемещения объектов с помощью transform.position это не столь важно, т.к. он с float работает,
/// то с точки зрения математических вычислений лучше не терять точность, если такая возможность есть.
/// Как компромиссный вариант - перемещать объект в пространстве Unity с использованием Vector3 (float,float,float),
/// параллельно хранить экземпляр этого объекта
/// </summary>
public struct Vector3Double
{
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }

    public Vector3 ToVector3()
    {
        return new Vector3((float)x, (float)y, (float)z);
    }
}
