using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject levelCompleteUI;
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private GameObject levelTransitionEffect;

    private void Awake()
    {
        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(false);
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnEnable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelComplete += ShowLevelCompleteUI;
            LevelManager.Instance.OnLevelFailed += ShowLevelFailedUI;
        }
    }

    private void OnDisable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelComplete -= ShowLevelCompleteUI;
            LevelManager.Instance.OnLevelFailed -= ShowLevelFailedUI;
        }
    }

    private void ShowLevelCompleteUI()
    {
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(true);
            if (levelCompleteText != null)
                levelCompleteText.text = $"Level {LevelManager.Instance.CurrentLevel} Complete!";
            if (rewardText != null)
                rewardText.text = $"Rewards:\nMoney: +{LevelManager.Instance.CurrentLevelData.completionReward}";
            if (nextLevelButton != null)
                nextLevelButton.gameObject.SetActive(LevelManager.Instance.CurrentLevel < LevelManager.Instance.MaxLevel);
        }
    }

    private void ShowLevelFailedUI()
    {
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(true);
            if (levelCompleteText != null)
                levelCompleteText.text = "Level Failed!";
            if (rewardText != null)
                rewardText.text = "Try again!";
            if (nextLevelButton != null)
                nextLevelButton.gameObject.SetActive(false);
        }
    }

    private void OnNextLevelClicked()
    {
        LevelManager.Instance?.NextLevel();
    }

    private void OnRetryClicked()
    {
        LevelManager.Instance?.RestartLevel();
    }

    private void OnMainMenuClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
} 