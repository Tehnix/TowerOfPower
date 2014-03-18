using UnityEngine;
using System.Collections;

public class ChallengeManager : MonoBehaviour {
    
    public Challenge[] challenges;
    
    public GUIStyle style;
    
    public bool hideChallenge = false;
    
    void Awake() {
        GameEventManager.challenge = this;
        GameEventManager.GameStart += GameStart;
        GameEventManager.GamePause += GamePause;
        GameEventManager.GameResume += GameResume;
    }
    
    void OnDestroy() {
		GameEventManager.GameStart -= GameStart;
        GameEventManager.GamePause -= GamePause;
        GameEventManager.GameResume -= GameResume;
    }
    
    void GameStart() {
        this.hideChallenge = false;
    }
    
    void GameOver() {
        this.hideChallenge = true;
    }
    
    void GamePause() {
        this.hideChallenge = true;
    }
    
    void GameResume() {
        this.hideChallenge = false;
    }
    
	public IEnumerator activate(NPC npc, int difficulty, string[] won, string[] lost) {
        if (npc.defeated) {
            StartCoroutine(GameEventManager.conversation.speak(new string[] { "You already defeated this challenge!" }));
        } else {
            while (GameEventManager.player.isTalking) {
                yield return null;
            }
            if (this.challenges.Length > 0) {
                Challenge challenge = this.challenges[Random.Range(0, this.challenges.Length)];
                GameEventManager.player.inChallenge = true;
                StartCoroutine(GameEventManager.conversation.speak(challenge.explanation));
                StartCoroutine(challenge.play(this, difficulty));
                while (challenge.active()) {
                    yield return null;
                }
                GameEventManager.player.inChallenge = false;
                if (challenge.won()) {
                    if (won.Length > 0) {
                        StartCoroutine(GameEventManager.conversation.speak(won));
                    }
                    npc.defeated = true;
                } else {
                    if (lost.Length > 0) {
                        StartCoroutine(GameEventManager.conversation.speak(lost));
                    }
                }
            }
        }
        yield return 0;
    }
}
