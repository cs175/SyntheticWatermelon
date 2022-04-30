using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Top : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public bool isMove = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            GetComponent<Transform>().position += new Vector3(0, -moveSpeed * Time.deltaTime, 0);
        }

        float botBound = Camera.main.ScreenToWorldPoint(new Vector3(0,0,0)).y;
        if (GetComponent<Transform>().position.y < botBound) 
        {
            GameManager.Instance.state = GameState.ShowScore;
            Invoke("ReloadScene", 1.0f);
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (GameManager.Instance.state != GameState.GameOver)
        {
            if (other.tag.Contains("Fruit") && other.gameObject.GetComponent<Fruit>().state == FruitState.Colliding)
            {
                GameManager.Instance.state = GameState.GameOver;
                Debug.Log("Game Over");

                Invoke("SetIsMove", 0.5f);
            }
        }

        if (GameManager.Instance.state == GameState.GameOver)
        {
            if (other.tag.Contains("Fruit"))
            {
                GameManager.Instance.totalScore += other.gameObject.GetComponent<Fruit>().fruitScore;
                GameManager.Instance.uiScore.text = GameManager.Instance.totalScore.ToString();

                Destroy(other.gameObject);
            }
        }
    }

    private void SetIsMove()
    {
        isMove = true;
    }

    private void ReloadScene()
    {
        var highestScore = PlayerPrefs.GetFloat("HeightestScore");
        var curTotalScore = GameManager.Instance.totalScore;

        if (curTotalScore > highestScore)
        {
            PlayerPrefs.SetFloat("HighestScore", GameManager.Instance.totalScore);
        }

        SceneManager.LoadScene("MainScene");
    }
}
