using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour {

	public Transform destination;
    
    public int repNeededToUse = 0;
    
    public NPC npcThatUnlocks = null;
    
    public string npcMustBeDefatedMsg = "...";
    
    public enum Direction {
        Left, Right, Up, Down
    }
    
    public Direction endingPosition;

	// Teleport the player to the destination when he enters the trigger area
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
            if (this.npcThatUnlocks != null) {
                if (this.npcThatUnlocks.defeated) {
                    other.SendMessageUpwards("teleport", this);
                } else {
                    string[] text = new string[] { this.npcMustBeDefatedMsg };
                    StartCoroutine(GameEventManager.conversation.speak(text));
                }
            } else {
                if (GameEventManager.player.reputation() >= this.repNeededToUse) {
                    other.SendMessageUpwards("teleport", this);
                } else {
                    string[] text = new string[] { "Sorry, You need " + this.repNeededToUse + " reputation to go here!" };
                    StartCoroutine(GameEventManager.conversation.speak(text));
                }
            } 
		}
	}
    
    public Vector2 lookingDirection() {
        if (this.endingPosition == Direction.Left) {
            return new Vector2(-1, 0);
        } else if (this.endingPosition == Direction.Right) {
            return new Vector2(1, 0);
        }  else if (this.endingPosition == Direction.Up) {
            return new Vector2(0, 1);
        }  else {
            return new Vector2(0, -1);
        } 
    }
}
