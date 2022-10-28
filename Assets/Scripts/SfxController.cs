using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxController : MonoBehaviour {
    public AudioClip turnSFX;
    public AudioClip placementSFX;
    public float volume;


    void Start() {
    }

    void Update() {
    }

    public void OnTurn(TargetTile tile) {
        AudioSource.PlayClipAtPoint(turnSFX, tile.transform.position, volume);
    }

    public void OnPlace(TargetTile tile) {
        AudioSource.PlayClipAtPoint(placementSFX, tile.transform.position, volume);
    }


}
