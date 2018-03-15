using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Board : MonoBehaviour
{
    private readonly Peanut[,] _board = new Peanut[8, 8];

    private List<string> _images = new List<string>();

    private GameObject _boardObject;

    private List<Vector2Int> canMove = new List<Vector2Int>();
    private Vector2Int _selectedNut = new Vector2Int(-1, -1);

    private RectTransform rt;
    private float width, height, stepX, stepY, spawnHeight;
    private float checkMatchDelay = 0.5f;
    private float timeLeft;
    private bool moving;
    Random random = new Random(15);

    void fillBoard()
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                addNut(new Vector2Int(x, y));
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

        UpdatePossilbeMatches();

        if (canMove.Count == 0)
        {
            resetBoard();
        }
    }

    void resetBoard()
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                removeNut(new Vector2Int(x, y), y);
                moving = true;
            }
        }
    }

    private Peanut addNut(Vector2Int pos)
    {
        GameObject go = new GameObject();
        Peanut peanut = go.AddComponent<Peanut>();
//        peanut.Setup(random.Next(0, Peanut.NutTypes.Length));
        peanut.Setup();
        peanut.Position = pos;
        _board[pos.x, pos.y] = peanut;

        string imageToLoad = "Sprites/Nuts/" + peanut.Type;
        Sprite sprite = Resources.Load(imageToLoad, typeof(Sprite)) as Sprite;

        go.name = "Nut";
        go.transform.SetParent(_boardObject.transform, false);

        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 0;
        spriteRenderer.sortingLayerName = "Game";
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
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
                            canMove.Add(new Vector2Int(x - 2, y));
                        }
                    }

                    if (y + 1 < _board.GetLength(1) && x - 1 >= 0)
                    {
                        // check 1 left 1 above
                        if (_board[x, y].Type == _board[x - 1, y + 1].Type)
                        {
                            canMove.Add(new Vector2Int(x - 1, y + 1));
                        }
                    }

                    if (y - 1 >= 0 && x - 1 >= 0)
                    {
                        // check 1 left 1 below
                        if (_board[x, y].Type == _board[x - 1, y - 1].Type)
                        {
                            canMove.Add(new Vector2Int(x - 1, y - 1));
                        }
                    }

                    if (x + 3 < _board.GetLength(0))
                    {
                        // check 2 right
                        if (_board[x, y].Type == _board[x + 3, y].Type)
                        {
                            canMove.Add(new Vector2Int(x + 3, y));
                        }
                    }

                    if (y - 1 >= 0 && x + 2 < _board.GetLength(0))
                    {
                        // check 1 right 1 above
                        if (_board[x, y].Type == _board[x + 2, y - 1].Type)
                        {
                            canMove.Add(new Vector2Int(x + 2, y - 1));
                        }
                    }

                    if (y + 1 < _board.GetLength(1) && x + 2 < _board.GetLength(0))
                    {
                        // check 1 right 1 below
                        if (_board[x, y].Type == _board[x + 2, y + 1].Type)
                        {
                            canMove.Add(new Vector2Int(x + 2, y + 1));
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
                        canMove.Add(new Vector2Int(x + 1, y + 1));
                    }

                    // check below middle
                    if (y - 1 >= 0 && _board[x, y].Type == _board[x + 1, y - 1].Type)
                    {
                        canMove.Add(new Vector2Int(x + 1, y - 1));
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
                            canMove.Add(new Vector2Int(x, y - 2));
                        }
                    }

                    if (y - 1 >= 0)
                    {
                        if (x - 1 >= 0)
                        {
                            // check 1 below 1 left
                            if (_board[x, y].Type == _board[x - 1, y - 1].Type)
                            {
                                canMove.Add(new Vector2Int(x - 1, y - 1));
                            }
                        }

                        if (x + 1 < _board.GetLength(0))
                        {
                            // check 1 below 1 right
                            if (_board[x, y].Type == _board[x + 1, y - 1].Type)
                            {
                                canMove.Add(new Vector2Int(x + 1, y - 1));
                            }
                        }
                    }

                    if (y + 3 < _board.GetLength(1))
                    {
                        // check 2 above
                        if (_board[x, y].Type == _board[x, y + 3].Type)
                        {
                            canMove.Add(new Vector2Int(x, y + 3));
                        }
                    }

                    if (y + 2 < _board.GetLength(1))
                    {
                        if (x - 1 >= 0)
                        {
                            // check 1 above 1 left
                            if (_board[x, y].Type == _board[x - 1, y + 2].Type)
                            {
                                canMove.Add(new Vector2Int(x - 1, y + 2));
                            }
                        }

                        if (x + 1 < _board.GetLength(0))
                        {
                            // check 1 above 1 right
                            if (x + 1 < _board.GetLength(0) && _board[x, y].Type == _board[x + 1, y + 2].Type)
                            {
                                canMove.Add(new Vector2Int(x + 1, y + 2));
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
                        canMove.Add(new Vector2Int(x + 1, y + 1));
                    }

                    // check left middle
                    if (x - 1 >= 0 && _board[x, y].Type == _board[x - 1, y + 1].Type)
                    {
                        canMove.Add(new Vector2Int(x - 1, y + 1));
                    }
                }
            }
        }

        Debug.Log(canMove.Count + " possible moves found");
    }

    public void removeNut(Vector2Int position, int offset = 0)
    {
        float lastX = _board[position.x, position.y].GObject.transform.localPosition.x;
        Destroy(_board[position.x, position.y].GObject);
        for (int i = position.y; i < _board.GetLength(1) - 1; i++)
        {
            _board[position.x, i] = _board[position.x, i + 1];
            _board[position.x, i].Position = new Vector2Int(position.x, i);
        }

        Peanut peanut = addNut(new Vector2Int(position.x, _board.GetLength(1) - 1));
        drawNut(new Vector3(lastX, spawnHeight + offset * stepY, 1), peanut);
    }

    private void drawNut(Vector3 position, Peanut nut)
    {
        nut.GObject.transform.localPosition = position;
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

    private void updateBoard(bool animate = true)
    {
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                float drawX = stepX * x - width / 2 + stepX / 2;
                float drawY = stepY * y - height / 2 + stepY / 2;
                if (_board[x, y].transform.localPosition != new Vector3(drawX, drawY, 1))
                {
                    updateNut(new Vector3(drawX, drawY, 1), _board[x, y].GObject, animate);
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

    public void restartTimer()
    {
        timeLeft = checkMatchDelay;
        moving = true;
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
        drawBoard();
        UpdatePossilbeMatches();
    }

    private void FixedUpdate()
    {
        updateBoard();
        timeLeft -= Time.deltaTime;
        if (moving) //while there are still matches keep 
        {
            List<List<Vector2Int>> matches = getMatches();
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    for (var i = match.Count - 1; i >= 0; i--)
                    {
                        var nut = match[i];
                        if (match[0].x == match[1].x)
                        {
                            removeNut(nut, match.Count - i - 1);
                        }
                        else
                        {
                            removeNut(nut);
                        }
                    }
                }

                timeLeft = checkMatchDelay;
            }
        }

        if (timeLeft < 0 && moving) //end of turn
        {
            moving = false;
            Debug.Log("Done!");
            UpdatePossilbeMatches();
            if (canMove.Count == 0)
            {
                resetBoard();
            }
        }
    }

    public void SelectNut(Vector2Int position)
    {
        if (_selectedNut == new Vector2Int(-1, -1))
        {
            _selectedNut = position;
            _board[_selectedNut.x, _selectedNut.y].GObject.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            return;
        }

        if (_selectedNut == position)
        {
            _board[_selectedNut.x, _selectedNut.y].GObject.GetComponent<SpriteRenderer>().color =
                new Color(255, 255, 255);
            _selectedNut = new Vector2Int(-1, -1);
            return;
        }

        if (_selectedNut.x == position.x && (_selectedNut.y == position.y - 1 || _selectedNut.y == position.y + 1)
            || _selectedNut.y == position.y && (_selectedNut.x == position.x - 1 || _selectedNut.x == position.x + 1))
        {
            _board[_selectedNut.x, _selectedNut.y].GObject.GetComponent<SpriteRenderer>().color =
                new Color(255, 255, 255);
            SwapNuts(_selectedNut, position);
            _selectedNut = new Vector2Int(-1, -1);
        }
        else
        {
            _board[_selectedNut.x, _selectedNut.y].GObject.GetComponent<SpriteRenderer>().color =
                new Color(255, 255, 255);
            _selectedNut = position;
            _board[_selectedNut.x, _selectedNut.y].GObject.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            return;
        }

        _selectedNut = new Vector2Int(-1, -1);
    }

    private void SwapNuts(Vector2Int pos1, Vector2Int pos2)
    {
        if (canMove.Contains(pos1) || canMove.Contains(pos2))
        {
            restartTimer();
            Peanut peanut = _board[pos1.x, pos1.y];
            _board[pos1.x, pos1.y] = _board[pos2.x, pos2.y];
            _board[pos1.x, pos1.y].Position = pos1;
            _board[pos2.x, pos2.y] = peanut;
            peanut.Position = pos2;
        }
        else
        {
            Hashtable punchOptions = new Hashtable();
            punchOptions.Add(
                "amount",
                (_board[pos2.x, pos2.y].GObject.transform.localPosition
                 - _board[pos1.x, pos1.y].GObject.transform.localPosition));
            punchOptions.Add("time", 1);
            punchOptions.Add("EasyType", "easeOutQuad");
            iTween.PunchPosition(_board[pos1.x, pos1.y].GObject, punchOptions);
            punchOptions["amount"] = (_board[pos1.x, pos1.y].GObject.transform.localPosition
                                      - _board[pos2.x, pos2.y].GObject.transform.localPosition);
            iTween.PunchPosition(_board[pos2.x, pos2.y].GObject, punchOptions);
        }
    }
}