using UnityEngine;
using System.Collections;

public abstract class MenuBase : MonoBehaviour {

	protected GUIText selected = null;

	protected string selectedOriginalText;

	protected GUIText[] menuItems;
    
    public GameObject menuMusic;
    
    protected AudioSource[] audios;
    
    protected AudioSource audioPlaying = null;
    
    protected int lastRandom = -1;

	// Listen to the users input
	public void Update() {
        #if UNITY_IPHONE
    		var touches = Input.touches;
    		if (touches.Length > 0) {
    			for (int i = 0; i < this.menuItems.Length; i++) {
    				if (this.menuItems[i].HitTest(touches[0].position)) {
                        selectItem(this.menuItems[i]);
                        activateItem(this.menuItems[i]);
    					break;
    				}
    			}
            }
        #else
    		if (Input.GetKeyDown(KeyCode.Return)) {
                activateItem(this.selected);
    		} else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
    			var pressed = Input.GetAxisRaw("Vertical");
    			getMenuItem(System.Math.Sign(pressed) * -1, this.menuItems);
    		}
        #endif
	}
    
    public void play() {
        int i = Random.Range(0, this.audios.Length);
        if (i == this.lastRandom && i != this.audios.Length - 1) {
            i += 1;
        } else if (i == this.lastRandom && i == this.audios.Length - 1) {
            i -= 1;
        }
        this.audioPlaying = this.audios[i];
        this.audios[i].Play();
        this.lastRandom = i;
    }

	// Do the action associated with a menu item
	public abstract void activateItem(GUIText item);

	// Get the next/previous menu item
	public void getMenuItem(int direction, GUIText[] items) {
		if (direction == -1 && this.selected == items[0]) {
			selectItem(items[items.Length - 1]);
		} else if (direction == 1 && this.selected == items[items.Length - 1]) {
			selectItem(items[0]);
		} else {
			for (int i = 0; i < items.Length; i++) {
				if (this.selected == items[i]) {
					selectItem(items[i + direction]);
					break;
				}
			}
		}
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
}
