using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public float viewRadius; // Görüþ yarýçapý
    [Range(0, 360)]
    public float viewAngle; // Görüþ açýsý

    public LayerMask targetMask; // Hedef nesnelerin katman maskesi
    public LayerMask obstacleMask; // Engellerin katman maskesi

    public List<Transform> visibleTargets = new List<Transform>(); // Görüþ alanýndaki hedefleri tutan liste

    private MeleeEnemyAI meleeEnemyAI; // MeleeEnemyAI scriptine eriþmek için referans

    void Start()
    {
        meleeEnemyAI = GetComponent<MeleeEnemyAI>(); // MeleeEnemyAI scriptine eriþimi al
    }

    void Update()
    {
        FindVisibleTargets(); // Görüþ alanýndaki hedefleri bul
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear(); // Önceki bulunan hedefleri temizle
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask); // Görüþ yarýçapý içindeki hedefleri al

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform; // Hedefin transformu
            Vector2 dirToTarget = (target.position - transform.position).normalized; // Karakterden hedefe doðru vektör

            // Hedef karakterin görüþ açýsýnda mý?
            if (meleeEnemyAI.isFacingRight && Vector2.Angle(transform.right, dirToTarget) < viewAngle / 2 || !meleeEnemyAI.isFacingRight && Vector2.Angle(-transform.right, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector2.Distance(transform.position, target.position); // Karakterden hedefe mesafe

                // Engelleri kontrol et
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target); // Engel yoksa hedefi görünür hedeflere ekle
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
