using UnityEngine;
using System.Collections;

public abstract class Challenge : MonoBehaviour {

	public string[] explanation;
    
    public GUITexture outerBorder;
    
    public GUITexture inner;
    
    public GUITexture fakeChatBox;
    
    protected bool _active;
    
    protected bool _won;
    
	protected GUIText selected = null;

	protected string selectedOriginalText;
    
    protected string _infoText = "";
    
    protected int difficulty = 0;
    
    void Awake() {
		GameEventManager.GameOver += GameOver;
    }
    
    void OnDestroy() {
        GameEventManager.GameOver -= GameOver;
    }
    
    void GameOver() {
        this.active(false);
    }
    
    public abstract IEnumerator play(ChallengeManager manager, int difficulty);
    
    public void active(bool act) {
        this.outerBorder.enabled = act;
        this.inner.enabled = act;
        this.fakeChatBox.enabled = act;
        this._active = act;
    }
    
    public bool active() {
        return this._active;
    }
    
    public void won(bool won) {
        this._won = won;
    }
    
    public bool won() {
        return this._won;
    }
    
    public string infoText() {
        return this._infoText;
    }
    
    public void infoText(string text) {
        this._infoText = text;
    }
    
	// Mark the item selected and store it
	public void selectItem(GUIText item) {
		// Reset the text of the former selected item
		if (this.selected != null) {
			this.selected.text = this.selectedOriginalText;
            #if UNITY_IPHONE
                // Don't think there exists an #ifnot...
            #else
                audio.Play();
            #endif
		}
		this.selected = item;
		this.selectedOriginalText = (string)item.text.Clone();
		item.text = "> " + item.text;
	}
    
    public void OnGUI() {
        Vector3 startPos = Camera.main.ViewportToScreenPoint(new Vector3(0f, 1f, 0f));
        Vector3 endPos = Camera.main.ViewportToScreenPoint(new Vector3(1f, 0.7f, 0f));
        int border = 5;
        this.outerBorder.pixelInset = new Rect(0, startPos.y - endPos.y, endPos.x, endPos.y);
        this.inner.pixelInset = new Rect(border, (startPos.y - endPos.y) + border, endPos.x - (border * 2), endPos.y - (border * 2));
        this.fakeChatBox.pixelInset = new Rect(0, 0, endPos.x, startPos.y - endPos.y);
    }
}
