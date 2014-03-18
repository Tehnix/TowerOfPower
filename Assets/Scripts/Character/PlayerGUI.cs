using UnityEngine;
using System.Collections;

public class PlayerGUI : MonoBehaviour {

	public GUIText reputationCounter;
    
    public GUIText reputationCounterBackground;
    
    public GUIText hpCounter;
    
    public GUIText hpCounterBackground;
    
    public GUIText iOSMenuButton;
    
    public GUIText iOSMenuButtonBackground;
    
    void Awake() {
        GameEventManager.playerGUI = this;
        GameEventManager.GamePause += GamePause;
        GameEventManager.GameResume += GameResume;
    }
    
    void OnDestroy() {
        GameEventManager.GamePause -= GamePause;
        GameEventManager.GameResume -= GameResume;
    }
    
	private void GamePause() {
        this.hpCounterBackground.enabled = false;
        this.reputationCounterBackground.enabled = false;
        this.hpCounter.enabled = false;
        this.reputationCounter.enabled = false;
        #if UNITY_IPHONE
            this.iOSMenuButtonBackground.enabled = false;
            this.iOSMenuButton.enabled = false;
        #endif
	}
    
	private void GameResume() {
        this.hpCounterBackground.enabled = true;
        this.reputationCounterBackground.enabled = true;
        this.hpCounter.enabled = true;
        this.reputationCounter.enabled = true;
        #if UNITY_IPHONE
            this.iOSMenuButtonBackground.enabled = true;
            this.iOSMenuButton.enabled = true;
        #endif
	}
    
	void OnGUI() {
        #if UNITY_IPHONE
            this.reputationCounter.transform.position = new Vector2(0.02f, 0.92f);
            this.reputationCounter.fontSize = 40;
            this.hpCounter.transform.position = new Vector2(0.98f, 0.92f);
            this.hpCounter.fontSize = 28;
            this.reputationCounterBackground.transform.position = new Vector2(0.02f, 0.92f);
            this.reputationCounterBackground.fontSize = 40;
            this.hpCounterBackground.transform.position = new Vector2(0.98f, 0.92f);
            this.hpCounterBackground.fontSize = 28;
            this.iOSMenuButton.enabled = true;
            this.iOSMenuButtonBackground.enabled = true;
        #endif
        hp(GameEventManager.player.hp(), GameEventManager.player.maxHP());
        reputation(GameEventManager.player.reputation());
	}
    
    public void hp(int hp, int maxHP) {
        this.hpCounter.text = hp.ToString() + " of " + maxHP.ToString() + " HP";
        this.hpCounterBackground.text = hp.ToString() + " of " + maxHP.ToString() + " HP";
    }
    
    public void reputation(int rep) {
        this.reputationCounter.text = rep.ToString();
        this.reputationCounterBackground.text = rep.ToString();
    }
}
