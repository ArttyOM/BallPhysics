using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// класс, реализующий "базовый уровень" исходной задачи
/// </summary>
public class BaseMovement : MonoBehaviour
{
    private static Transform _ballTransform;

    public float speed = 4f;

    private TrailRenderer _trail;

    public BallPath BallPathInfo { get; private set; }

    private Transform _cameraTransform;

    private int _index = 1;

    private Coroutine _coroutine;

    private void Awake()
    {

        //_json = Resources.Load<TextAsset>("ball_path");
        //Debug.Log(_json.text);

        //BallPathInfo = JsonUtility.FromJson<BallPath>(_json.text);
        BallPathInfo = BallPath.DeserializeFromFile("ball_path");

        //Debug.Log(BallPathInfo.x[1]);

        _ballTransform = GameObject.Find("Ball").transform;

        _trail = _ballTransform.GetComponent<TrailRenderer>();
        //StartCoroutine(BallMover());
        //Messenger<BallParams>.AddListener(MessengerEvents.ballClicked, OnBallClicked);

        _cameraTransform = Camera.main.transform;
        _cameraTransform.SetParent(_ballTransform, false);
    }

    private void OnEnable()
    {
        StartCoroutine(RestartBall());
        _index = 1;
        Messenger<BallParams>.AddListener(MessengerEvents.kBallClicked, OnBallClicked);
    }
    private void OnDisable()
    {
        Messenger<BallParams>.RemoveListener(MessengerEvents.kBallClicked, OnBallClicked);
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

    }

    //private void OnDestroy()
    //{
    //    Messenger<BallParams>.RemoveListener(MessengerEvents.ballClicked, OnBallClicked);
    //}

    #region невостребованный код
    //не шибко стабильно работает, так что переделаем.
    /*
    IEnumerator BallMover()
    {
        Vector3 nextPos;
        Vector3Double nextPosDouble;
        Vector3 point;
        Ray ray;
        RaycastHit hit;
        Transform hitObject;
        for (int i = 0; i < BallPathInfo.x.Length; )//TODO - проверка на кривые данные, когда массивы разной длины
        {
            // райкаст для влияния на шар с помощью клика
            //если кликом попали по шару и он не находился в процессе перехода - начать переход к следующей координате
            //иначе - ждать следующего клика
            
            if (Input.GetMouseButtonDown(0) & !EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("попали");
                point = Input.mousePosition;
                ray = _camera.ScreenPointToRay(point);
                
                if (Physics.Raycast(ray, out hit))
                {
                    hitObject = hit.transform;
                    if ((hitObject!=null)&&(hitObject == _ballTransform))
                    {
                        // Debug.Log("кликнут "+hitObject.tag);
                        //если кликом попали по шару, запускаем процедуру движения
                        //и ждем её окончания 
                        nextPosDouble = new Vector3Double { x = BallPathInfo.x[i], y = BallPathInfo.y[i], z = BallPathInfo.z[i]};
                        nextPos = nextPosDouble.ToVector3(); //new Vector3((float)(BallPathInfo.x[i]), (float)(BallPathInfo.y[i] ), (float)(BallPathInfo.z[i])); 
                        yield return ToNextPos(nextPos);                        
                        //Debug.Log("Дождались конца перемещения до " + i + "-й позиции с координатами: " + nextPos);
                        //Debug.Log (string.Format("Дождались конца перемещения до {0}-й позиции", i));
                        //Debug.Log(string.Format("Координыты {0}-й позиции: [{1},{2},{3}]", i, nextPos.x, nextPos.y, nextPos.z));
                        //Debug.Log("при этом _ballTransform.position.z = " + _ballTransform.position.z);
                        //double t = BallPathInfo.z[i];
                        //double t2 = t * scale;
                        //Debug.Log(string.Format("t={0}, (float)t={1}", t, (float)t));
                        //Debug.Log(string.Format("t*scale={0}, (float)t*scale={1}", t*scale, (float)t*scale));
                        //Debug.Log(string.Format("t2={0}, (float)t2={1}", t2, (float)t2));
                        i++;   
                    }                 
                }
            }
            else yield return null;
        }
        Debug.Log("Дошли до конца");
    }
    */
    #endregion


    private void OnBallClicked(BallParams ballParams)
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(OnBallClickedRoutine(ballParams));
        }
    }

    IEnumerator OnBallClickedRoutine(BallParams ballParams)
    {
        if (ballParams.BallTransform == _ballTransform)
        {
            _index = 1;
            yield return RestartBall();
            //if (_index < BallPathInfo.x.Length) это для одного шага, я вначале неправильно понял задание.
            while (_index < BallPathInfo.x.Length)
            {
                Vector3Double nextPosDouble = new Vector3Double { x = BallPathInfo.x[_index], y = BallPathInfo.y[_index], z = BallPathInfo.z[_index] };
                Vector3 nextPos = nextPosDouble.ToVector3(); //new Vector3((float)(BallPathInfo.x[i]), (float)(BallPathInfo.y[i] ), (float)(BallPathInfo.z[i])); 
                yield return ToNextPos(nextPos);

                _index++;
            }
            if (_index == BallPathInfo.x.Length)
            {
                Debug.Log("Шар прошел весь путь");
                _coroutine = null;
            }
        }
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
            if (speed != 0)
            {
                s += speed * Time.deltaTime;
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
}