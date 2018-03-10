using UnityEngine;

public class Peanut : MonoBehaviour
{

    private static readonly string[] _types = {"Almond", "Brazil", "Cashew", "Coconut", "Hazelnut", "Peanut", "Pecan", "Pine", "Pistachio", "Walnut"};

    public string _type;

    public string Type
    {
        get { return _type; }
    }

    public void Setup()
    {
        _type = _types[Random.Range(0, _types.Length)];
    }
}
