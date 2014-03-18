using UnityEngine;
using System.Collections;

public class NPC : Movement {
    
    public bool moveAround = true;
    
    public int timeBetweenMoveMin = 5;
    
    public int timeBetweenMoveMax = 60;
    
    public int moveRange = 5;
    
    public bool hasChallenge = false;
    
    public enum Difficulty {
        One, Two, Three, Four, Five, Six, Seven, Eight, Nine  
    }
    
    public Difficulty difficulty = Difficulty.One;
    
    public string[] conversation;
    
    public string[] wonTheChallenge;
    
    public string[] lostTheChallenge;
    
    private int nextMove = 0;
    
    private Vector2 rangeMin;
    
    private Vector2 rangeMax;
    
    private bool isColliding = false;
    
    public bool defeated = false;
    
    public bool canHeal = false;
    
    public string[] beforeChallenge;

	void Awake() {
        this.nextMove = Random.Range(this.timeBetweenMoveMin, this.timeBetweenMoveMax) + GameEventManager.unixtime();
        this.rangeMin = new Vector2(this.transform.position.x - this.moveRange, this.transform.position.y - this.moveRange);
        this.rangeMax = new Vector2(this.transform.position.x + this.moveRange, this.transform.position.y + this.moveRange);
		GameEventManager.GameStart += GameStart;
        GameEventManager.GamePause += GamePause;
        GameEventManager.GameResume += GameResume;
        GameEventManager.GameOver += GameOver;
        GameEventManager.GameSave += GameSave;
        GameEventManager.GameLoad += GameLoad;
	}
    
    void OnDestroy() {
		GameEventManager.GameStart -= GameStart;
        GameEventManager.GameOver -= GameOver;
        GameEventManager.GamePause -= GamePause;
        GameEventManager.GameResume -= GameResume;
        GameEventManager.GameSave -= GameSave;
        GameEventManager.GameLoad -= GameLoad;
    }
    
    private void GameSave() {
        if (this.defeated) {
            PlayerPrefs.SetInt(this.name + " Defeated", 1);
        } else {
            PlayerPrefs.SetInt(this.name + " Defeated", 0);
        }
    }
    
    private void GameLoad() {
        if (PlayerPrefs.GetInt(this.name + " Defeated") == 1) {
            this.defeated = true;
        } else {
            this.defeated = false;
        }
    }
    
	private void GameStart() {
        this.enabled = true;
	}
    
	private void GameOver() {
		this.enabled = false;
	}
    
	private void GamePause() {
		this.enabled = false;
	}
    
	private void GameResume() {
		this.enabled = true;
	}
    
    void Update() {
        randomMovement();
    }
    
    void randomMovement() {
        if (!this.isColliding && this.moveAround && GameEventManager.unixtime() > this.nextMove) {
            int typeOfMove = Random.Range(0, 2);
            if (typeOfMove == 0) {
                int moveX = Random.Range(0, 2);
                if (moveX == 0) {
                    moveX = -1;
                } else {
                    moveX = 1;
                }
                // If the movement is outside of the range, move in the opposite direction
                if (this.transform.position.x + moveX > this.rangeMax.x || this.transform.position.x + moveX < this.rangeMin.x) {
                    moveX = moveX * -1;
                }
                move(new Vector2(moveX, 0));
            } else {
                int moveY = Random.Range(0, 2);
                if (moveY == 0) {
                    moveY = -1;
                } else {
                    moveY = 1;
                }
                // If the movement is outside of the range, move in the opposite direction
                if (this.transform.position.y + moveY > this.rangeMax.y || this.transform.position.y + moveY < this.rangeMin.y) {
                    moveY = moveY * -1;
                }
                move(new Vector2(0, moveY));
            }
            this.nextMove = Random.Range(this.timeBetweenMoveMin, this.timeBetweenMoveMax) + GameEventManager.unixtime();
        }
    }
    
    void OnCollisionEnter2D(Collision2D other) {
        if (other.rigidbody != null && other.rigidbody.tag == "Player") {
            this.isColliding = true;
            // Turn towards the player
            if ((int)other.transform.position.x == (int)this.transform.position.x) {
                // On the same x axis, look towards the y
                int moveY = 1;
                if (other.transform.position.y < this.transform.position.y) {
                    moveY = -1;
                }
                this.lastMove = new Vector2(0, moveY);
                setCharacterTexture(this.lastMove, 0);
            } else {
                // On the same y axis, look towards the x
                int moveX = 1;
                if (other.transform.position.x < this.transform.position.x) {
                    moveX = -1;
                }
                this.lastMove = new Vector2(moveX, 0);
                setCharacterTexture(this.lastMove, 0);
            }
            // Speak to him
            if (this.conversation.Length > 0) {
                StartCoroutine(GameEventManager.conversation.speak(this.conversation));
            }
            // Initiate the challenge (if there is one)
            if (this.hasChallenge && !this.defeated) {
                if (this.beforeChallenge.Length > 0) {
                    StartCoroutine(GameEventManager.conversation.speak(this.beforeChallenge));
                }
                StartCoroutine(GameEventManager.challenge.activate(this, (int)difficulty, this.wonTheChallenge, this.lostTheChallenge));
            }
            if (this.canHeal) {
                GameEventManager.player.hp(GameEventManager.player.maxHP());
            }
        }
    }
    
    void OnCollisionExit2D(Collision2D other) {
        if (other.rigidbody != null && other.rigidbody.tag == "Player") {
            this.isColliding = false;
        }
    }
}
