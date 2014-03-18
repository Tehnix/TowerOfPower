using UnityEngine;
using System.Collections;

public class Player : Movement {

	private int _hp = 100;
    
    private int _maxHP = 100;

	private int _reputation = 0;
    
    public bool isTalking = false;
    
    public bool inChallenge = false;
    
    void Awake() {
        GameEventManager.player = this;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
        GameEventManager.GamePause += GamePause;
        GameEventManager.GameResume += GameResume;
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
        PlayerPrefs.SetInt("Resumeable", 1);
        PlayerPrefs.SetInt("Player HP", hp());
        PlayerPrefs.SetInt("Player Max HP", maxHP());
        PlayerPrefs.SetInt("Player Rep", reputation());
        PlayerPrefs.SetFloat("Player Position X", transform.position.x);
        PlayerPrefs.SetFloat("Player Position Y", transform.position.y);
    }
    
    private void GameLoad() {
        this._hp = PlayerPrefs.GetInt("Player HP");
        this._maxHP = PlayerPrefs.GetInt("Player Max HP");
        this._reputation = PlayerPrefs.GetInt("Player Rep");
        this.transform.position = new Vector2(
            PlayerPrefs.GetFloat("Player Position X"),
            PlayerPrefs.GetFloat("Player Position Y")
        );
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
	
	// Detect the input of the player and move the player character accordingly
	void Update() {        
        if (this.enabled && !this.isTalking && !this.inChallenge) {
            Vector2 movement = Vector2.zero;
    		#if UNITY_IPHONE
        		// We divide the touch zones into 4 squares, this.touchOffset high, leaving a free 
        		// square in all 4 cornors that's this.touchOffset x this.touchOffset
        		var touches = Input.touches;
        		if (touches.Length > 0) {
        			Vector2 pos = touches[0].position;
        			if (pos.x < this.touchOffset && pos.y < (Screen.height - this.touchOffset) && pos.y > this.touchOffset) {
        				movement = new Vector2(-1, 0); // left
        			} else if (pos.x > (Screen.width - this.touchOffset) && pos.y < (Screen.height - this.touchOffset) && pos.y > this.touchOffset) {
        				movement = new Vector2(1, 0); // right
        			} else if (pos.y < this.touchOffset && pos.x < (Screen.width - this.touchOffset) && pos.x > this.touchOffset) {
        				movement = new Vector2(0, -1); // bottom
        			} else if (pos.y > (Screen.height - this.touchOffset) && pos.x < (Screen.width - this.touchOffset) && pos.x > this.touchOffset) {
        				movement = new Vector2(0, 1); // top
        			}
                    if (GameEventManager.playerGUI.iOSMenuButton.HitTest(touches[0].position)) {
                        GameEventManager.TriggerGamePause();
                    }
        		}
    		#else
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    GameEventManager.TriggerGamePause();
                }
        		// Normal movement using arrow keys or wasd
        		movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        		// Diagonal movement is not allowed, so, we check which is greater and give it higher priority
        		if (Mathf.Abs(movement.x) < Mathf.Abs(movement.y)) {
        			movement.x = 0;
        		} else {
        			movement.y = 0;
        		}
    		#endif
    		move(movement);
        }
	}
    
    public void consumedItem(Item item) {
        hp(item.givesHP);
        maxHP(item.givesMaxHP);
        reputation(item.givesRep);
    }
    
    public int hp() {
        return this._hp;
    }
    
    public void hp(int hp) {
        this._hp += hp;
        if (this._hp > maxHP()) {
            this._hp = maxHP();
        } else if (this._hp <= 0) {
            this._hp = 0;
            GameEventManager.TriggerGameOver();
        }
        GameEventManager.playerGUI.hp(hp, maxHP());
    }
    
    public int maxHP() {
        return this._maxHP;
    }
    
    public void maxHP(int maxHP) {
        this._maxHP += maxHP;
        GameEventManager.playerGUI.hp(hp(), maxHP);
    }
    
    public int reputation() {
        return this._reputation;
    }
    
    public void reputation(int rep) {
        this._reputation += rep;
        GameEventManager.playerGUI.reputation(rep);
    }
}
