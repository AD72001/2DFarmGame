using UnityEngine;

public class Lock : MonoBehaviour
{
    private void Start() {
        GetComponent<SpriteRenderer>().sortingOrder = int.Parse( 
            transform.parent.name.Split('_')[1]
        );
    }

    public void PurchasePlot()
    {
        if (GameManager.Instance.Money >= GameManager.Instance.plotPrice)
        {
            GameManager.Instance.Money -= GameManager.Instance.plotPrice;
            GetComponentInParent<Plot>().Locked = false;
            gameObject.SetActive(false);
        }
        else {
            Debug.Log("Not enough money to purchase plot!");
        }
    }
}
