using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DiceRoller : MonoBehaviour
{
    private static Dictionary<int, Vector3> dice = new Dictionary<int, Vector3>()
    {
        {1, new Vector3(90, 90, 0)},
        {2, new Vector3(180, 0, 0)},
        {3, new Vector3(270, 0, 0)},
        {4, new Vector3(0, 180, 0)},
        {5, new Vector3(0, 90, 0)},
        {6, new Vector3(0, 270, 0)},
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {               
        if (Input.GetMouseButtonDown(0))
        {
          //StartCoroutine(RollDice());
         }
    }

    public int GetValue()
    {
        return diceVal_;
    }

    public void ResetValue()
    {
        diceVal_ = 0;
    }

    public IEnumerator RollDice(int value)
    {        
        var totalTime = UnityEngine.Random.Range(2.0f, 5.0f);
        
        while (rollTime_ < totalTime)
        {
            // Rotate objects
            gameObject.transform.Rotate(Vector3.one * 200 * Time.deltaTime, Space.Self);
            
            // update time
            rollTime_ += Time.deltaTime;  
            yield return null;          
        }

        // Set dice to value
        Vector3 rot = gameObject.transform.eulerAngles;        
        bool okay = dice.TryGetValue(value, out rot);
        gameObject.transform.eulerAngles = rot;
        
        // Reset and return
        rollTime_ = 0.0f;
        yield return null;        
    }

    /// Get dice number
    public void GetNumber()
    {        
        // Rotation
        var rot = gameObject.transform.rotation.eulerAngles;

        // Get number
        if (0.0f == rot.x & 90.0f == rot.y)
        {
            diceVal_ = 5;
        }
        else if (0.0f == rot.x & 180.0f == rot.y)
        {
            diceVal_ = 2;
        } 
        else if (0.0f == rot.x & 270.0f == rot.y)
        {
            diceVal_ = 6;
        }
        else if (90.0f == rot.x & 0.0f == rot.y)
        {
            diceVal_ = 1;
        }
        else if (180.0f == rot.x & 0.0f == rot.y)
        {
            diceVal_ = 2;
        }
        else if (270.0f == rot.x & 0.0f == rot.y)
        {
            diceVal_ = 3;
        }
        else
        {
            diceVal_ = 4;        
        }
    }

    // private variables
    private float rollTime_ = 0.0f;
    private int diceVal_ = 0;
    
}