using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FruitType
{
    One = 0,
    Two = 1,
    Three = 2,
    Four = 3,
    Five = 4,
    Six = 5,
    Seven = 6,
    Eight = 7,
    Nine = 8,
    Ten = 9,
    Eleven = 10,
}

public enum FruitState
{
    Waiting = 0,
    Dropping = 1,
    Colliding = 2,
}

public class Fruit : MonoBehaviour
{
    public FruitType fruitType = FruitType.One;
    public FruitState state = FruitState.Waiting;
    public float fruitScore = 1.0f;

    private bool isMove = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameState.GameOver)
        {
            return;
        }

        if (GameManager.Instance.state != GameState.StandBy || state != FruitState.Waiting) {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isMove = true;
        }
        if (Input.GetMouseButtonUp(0) && isMove)
        {
            GetComponent<CircleCollider2D>().enabled = true;
            GetComponent<Rigidbody2D>().gravityScale = 1.0f;
            GetComponent<Rigidbody2D>().mass = 1.0f * this.gameObject.GetComponent<CircleCollider2D>().radius;

            isMove = false;
            state = FruitState.Dropping;

            if ((int)GameManager.Instance.state < (int)GameState.GameOver)
                GameManager.Instance.state = GameState.InProgress;

            GameManager.Instance.InvokeCreateFruit(0.5f);
        }

        if (isMove && state == FruitState.Waiting)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 objCurPos = this.gameObject.GetComponent<Transform>().position;
            Vector3 objCurScale = this.gameObject.GetComponent<Transform>().localScale;

            // compute the limit on x axis so that the fruit won't be dragged out of screen
            float radius = this.gameObject.GetComponent<CircleCollider2D>().radius;
            float leftBound = Camera.main.ScreenToWorldPoint(new Vector3(0,0,0)).x;
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth,0,0)).x;

            float xPos = Mathf.Clamp(mousePos.x, leftBound + radius * objCurScale.x, rightBound - radius * objCurScale.x);

            this.gameObject.GetComponent<Transform>().position = new Vector3(xPos, objCurPos.y, objCurPos.z);
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if ((int)GameManager.Instance.state >= (int)GameState.GameOver)
        {
            return;
        }

        if (other.gameObject.tag.Contains("Floor") || other.gameObject.tag.Contains("Fruit"))
        {
            if ((int)GameManager.Instance.state < (int)GameState.GameOver)
                GameManager.Instance.state = GameState.StandBy;
            state = FruitState.Colliding;
        }

        if (other.gameObject.tag.Contains("Fruit"))
        {
            FruitType thisType = this.gameObject.GetComponent<Fruit>().fruitType;
            FruitType otherType = other.gameObject.GetComponent<Fruit>().fruitType;

            if (thisType == otherType)
            {
                Vector3 thisPos = this.gameObject.GetComponent<Transform>().position;
                Vector3 otherPos = other.gameObject.GetComponent<Transform>().position;

                if (thisPos.x + thisPos.y < otherPos.x + otherPos.y)
                {
                    GameManager.Instance.CombineFruits(thisType, thisPos, otherPos);
                    GameManager.Instance.totalScore += fruitScore;
                    GameManager.Instance.uiScore.text = GameManager.Instance.totalScore.ToString();                    

                    Destroy(this.gameObject);
                    Destroy(other.gameObject);
                }
            }
        }
    }
}
