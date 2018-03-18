﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Board : MonoBehaviour
{
    // Board settings
    [SerializeField] [Range(0, 1)] private float _checkMatchDelay = 0.3f;

    // Board Game Objects
    private GameObject _boardObject;
    private GameScore _score;
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _lostScreen;
    private RectTransform _rt;

    public ParticleSystem DestroyParticle;

    // Board Content
    private readonly Peanut[,] _board = new Peanut[8, 8];
    
    // Board variables
    private readonly List<string> _images = new List<string>();
    private List<List<Vector2Int>> _canMove = new List<List<Vector2Int>>();
    private Vector2Int _selectedNut = new Vector2Int(-1, -1);
    private float _width, _height, _stepX, _stepY, _spawnHeight, _timeLeft;
    private bool _moving, gameEnded;
    public string Gamemode;
    private int _moves = 0;
    private int _maxMoves = 25;
    private float _multiplier = 1;
    private int _targetScore = 10000;
    private int _highscore;
    private AudioSource audio;

    // Debug Variables
    Random _random = new Random(15);

    void FillBoard()
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                AddNut(new Vector2Int(x, y));
            }
        }

        List<List<Vector2Int>> matches = GetMatches();

        while (GetMatches().Count != 0)
        {
            foreach (var match in matches)
            {
                foreach (var nut in match)
                {
                    RemoveNut(nut);
                }
            }

            matches = GetMatches();
        }

        UpdatePossilbeMatches();

        if (_canMove.Count == 0)
        {
            ResetBoard();
        }
    }

    void ResetBoard()
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                RemoveNut(new Vector2Int(x, 0), y);
                RestartTimer(1);
            }
        }
    }

    private Peanut AddNut(Vector2Int pos)
    {
        GameObject go = new GameObject();
        Peanut peanut = go.AddComponent<Peanut>();
//        peanut.Setup(random.Next(0, Peanut.NutTypes.Length));
        peanut.Setup("easy");
        peanut.Position = pos;
        _board[pos.x, pos.y] = peanut;

        string imageToLoad = "Sprites/Nuts/" + peanut.Type;
        Sprite sprite = Resources.Load(imageToLoad, typeof(Sprite)) as Sprite;

        go.name = "Nut";
        go.transform.SetParent(_boardObject.transform, false);

        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 0;
        spriteRenderer.sortingLayerName = "Grid";
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        spriteRenderer.sprite = sprite;
        peanut.GObject = go;
        peanut.GObject.AddComponent<CircleCollider2D>();

        return peanut;
    }

    public List<List<Vector2Int>> GetMatches()
    {
        List<List<Vector2Int>> matches = new List<List<Vector2Int>>();
        matches.Add(new List<Vector2Int>());

        int totalMatches = 0;

        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0) - 2; x++)
            {
                bool newAdded = false;
                while (x + 1 < _board.GetLength(0) && _board[x, y].Type == _board[x + 1, y].Type)
                {
                    if (matches[totalMatches].Count == 0)
                    {
                        matches[totalMatches].Add(new Vector2Int(x, y));
                    }

                    matches[totalMatches].Add(new Vector2Int(x + 1, y));
                    newAdded = true;
                    x++;
                }

                if (newAdded)
                {
                    if (matches.Last().Count < 3)
                    {
                        matches.RemoveAt(totalMatches);
                    }
                    else
                    {
                        totalMatches++;
                    }

                    matches.Add(new List<Vector2Int>());
                }
            }
        }

        for (int x = 0; x < _board.GetLength(0); x++)
        {
            for (int y = 0; y < _board.GetLength(1) - 2; y++)
            {
                bool newAdded = false;
                while (y + 1 < _board.GetLength(1) && _board[x, y].Type == _board[x, y + 1].Type)
                {
                    if (matches[totalMatches].Count == 0)
                    {
                        matches[totalMatches].Add(new Vector2Int(x, y));
                    }

                    matches[totalMatches].Add(new Vector2Int(x, y + 1));
                    newAdded = true;
                    y++;
                }

                if (newAdded)
                {
                    if (matches.Last().Count < 3)
                    {
                        matches.RemoveAt(totalMatches);
                    }
                    else
                    {
                        totalMatches++;
                    }

                    matches.Add(new List<Vector2Int>());
                }
            }
        }

        matches.RemoveAt(totalMatches);

        return matches;
    }

    private void UpdatePossilbeMatches()
    {
        foreach (var pairs in _canMove)
        {
            foreach (var movable in pairs)
            {
                _board[movable.x, movable.y].GObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
        }

        _canMove = new List<List<Vector2Int>>();

        // check horizontal
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                if (x + 1 < _board.GetLength(0) && _board[x, y].Type == _board[x + 1, y].Type)
                {
                    if (x - 2 >= 0)
                    {
                        // check 2 left
                        if (_board[x, y].Type == _board[x - 2, y].Type)
                        {
                            List<Vector2Int> pair = new List<Vector2Int>();
                            pair.Add(new Vector2Int(x - 2, y));
                            pair.Add(new Vector2Int(x - 1, y));
                            _canMove.Add(pair);
                        }
                    }

                    if (y + 1 < _board.GetLength(1) && x - 1 >= 0)
                    {
                        // check 1 left 1 above
                        if (_board[x, y].Type == _board[x - 1, y + 1].Type)
                        {
                            List<Vector2Int> pair = new List<Vector2Int>();
                            pair.Add(new Vector2Int(x - 1, y + 1));
                            pair.Add(new Vector2Int(x - 1, y));
                            _canMove.Add(pair);
                        }
                    }

                    if (y - 1 >= 0 && x - 1 >= 0)
                    {
                        // check 1 left 1 below
                        if (_board[x, y].Type == _board[x - 1, y - 1].Type)
                        {
                            List<Vector2Int> pair = new List<Vector2Int>();
                            pair.Add(new Vector2Int(x - 1, y - 1));
                            pair.Add(new Vector2Int(x - 1, y));
                            _canMove.Add(pair);
                        }
                    }

                    if (x + 3 < _board.GetLength(0))
                    {
                        // check 2 right
                        if (_board[x, y].Type == _board[x + 3, y].Type)
                        {
                            List<Vector2Int> pair = new List<Vector2Int>();
                            pair.Add(new Vector2Int(x + 3, y));
                            pair.Add(new Vector2Int(x + 2, y));
                            _canMove.Add(pair);
                        }
                    }

                    if (y - 1 >= 0 && x + 2 < _board.GetLength(0))
                    {
                        // check 1 right 1 above
                        if (_board[x, y].Type == _board[x + 2, y - 1].Type)
                        {
                            List<Vector2Int> pair = new List<Vector2Int>();
                            pair.Add(new Vector2Int(x + 2, y - 1));
                            pair.Add(new Vector2Int(x + 2, y));
                            _canMove.Add(pair);
                        }
                    }

                    if (y + 1 < _board.GetLength(1) && x + 2 < _board.GetLength(0))
                    {
                        // check 1 right 1 below
                        if (_board[x, y].Type == _board[x + 2, y + 1].Type)
                        {
                            List<Vector2Int> pair = new List<Vector2Int>();
                            pair.Add(new Vector2Int(x + 2, y + 1));
                            pair.Add(new Vector2Int(x + 2, y));
                            _canMove.Add(pair);
                        }
                    }
                }

                if (x + 2 < _board.GetLength(0)
                    && _board[x, y].Type != _board[x + 1, y].Type
                    && _board[x, y].Type == _board[x + 2, y].Type)
                {
                    // check above middle
                    if (y + 1 < _board.GetLength(1) && _board[x, y].Type == _board[x + 1, y + 1].Type)
                    {
                        List<Vector2Int> pair = new List<Vector2Int>();
                        pair.Add(new Vector2Int(x + 1, y + 1));
                        pair.Add(new Vector2Int(x + 1, y));
                        _canMove.Add(pair);
                    }

                    // check below middle
                    if (y - 1 >= 0 && _board[x, y].Type == _board[x + 1, y - 1].Type)
                    {
                        List<Vector2Int> pair = new List<Vector2Int>();
                        pair.Add(new Vector2Int(x + 1, y - 1));
                        pair.Add(new Vector2Int(x + 1, y));
                        _canMove.Add(pair);
                    }
                }
            }
        }

        // check vertical
        for (int x = 0; x < _board.GetLength(0); x++)
        {
            for (int y = 0; y < _board.GetLength(1); y++)
            {
                if (y + 1 < _board.GetLength(0) && _board[x, y].Type == _board[x, y + 1].Type)
                {
                    if (y - 2 >= 0)
                    {
                        // check 2 below
                        if (_board[x, y].Type == _board[x, y - 2].Type)
                        {
                            List<Vector2Int> pair = new List<Vector2Int>();
                            pair.Add(new Vector2Int(x, y - 2));
                            pair.Add(new Vector2Int(x, y - 1));
                            _canMove.Add(pair);
                        }
                    }

                    if (y - 1 >= 0)
                    {
                        if (x - 1 >= 0)
                        {
                            // check 1 below 1 left
                            if (_board[x, y].Type == _board[x - 1, y - 1].Type)
                            {
                                List<Vector2Int> pair = new List<Vector2Int>();
                                pair.Add(new Vector2Int(x - 1, y - 1));
                                pair.Add(new Vector2Int(x, y - 1));
                                _canMove.Add(pair);
                            }
                        }

                        if (x + 1 < _board.GetLength(0))
                        {
                            // check 1 below 1 right
                            if (_board[x, y].Type == _board[x + 1, y - 1].Type)
                            {
                                List<Vector2Int> pair = new List<Vector2Int>();
                                pair.Add(new Vector2Int(x + 1, y - 1));
                                pair.Add(new Vector2Int(x, y - 1));
                                _canMove.Add(pair);
                            }
                        }
                    }

                    if (y + 3 < _board.GetLength(1))
                    {
                        // check 2 above
                        if (_board[x, y].Type == _board[x, y + 3].Type)
                        {
                            List<Vector2Int> pair = new List<Vector2Int>();
                            pair.Add(new Vector2Int(x, y + 3));
                            pair.Add(new Vector2Int(x, y + 2));
                            _canMove.Add(pair);
                        }
                    }

                    if (y + 2 < _board.GetLength(1))
                    {
                        if (x - 1 >= 0)
                        {
                            // check 1 above 1 left
                            if (_board[x, y].Type == _board[x - 1, y + 2].Type)
                            {
                                List<Vector2Int> pair = new List<Vector2Int>();
                                pair.Add(new Vector2Int(x - 1, y + 2));
                                pair.Add(new Vector2Int(x, y + 2));
                                _canMove.Add(pair);
                            }
                        }

                        if (x + 1 < _board.GetLength(0))
                        {
                            // check 1 above 1 right
                            if (x + 1 < _board.GetLength(0) && _board[x, y].Type == _board[x + 1, y + 2].Type)
                            {
                                List<Vector2Int> pair = new List<Vector2Int>();
                                pair.Add(new Vector2Int(x + 1, y + 2));
                                pair.Add(new Vector2Int(x, y + 2));
                                _canMove.Add(pair);
                            }
                        }
                    }
                }

                if (y + 2 < _board.GetLength(0)
                    && _board[x, y].Type != _board[x, y + 1].Type
                    && _board[x, y].Type == _board[x, y + 2].Type)
                {
                    // check right middle
                    if (x + 1 < _board.GetLength(0) && _board[x, y].Type == _board[x + 1, y + 1].Type)
                    {
                        List<Vector2Int> pair = new List<Vector2Int>();
                        pair.Add(new Vector2Int(x + 1, y + 1));
                        pair.Add(new Vector2Int(x, y + 1));
                        _canMove.Add(pair);
                    }

                    // check left middle
                    if (x - 1 >= 0 && _board[x, y].Type == _board[x - 1, y + 1].Type)
                    {
                        List<Vector2Int> pair = new List<Vector2Int>();
                        pair.Add(new Vector2Int(x - 1, y + 1));
                        pair.Add(new Vector2Int(x, y + 1));
                        _canMove.Add(pair);
                    }
                }
            }
        }

        foreach (var pairs in _canMove)
        {
            foreach (var movable in pairs)
            {
//                _board[movable.x,movable.y].GObject.GetComponent<Renderer>().material.SetColor("_Color",Color.blue);
            }
        }

        Debug.Log(_canMove.Count + " possible moves found");
    }

    public Vector3 ToLocalPosition(Vector2Int arrayPos)
    {
        float drawX = _stepX * arrayPos.x - _width / 2 + _stepX / 2;
        float drawY = _stepY * arrayPos.y - _height / 2 + _stepY / 2;
        return new Vector2(drawX, drawY);
    }

    public Vector3 GetNutWorldPosition(Vector2Int arrayPos)
    {
        return _board[arrayPos.x, arrayPos.y].transform.position;
    }

    private void RemoveNut(Vector2Int position, int offset = 0, bool particles = false)
    {
        if (particles)
        {
            var particle = Instantiate(DestroyParticle, _board[position.x, position.y].transform.position,
                new Quaternion());
            Destroy(particle.gameObject, particle.duration);
        }

        float spawnX = ToLocalPosition(position).x;
        Destroy(_board[position.x, position.y].GObject);
        for (int i = position.y; i < _board.GetLength(1) - 1; i++)
        {
            _board[position.x, i] = _board[position.x, i + 1];
            _board[position.x, i].Position = new Vector2Int(position.x, i);
        }

        Peanut peanut = AddNut(new Vector2Int(position.x, _board.GetLength(1) - 1));
        DrawNut(new Vector3(spawnX, _spawnHeight + offset * _stepY, 1), peanut);
    }

    private void DrawNut(Vector3 position, Peanut nut)
    {
        updateNut(position, nut.GObject, false);
    }

    private void updateNut(Vector3 position, GameObject nut, bool animate = true)
    {
        if (animate)
        {
            nut.transform.localPosition = Vector3.MoveTowards(nut.transform.localPosition, position, 8);
        }
        else
        {
            nut.transform.localPosition = position;
        }
    }

    private void DrawGrid(Vector3 position, Vector2 size)
    {
        GameObject go = new GameObject();
        const string imageToLoad = "Sprites/gridSquare";
        Sprite sprite = Resources.Load(imageToLoad, typeof(Sprite)) as Sprite;
        go.transform.position = position;
        go.transform.localScale = size;
        go.name = "Square";
        go.transform.SetParent(_boardObject.transform, false);
        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 0;
        spriteRenderer.sortingLayerName = "Grid";
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.05f);
        spriteRenderer.sprite = sprite;
    }

    private void DrawBoard()
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                float drawX = _stepX * x - _width / 2 + _stepX / 2;
                float drawY = _stepY * y - _height / 2 + _stepY / 2;
                DrawNut(new Vector3(drawX, drawY, 1), _board[x, y]);
                if ((x + y) % 2 == 0)
                {
                    DrawGrid(new Vector3(drawX, drawY, 1), new Vector2Int((int) _stepX, (int) _stepY));
                }
            }
        }
    }

    private void UpdateBoard(bool animate = true)
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                if (_board[x, y])
                {
                    float drawX = _stepX * x - _width / 2 + _stepX / 2;
                    float drawY = _stepY * y - _height / 2 + _stepY / 2;
                    if (_board[x, y].transform.localPosition != new Vector3(drawX, drawY, 1))
                    {
                        updateNut(new Vector3(drawX, drawY, 1), _board[x, y].GObject, animate);
                        RestartTimer();
                    }
                }
            }
        }
    }

    private void LoadImages()
    {
        foreach (var image in _images)
        {
            _images.Add(image);
        }
    }

    public void RestartTimer()
    {
        RestartTimer(_checkMatchDelay);
    }

    public void RestartTimer(float delay)
    {
        _timeLeft = delay;
        _moving = true;
    }

    private void Init()
    {
        //set board gameobject as private variable
        _boardObject = GameObject.Find("Board");
        //set board info
        _rt = _boardObject.GetComponent<RectTransform>();
        _width = _rt.rect.width;
        _height = _rt.rect.height;
        _stepX = _width / _board.GetLength(0);
        _stepY = _height / _board.GetLength(1);
        _spawnHeight = (int) (_height / 2 + _stepY / 2);
        //load all peanut images
        LoadImages();
        //set score object
        _score = GameObject.Find("ScoreBoard").GetComponent<GameScore>();
        _score.SetTargetScore(_targetScore);
        _score.SetRemainingMoves(_maxMoves-_moves);
        _highscore = PlayerPrefs.GetInt("highscore", _highscore);
        _score.SetHighScore(_highscore);
        audio = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        Init();
        FillBoard();
        DrawBoard();
        UpdatePossilbeMatches();
    }

    private void FixedUpdate()
    {
        UpdateBoard();
        _timeLeft -= Time.deltaTime;
        if (_timeLeft < 0 && _moving) //while there are still matches keep 
        {
            var matches = GetMatches();
            var toRemove = new List<Vector2Int>();
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    foreach (var nut in match)
                    {
                        toRemove.Add(nut);
                    }

                    _score.Add(match, _multiplier);

                    _multiplier += 0.5f;
                }

                toRemove.Sort((a, b) => b.y.CompareTo(a.y));
                int spawnY = 0;
                int lastY = toRemove[0].y;
                foreach (var nut in toRemove)
                {
                    if (lastY != nut.y) spawnY++;
                    lastY = nut.y;
                    RemoveNut(nut, spawnY, true);
                    RestartTimer();
                }

                RestartTimer();
            }
        }

        if (_timeLeft < 0 && _moving) //end of turn
        {
            _moving = false;
            Debug.Log("Done!");
            UpdatePossilbeMatches();

            _multiplier = 1;
            if (CheckWin())
            {
                if (_moves < _maxMoves)
                {
                    _score.GetComponent<GameScore>().AddEnd();
                    int row = _random.Next(8);
                    bool rowOrColumn = _random.Next(2) == 1;
                    for (int i = _board.GetLength(0) - 1; i >= 0; i--)
                    {
                        if (rowOrColumn)
                        {
                            RemoveNut(new Vector2Int(i, row));
                        }
                        else
                        {
                            RemoveNut(new Vector2Int(row, i));
                        }
                    }
                    AudioClip clip = Resources.Load<AudioClip>("Sounds/match-1");
                    audio.clip = clip;
                    audio.Play();

                    _moves++;

                    RestartTimer();
                }
                else
                {
                    gameEnded = true;
                    _winScreen.gameObject.SetActive(true);
                    if (_score.Score > _highscore){
                        _highscore = (int) _score.Score;
                        PlayerPrefs.SetInt("highscore", _highscore);
                        _score.SetHighScore(_highscore);
                    }
                }
            }
            
            if (CheckLose() && !gameEnded)
            {
                Debug.Log("You have lost!");
                _lostScreen.gameObject.SetActive(true);
            }

            if (_canMove.Count == 0)
            {
                ResetBoard();
            }
        }
    }

    public void SelectNut(Vector2Int position) // used in non-mobile enviroments
    {
        if (CheckLose())
        {
            return;
        }

        if (CheckWin())
        {
            return;
        }

        if (_moving)
        {
            return;
        }

        if (_selectedNut == new Vector2Int(-1, -1))
        {
            _selectedNut = position;
            _board[_selectedNut.x, _selectedNut.y].GObject.GetComponent<SpriteRenderer>().color =
                new Color(255, 255, 255, 0.7f);
            return;
        }

        if (_selectedNut == position)
        {
            _board[_selectedNut.x, _selectedNut.y].GObject.GetComponent<SpriteRenderer>().color =
                new Color(255, 255, 255, 1);
            _selectedNut = new Vector2Int(-1, -1);
            return;
        }

        if (_selectedNut.x == position.x && (_selectedNut.y == position.y - 1 || _selectedNut.y == position.y + 1)
            || _selectedNut.y == position.y && (_selectedNut.x == position.x - 1 || _selectedNut.x == position.x + 1))
        {
            _board[_selectedNut.x, _selectedNut.y].GObject.GetComponent<SpriteRenderer>().color =
                new Color(255, 255, 255, 1);
            SwapNuts(_selectedNut, position);
            _selectedNut = new Vector2Int(-1, -1);
        }
        else
        {
            _board[_selectedNut.x, _selectedNut.y].GObject.GetComponent<SpriteRenderer>().color =
                new Color(255, 255, 255, 1);
            _selectedNut = position;
            _board[_selectedNut.x, _selectedNut.y].GObject.GetComponent<SpriteRenderer>().color =
                new Color(255, 255, 255, 0.7f);
            return;
        }

        _selectedNut = new Vector2Int(-1, -1);
    }

    public void Move(Vector2Int pos, Vector2Int direction)
    {
        if (CheckLose())
        {
            return;
        }

        if (CheckWin())
        {
            return;
        }

        if (_moving)
        {
            return;
        }
        
        Vector2Int newPos = pos + direction;
        if ((int) direction.magnitude == 1 && 
            newPos.x >= 0 && newPos.x < _board.GetLength(0) && 
            newPos.y >= 0 && newPos.y < _board.GetLength(1))
        {
            SwapNuts(pos, newPos);
        }
    }

    private void SwapNuts(Vector2Int pos1, Vector2Int pos2)
    {
        if (CheckLose() || _moving || CheckWin())
        {
            return;
        }

        bool canMove = false;
        foreach (var pair in _canMove)
        {
            if (pair.Contains(pos1) && pair.Contains(pos2))
            {
                canMove = true;
            }
        }

        if (canMove)
        {
            Peanut peanut = _board[pos1.x, pos1.y];
            _board[pos1.x, pos1.y] = _board[pos2.x, pos2.y];
            _board[pos1.x, pos1.y].Position = pos1;
            _board[pos2.x, pos2.y] = peanut;
            peanut.Position = pos2;
            _moves++;
            _score.SetRemainingMoves(_maxMoves - _moves);
        }
        else
        {
            Hashtable punchOptions = new Hashtable
            {
                {
                    "amount", _board[pos2.x, pos2.y].GObject.transform.localPosition
                              - _board[pos1.x, pos1.y].GObject.transform.localPosition
                },
                {"time", 1},
                {"EasyType", "easeOutQuad"}
            };
            iTween.PunchPosition(_board[pos1.x, pos1.y].GObject, punchOptions);
            punchOptions["amount"] = _board[pos1.x, pos1.y].GObject.transform.localPosition
                                     - _board[pos2.x, pos2.y].GObject.transform.localPosition;
            iTween.PunchPosition(_board[pos2.x, pos2.y].GObject, punchOptions);
        }

        _selectedNut = new Vector2Int(-1, -1);
        Debug.Log(_maxMoves - _moves + " moves left");
    }

    private bool CheckWin()
    {
        switch (Gamemode)
        {
            case "moves":
                return false;
            case "time":
                return false;
            case "score":
                goto default;
            default:
                return CheckScoreWin();
        }
    }

    private bool CheckLose()
    {
        switch (Gamemode)
        {
            case "moves":
                return false;
            case "time":
                return false;
            case "score":
                goto default;
            default:
                return CheckScoreLose();
        }
    }

    private bool CheckScoreWin()
    {
        return _score.GetComponent<GameScore>().Score > _targetScore;
    }

    private bool CheckScoreLose()
    {
        return _moves >= _maxMoves;
    }
}