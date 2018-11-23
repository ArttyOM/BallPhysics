using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// класс, реализующий продвинутый уровень
/// </summary>
public class AdvancedMovement : MonoBehaviour
{
    public static List<BallParams> balls = new List<BallParams>();
    public static BallParams currentBall;
    private static int _currentBallID = 0;

    public Transform ballsPivot;

    public Slider slider;
    public Text textSpeed;

    public Text textCurrentBall;

    private float _maxSpeed = 12f;

    //private Camera _camera;
    private Transform _cameraTransform;
    //private posIndex = 0;//начальная позиция шара;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        //Messenger<BallParams>.AddListener(MessengerEvents.doubleClick, ToBegin);

        slider.maxValue = _maxSpeed;

        foreach (BallParams children in ballsPivot.GetComponentsInChildren<BallParams>())
        {
            balls.Add(children);
            children.ballMover = StartCoroutine(BallMover(children));
        }

        //Transform tmp;
        //BallParams tmpBP;
        //for (int i = 0; i < ballsPivot.childCount; i++)
        //{
        //    tmp = ballsPivot.GetChild(i);
        //    tmpBP = tmp.GetComponent<BallParams>();
        //    if (tmpBP != null)
        //    {
        //        balls.Add(tmpBP);
        //        tmpMoverRoutine = StartCoroutine(BallMover(tmpBP));
        //        tmpBP.ballMover = tmpMoverRoutine;
        //    }
        //}

        currentBall = balls[0];
        //_cameraTransform.SetParent(currentBall.transform, false);

        slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
        Messenger<BallParams>.AddListener(MessengerEvents.kDoubleClick, ToBegin);
    }

    private void OnDestroy()
    {
        Messenger<BallParams>.RemoveListener(MessengerEvents.kDoubleClick, ToBegin);
    }

    private void ToBegin(BallParams ballParams)
    {
        StopCoroutine(ballParams.ballMover);

        StartCoroutine(RestartBall(ballParams));
    }

    IEnumerator RestartBall(BallParams ballParams)
    {
        if (ballParams.BallPathInfo.x == null) Debug.Log("не найден маршрут");
        Vector3Double vector3Double = new Vector3Double { x = ballParams.BallPathInfo.x[0], y = ballParams.BallPathInfo.y[0], z = ballParams.BallPathInfo.z[0] };
        ballParams.Trail.Clear();
        ballParams.Trail.emitting = false;//иначе при прыжке на начальную позицию это хвост отрисует
        yield return null;
        //Debug.Log(string.Format("перемещаем шар с маршрутом {0} в точку {1}", ballParams.pathToJSON, vector3Double.ToVector3().ToString()));
        ballParams.BallTransform.position = vector3Double.ToVector3();//перемещаем в начальную точку
        yield return null; //пропуск кадра чтобы объекты успели устаканиться 
        //Debug.Log(vector3Double.ToVector3());

        ballParams.Trail.Clear(); ;//чистим "хвост"
        ballParams.Trail.emitting = true; //возвращаем как было

        ballParams.ballMover= StartCoroutine(BallMover(ballParams));//запускаем по-новой
    }

    IEnumerator BallMover(BallParams ball)
    {
        Vector3 nextPos;
        Vector3Double nextPosDouble;

        yield return null; //придется пропустить кадр, потому что иначе ball.BallPathInfo еще не считан с файла
        //Debug.Log(ball.BallPathInfo.x.Length);
        for (int i = 1; i < ball.BallPathInfo.x.Length; i++)//TODO - проверка на кривые данные, когда массивы разной длины
        {
            nextPosDouble = new Vector3Double { x = ball.BallPathInfo.x[i], y = ball.BallPathInfo.y[i], z = ball.BallPathInfo.z[i] };
            nextPos = nextPosDouble.ToVector3(); //new Vector3((float)(BallPathInfo.x[i]), (float)(BallPathInfo.y[i] ), (float)(BallPathInfo.z[i]));
            yield return ToNextPos(ball, nextPos);
        }
        Debug.Log(ball.pathToJson + " путь пройден до конца");
    }

    IEnumerator ToNextPos(BallParams ball, Vector3 pos)
    {
        Vector3 difference = pos - ball.BallTransform.position; //разность векторов. Позволяет найти направление движения и длину пути
        Vector3 startPosition = ball.BallTransform.position;

        float actualDistance = difference.magnitude;//нужно для слабых устройств, у которых за один кадр у шарика может произойти "перелет"

        float s = 0f;//пройденный путь (с момента вызова метода)

        Vector3 direction = difference.normalized;// (pos - ball.BallTransform.position).normalized;
        do
        {
            if (ball.speed != 0)
            {
                s += ball.speed * Time.deltaTime;
                ball.BallTransform.position = startPosition + direction * s;
                //Debug.Log(string.Format("Пройдено: {0}/{1}" , s, actualDistance));
            }
            yield return null;
        }
        while (actualDistance> s);
        //Debug.Log(string.Format(
        //    "Получили: [{0},{1},{2}]",
        //    ball.BallTransform.position.x, ball.BallTransform.localPosition.y, ball.BallTransform.localPosition.z));
        //Debug.Log(string.Format("Хотели: [{0},{1},{2}]", pos.x, pos.y, pos.z));
        ball.BallTransform.position = pos;
        //чуть корректируем положение
    }

    public void OnSliderValueChanged()
    {
        currentBall.speed = slider.value;
        textSpeed.text = "Speed: " + currentBall.speed.ToString("F");
    }

    public void OnBtnNext()
    {
        _currentBallID++;
        if (_currentBallID == balls.Count) _currentBallID = 0;
        textCurrentBall.text = "Current ball:" + _currentBallID.ToString();

        currentBall = balls[_currentBallID];

        slider.value =currentBall.speed ;
        textSpeed.text = "Speed: " + currentBall.speed.ToString("F");

        _cameraTransform.SetParent(currentBall.BallTransform, false);
        Messenger<Transform>.Broadcast(MessengerEvents.kCameraOwnerChanged, currentBall.BallTransform);
    }
    public void OnBtnPrevious()
    {
        _currentBallID--;
        if (_currentBallID < 0) _currentBallID = balls.Count - 1;

        textCurrentBall.text = "Current ball:" + _currentBallID.ToString();

       currentBall = balls[_currentBallID];

        slider.value =currentBall.speed;
        textSpeed.text = "Speed: " + currentBall.speed.ToString("F");

        _cameraTransform.SetParent(currentBall.BallTransform, false);
        Messenger<Transform>.Broadcast(MessengerEvents.kCameraOwnerChanged, currentBall.BallTransform);
    }
}