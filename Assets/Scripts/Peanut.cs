using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Peanut : MonoBehaviour
{
    public List<string> NutTypes;
//    public string[] NutTypes;

    // Peanut properties
    private string _type;
    public Vector2Int Position;
    public bool Animating;
    private int _startTime;

    // Board
    private Board _board;
        
    // Swipe variables
    private Vector3 _touchPosition;
    private float _swipeResistanceX = 50.0f;
    private float _swipeResistanceY = 50.0f;
    private Vector2Int _swipeDirection = Vector2Int.zero;

    public string Type
    {
        get { return _type; }
    }

    public GameObject GObject { get; set; }

    private void Setup()
    {
        _board = GameObject.Find("Board").GetComponent<Board>();
        _type = NutTypes[Random.Range(0, NutTypes.Count)];
    }
    
    public void Setup(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                NutTypes = new List<string> {"Coconut", "Hazelnut", "Peanut", "Pistachio", "Walnut"};
                break;
            case "Medium":
                NutTypes = new List<string> {"Coconut", "Hazelnut", "Peanut", "Pistachio", "Walnut", "Cashew"};
                break;
            case "Hard":
                NutTypes = new List<string> {"Coconut", "Hazelnut", "Peanut", "Pistachio", "Walnut", "Cashew", "Almond"};
                break;
        }
        Setup();
    }

    public void startAnimation(int startTime)
    {
        _startTime = startTime;
        Animating = true;
    }

    private void FixedUpdate()
    {
        if (Animating)
        {
            
        }
    }

    private void OnMouseDown()
    {
        _touchPosition = Input.mousePosition;
        GObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.7f);
    }

    private void OnMouseUp()
    {
        Vector2 deltaSwipe = _touchPosition - Input.mousePosition;
        
        if(Mathf.Abs(deltaSwipe.x) > _swipeResistanceX)
        {
            _swipeDirection = deltaSwipe.x < 0 ? Vector2Int.right : Vector2Int.left;
        }
        if(Mathf.Abs(deltaSwipe.y) > _swipeResistanceY && Mathf.Abs(deltaSwipe.y) > Mathf.Abs(deltaSwipe.x))
        {
            _swipeDirection = deltaSwipe.y < 0 ? Vector2Int.up : Vector2Int.down;
        }
        if (_swipeDirection != Vector2Int.zero)
        {
            _board.Move(Position,_swipeDirection);
            _swipeDirection = Vector2Int.zero;
        }
        else if(Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
        {
            _board.SelectNut(Position);
        }
        GObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
    }
}
