using UnityEngine;
using TMPro;

public class LogManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    private float displayDuration = 2f;
    private float displayTimer;

    void Update()
    {
        if (displayTimer > 0)
        {
            displayTimer -= Time.deltaTime;

            if (displayTimer <= 0)
            {
                ClearLog();
            }
        }
    }

    public void Log(string message)
    {
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.text += message;
            
            displayTimer = displayDuration;
        }
    }
    private void ClearLog()
    {
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.text = "";
        }
    }
}
