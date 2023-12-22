using System.Collections;
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
