using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameScore : MonoBehaviour
{
    private float _score;
    private float _displayedScore;
    private bool _updated = false;
    [SerializeField] private int _fontSize;
    private float _t;
    float _timeToMove = 2f;
    private AudioSource audio;

    public float Score
    {
        get { return _score; }
    }

    private static GameObject _scoreGameObject;
    private GameObject _gameObject;
    private Board _board;
    private Text _scoreObject;
    public Text ScorePopup;

    public void Add(List<Vector2Int> match, float multiplier) // 3: 100 4: 200 5: 400
    {
        AudioClip clip;
        if (match.Count > 4)
        {
            clip = Resources.Load<AudioClip>("Sounds/match-1");
        }
        else
        {
            clip = Resources.Load<AudioClip>("Sounds/match-0");
        }
        audio.clip = clip;
        audio.Play();
        int points = match.Count - 3;
        int addedScore = (int) (100f * Math.Pow(2, points) * multiplier);
        _score += addedScore;
        _t = 0f;
        Popup(match,addedScore);
    }

    private void UpdateScore()
    {
        _scoreObject.text = ((int) _displayedScore).ToString();
    }

    private void Popup(List<Vector2Int> match, int score)
    {
        Vector3 pos = (_board.GetNutWorldPosition(match.First()) + _board.GetNutWorldPosition(match.Last())) / 2;
        var scorePopup = Instantiate(ScorePopup, new Vector3(), new Quaternion());
        scorePopup.text = score.ToString();
        scorePopup.transform.parent = _gameObject.transform;
        scorePopup.transform.position = pos;
        if (score >= 500)
        {
            scorePopup.color = new Color(239,28,36,1);
        }

        Destroy(scorePopup.gameObject, 1);
    }

    public void SetTargetScore(int targetScore)
    {
        GameObject.Find("TargetScore").GetComponent<Text>().text = targetScore.ToString();
    }

    public void SetRemainingMoves(int moves)
    {
        GameObject.Find("Moves").GetComponent<Text>().text = moves.ToString();
    }

    private void Init()
    {
        _gameObject = GameObject.Find("ScoreBoard");
        _scoreGameObject = GameObject.Find("Score");
        _board = GameObject.Find("Board").GetComponent<Board>();
        _scoreObject = _scoreGameObject.GetComponent<Text>();
        audio = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        Init();
        UpdateScore();
    }

    private void FixedUpdate()
    {
        _t += Time.deltaTime / _timeToMove;

        if (_displayedScore != _score)
        {
            _displayedScore = Mathf.Lerp(_displayedScore, _score, _t);
            _scoreObject.fontSize = (int) (_fontSize + 10 + Math.Sin(Time.time * 10) * 5);
        }
        else if (_scoreObject.fontSize > _fontSize)
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