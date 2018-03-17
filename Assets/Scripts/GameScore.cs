using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScore : MonoBehaviour
{
    public float _score = 0;
    private float _displayedScore = 0;
    private float _lastScore = 0;
    private bool _updated = false;
    [SerializeField] private int _fontSize;
    private float _t;
    float _timeToMove = 2f;

    public float Score
    {
        get { return _score; }
    }

    private static GameObject _scoreGameObject;
    private GameObject _gameObject;
    private Text _scoreObject;

    public void Add(List<Vector2Int> match) // 3: 100 4: 200 5: 400
    {
        int points = match.Count - 3;
        _score += 100 * (int) Math.Pow(2, points);
        _lastScore = _displayedScore;
        _t = 0f;
    }

    private void UpdateScore()
    {
        _scoreObject.text = ((int)_displayedScore).ToString();
    }

    private void Init()
    {
        _gameObject = GameObject.Find("ScoreBoard");
        _scoreGameObject = GameObject.Find("Score");
        _scoreObject = _scoreGameObject.GetComponent<Text>();
    }

    private void Start()
    {
        Init();
        UpdateScore();
    }

    private void FixedUpdate()
    {
        _t += Time.deltaTime/_timeToMove;
        
        if (_displayedScore != _score)
        {
            _displayedScore = Mathf.Lerp(_displayedScore, _score, _t);
            _scoreObject.fontSize = (int) (_fontSize + 10 + Math.Sin(Time.time*10) * 5);
        }
        else if (_scoreObject.fontSize > _fontSize)
        {
            _scoreObject.fontSize--;
        }
        UpdateScore();
    }
}