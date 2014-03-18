using UnityEngine;
using System.Collections;

public class ConversationManager : MonoBehaviour {
    
    public string[] gameIntroText;
    
    public Texture2D background;
    
    public GUIStyle style;
    
    public string text = "";
    
    public bool displayText = false;
    
    private bool hideText = false;
    
    private bool doIntroText = false;
    
    void Awake() {
        GameEventManager.conversation = this;
        GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
        GameEventManager.GamePause += GamePause;
        GameEventManager.GameResume += GameResume;
    }
    
    void OnDestroy() {
		GameEventManager.GameStart -= GameStart;
        GameEventManager.GameOver -= GameOver;
        GameEventManager.GamePause -= GamePause;
        GameEventManager.GameResume -= GameResume;
    }
    
    void GameStart() {
        this.hideText = false;
        this.doIntroText = true;
    }
    
    void GameOver() {
        this.hideText = true;
        this.displayText = false;
    }
    
    void GamePause() {
        this.hideText = true;
    }
    
    void GameResume() {
        this.hideText = false;
    }

	public IEnumerator speak(string[] conversation) {
        GameEventManager.player.isTalking = true;
        foreach (string sentence in conversation) {
            this.text = sentence;
            this.displayText = true;
            bool waitForReponse = true;
            while (waitForReponse) {
                #if UNITY_IPHONE
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
                        waitForReponse = false;
                    }
                #else
                    if (Input.GetKeyDown(KeyCode.Space)) {
                        waitForReponse = false;
                    }
                #endif
                yield return null;
            }
            yield return null;
        }
        this.displayText = false;
        GameEventManager.player.isTalking = false;
        yield return 0;
    }
    
    public void OnGUI() {
        Vector3 startPos = Camera.main.ViewportToScreenPoint(new Vector3(0f, 0.7f, 0f));
        Vector3 endPos = Camera.main.ViewportToScreenPoint(new Vector3(1f, 1f, 0f));
        #if UNITY_EDITOR
        #elif UNITY_IPHONE
            int padding = 40;
            this.style.fontSize = 40;
            this.style.padding = new RectOffset(padding, padding, padding - 10, padding);
        #else
            int padding = 40;
            this.style.fontSize = 40;
            this.style.padding = new RectOffset(padding, padding, padding - 10, padding);
        #endif
        
        if (this.displayText && !this.hideText) {
            GUI.Box(new Rect(startPos.x, startPos.y, endPos.x - startPos.x, endPos.y - startPos.y), this.text, this.style);
        }
        if (this.doIntroText && this.gameIntroText != null) {
            StartCoroutine(this.speak(this.gameIntroText));
            this.doIntroText = false;
        }
    }
}
