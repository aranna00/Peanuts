﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Board : MonoBehaviour
{
    private readonly Peanut[,] _board = new Peanut[8, 8];

    private List<string> _images = new List<string>();

    private GameObject _boardObject;

    private List<Vector2Int> canMove = new List<Vector2Int>();

    private RectTransform rt;
    private float width, height, stepX, stepY, spawnHeight;
    private float checkMatchDelay = 1f;
    private float timeLeft;
    Random random = new Random(6);


    void fillBoard()
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                addNut(new Vector2Int(x,y));
            }
        }

        List<List<Vector2Int>> matches = getMatches();

        while (matches.Count != 0)
        {
            foreach (var match in matches)
            {
                foreach (var nut in match)
                {
                    removeNut(nut);
                }
            }

            matches = getMatches();
        }
    }

    private Peanut addNut(Vector2Int pos)
    {
        GameObject go = new GameObject();
        Peanut peanut = go.AddComponent<Peanut>();
        peanut.Setup(random.Next(0, Peanut.NutTypes.Length));
//        peanut.Setup();
        peanut.Position = pos;
        _board[pos.x, pos.y] = peanut;

        string imageToLoad = "Sprites/Nuts/" + peanut.Type;
        Sprite sprite = Resources.Load(imageToLoad, typeof(Sprite)) as Sprite;

        go.name = "Nut";
        go.transform.SetParent(_boardObject.transform, false);

        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 0;
        spriteRenderer.sortingLayerName = "Game";
        spriteRenderer.sprite = sprite;
        peanut.GObject = go;
        peanut.GObject.AddComponent<CircleCollider2D>();

        return peanut;
    }

    List<List<Vector2Int>> getMatches()
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

    public void UpdatePossilbeMatches()
    {
        canMove = new List<Vector2Int>();
        List<List<Peanut>> matches = new List<List<Peanut>>();
        List<List<Peanut>> horMatches = new List<List<Peanut>>();
        List<List<Peanut>> verMatches = new List<List<Peanut>>();
        matches.Add(new List<Peanut>());

        int totalMatches = 0;

        // check horizontal
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0) - 2; x++)
            {
                List<Peanut> orgMatches = new List<Peanut>();
                if (
                    x + 2 < _board.GetLength(0)
                    && x - 1 > 0
                    && _board[x, y].Type != _board[x - 1, y].Type
                    && _board[x, y].Type == _board[x + 1, y].Type
                    && _board[x, y].Type != _board[x + 2, y].Type
                )
                {
                    orgMatches.Add(_board[x, y]);
                    orgMatches.Add(_board[x + 1, y]);
                }
                else if (
                    x + 2 < _board.GetLength(0)
                    && _board[x, y].Type != _board[x + 1, y].Type
                    && _board[x, y].Type == _board[x + 2, y].Type
                )
                {
                    orgMatches.Add(_board[x, y]);
                    orgMatches.Add(_board[x + 2, y]);
                    // check above middle
                    if (y + 1 < _board.GetLength(1) && _board[x, y].Type == _board[x + 1, y + 1].Type)
                    {
                        List<Peanut> currentMatch = new List<Peanut>();
                        currentMatch.AddRange(orgMatches);
                        currentMatch.Add(_board[x + 1, y + 1]);
                        horMatches.Add(currentMatch);
                        totalMatches++;
                        canMove.Add(new Vector2Int(x + 1, y + 1));
                    }

                    // check below middle
                    if (y - 1 > 0 && _board[x, y].Type == _board[x + 1, y - 1].Type)
                    {
                        List<Peanut> currentMatch = new List<Peanut>();
                        currentMatch.AddRange(orgMatches);
                        currentMatch.Add(_board[x + 1, y - 1]);
                        horMatches.Add(currentMatch);
                        totalMatches++;
                        canMove.Add(new Vector2Int(x + 1, y - 1));
                    }

                    continue;
                }
                else
                {
                    continue;
                }

                // check 2 left
                if (x - 2 > 0 && _board[x, y].Type == _board[x - 2, y].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x - 2, y]);
                    horMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x - 2, y));
                }

                // check 1 left 1 above
                if (y + 1 < _board.GetLength(1)&& _board[x, y].Type == _board[x - 1, y + 1].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x - 1, y + 1]);
                    horMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x - 1, y + 1));
                }

                // check 1 left 1 below
                if (y - 1 > 0  && _board[x, y].Type == _board[x - 1, y - 1].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x - 1, y - 1]);
                    horMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x - 1, y - 1));
                }

                // check 2 right
                if (x + 3 < _board.GetLength(0) && _board[x, y].Type == _board[x + 3, y].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x + 3, y]);
                    horMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x + 3, y));
                }

                // check 1 right 1 above
                if (y - 1 > 0 && _board[x, y].Type == _board[x + 2, y - 1].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x + 2, y - 1]);
                    horMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x + 2, y - 1));
                }

                // check 1 right 1 below
                if (y + 1 < _board.GetLength(1) && _board[x, y].Type == _board[x + 2, y + 1].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x + 2, y + 1]);
                    horMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x + 2, y + 1));
                }
            }
        }

        for (int x = 0; x < _board.GetLength(0); x++)
        {
            for (int y = 0; y < _board.GetLength(1) - 2; y++)
            {
                List<Peanut> orgMatches = new List<Peanut>();
                if (
                    y + 2 < _board.GetLength(1)
                    && y - 1 > 0
                    && _board[x, y].Type != _board[x, y - 1].Type
                    && _board[x, y].Type == _board[x, y + 1].Type
                    && _board[x, y].Type != _board[x, y + 2].Type
                )
                {
                    orgMatches.Add(_board[x, y]);
                    orgMatches.Add(_board[x, y + 1]);
                }
                else if (
                    y + 2 < _board.GetLength(0)
                    && _board[x, y].Type != _board[x, y + 1].Type
                    && _board[x, y].Type == _board[x, y + 2].Type
                )
                {
                    orgMatches.Add(_board[x, y]);
                    orgMatches.Add(_board[x, y + 2]);

                    // check right middle
                    if (x + 1 < _board.GetLength(0) && _board[x, y].Type == _board[x + 1, y + 1].Type)
                    {
                        List<Peanut> currentMatch = new List<Peanut>();
                        currentMatch.AddRange(orgMatches);
                        currentMatch.Add(_board[x + 1, y + 1]);
                        verMatches.Add(currentMatch);
                        totalMatches++;
                        canMove.Add(new Vector2Int(x + 1, y + 1));
                    }

                    // check left middle
                    if (x - 1 > 0 && _board[x, y].Type == _board[x - 1, y + 1].Type)
                    {
                        List<Peanut> currentMatch = new List<Peanut>();
                        currentMatch.AddRange(orgMatches);
                        currentMatch.Add(_board[x - 1, y + 1]);
                        verMatches.Add(currentMatch);
                        totalMatches++;
                        canMove.Add(new Vector2Int(x - 1, y + 1));
                    }

                    continue;
                }
                else
                {
                    continue;
                }

                // check 2 below
                if (y - 2 > 0 && _board[x, y].Type == _board[x, y - 2].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x, y - 2]);
                    verMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x, y - 2));
                }

                // check 1 below 1 left
                if (x - 1 > 0 && _board[x, y].Type == _board[x - 1, y - 1].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x - 1, y - 1]);
                    verMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x - 1, y - 1));
                }

                // check 1 below 1 right
                if (x + 1 < _board.GetLength(0) && _board[x, y].Type == _board[x + 1, y - 1].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x + 1, y - 1]);
                    verMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x + 1, y - 1));
                }

                // TODO not working
                // check 3 above
                if (y + 3 < _board.GetLength(1) && _board[x, y].Type == _board[x, y + 3].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x, y + 3]);
                    verMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x, y + 3));
                }

                // check 1 above 1 left
                if (x - 1 > 0 && _board[x, y].Type == _board[x - 1, y + 2].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x - 1, y + 2]);
                    verMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x - 1, y + 2));
                }

                // TODO not working
                // check 1 above 1 right
                if (x + 1 < _board.GetLength(0) && _board[x, y].Type == _board[x + 1, y + 2].Type)
                {
                    List<Peanut> currentMatch = new List<Peanut>();
                    currentMatch.AddRange(orgMatches);
                    currentMatch.Add(_board[x + 1, y + 2]);
                    verMatches.Add(currentMatch);
                    totalMatches++;
                    canMove.Add(new Vector2Int(x + 1, y + 2));
                }
            }
        }
        Debug.Log(totalMatches + " possible matches found");
        Debug.Log(canMove.Count + " possible moves found");
    }

    public void removeNut(Vector2Int position, bool spawnNew = true) //TODO Delay spawn when multiple nuts are spawned on the same row.
    {
        float lastX = _board[position.x, position.y].GObject.transform.localPosition.x;
        Destroy(_board[position.x, position.y].GObject);
        for (int i = position.y; i < _board.GetLength(1) - 1; i++)
        {
            _board[position.x, i] = _board[position.x, i + 1];
            _board[position.x, i].Position = new Vector2Int(position.x, i);
        }
        
        Peanut peanut = addNut(new Vector2Int(position.x, _board.GetLength(1) - 1));
        drawNut(new Vector3(lastX, spawnHeight, 1), peanut);
    }

    private void drawNut(Vector3 position, Peanut nut)
    {
        nut.GObject.transform.localPosition = position;
    }

    private void updateNut(Vector3 position, GameObject nut, bool animate = true)
    {
        if (animate)
        {
            nut.transform.localPosition = Vector3.MoveTowards(nut.transform.localPosition, position, 4);
        }
        else
        {
            nut.transform.localPosition = position;
        }
        
    }

    private void drawGrid(Vector3 position, Vector2 size)
    {
        GameObject go = new GameObject();

        string imageToLoad = "Sprites/gridSquare";
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

    private void drawBoard()
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                float drawX = stepX * x - width / 2 + stepX / 2;
                float drawY = stepY * y - height / 2 + stepY / 2;
                drawNut(new Vector3(drawX, drawY, 1), _board[x, y]);
                if ((x + y) % 2 == 0)
                {
                    drawGrid(new Vector3(drawX, drawY, 1), new Vector2Int((int) stepX, (int) stepY));
                }
            }
        }
    }

    private void loadImages()
    {
        foreach (var image in _images)
        {
            _images.Add(image);
        }
    }

    private void init()
    {
        
        //set board gameobject as private variable
        _boardObject = GameObject.Find("Board");
        //set board info
        rt = _boardObject.GetComponent<RectTransform>();
        width = rt.rect.width;
        height = rt.rect.height;
        stepX = width / _board.GetLength(0);
        stepY = height / _board.GetLength(1);
        spawnHeight = (int) (height / 2 + stepY / 2);
        //load all peanut images
        loadImages();
    }

    private void Start()
    {
        init();

        fillBoard();

        Debug.Log(getMatches().Count + " matches found");

        drawBoard();

        Debug.Log(getMatches().Count + " matches found");
        

//        removeNut(new Vector2(0, 0));
    }

    private void FixedUpdate()
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                float drawX = stepX * x - width / 2 + stepX / 2;
                float drawY = stepY * y - height / 2 + stepY / 2;
                if (_board[x, y].transform.localPosition != new Vector3(drawX, drawY, 1))
                {
                    updateNut(new Vector3(drawX, drawY, 1), _board[x, y].GObject);
                }
            }
        }
        
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            List<List<Vector2Int>> matches = getMatches();
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    match.Reverse();
                    foreach (var nut in match)
                    {
                        removeNut(nut);
                    }
                }

                timeLeft = checkMatchDelay;
            }
            UpdatePossilbeMatches();

            foreach (Vector2Int i in canMove)
            {
                _board[i.x, i.y].GObject.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            }
        }
    }

    private void OnMouseDown()
    {
        removeNut(new Vector2Int(2, 2));
        timeLeft = checkMatchDelay;
    }
}