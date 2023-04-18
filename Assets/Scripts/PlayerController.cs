using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    // Must be connected via unity editor
    public GameObject gameController = null;
    private GameObject currTargeted = null;
    public TMPro.TMP_Text movementTXT = null;

    public bool ourTurn = false;

    // Must be connected via unity editor
    public Grid currGrid = null;
    public Tilemap currTilemap = null;
    public Tile targetTile = null;
    private Vector3Int prevTarget = Vector3Int.zero; 
    // dont set in here, they grab value from GameController
    private float tileX = 0;
    private float tileY = 0;

    private GameObject[] playerUnits;

    int moveRange = 2;
    float moveLeft; 
    // Start is called before the first frame update
    void Start()
    {
        tileX = gameController.GetComponent<GameController>().tileX;
        tileY = gameController.GetComponent<GameController>().tileY;

        // get a handle on each child for PlayerController
        playerUnits = new GameObject[transform.childCount];
        int i = 0;
        foreach(Transform child in transform)
        {
            playerUnits[i] = child.gameObject;

            Vector3 startPos = new Vector3(3f, -3f, 0f);
            playerUnits[i].transform.position = startPos;
           
            i += 1;      
        }



    }

    // Update is called once per frame
    void Update()
    {
            
        if (ourTurn)
        {
            // if player left clicks
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                // adjust to z level for units
                Vector3Int mousePos = GetMousePosition();
                
                
                Debug.Log("Clicked here: " + mousePos);
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (mousePos == playerUnits[i].transform.position && currTargeted == null)
                    {
                        Debug.Log("Clicked unit");                                                        
                        currTargeted = playerUnits[i];
                        currTargeted.transform.GetChild(0).gameObject.SetActive(true);
                        currTargeted.transform.GetChild(1).gameObject.SetActive(true);
                        movementTXT.gameObject.SetActive(true);
                        updateMoveTXT();
                    }
                    // clicked in move range
                    else if(currTargeted != null && inMovementRange(mousePos) && moveLeft > 0)
                    {
                        Vector3 distanceTraveled = mousePos - currTargeted.transform.position;
                        currTargeted.transform.position = mousePos;

                        moveLeft = moveLeft - Mathf.Abs(distanceTraveled.x);
                        moveLeft = moveLeft - Mathf.Abs(distanceTraveled.y);
                        //Debug.Log("moveUsedX: " + Mathf.Abs(distanceTraveled.x));
                        //Debug.Log("moveUsedY: " + Mathf.Abs(distanceTraveled.y));
                        //Debug.Log("moveLeft: " + moveLeft);
                        updateMoveTXT();

                    }
                    // clicked nothing and outside of moverange, deselect target
                    else
                    {
                        if (currTargeted == null)
                            return;
                        currTargeted.transform.GetChild(0).gameObject.SetActive(false);
                        currTargeted.transform.GetChild(1).gameObject.SetActive(false);
                        currTargeted = null;
                        movementTXT.gameObject.SetActive(false);

                    }
                }
                

            }
        }
    }

    bool inMovementRange(Vector3Int mousePos)
    {
        Vector3 distanceTraveled = mousePos - currTargeted.transform.position;
        if (Mathf.Abs(distanceTraveled.x) + Mathf.Abs(distanceTraveled.y) <= moveLeft)
        {             
            return true;
        }
        else
            return false;
    }

    Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return currGrid.WorldToCell(mouseWorldPos);
    }

    public void resetMove()
    {
        moveLeft = moveRange;
    }

    public void updateMoveTXT()
    {
        movementTXT.text = "Movement: " + moveLeft;
    }
}
