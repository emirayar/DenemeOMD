using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public float viewRadius; // G�r�� yar��ap�
    [Range(0, 360)]
    public float viewAngle; // G�r�� a��s�

    public LayerMask targetMask; // Hedef nesnelerin katman maskesi
    public LayerMask obstacleMask; // Engellerin katman maskesi

    public List<Transform> visibleTargets = new List<Transform>(); // G�r�� alan�ndaki hedefleri tutan liste

    private MeleeEnemyAI meleeEnemyAI; // MeleeEnemyAI scriptine eri�mek i�in referans

    void Start()
    {
        meleeEnemyAI = GetComponent<MeleeEnemyAI>(); // MeleeEnemyAI scriptine eri�imi al
    }

    void Update()
    {
        FindVisibleTargets(); // G�r�� alan�ndaki hedefleri bul
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear(); // �nceki bulunan hedefleri temizle
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask); // G�r�� yar��ap� i�indeki hedefleri al

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform; // Hedefin transformu
            Vector2 dirToTarget = (target.position - transform.position).normalized; // Karakterden hedefe do�ru vekt�r

            // Hedef karakterin g�r�� a��s�nda m�?
            if (meleeEnemyAI.isFacingRight && Vector2.Angle(transform.right, dirToTarget) < viewAngle / 2 || !meleeEnemyAI.isFacingRight && Vector2.Angle(-transform.right, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector2.Distance(transform.position, target.position); // Karakterden hedefe mesafe

                // Engelleri kontrol et
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target); // Engel yoksa hedefi g�r�n�r hedeflere ekle
                    meleeEnemyAI.isAggressive = true;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        foreach (Transform visibleTarget in visibleTargets)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, visibleTarget.position);
        }
    }
}
