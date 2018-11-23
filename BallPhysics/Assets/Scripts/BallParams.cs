using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Класс, прикрепляемый непосредственно к шарику
/// используется в основном для "продвинутого" уровня,
/// для других уровней эти свойства указаны в самих скриптах уровней
/// (т.к. там использовался всего один шарик).
/// Содержит основные поля/свойства шара:
/// 1) Путь к JSON-файлу 
/// 2) Объект, полученный парсингом из этого файла
/// 3) Кешированная ссылка на Transform шарика (для оптимизации работы приложения)
/// 4) "хвост", логирующий пройденный шариком путь
/// 5) параметр скорости
/// 6) делегат движения шарика
/// </summary>
public class BallParams : MonoBehaviour, IPointerClickHandler
{
    public string pathToJson;
    public BallPath BallPathInfo { get; private set; }
    //по-другому это свойство сложно описать, понятнее 
    public Transform BallTransform { get; private set;}
    public TrailRenderer Trail { get; private set; }
    [HideInInspector]public float speed = 0f;

    public Coroutine ballMover;//храним обобщенную процедуру движения
    //public Coroutine moveNextRoutine;//храним процедуру перехода к конкретной точке

    void Awake()
    {
        BallPathInfo = BallPath.DeserializeFromFile(pathToJson);
        BallTransform = this.transform;

        Vector3Double posDouble = new Vector3Double { x = BallPathInfo.x[0], y=BallPathInfo.y[0], z = BallPathInfo.z[0] };
        BallTransform.position = posDouble.ToVector3(); 
        Trail = GetComponent<TrailRenderer>();

        
    }

    /// <summary>
    /// Реализация метода из IPointerClickHandler
    /// отслеживает клик по шарику для базового уровня
    /// и отслеживает двойной клик для среднего и продвинутого
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount==1)
        {
            Messenger<BallParams>.Broadcast(MessengerEvents.kBallClicked, this);
        }

        if (eventData.clickCount == 2)
        {
            Messenger<BallParams>.Broadcast(MessengerEvents.kDoubleClick, this);
            //Debug.Log("double click");
        }
    }
}