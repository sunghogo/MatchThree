using UnityEngine;
using TMPro;

public class UpdatedTMP : MonoBehaviour
{
    TMP_Text tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (gameObject.CompareTag("Score"))
        {
            if (GameManager.Instance) tmp.text = GameManager.Instance.Score.ToString();
            GameManager.OnScoreChanged += UpdateText;
        }
        else if (gameObject.CompareTag("High Score"))
        {
            if (GameManager.Instance) tmp.text = GameManager.Instance.HighScore.ToString();
            GameManager.OnHighScoreChanged += UpdateText;
        }
        else if (gameObject.CompareTag("Moves"))
        {
            if (GameManager.Instance) tmp.text = GameManager.Instance.Moves.ToString();
            GameManager.OnMovesChanged += UpdateText;
        }

        if (!gameObject.CompareTag("High Score")) GameManager.OnScreenStart += HandleScreenStart;
        GameManager.OnGameStart += HandleGameStart;
    }

    void OnDestroy()
    {
        if (gameObject.CompareTag("Score")) GameManager.OnScoreChanged -= UpdateText;
        else if (gameObject.CompareTag("High Score")) GameManager.OnHighScoreChanged -= UpdateText;
        else if (gameObject.CompareTag("Moves")) GameManager.OnMovesChanged -= UpdateText;

        if (!gameObject.CompareTag("High Score")) GameManager.OnScreenStart -= HandleScreenStart;
        GameManager.OnGameStart -= HandleGameStart;
    }

    void UpdateText(int number)
    {
        tmp.text = $"{gameObject.tag}: {number}";
    }

    void HandleGameStart()
    {
        tmp.enabled = true;
    }

    void HandleScreenStart()
    { 
        tmp.enabled = false;
    }
}
