using UnityEngine;
using System.Collections;

public class GroundAudio : MonoBehaviour {

    private AudioSource[] audios;
    
    private int lastRandom = -1;
    
    public int priority = 1;
    
    void Awake() {
        this.audios = GetComponents<AudioSource>();
    }
    
    public void play() {
        int i = Random.Range(0, this.audios.Length);
        if (i == this.lastRandom && i != this.audios.Length - 1) {
            i += 1;
        } else if (i == this.lastRandom && i == this.audios.Length - 1) {
            i -= 1;
        }
        this.audios[i].Play();
        this.lastRandom = i;
    }
    
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "PlayerFeet") {
			GameEventManager.currentGround = this;
		}
	}
}
