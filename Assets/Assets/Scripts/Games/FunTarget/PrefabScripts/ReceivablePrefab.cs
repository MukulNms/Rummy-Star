using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ReceivablePrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI From;
    [SerializeField] private TextMeshProUGUI To;
    [SerializeField] private TextMeshProUGUI Date;
    [SerializeField] private TextMeshProUGUI Amount;

    public Button Reject;
    public Button Accept;
    public void SetData(string from,string to,string amount,string date)
    {
        From.text = from;
        To.text = to;
        Amount.text = amount;
        Date.text = date;
    }
}
