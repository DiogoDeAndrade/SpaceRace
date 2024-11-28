using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    [SerializeField] Image      highlighterImage;
    [SerializeField] UIControl  _navUp;
    [SerializeField] UIControl  _navDown;

    static UIControl selectedControl;

    public static UIControl currentlySelected => selectedControl;

    public bool isSelected => selectedControl == this;
    public UIControl navUp => _navUp;
    public UIControl navDown => _navDown;

    void Update()
    {
        highlighterImage.enabled = isSelected;
    }

    public void Select()
    {
        selectedControl = this;
    }

    public void Deselect()
    {
        selectedControl = null;
    }
}
