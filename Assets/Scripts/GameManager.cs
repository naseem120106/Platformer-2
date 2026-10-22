using UnityEngine;

// Put one of these on an empty object in the scene. Tracks treasure and shows the HUD.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int treasureCollected;
    private int treasureTotal;
    private int score;
    private bool won;
    private string message = "";
    private float messageTimer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Every Treasure in the scene counts towards the total needed to win
        treasureTotal = FindObjectsByType<Treasure>(FindObjectsSortMode.None).Length;
    }

    void Update()
    {
        if (messageTimer > 0f) messageTimer -= Time.deltaTime;
    }

    public void CollectTreasure(int points)
    {
        treasureCollected++;
        score += points;
        if (treasureCollected >= treasureTotal)
        {
            won = true;
        }
    }

    public void ShowMessage(string text, float seconds = 2f)
    {
        message = text;
        messageTimer = seconds;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 80, 300, 20), "Treasure: " + treasureCollected + " / " + treasureTotal);
        GUI.Label(new Rect(10, 100, 300, 20), "Score: " + score);

        if (messageTimer > 0f)
        {
            GUI.Label(new Rect(Screen.width / 2f - 100, 40, 200, 20), message);
        }

        if (won)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label) { fontSize = 48, alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "You win!", style);
        }
    }
}
