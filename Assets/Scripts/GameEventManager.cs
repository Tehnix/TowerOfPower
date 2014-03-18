public static class GameEventManager {

	public delegate void GameEvent();

	public static event GameEvent GameStart, GameOver, GamePause, GameResume, GameSave, GameLoad;
    
    public static Player player;
    
    public static PlayerGUI playerGUI;

    public static bool resumeGame = false;
    
    public static PlayerCamera camera = null;
    
    public static Map currentMap = null;
    
    public static ConversationManager conversation = null;
    
    public static ChallengeManager challenge = null;
    
    public static GroundAudio currentGround = null;
    
    private static System.DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
    
	public static void TriggerGameStart() {
		if (GameStart != null) {
			GameStart();
		}
	}
    
	public static void TriggerGamePause() {
		if (GamePause != null) {
			GamePause();
		}
	}
    
	public static void TriggerGameResume() {
		if (GameResume != null) {
			GameResume();
		}
	}

	public static void TriggerGameOver() {
		if (GameOver != null) {
			GameOver();
		}
	}
    
	public static void TriggerGameSave() {
		if (GameSave != null) {
			GameSave();
		}
	}
    
	public static void TriggerGameLoad() {
		if (GameLoad != null) {
			GameLoad();
		}
	}
    
    public static int unixtime() {
        return (int)(System.DateTime.UtcNow - GameEventManager.epochStart).TotalSeconds;
    }
}
