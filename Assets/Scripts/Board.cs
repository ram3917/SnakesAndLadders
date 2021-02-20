using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Player
{
    public int position { get; set; }
    public GameObject go { get; set; }

    public Player(int x, GameObject y)
    {
        position = x;
        go = y;
    }
}

public class Board : MonoBehaviour
{
    /// Dictionary for special powers
    public static Dictionary<int,int> lookupTable = new Dictionary<int, int>()
    {
        {1, 10},
        {45, 3}
    };

    void Awake()
    {
        dice_ = GameObject.Find("Dice").GetComponent<DiceRoller>();
    }

    public void Start()
    {   
        // Instantiate first object
        var obj = Instantiate(coinPrefab_, new Vector3(0.25f, 0.25f, -1.0f), Quaternion.Euler(90,0,0));        
        var renderer = obj.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_Color", Color.red);
        Player p = new Player(1, obj);
        players_.Add(p);

        // Instantiate second object
        obj = Instantiate(coinPrefab_, new Vector3(0.75f, 0.75f, -1.0f), Quaternion.Euler(90,0,0));  
        renderer = obj.GetComponent<MeshRenderer>();      
        renderer.material.SetColor("_Color", Color.blue);
        p.go = obj;
        players_.Add(p);

        Debug.Log("List count" + players_.Count);        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           var diceVal = StartCoroutine(dice_.RollDice());     

            if (dice_.GetValue() != 0)
            {
                UpdatePlayers(dice_.GetValue());
            }       
        }
    }

    public void UpdatePlayers(int diceVal)
    {        
        // Update player turn
        playerTurn_ = ((playerTurn_ + 1) % 2);
        Debug.Log("Who's turn is it? \t" + playerTurn_);

        // Update number
        var tPlayer = new Player();         
        tPlayer.position =  players_[playerTurn_].position + diceVal;
        
        // Get the new position
        var newPos = Cell2Pos(tPlayer.position);
        tPlayer.go.transform.position += new Vector3(newPos.x, newPos.y, 0);

        // Update player
        players_[playerTurn_] = tPlayer;

        // reset dice value
        dice_.ResetValue();
    }

    public Vector2 Cell2Pos(int cell)
    {
        Vector2 pos = new Vector2();

        //lookupTable.Containskey(startVal)
        //int endValue  =lookupTable[startvalue]
        // Get the X and Y values
        pos.x = (cell % 10) - 1;
        pos.y = (cell / 10) - 1;

        return pos;
    }

    public int Pos2Cell(Vector2 pos)
    {
        // Do we need it?
        return 0;
    }

    // Prefab for coin
    public GameObject coinPrefab_; 
    public int numPlayers_ = 2; // Total number of players
    
    // Private variables    
    private List<Player> players_ = new List<Player>();
    private int playerTurn_ = 0; // Whose turn increment after every update   
    // Instantiate dice
    private static DiceRoller dice_;

}