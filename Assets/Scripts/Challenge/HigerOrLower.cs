using UnityEngine;
using System.Collections;

public class HigerOrLower : Challenge {
    
    public GUITexture[] cards;
    
    public GUIText higher;
    
    public GUIText lower;
    
    public GUIText rightInARow;
    
    private int rightsInARowNeeded = 2;
    
    private int hpLost = 0;
    
    private int repGained = 100;
    
    private int rightInARowCount = 0;
    
    public CardDeck cardDeck;
    
    private int currentCard = 0;
    
    private int[] takenCards = new int[10];
    
    void Awake() {
		GameEventManager.GameOver += GameOver;
    }
    
    void OnDestroy() {
        GameEventManager.GameOver -= GameOver;
    }
    
    void GameOver() {
        this.active(false);
    }

    public override IEnumerator play(ChallengeManager manager, int difficulty) {
        this.currentCard = 0;
        this.takenCards = new int[10];
        this.rightInARowCount = 0;
        this.won(false);
        foreach (GUITexture card in this.cards) {
            card.texture = cardDeck.unturned;
        }
        this.difficulty = difficulty;
        if (this.difficulty == 0) {
            this.rightsInARowNeeded = 2;
            this.hpLost = 2;
            this.repGained = 100;
        } else if (this.difficulty == 1) {
            this.rightsInARowNeeded = 3;
            this.hpLost = 4;
            this.repGained = 150;
        } else if (this.difficulty == 2) {
            this.rightsInARowNeeded = 3;
            this.hpLost = 6;
            this.repGained = 200;
        } else if (this.difficulty == 3) {
            this.rightsInARowNeeded = 4;
            this.hpLost = 8;
            this.repGained = 250;
        } else if (this.difficulty == 4) {
            this.rightsInARowNeeded = 5;
            this.hpLost = 12;
            this.repGained = 400;
        } else if (this.difficulty > 5) {
            this.rightsInARowNeeded = 6;
            this.hpLost = 15;
            this.repGained = 1000;
        }
        
        selectItem(this.higher);
        this.active(true);
        this.turnCard(); // Turn the first card
        while (this.active()) {
            if (this.rightInARowCount >= this.rightsInARowNeeded) {
                StartCoroutine(doneWithGame());
                break;
            }
            #if UNITY_IPHONE
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
                    if (this.higher.HitTest(Input.GetTouch(0).position)) {
                        selectItem(this.higher);
                        turnCard();
                        GameEventManager.conversation.displayText = true;
                        audio.Play();
                    } else if (this.lower.HitTest(Input.GetTouch(0).position)) {
                        selectItem(this.lower);
                        turnCard();
                        GameEventManager.conversation.displayText = true;
                        audio.Play();
                    }
                }
            #else
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                    selectItem(this.higher);
                } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                    selectItem(this.lower);
                } else if (Input.GetKeyDown(KeyCode.Return)) {
                    turnCard();
                    GameEventManager.conversation.displayText = true;
                    audio.Play();
                }
            #endif
            yield return null;
        }
        yield return null;
    }
    
    public void turnCard() {
        if (this.cards.Length > this.currentCard && this.rightInARowCount < this.rightsInARowNeeded) {
            int getCard = Random.Range(0, cardDeck.hearts.Length);
            if (this.currentCard > 0) {
                while (this.takenCards[this.currentCard - 1] == getCard) {
                    getCard = Random.Range(0, cardDeck.hearts.Length);
                }
                if (this.selected == this.lower) {
                    if (this.takenCards[this.currentCard - 1] > getCard) {
                        GameEventManager.conversation.text = "You guessed correctly! It was indeed lower...";
                        this.rightInARowCount += 1;
                    } else {
                        GameEventManager.conversation.text = "Wrong, you take a drink of your beer and lose some HP";
                        this.rightInARowCount = 0;
                        penalty();
                    }
                } else {
                    if (this.takenCards[this.currentCard - 1] < getCard) {
                        GameEventManager.conversation.text = "You guessed correctly! It was indeed higher...";
                        this.rightInARowCount += 1;
                    } else {
                        GameEventManager.conversation.text = "Wrong, you take a drink of your beer and lose some HP";
                        this.rightInARowCount = 0;
                        penalty();
                    }
                }
            }
            this.takenCards[this.currentCard] = getCard;
            this.cards[this.currentCard].texture = cardDeck.hearts[getCard];
            this.currentCard += 1;
            this.rightInARow.text = "You have guessed " + this.rightInARowCount + " of " + this.rightsInARowNeeded + " needed in a row so far";
        } else {
            StartCoroutine(doneWithGame());
        }
    }
    
    public IEnumerator doneWithGame() {
        if (this.rightInARowCount >= this.rightsInARowNeeded) {
            this.won(true);
            GameEventManager.player.reputation(this.repGained);
        }
        string[] text = new string[1];
        if (this.won()) {
            #if UNITY_IPHONE
                text = new string[] { "Congratulations, you won! You have gained +" + this.repGained + " reputation! (Touch to continue)" };
            #else
                text = new string[] { "Congratulations, you won! You have gained +" + this.repGained + " reputation! (Press space to continue)" };
            #endif
        } else {
            #if UNITY_IPHONE
                text = new string[] { "Sorry man, you lost the challenge! (Touch to continue)" };
            #else
                text = new string[] { "Sorry man, you lost the challenge! (Press space to continue)" };
            #endif
        }
        StartCoroutine(GameEventManager.conversation.speak(text));
        while (GameEventManager.player.isTalking) {
            yield return null;
        }
        this.active(false);
        yield return 0;
    }
    
    public void penalty() {
        GameEventManager.player.hp(-this.hpLost);
    }
    
    public new void active(bool act) {
        base.active(act);
        this.higher.enabled = act;
        this.lower.enabled = act;
        this.rightInARow.enabled = act;
        foreach (GUITexture card in this.cards) {
            card.enabled = act;
        }
    }
    
    public void OnGUI() {
        base.OnGUI();
        #if UNITY_IPHONE
            this.higher.fontSize = 45;
            this.higher.pixelOffset = new Vector2(-100, -20);
            this.lower.fontSize = 45;
            this.lower.pixelOffset = new Vector2(100, -20);
            this.rightInARow.fontSize = 45;
        #endif
        Vector3 startPos = Camera.main.ViewportToScreenPoint(new Vector3(0.05f, 0.2f, 0f));
        Vector3 endPos = Camera.main.ViewportToScreenPoint(new Vector3(0.95f, 0.2f, 0f));
        float widthForEachCard = (endPos.x - startPos.x) / this.cards.Length;
        for (int i = 0; i < this.cards.Length; i++) {
            #if UNITY_IPHONE
                this.cards[i].pixelInset = new Rect(startPos.x + (widthForEachCard * i) - (Screen.width / 2), startPos.y, 50f, 72.8f);
            #else
                this.cards[i].pixelInset = new Rect(startPos.x + (widthForEachCard * i) - (Screen.width / 2), startPos.y, 30f, 43.7f);
            #endif
        }
    }
}
