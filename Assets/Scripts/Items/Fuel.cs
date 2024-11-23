using TMPro;
using UnityEngine;

public class Fuel : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private float  _energy;
    [SerializeField] private float  _ammount;
    [SerializeField] private Color  _color = Color.white;

    public Sprite sprite => _sprite;
    public float energy => _energy;
    public float ammount => _ammount;
    public Color color => _color;
}
