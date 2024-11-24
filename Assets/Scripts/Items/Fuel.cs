using TMPro;
using UnityEngine;

public class Fuel : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private float  _energy;
    [SerializeField] private float  _ammount;
    [SerializeField] private int    _score = 50;
    [SerializeField] private Color  _color = Color.white;

    public Sprite sprite => _sprite;
    public float energy => _energy;
    public float ammount => _ammount;
    public int score => _score;
    public Color color => _color;
}
