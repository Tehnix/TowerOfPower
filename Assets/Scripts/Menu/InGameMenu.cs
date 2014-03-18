using UnityEngine;
using System.Collections;

public class InGameMenu : MenuBase {

	public GUIText gameTitle;

	public GUIText resumeGame;

	public GUIText startGame;

	public GUIText saveAndQuitGame;
    
    public GUIText yes;
    
    public GUIText no;
    
    public GUIText overwriteGame;
    
    public GUIText gameOverTitle;
    
    public GUIText gameOverInfo1;
    
    public GUIText gameOverInfo2;
    
    public GUIText tryAgain;
    
    public GUIText quitGame;
    
    private bool dialogActivated = false;
    
    private bool showingDialog = false;
    
    private bool menuIsHiding = true;

	void Awake() {
		GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
        GameEventManager.GamePause += GamePause;
        GameEventManager.GameResume += GameResume;
        
        this.audios = this.menuMusic.GetComponents<AudioSource>();
		selectItem(this.resumeGame);
		this.menuItems = new GUIText[] {
			this.resumeGame,
			this.startGame,
			this.saveAndQuitGame
		};
        hide();
	}
    
    void OnDestroy() {
		GameEventManager.GameStart -= GameStart;
        GameEventManager.GameOver -= GameOver;
        GameEventManager.GamePause -= GamePause;
        GameEventManager.GameResume -= GameResume;
    }
    
	private void GameStart() {
        hide();
	}
    
    private void GamePause() {
        show();
    }
    
	private void GameResume() {
        hide();
	}
    
    private void GameOver() {
        if (this.audioPlaying != null) {
            this.audioPlaying.Play();
        } else {
            this.play();
        }
        this.enabled = true;
        var lights = GameObject.FindGameObjectsWithTag("Lights");
        foreach (var light in lights) {
            light.light.color = Color.black;
        }
        this.gameOverTitle.enabled = true;
        this.gameOverInfo1.enabled = true;
        this.gameOverInfo2.enabled = true;
        this.tryAgain.enabled = true;
        this.quitGame.enabled = true;
        #if UNITY_IPHONE
            this.quitGame.enabled = false;
        #else
            this.quitGame.enabled = true;
        #endif
		this.menuItems = new GUIText[] {
			this.tryAgain,
            this.quitGame
		};
        selectItem(this.tryAgain);
    }
    
	// Listen to the users input
	public new void Update() {
        #if UNITY_IPHONE
    		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
    			if (this.yes.HitTest(Input.GetTouch(0).position)) {
                    if (this.dialogActivated) {
                        activateItem(this.yes);
                    }
                } else if (this.no.HitTest(Input.GetTouch(0).position)) {
                    if (this.dialogActivated) {
                        activateItem(this.no);
                    }
                } else {
        			for (int i = 0; i < this.menuItems.Length; i++) {
        				if (this.menuItems[i].HitTest(Input.GetTouch(0).position)) {
                            selectItem(this.menuItems[i]);
                            activateItem(this.menuItems[i]);
        					break;
        				}
        			}
                }
            }
        #else
            if (Input.GetKeyDown(KeyCode.Escape)) {
                GameEventManager.TriggerGameResume();
            } else if (Input.GetKeyDown(KeyCode.Return)) {
                activateItem(this.selected);
    		} else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
    			var pressed = Input.GetAxisRaw("Vertical");
                if (this.showingDialog) {
                    if (this.selected == this.yes) {
                        selectItem(this.no);
                    } else {
                        selectItem(this.yes);
                    }
                } else {
                    getMenuItem(System.Math.Sign(pressed) * -1, this.menuItems);
                }
    		} else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                if (this.showingDialog) {
                    selectItem(this.yes);
                }
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                if (this.showingDialog) {
                    selectItem(this.no);
                }
            }
        #endif
        
        if (this.audioPlaying != null) {
            if (this.audioPlaying.time + 2f >= this.audioPlaying.clip.length && this.audioPlaying.volume > 0f) {
                // Fade out the audio
                this.audioPlaying.volume -= (0.05f * Time.deltaTime);
            } else if (this.audioPlaying.volume < 2f) {
                // Fade in the audio
                this.audioPlaying.volume += (0.05f * Time.deltaTime);
            }
            if (!this.menuIsHiding && !this.audioPlaying.isPlaying) {
                this.play();
            }
        }
	}
    
	void OnGUI() {
        #if UNITY_IPHONE
            this.gameTitle.pixelOffset = new Vector2(0, 70);
            this.gameTitle.fontSize = 70;
            this.resumeGame.fontSize = 80;
            this.startGame.pixelOffset = new Vector2(0, -90);
            this.startGame.fontSize = 80;
            this.overwriteGame.fontSize = 80;
            this.yes.pixelOffset = new Vector2(-150, -90);
            this.yes.fontSize = 80;
            this.no.pixelOffset = new Vector2(150, -90);
            this.no.fontSize = 80;
            
            this.gameOverTitle.pixelOffset = new Vector2(0, 70);
            this.gameOverTitle.fontSize = 85;
            this.gameOverInfo1.fontSize = 50;
            this.gameOverInfo2.pixelOffset = new Vector2(0, -10);
            this.gameOverInfo2.fontSize = 50;
            this.tryAgain.pixelOffset = new Vector2(0, -90);
            this.tryAgain.fontSize = 80;
        #endif
	}
    
    private void hide() {
        this.menuIsHiding = true;
        if (this.audioPlaying != null) {
            this.audioPlaying.Pause();
        }
        audio.Play();
        this.enabled = false;
        var lights = GameObject.FindGameObjectsWithTag("Lights");
        foreach (var light in lights) {
            light.light.color = Color.white;
        }
		for (int i = 0; i < this.menuItems.Length; i++) {
			this.menuItems[i].enabled = false;
		}
        this.gameTitle.enabled = false;
    }
    
    private void show() {
        this.menuIsHiding = false;
        if (this.audioPlaying != null) {
            this.audioPlaying.Play();
        } else {
            this.play();
        }
        audio.Play();
        this.enabled = true;
        var lights = GameObject.FindGameObjectsWithTag("Lights");
        foreach (var light in lights) {
            light.light.color = Color.black;
        }
		for (int i = 0; i < this.menuItems.Length; i++) {
            #if UNITY_IPHONE
                if (this.menuItems[i] == this.saveAndQuitGame) {
                    continue;
                }
            #endif
			this.menuItems[i].enabled = true;
		}
        this.gameTitle.enabled = true;
    }
    
    IEnumerator showDialog(bool show) {
        this.showingDialog = show;
		for (int i = 0; i < this.menuItems.Length; i++) {
            #if UNITY_IPHONE
                if (this.menuItems[i] == this.saveAndQuitGame) {
                    continue;
                }
            #endif
			this.menuItems[i].enabled = !show;
		}
        this.overwriteGame.enabled = show;
        this.yes.enabled = show;
        this.no.enabled = show;
        if (show) {
            selectItem(this.no);
            #if UNITY_IPHONE
                // To avoid iOS detecting too fast and not showing dialog
                yield return new WaitForSeconds(1f);
            #else
                yield return new WaitForSeconds(0.1f);
            #endif
        }
        this.dialogActivated = show;
        yield return 0;
    }

	// Do the action associated with a menu item
	public override void activateItem(GUIText item) {
		if (item == this.resumeGame) {
            audio.Play();
			GameEventManager.TriggerGameResume();
		} else if (item == this.saveAndQuitGame) {
            GameEventManager.TriggerGameSave();
            audio.Play();
			Application.Quit();
		} else if (item == this.startGame) {
            audio.Play();
			StartCoroutine(showDialog(true));
		} else if (item == this.no) {
            audio.Play();
            selectItem(this.startGame);
            StartCoroutine(showDialog(false));
        } else if (item == this.yes) {
            Application.LoadLevel(1);
        } else if (item == this.tryAgain) {
            Application.LoadLevel(1);
        }
	}
}
