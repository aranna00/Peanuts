using UnityEngine;

public class Peanut : MonoBehaviour
{
    public static readonly string[] NutTypes = {"Almond", "Brazil", "Cashew", "Coconut", "Hazelnut", "Peanut", "Pecan", "Pine", "Pistachio", "Walnut"};

    public string _type;

    public string Type
    {
        get { return _type; }
    }

    public GameObject GObject { get; set; }

    public void Setup()
    {
        _type = NutTypes[Random.Range(0, NutTypes.Length)];
    }
    public void Setup(int seed)
    {
        Random.seed = seed;
        _type = NutTypes[Random.Range(0, NutTypes.Length)];
    }
}
