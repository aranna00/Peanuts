using UnityEngine;
using UnityEngine.XR.WSA;

public class Peanut : MonoBehaviour
{
    public static readonly string[] NutTypes = {"Almond", "Brazil", "Cashew", "Coconut", "Hazelnut", "Peanut", "Pecan", "Pine", "Pistachio", "Walnut"};

    public string _type;

    public Vector2Int Position;

    public string Type
    {
        get { return _type; }
    }

    public GameObject GObject { get; set; }

    public void Setup()
    {
        _type = NutTypes[Random.Range(0, NutTypes.Length)];
    }
    public void Setup(int nutNumber)
    {
        _type = NutTypes[nutNumber];
    }

    private void OnMouseDown()
    {
        Board board = GameObject.Find("Board").GetComponent<Board>();
        board.SelectNut(Position);
    }
}
