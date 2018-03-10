using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    private readonly Peanut[,] _board = new Peanut[8, 8];

    private List<string> _images = new List<string>();

    private GameObject _boardObject;

    void fillBoard()
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                Peanut peanut = gameObject.AddComponent<Peanut>();
                peanut.Setup();
                _board[x, y] = peanut;
            }
        }

        List<List<Vector2>> matches = getMatches();

        while (matches.Count != 0)
        {
            foreach (var match in matches)
            {
                foreach (var nut in match)
                {
                    Peanut peanut = gameObject.AddComponent<Peanut>();
                    peanut.Setup();
                    _board[(int) nut.x, (int) nut.y] = peanut;
                }
            }

            matches = getMatches();
        }
    }

    List<List<Vector2>> getMatches()
    {
        List<List<Vector2>> matches = new List<List<Vector2>>();
        matches.Add(new List<Vector2>());

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
                        matches[totalMatches].Add(new Vector2(x, y));
                    }

                    matches[totalMatches].Add(new Vector2(x, y));
                    newAdded = true;
                    x++;
                }

                if (newAdded)
                {
                    if (matches.Last().Count < 3)
                    {
                        matches.RemoveAt(totalMatches);
                        totalMatches--;
                    }

                    matches.Add(new List<Vector2>());
                    totalMatches++;
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
                        matches[totalMatches].Add(new Vector2(x, y));
                    }

                    matches[totalMatches].Add(new Vector2(x, y));
                    newAdded = true;
                    y++;
                }

                if (newAdded)
                {
                    if (matches.Last().Count < 3)
                    {
                        matches.RemoveAt(totalMatches);
                        totalMatches--;
                    }

                    matches.Add(new List<Vector2>());
                    totalMatches++;
                }
            }
        }

        matches.RemoveAt(totalMatches);

        return matches;
    }

    public void removeNut(Vector2 position)
    {
        for (int i = (int)position.y; i < _board.GetLength(1) - 1; i++)
        {
            _board[(int) position.x, i] = _board[(int) position.x, i + 1];
        }

        Peanut peanut = gameObject.AddComponent<Peanut>();
        peanut.Setup();
        _board[(int) position.x, _board.GetLength(1) - 1] = peanut;
        
        Debug.Log("nut removed");
        drawBoard();
    }

    private void drawNut(Vector3 position, Peanut nut)
    {
        GameObject go = new GameObject();

        string imageToLoad = "Sprites/Nuts/" + nut.Type;
        Sprite sprite = Resources.Load(imageToLoad, typeof(Sprite)) as Sprite;

        go.transform.position = position;
        go.name = "Nut";
        go.transform.SetParent(_boardObject.transform, false);

        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 0;
        spriteRenderer.sortingLayerName = "Game";
        spriteRenderer.sprite = sprite;
    }

    private void updateNut(Vector3 position, GameObject nut)
    {
        nut.transform.position = position;
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
        RectTransform rt = _boardObject.GetComponent<RectTransform>();
        float width = rt.rect.width;
        float height = rt.rect.height;
        float stepX = width / _board.GetLength(0);
        float stepY = height / _board.GetLength(1);

        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                float drawX = stepX * x - width / 2 + stepX / 2;
                float drawY = stepY * y - height / 2 + stepY / 2;
                drawNut(new Vector3(drawX, drawY, 1), _board[x, y]);
                if ((x + y) % 2 == 0)
                {
                    drawGrid(new Vector3(drawX, drawY, 1), new Vector2(stepX, stepY));
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
        //load all peanut images
        loadImages();
    }

    private void Start()
    {
        init();

        fillBoard();

        Debug.Log(getMatches().Count + " matches found");
        
        drawBoard();
        
        removeNut(new Vector2(0,0));
    }
    
    private void update()
    {
        RectTransform rt = _boardObject.GetComponent<RectTransform>();
        float width = rt.rect.width;
        float height = rt.rect.height;
        float stepX = width / _board.GetLength(0);
        float stepY = height / _board.GetLength(1);

        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                float drawX = stepX * x - width / 2 + stepX / 2;
                float drawY = stepY * y - height / 2 + stepY / 2;
//                updateNut(new Vector3(drawX, drawY, 1), _board[x, y]);
            }
        }
    }
}

