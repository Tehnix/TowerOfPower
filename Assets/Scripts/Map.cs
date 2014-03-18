using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {
    
    public string mapName = "Unknown Area";
    
    public bool spawnItems = false;
    
    public Item[] items;
    
    public int spawnTimeRangeMin = 30;
    
    public int spawnTimeRangeMax = 120;
    
    private int nextItemSpawn = 0;
    
    private bool isSpawningItem = false;
    
    private float planMinX;
    
    private float planMaxX;
    
    private float planMinY;
    
    private float planMaxY;
    
    void Awake() {
        this.nextItemSpawn = Random.Range(spawnTimeRangeMin, spawnTimeRangeMax) + GameEventManager.unixtime();
        this.planMinX = this.renderer.bounds.min.x;
        this.planMaxX = this.renderer.bounds.max.x;
        this.planMinY = this.renderer.bounds.min.y;
        this.planMaxY = this.renderer.bounds.max.y;
    }
    
    void Update() {
        if (this.spawnItems && !this.isSpawningItem && GameEventManager.unixtime() > this.nextItemSpawn) {
            StartCoroutine(spawnItem());
        }
    }
    
    IEnumerator spawnItem() {
        this.isSpawningItem = true;
        Item item = items[Random.Range(0, items.Length)];
        if (Random.Range(0, 100) < item.changeToSpawn) {
            Vector2 target = new Vector2(
                Random.Range(planMinX, planMaxX),
                Random.Range(planMinY, planMaxY)
            );
            
            // Check if it's spawned too close to a blocking object
            bool tryToSpawn = true;
            while (tryToSpawn) {
                Collider2D[] overlaps = Physics2D.OverlapCircleAll(target, 1f);
                bool collided = false;
                foreach (Collider2D obj in overlaps) {
                    if (obj.tag == "Blocking" || obj.tag == "Teleport") {
                        collided = true;
                        break;
                    }
                }
                if (!collided) {
                    tryToSpawn = false;
                    break;
                }
                target = new Vector2(
                    (int)Random.Range(planMinX, planMaxX),
                    (int)Random.Range(planMinY, planMaxY)
                );
                yield return null;
            }
            Instantiate(item, target, Quaternion.identity);
        }
        this.nextItemSpawn = GameEventManager.unixtime() + Random.Range(spawnTimeRangeMin, spawnTimeRangeMax);
        this.isSpawningItem = false;
        yield return 0;
    }
    
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			GameEventManager.currentMap = this;
            audio.Play();
            GameEventManager.camera.centerIfTooSmallMap();
		}
	}
    
	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
            audio.Pause();
		}
	}
}
