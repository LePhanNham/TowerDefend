using UnityEngine;
using TMPro;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject goText;
    
    private void Start()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
        if (readyText != null)
            readyText.SetActive(false);
        if (goText != null)
            goText.SetActive(false);
    }

    public void ShowCountdown(int number)
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = number.ToString();
        }
    }

    public void ShowReady()
    {
        if (readyText != null)
        {
            readyText.SetActive(true);
            countdownText.gameObject.SetActive(false);
        }
    }

    public void ShowGo()
    {
        if (goText != null)
        {
            goText.SetActive(true);
            readyText.SetActive(false);
        }
    }

    public void HideAll()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
        if (readyText != null)
            readyText.SetActive(false);
        if (goText != null)
            goText.SetActive(false);
    }
} 