using UnityEngine;
using TMPro;

public class LogManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    [SerializeField]
    private AudioClip logSound;

    private float displayDuration = 2f;
    private float displayTimer;
    private bool isDisplayingLog = false;
    void Start()
    {
    }
    void Update()
    {
        if (isDisplayingLog)
        {
            if (displayTimer > 0)
            {
                displayTimer -= Time.deltaTime;
            }
            else
            {
                ClearLog();
                isDisplayingLog = false;
            }
        }
    }

    public void Log(string message)
    {
        if (textMeshProUGUI != null)
        {
            if (isDisplayingLog)
            {
                ClearLog();
            }

            textMeshProUGUI.text += message;
            displayTimer = displayDuration;
            isDisplayingLog = true;
            AudioSource.PlayClipAtPoint(logSound, transform.position);
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
