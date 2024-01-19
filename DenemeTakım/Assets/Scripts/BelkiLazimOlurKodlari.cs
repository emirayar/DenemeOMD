/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BelkiLazimOlurKodlari : MonoBehaviour
{
    public AudioClip[] audioClips;
    private int currentAudioClipsIndex = 0;
    
    void AttackSounds()
    {
        if (audioClips.Length > 0)
        {
            AudioSource.PlayClipAtPoint(audioClips[currentAudioClipsIndex], transform.position);

            currentAudioClipsIndex = (currentAudioClipsIndex + 1) % audioClips.Length;
        }
    }
}
*/

//isGrounded = playerMovement.rb.velocity.y <= 0;

/*
    // Dodge ozellikleri
    public float dodgeDistance = 0.5f;
    public float dodgeDuration = 0.1f;

    // Dodge kontrol degiskenleri
    private bool isDodging;
    private bool canDodge = true;


     // Dodge girisini kontrol etme metodu
    void CheckDodgeInput()
    {
        // "Dodge" tusuna basildiginda ve dodge kullanilabilir durumdaysa
        if (Input.GetButtonDown("Dodge") && canDodge)
        {
            // Dodge Coroutine'ini baslat
            StartCoroutine(Dodge());
        }
    }
    // Dodge islemini gerceklestiren Coroutine metodu
    IEnumerator Dodge()
    {
        // Dodge animasyonunu baslat ve dodge durumunu aktiflestir
        isDodging = true;
        canDodge = false;

        // Dodge yonunu belirle
        float dodgeDirection = isFacingRight ? -1f : 1f;
        Vector2 dodgeVelocity = new Vector2(dodgeDirection, 0f).normalized;

        // Dodge hareketini hesapla
        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + dodgeVelocity * dodgeDistance;

        // Dodge suresince linner interpolasyon kullanarak hareket et
        float startTime = Time.time;
        while (Time.time < startTime + dodgeDuration)
        {
            float t = (Time.time - startTime) / dodgeDuration;
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
            yield return null;
        }

        // Dodge durumunu kapat ve bir sonraki Dodge için bekle
        isDodging = false;
        yield return new WaitForSeconds(1f);
        canDodge = true;
    } */

