using UnityEngine;

public class ControllerArrow : MonoBehaviour
{
    public GameObject player;
    public float distance = 0.3f;

    private SpriteRenderer arrowRenderer;

    void Start()
    {
        arrowRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector3 offset = Vector3.zero;
        KeyCode[] keys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
        Vector3[] directions = { Vector3.up, Vector3.left, Vector3.down, Vector3.right };

        bool anyKeyIsPressed = false; // Herhangi bir tuşa basıldı mı?

        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKey(keys[i]))
            {
                offset += directions[i] * distance;
                anyKeyIsPressed = true; // Tuşa basıldığında bu değeri true yap
            }
        }

        if (anyKeyIsPressed)
        {
            // Herhangi bir tuşa basıldığında okun pozisyonunu güncelle
            transform.position = player.transform.position + offset;

            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            angle += player.transform.localScale.x < 0 ? 180f : 0f;

            arrowRenderer.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

            // Oku göster
            arrowRenderer.enabled = true;
        }
        else
        {
            // Hiçbir tuşa basılmadığında oku gizle
            arrowRenderer.enabled = false;
        }
    }
}
