using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isBlocking = false; // Blok durumu
    private Stamina stamina; // Stamina scriptine eriþim
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        stamina = GetComponent<Stamina>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Blocking();
        if (stamina.currentStamina < 15f)
        {
            StopBlocking();
        }
    }

    void Blocking()
    {
        if (Input.GetKey(KeyCode.B))
        {
            isBlocking = true;
            animator.SetBool("isBlocking", true);
        }
        else
        {
            isBlocking = false;
            animator.SetBool("isBlocking", false);
        }
    }
    void StopBlocking()
    {
        isBlocking = false;
        animator.SetBool("isBlocking", false);
    }

    public void Parry()
    {
        if (Input.GetKey(KeyCode.B))
        {
            Debug.Log("Parry!");
            isBlocking = true;
            animator.SetBool("isBlocking", true);
        }
        else
        {
            isBlocking = false;
            animator.SetBool("isBlocking", false);
        }
    }
}
