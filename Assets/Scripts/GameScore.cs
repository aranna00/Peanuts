using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScore : MonoBehaviour
{
    private int _score;

    public int Score
    {
        get { return _score; }
    }

    private GameObject _scoreGameObject;
    private GameObject _gameObject;

    public void Add(List<Vector2Int> match)  // 3: 100 4: 200 5: 400
    {
        int points = match.Count - 3;
        _score += 100 * (int) Math.Pow(2,points);
        
        UpdateScore();
    }

    private void UpdateScore()
    {
        _scoreGameObject.GetComponent<Text>().text = _score.ToString();
    }

    private void init()
    {
        _gameObject = GameObject.Find("ScoreBoard");
        _scoreGameObject = GameObject.Find("Score");
    }
    
    private void Start()
    {
        init();
        UpdateScore();
    }
}
