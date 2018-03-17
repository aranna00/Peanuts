using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScore : MonoBehaviour
{
    private int _score = 0;
    private int _displayedScore = 0;
    private bool _updated = false;
    [SerializeField] private int _incrementRate;
    [SerializeField] private int _fontSize;

    public float Score
    {
        get { return _score; }
    }

    private static GameObject _scoreGameObject;
    private GameObject _gameObject;
    private Text _scoreObject;

    public void Add(List<Vector2Int> match,float multiplier) // 3: 100 4: 200 5: 400
    {
        Debug.Log(multiplier);
        int points = match.Count - 3;
        int addedScore = (int)(100f * Math.Pow(2, points)*multiplier);
        Debug.Log(addedScore);
        _score += addedScore;
    }

    private void UpdateScore()
    {
        _scoreObject.text = _displayedScore.ToString();
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
        if (_displayedScore != _score)
        {
            if (_displayedScore < _score)
            {
                _displayedScore += _incrementRate;
                _scoreObject.fontSize = _fontSize + 10;
            }
            else
            {
                _displayedScore -= _incrementRate;
            }
        }

        if (_scoreObject.fontSize > _fontSize)
        {
            _scoreObject.fontSize--;
        }
        UpdateScore();
    }

    public void AddEnd()
    {
        _score += 400;
    }
}