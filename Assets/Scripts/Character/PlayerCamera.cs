using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
    
    public GameObject player;
    
    void Awake() {
        GameEventManager.camera = this;
    }
    
	void LateUpdate () {
        smartFollowPlayer();
	}
    
    // Keep the camera inside of the world/map
    void smartFollowPlayer() {
        if (GameEventManager.currentMap != null) {
            float posX = player.transform.position.x;
            float posY = player.transform.position.y;
            float planMinX = GameEventManager.currentMap.renderer.bounds.min.x;
            float planMaxX = GameEventManager.currentMap.renderer.bounds.max.x;
            float planMinY = GameEventManager.currentMap.renderer.bounds.min.y;
            float planMaxY = GameEventManager.currentMap.renderer.bounds.max.y;
            Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
            Vector3 leftBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
            
            if (leftBottom.x <= planMinX && posX < this.transform.position.x) {
                posX = this.transform.position.x;
            } else if (topRight.x >= planMaxX && posX > this.transform.position.x) {
                posX = this.transform.position.x;
            }
            if (leftBottom.y <= planMinY && posY < this.transform.position.y) {
                posY = this.transform.position.y;
            } else if (topRight.y >= planMaxY && posY > this.transform.position.y) {
                posY = this.transform.position.y;
            }
            this.transform.position = new Vector3(posX, posY, -10);
        }
    }
    
    // If the map is smaller than the camera view area, center the camera to the map
    public void centerIfTooSmallMap() {
        if (GameEventManager.currentMap != null) {
            float planMinX = GameEventManager.currentMap.renderer.bounds.min.x;
            float planMaxX = GameEventManager.currentMap.renderer.bounds.max.x;
            float planMinY = GameEventManager.currentMap.renderer.bounds.min.y;
            float planMaxY = GameEventManager.currentMap.renderer.bounds.max.y;
            Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
            Vector3 leftBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
            if (leftBottom.x < planMinX || topRight.x > planMaxX) {
                this.transform.position = new Vector3(
                    GameEventManager.currentMap.renderer.bounds.center.x, 
                    Camera.main.transform.position.y, 
                    -10
                );
            }
            if (leftBottom.y < planMinY || topRight.y > planMaxY) {
                this.transform.position = new Vector3(
                    Camera.main.transform.position.x, 
                    GameEventManager.currentMap.renderer.bounds.center.y,
                    -10
                );
            }
        }
    }
}
