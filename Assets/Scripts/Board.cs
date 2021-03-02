using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player
{
    public int position_  { get; set; }
    public GameObject go_ { get; set; }

    public Player(int x, GameObject go)
    {
        position_ = x;
        go_ = go;
    }
}

public class Board : MonoBehaviour
{
    public void Start()
    {   
        // Instantiate first object
        var obj = Instantiate(coinPrefab_, new Vector3(-2.5f, 2.5f, -1.0f), Quaternion.Euler(90,0,0));        
        var renderer = obj.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_Color", Color.red);
        players_.Add(new Player(0, obj));

        //Instantiate second object
        obj = Instantiate(coinPrefab_, new Vector3(-7.5f, 7.5f, -1.0f), Quaternion.Euler(90,0,0));  
        renderer = obj.GetComponent<MeshRenderer>();      
        renderer.material.SetColor("_Color", Color.blue);
        players_.Add(new Player(0, obj));
        Debug.Log("Dice Count: \t" + obj.transform.position);

        scoreBoard.text = "Let the game begin!";
    }

    void Update()
    {
        // If game is not done
        if (!isGameDone_)
        {
            if (Input.GetMouseButtonDown(0))
            {
            // Get a random number on dice
            diceVal_ = rnd.Next(1,6);
            Debug.Log("Dice Roll: \t" + diceVal_);

            // Animation to roll dice            
            StartCoroutine(RollDice(diceVal_));
            }

            if (isDiceRolled_)
            {
                // Update players
                StartCoroutine(UpdatePlayers(diceVal_));
                isDiceRolled_ = false;

                // Update Score Board
                Invoke("UpdateScoreBoard", 1);
            }
        }
        else
        {
            Debug.Log("Game Over!");
            scoreBoard.text = "Player " + (playerTurn_ + 1)+ " Wins !!!";

            Invoke("EndGame", 2);
        }
    }

    public IEnumerator UpdatePlayers(int diceVal)
    {        
        // Update player turn
        playerTurn_ = ((playerTurn_ + 1) % 2);
        Debug.Log("Who's turn is it? \t" + playerTurn_);

        // Increment cell
        int curCell = players_[playerTurn_].position_ + diceVal;
        
        // Get the end value based on snakes and ladders
        int toCell = curCell;
        bool keyExists = lookupTable.TryGetValue(curCell, out toCell);
        if (!keyExists)
        {
            toCell = curCell;
        }
        Debug.Log("To Cell \t" + toCell);

        if (100 <= toCell)
        {
            isGameDone_ = true;
            yield return null;
        }

        // Update cell
        players_[playerTurn_].position_ = toCell;
         
        // Get the new position
        var newPos = Cell2Pos(toCell);        
        Debug.Log("New Pos \t" + newPos); 
        
        // Move object
        var step = 1 * Time.deltaTime;
        players_[playerTurn_].go_.transform.position = 
                                new Vector3(newPos.x, newPos.y, -8.0f);
                 //Vector3.MoveTowards(players_[playerTurn_].go_.transform.position,
                 //                   new Vector3(newPos.x, newPos.y, -8.0f), step);

        yield return null;
    }

    public Vector2 Cell2Pos(int cell)
    {        
        // Get the X and Y values
        Vector2 pos = new Vector2();    
        
        // Constant 10 as board is 10 x 10
        pos.x = (float)Math.Ceiling(( ((cell-1) % 10.0f) * multiplier_)) + 5.0f;
        pos.y = ( (cell / 10) * multiplier_) + 5.0f;

        Debug.Log(pos);

        return pos;
    }

    public int Pos2Cell(Vector2 pos)
    {
        // Do we need it?
        return 0;
    }

    void UpdateScoreBoard()
    {
        scoreBoard.text = " Next Turn : Player \t" + (playerTurn_ + 1) + "\n" +  "\n" +
                          " Player 1 : \t" + players_[0].position_ + "\n" +
                          " Player 2 : \t" + players_[1].position_;
    }

    void EndGame()
    {                
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator RollDice(int value)
    {        
        // Set times
        var rollTime_ = 0.0f;
        var totalTime = UnityEngine.Random.Range(2.0f, 5.0f);
        var speed =  UnityEngine.Random.Range(15, 75);

        while (rollTime_ < totalTime)
        {
            // Rotate objects            
            var toRot = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-180.0f, 180.0f),
                                        UnityEngine.Random.Range(-180.0f, 180.0f), UnityEngine.Random.Range(-180.0f, 180.0f)));
            dice_.transform.rotation = Quaternion.Slerp(dice_.transform.rotation,
                                         toRot, Time.deltaTime * speed);
            
            // update time
            rollTime_ += Time.deltaTime;  
            yield return null;          
        }

        // Set dice to value
        Vector3 rot = dice_.transform.eulerAngles;        
        bool okay = diceRotations.TryGetValue(value, out rot);
        dice_.transform.eulerAngles = rot;
        
        // Set dice rolled true and return
        isDiceRolled_ = true;
        yield return null;        
    }

    // Prefab for coin
    public GameObject coinPrefab_; 
    public int numPlayers_ = 2; // Total number of players
    /// Dictionary for special powers
    public static Dictionary<int,int> lookupTable = new Dictionary<int, int>()
    {
        {3, 51},
        {6, 27},
        {20, 70},
        {25, 5},
        {34, 1},
        {36, 55},
        {47, 9},
        {65, 52},
        {68, 98},
        {87, 57},   
        {91, 61},
        {99, 69},
    };
    public GameObject dice_;    
    private static Dictionary<int, Vector3> diceRotations = new Dictionary<int, Vector3>()
    {
        {1, new Vector3(90, 0, 0)},
        {2, new Vector3(180, 0, 0)},
        {3, new Vector3(270, 0, 0)},
        {4, new Vector3(0, 0, 0)},
        {5, new Vector3(0, 90, 0)},
        {6, new Vector3(0, 270, 0)},
    };

    public Text scoreBoard;
    
    // Private variables    
    private List<Player> players_ = new List<Player>();
    private int playerTurn_ = 0; // Whose turn increment after every update 
    private int diceVal_ = 0;  
    private bool isGameDone_ = false;
    private bool isDiceRolled_ = false;

    // Multiplier for board size
    private float multiplier_ = 10.0f; // Multiples for 10 x scaling of board

    private static System.Random rnd = new System.Random();  
}