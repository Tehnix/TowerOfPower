using UnityEngine;
using System.Collections;

public class GameEvents : MonoBehaviour {
    
    void Awake() {
        GameEventManager.GamePause += GamePause;
    }
    
	void Start() {
        if (GameEventManager.resumeGame) {
            GameEventManager.resumeGame = false;
            GameEventManager.TriggerGameLoad();
        } else {
            GameEventManager.TriggerGameStart();
        }
    }
    
    void OnDestroy() {
        GameEventManager.GamePause -= GamePause;
    }
    
    void OnApplicationPause(bool paused) {
        #if UNITY_IPHONE
            if (paused) {
                GameEventManager.TriggerGamePause();
            } else {
                GameEventManager.TriggerGameResume();
            }
        #endif
    }
    
    void OnApplicationQuit() {
        #if UNITY_IPHONE
            GameEventManager.TriggerGameSave();
        #endif
    }
    
    void GamePause() {
        GameEventManager.TriggerGameSave();
    }
}
