using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Ready = 0,
    StandBy = 1,
    InProgress = 2,
    GameOver = 3,
    ShowScore = 4,
}

public class GameManager : MonoBehaviour
{
    public GameObject[] fruitList;
    public GameObject fruitSpawnPos;

    public GameObject startBtn;

    public static GameManager Instance;

    public GameState state = GameState.Ready;

    public float totalScore = 0.0f;
    public float highestScore = 0.0f;

    public Text uiScore;
    public Text uiHighestScore;

    void Awake() 
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        highestScore = PlayerPrefs.GetFloat("HighestScore");
        uiHighestScore.text = "Highest: " + highestScore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        startBtn.SetActive(false);
        CreateFruit();
        state = GameState.StandBy;
    }

    void CreateFruit()
    {
        int idx = Random.Range(0, 5);
        GameObject fruitObj = fruitList[idx];
        var curFruit = Instantiate(fruitObj, fruitSpawnPos.transform.position, fruitObj.transform.rotation);
        curFruit.GetComponent<CircleCollider2D>().enabled = false;
        
        if ((int)state < (int)GameState.GameOver)
            state = GameState.InProgress;
    }

    public void InvokeCreateFruit(float interval)
    {
        Invoke("CreateFruit", interval);
    }

    public void CombineFruits(FruitType type, Vector3 firstPos, Vector3 secondPos)
    {
        // If it is the biggest fruit don't combine
        if (type == FruitType.Eleven)
        {
            return;
        }

        Vector3 newPos = 0.5f * (firstPos + secondPos);

        GameObject newFruitObj = fruitList[(int)type + 1];
        var newFruit = Instantiate(newFruitObj, newPos, newFruitObj.transform.rotation);

        newFruit.GetComponent<Fruit>().state = FruitState.Dropping;
        newFruit.GetComponent<CircleCollider2D>().enabled = true;
        newFruit.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        newFruit.GetComponent<Rigidbody2D>().mass = 1.0f * newFruit.GetComponent<CircleCollider2D>().radius;
    }
}
