using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	
	protected bool isMoving = false;
	
	protected bool inCollision = false;

	protected bool teleporting = false;
    
    protected bool waitedAfterTeleport = false;
    
    protected Vector2 lookingDirection = new Vector2(0, 0);
	
	protected float t;
	
	protected Vector2 lastMove;
	
	protected Vector2 collidingMove;

	public bool canTeleport = false;
    
    public bool playsSoundOnMovement = false;

	public float stepsPerMove = 8f;
    
    public float stepsPerMoveIOS = 10f;
	
	public float gridSize = 2f;

	public int touchOffset = 150;
	
	public Texture2D[] downMovement = new Texture2D[3];
	
	public Texture2D[] upMovement = new Texture2D[3];
	
	public Texture2D[] leftMovement = new Texture2D[3];
	
	public Texture2D[] rightMovement = new Texture2D[3];

	// Detect the input of the player and move the player character accordingly
	public void move(Vector2 movement) {
		// Make sure moves doesn't "stack"
		if (!this.isMoving) {
			if (movement != Vector2.zero) {
				StartCoroutine(doMovement(movement, this.transform));
			} else {
				// The player is standing still, so, we reset his stance
				setCharacterTexture(this.lastMove, 0);
			}
		}
	}
	
	// Move the player character/gameobject around
	public IEnumerator doMovement(Vector2 movement, Transform transform) {
		Vector2 startPosition = transform.position;
		this.isMoving = true;
		this.lastMove = movement;
		this.t = 0;
        
		var targetPosition = new Vector2(
			startPosition.x + System.Math.Sign(movement.x) * this.gridSize,
			startPosition.y + System.Math.Sign(movement.y) * this.gridSize
			);
		setCharacterTexture(movement, 1);
		while (this.t < 0.5f) {
			if (this.inCollision && this.collidingMove == this.lastMove || this.teleporting) {
				break;
			}
            #if UNITY_IPHONE
                this.t += (this.stepsPerMoveIOS / this.gridSize) * Time.deltaTime;
            #else
                this.t += (this.stepsPerMove / this.gridSize) * Time.deltaTime;
            #endif
			this.transform.position = Vector2.Lerp(startPosition, targetPosition, this.t);
			yield return null;
		}
		setCharacterTexture(movement, 2);
		if (this.collidingMove != this.lastMove && !this.teleporting) {
            if (this.playsSoundOnMovement && GameEventManager.currentGround != null) {
                GameEventManager.currentGround.play();
            }
		}
        #if UNITY_IPHONE
            float tempT = (this.stepsPerMoveIOS / this.gridSize) * Time.deltaTime;
        #else
            float tempT = (this.stepsPerMove / this.gridSize) * Time.deltaTime;
        #endif
		while (this.t < (1f - tempT)) {
			if (this.inCollision && this.collidingMove == this.lastMove || this.teleporting) {
				break;
			}
            #if UNITY_IPHONE
                this.t += (this.stepsPerMoveIOS / this.gridSize) * Time.deltaTime;
            #else
                this.t += (this.stepsPerMove / this.gridSize) * Time.deltaTime;
            #endif
			this.transform.position = Vector2.Lerp(startPosition, targetPosition, this.t);
			yield return null;
		}
        // Avoid the player walking directly back into the teleporter, by giving movement a little delay
        if (this.teleporting && !this.waitedAfterTeleport) {
            this.lastMove = this.lookingDirection;
            setCharacterTexture(this.lookingDirection, 0);
            yield return new WaitForSeconds(0.5f);
            this.waitedAfterTeleport = true;
            this.isMoving = false;
        } else {
            // For a more smooth continues walk, we allow the input from the next frame, 1 frame before the walk is done
            this.isMoving = false;
            if (this.collidingMove != this.lastMove && !this.teleporting) {
                #if UNITY_IPHONE
                    this.t += (this.stepsPerMoveIOS / this.gridSize) * Time.deltaTime;
                #else
                    this.t += (this.stepsPerMove / this.gridSize) * Time.deltaTime;
                #endif
    			this.transform.position = Vector2.Lerp(startPosition, targetPosition, this.t);
            }
        }
        
        this.teleporting = false;
		yield return 0;
	}
	
	// Set the texture of the player. State index 0 is standing still, 1 and 2 are movements
	protected void setCharacterTexture(Vector2 movement, int state) {
		if (System.Math.Sign(movement.x) == -1) {
			this.renderer.material.mainTexture = leftMovement[state];
		} else if (System.Math.Sign(movement.x) == 1) {
			this.renderer.material.mainTexture = rightMovement[state];
		} else if (System.Math.Sign(movement.y) == -1) {
			this.renderer.material.mainTexture = downMovement[state];
		} else if (System.Math.Sign(movement.y) == 1) {
			this.renderer.material.mainTexture = upMovement[state];
		}
	}
	
	// Check if the player is colliding with an object (i.e. buildings etc)
	void OnCollisionEnter2D(Collision2D collision) {
		if (this.collidingMove == Vector2.zero) {
			this.collidingMove = this.lastMove;
		}
		this.inCollision = true;
	}
	
	// Check when the player leaves the collision
	void OnCollisionExit2D() {
		this.inCollision = false;
		this.collidingMove = Vector2.zero;
	}

	public void teleport(Teleport tele) {
		if (this.canTeleport) {
			this.teleporting = true;
            this.waitedAfterTeleport = false;
			this.transform.position = tele.destination.position;
            this.lookingDirection = tele.lookingDirection();
		}
	}
	
}
