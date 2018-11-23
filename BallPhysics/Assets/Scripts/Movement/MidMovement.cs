using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// класс, реализующий средний уровень
/// </summary>
public class MidMovement : MonoBehaviour
{

    public Slider slider;
    public Text sliderText;

    public BallPath BallPathInfo { get; private set; }


    private static Transform _ballTransform;
    private float _maxSpeed = 12f;
    private float _speed = 0f;
    private TrailRenderer _trail;

    private Transform _cameraTransform;
    //private posIndex = 0;//начальная позиция шара;

    private void Awake()
    {
        //Messenger<BallParams>.AddListener(MessengerEvents.doubleClick, ToBegin); 

        slider.maxValue = _maxSpeed;

        BallPathInfo = BallPath.DeserializeFromFile("ball_path");
        _ballTransform = GameObject.Find("Ball").transform;
        _trail = _ballTransform.GetComponent<TrailRenderer>();
        StartCoroutine(BallMover());

        _cameraTransform = Camera.main.transform;
        _cameraTransform.SetParent(_ballTransform, false);

        slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });

        _speed = 0f;
        //ToBegin();
        Messenger<BallParams>.AddListener(MessengerEvents.kDoubleClick, ToBegin);
    }



    private void OnDisable()
    {
        Messenger<BallParams>.RemoveListener(MessengerEvents.kDoubleClick, ToBegin);
        slider.onValueChanged.RemoveAllListeners();
    }

    //private void OnDestroy()
    //{
    //    Messenger<BallParams>.RemoveListener(MessengerEvents.doubleClick, ToBegin);
    //}

    private void ToBegin()
    {
        StopAllCoroutines();

        StartCoroutine(RestartBall());
    }
    private void ToBegin(BallParams ballParams)
    {
        //StopCoroutine(BallMover());//брякнули всё движение
        //StopCoroutine("ToNextPos");
        StopAllCoroutines();

        StartCoroutine(RestartBall());

    }

    IEnumerator RestartBall()
    {
        Vector3Double vector3Double = new Vector3Double { x = BallPathInfo.x[0], y = BallPathInfo.y[0], z = BallPathInfo.z[0] };
        _trail.Clear();
        _trail.emitting = false;//иначе при прыжке на начальную позицию это хвост отрисует
        yield return null;
        _ballTransform.position = vector3Double.ToVector3();//перемещаем в начальную точку
        yield return null; //пропуск кадра чтобы объекты успели устаканиться 
        //Debug.Log(vector3Double.ToVector3());
        _trail.Clear();//чистим "хвост"
        _trail.emitting = true; //возвращаем как было
        StartCoroutine(BallMover());//запускаем по-новой
    }

    IEnumerator BallMover()
    {
        Vector3 nextPos;
        Vector3Double nextPosDouble;

        for (int i = 1; i < BallPathInfo.x.Length; i++)//TODO - проверка на кривые данные, когда массивы разной длины
        {
            nextPosDouble = new Vector3Double { x = BallPathInfo.x[i], y = BallPathInfo.y[i], z = BallPathInfo.z[i] };
            nextPos = nextPosDouble.ToVector3(); //new Vector3((float)(BallPathInfo.x[i]), (float)(BallPathInfo.y[i] ), (float)(BallPathInfo.z[i]));
            yield return ToNextPos(nextPos);
        }
        Debug.Log("Дошли до конца");
    }

    IEnumerator ToNextPos(Vector3 pos)
    {
        Vector3 difference = pos - _ballTransform.position; //разность векторов. Позволяет найти направление движения и длину пути
        Vector3 startPosition = _ballTransform.position; 

        float actualDistance = difference.magnitude;//нужно для слабых устройств, у которых за один кадр у шарика может произойти "перелет"

        float s = 0f;//пройденный путь (с момента вызова метода)

        Vector3 direction = difference.normalized;// (pos - ball.BallTransform.position).normalized;
        do
        {
            if (_speed != 0)
            {
                s += _speed * Time.deltaTime;
                _ballTransform.position = startPosition + direction * s;
                //Debug.Log(string.Format("Пройдено: {0}/{1}" , s, actualDistance));
            }
            yield return null;
        }
        while (actualDistance > s);
        //Debug.Log(string.Format(
        //    "Получили: [{0},{1},{2}]",
        //    ball.BallTransform.position.x, ball.BallTransform.localPosition.y, ball.BallTransform.localPosition.z));
        //Debug.Log(string.Format("Хотели: [{0},{1},{2}]", pos.x, pos.y, pos.z));
        _ballTransform.position = pos;
        //чуть корректируем положение
    }

    public void OnSliderValueChanged()
    {
        _speed = slider.value;
        sliderText.text = "Speed: "+_speed.ToString("F");
    }


}
