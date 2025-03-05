using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoldButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlotItem soldItem;
    public TMP_Text infoText;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateInfo();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void UpdateInfo()
    {
        infoText.text = $"{ShopManager.Instance.SellPrices[soldItem.ID]}\n{GameManager.Instance.harvestProducts[soldItem.ID]}";
    }

    void ButtonClick()
    {
        Debug.Log("Button Clicked");
        ShopManager.Instance.SoldItem(soldItem);
    }
}
