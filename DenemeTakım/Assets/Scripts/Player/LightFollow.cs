using UnityEngine;

public class LightFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f; 

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
