using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

    public string itemName = "Unknown Item";
    
    public int givesHP = 0;
    
    public int givesMaxHP = 0;
    
    public int givesRep = 0;
    
    public int givesXP = 0;
    
    public int changeToSpawn = 100;
    
    public int lifeTime = 40;
    
    private int timeOfDestruction;
    
    private float step = 1f;
    
    private float hoverOffset;
    
    private float scaleX;
    
    private float scaleY;
    
    void Awake() {
        this.timeOfDestruction = GameEventManager.unixtime() + lifeTime;
        this.hoverOffset = this.transform.position.y + 0.5f;
        this.scaleX = this.transform.localScale.x;
        this.scaleY = this.transform.localScale.y;
    }
    
    void Update() {
        if (GameEventManager.unixtime() > this.timeOfDestruction) {
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(this.transform.position, 10f);
            bool playerFound = false;
            foreach (Collider2D obj in overlaps) {
                if (obj.tag == "Player") {
                    playerFound = true;
                    break;
                }
            }
            if (playerFound) {
                this.timeOfDestruction += 10;
            } else {
                transform.position = new Vector3(0, 0, -100);
                gameObject.SetActive(false);
                UnityEngine.Object.Destroy(this);
            }
        }
    }
    
    void FixedUpdate() {
        // Make the item hover if it's visible
        if (this.renderer.isVisible) {
            this.step += 0.05f;
            if (this.step > 999) { 
                this.step = 1;
            }
            float sinHover = Mathf.Sin(this.step) * 0.2f;
            this.transform.position = new Vector2(this.transform.position.x, sinHover + this.hoverOffset);
            this.transform.localScale = new Vector3(
                this.scaleX + sinHover * 0.5f,
                this.scaleY + sinHover * 0.5f,
                this.transform.localScale.z  
            );
        }
    }
    
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
            audio.Play();
			other.SendMessageUpwards("consumedItem", this);
            StartCoroutine(DestroyAfterAudio());
		}
	}
    
    IEnumerator DestroyAfterAudio() {
        // Hide it away
        transform.position = new Vector3(0, 0, -100);
        while(audio.isPlaying) {
            yield return null;
        }
        gameObject.SetActive(false);
        UnityEngine.Object.Destroy(this);
        yield return 0;
    }
}
