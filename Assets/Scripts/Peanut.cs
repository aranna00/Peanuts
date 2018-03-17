using UnityEngine;
using UnityEngine.XR.WSA;

public class Peanut : MonoBehaviour
{
    public static readonly string[] NutTypes = {"Almond", "Brazil", "Cashew", "Coconut", "Hazelnut", "Peanut", "Pecan", "Pine", "Pistachio", "Walnut"};

    // Peanut properties
    private string _type;
    public Vector2Int Position;

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

    public void Setup()
    {
        Setup(Random.Range(0, NutTypes.Length));
    }
    
    public void Setup(int nutNumber)
    {
        _type = NutTypes[nutNumber];
        _board = GameObject.Find("Board").GetComponent<Board>();
    }

    private void OnMouseDown()
    {
        _touchPosition = Input.mousePosition;
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
    }
}
