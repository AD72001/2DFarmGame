using UnityEngine;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour
{
    public GameObject UIObject;
    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(SetState);
    }

    public void SetState()
    {
        UIObject.SetActive(!UIObject.activeSelf);
    }
}
