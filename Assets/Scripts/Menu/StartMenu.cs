using UnityEngine;
using System.Collections;

public class StartMenu : MenuBase {

	public GUIText gameTitle;
    
    public GUIText resumeGame;

	public GUIText startGame;

	public GUIText quitGame;
    
    public GUIText yes;
    
    public GUIText no;
    
    public GUIText overwriteGame;
    
    private bool dialogActivated = false;
    
    private bool showingDialog = false;

	void Awake() {
        this.audios = this.menuMusic.GetComponents<AudioSource>();
        if (PlayerPrefs.GetInt("Resumeable") == 1) {
            selectItem(this.resumeGame);
            this.menuItems = new GUIText[] {
                this.resumeGame,
                this.startGame,
                this.quitGame
            };
        } else {
            selectItem(this.startGame);
            this.menuItems = new GUIText[] {
                this.startGame,
                this.quitGame
            };
            this.resumeGame.enabled = false;
        }
        #if UNITY_IPHONE
            this.quitGame.enabled = false;
        #endif
        
        this.play();
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
        #endif
	}
    
    IEnumerator showDialog(bool show) {
        this.showingDialog = show;
		for (int i = 0; i < this.menuItems.Length; i++) {
            #if UNITY_IPHONE
                if (this.menuItems[i] == this.quitGame) {
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
    		if (Input.GetKeyDown(KeyCode.Return)) {
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
            if (!this.audioPlaying.isPlaying) {
                this.play();
            }
        }
	}

	// Do the action associated with a menu item
	public override void activateItem(GUIText item) {
		if (item == this.startGame) {
            if (PlayerPrefs.GetInt("Resumeable") == 1) {
                StartCoroutine(showDialog(true));
            } else {
                Application.LoadLevel(1);
            }
		} else if (item == this.quitGame) {
			Application.Quit();
		} else if (item == this.resumeGame) {
            if (PlayerPrefs.GetInt("Resumeable") == 1) {
                GameEventManager.resumeGame = true;
                Application.LoadLevel(1);
            }
		} else if (item == this.no) {
            selectItem(this.startGame);
            StartCoroutine(showDialog(false));
        } else if (item == this.yes) {
            Application.LoadLevel(1);
        }
	}
}
