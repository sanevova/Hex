using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {
    private AudioSource music;
    private AudioClip[] musicClips;
    private int nextMusicClipIndex;

    void Start() {
        Random.InitState(System.DateTime.Now.Millisecond);
        music = GetComponent<AudioSource>();
        musicClips = Resources.LoadAll<AudioClip>("Sounds/Music");
        nextMusicClipIndex = Random.Range(0, musicClips.Length);

    }

    void Update() {
        if (music.isPlaying) {
            if (Input.GetKeyDown(KeyCode.N)) {
                PlayNextMusicClip();
            }
            return;
        }
        PlayNextMusicClip();
    }

    void PlayNextMusicClip() {
        music.clip = musicClips[nextMusicClipIndex];
        nextMusicClipIndex = (nextMusicClipIndex + 1) % musicClips.Length;
        music.PlayDelayed(0.2f); // 0.2s delay
        // AudioSource.PlayClipAtPoint(music.clip, transform.position);
    }
}
