using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// класс, отвечающий за переключение между
/// "уровнями" исходной задачи.
/// </summary>
public class LevelSwicher : MonoBehaviour
{
    [SerializeField] private Dropdown _dropddown;
    //[SerializeField] private Camera _camera;//камера-заглушка, чтобы при переключении сцен не видеть всяких ужасов
    private static int _currentSceneID = (int)MessengerEvents.Level.Base;

    public void OnDropdownOptionChanged()
    {
        LoadLevel((MessengerEvents.Level)(_dropddown.value+1));
        //Messenger<MessengerEvents.Level>.Broadcast(MessengerEvents.levelChanged, (MessengerEvents.Level)_dropddown.value);

    }

    private void Awake()
    {
        // DontDestroyOnLoad(this.gameObject);
        
        SceneManager.LoadScene((int)MessengerEvents.Level.Base, LoadSceneMode.Additive);
    }

    private void LoadLevel(MessengerEvents.Level level)
    {
        if (_currentSceneID != (int)level)
        {
            int prevID = _currentSceneID;
            
            _currentSceneID = (int)level;
            SceneManager.UnloadSceneAsync(prevID);
            SceneManager.LoadSceneAsync(_currentSceneID, LoadSceneMode.Additive);
        }
    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
