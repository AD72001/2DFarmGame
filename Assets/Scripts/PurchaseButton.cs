using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlotItem purchaseItem;
    public int numberOfPurchases = 1;

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
        infoText.text = $"{ShopManager.Instance.PurchasePrices[purchaseItem.ID]}/{numberOfPurchases}";
    }

    void ButtonClick()
    {
        Debug.Log("Purchase Button Clicked");
        ShopManager.Instance.PurchaseItem(purchaseItem, numberOfPurchases);
    }   
}
