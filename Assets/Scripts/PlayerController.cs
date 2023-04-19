using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Must be connected via unity editor
    public GameObject gameController = null;
    public GameObject enemyControllerObj = null; 
    private EnemyController enemyController;
    private GameObject currTargeted = null;
    public GameObject charInfoPanel = null;

    public bool ourTurn = false;
    public bool isTargetEnemy = false; 

    // Must be connected via unity editor
    public Grid currGrid = null;
    public Tilemap currTilemap = null;
    public Tile targetTile = null;
    private Vector3Int prevTarget = Vector3Int.zero; 
    // dont set in here, they grab value from GameController
    private float tileX = 0;
    private float tileY = 0;

    private GameObject[] playerUnits;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = enemyControllerObj.GetComponent<EnemyController>();

        tileX = gameController.GetComponent<GameController>().tileX;
        tileY = gameController.GetComponent<GameController>().tileY;

        // get a handle on each child for PlayerController
        playerUnits = new GameObject[transform.childCount];
        int i = 0;
        foreach(Transform child in transform)
        {
            playerUnits[i] = child.gameObject;

            Vector3 startPos = new Vector3(3f, -3f + i, 0f);
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


                //Debug.Log("Clicked here: " + mousePos);
                //Debug.Log("currTargeted is " + currTargeted.name);
                //Debug.Log("childCount is " + transform.childCount);
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (mousePos == playerUnits[i].transform.position)
                    {
                        // target ally
                        if (currTargeted == null)
                        {
                            targetAlly(i);
                        }
                        else
                        {
                            deselectTarget();
                            targetAlly(i);
                        }

                        return;
                    }
                    else if (mousePos == enemyController.enemyUnits[i].transform.position)
                    {
                        // target ally
                        if (currTargeted == null)
                        {
                            targetEnemy(i);
                        }
                        else
                        {
                            deselectTarget();
                            targetEnemy(i);
                        }

                        return;
                    }
                }

                // clicked in move range, move ally
                if (currTargeted != null && inMovementRange(mousePos) && currTargeted.GetComponent<Character>().movLeft > 0 && isTargetEnemy != true)
                {
                    moveAlly(mousePos);
                }
                // clicked nothing and/or outside of moverange, deselect target
                else
                {
                    deselectTarget();
                }
                
                

            }
        }
    }

    void targetAlly(int i)
    {
        Debug.Log("Clicked ally");
        //Debug.Log("i: " + i);
        //Debug.Log("playerUnit @ " + i + " is " + playerUnits[i].transform.name);
        currTargeted = playerUnits[i];
        isTargetEnemy = false;
        //Debug.Log("currTargeted is " + currTargeted.name);

        currTargeted.transform.GetChild(0).gameObject.SetActive(true);
        //currTargeted.transform.GetChild(1).gameObject.SetActive(true);

        charInfoPanel.gameObject.SetActive(true);
        updateCharInfo();
    }

    void deselectTarget()
    {
        if (currTargeted == null)
            return;
        currTargeted.transform.GetChild(0).gameObject.SetActive(false);
        //currTargeted.transform.GetChild(1).gameObject.SetActive(false);
        currTargeted = null;
        charInfoPanel.gameObject.SetActive(false);
    }

    void moveAlly(Vector3Int mousePos)
    {
        Vector3 distanceTraveled = mousePos - currTargeted.transform.position;
        currTargeted.transform.position = mousePos;

        // Flip image based on the movement direction (if you move left sprite should face left
        //Debug.Log("distance traveled.x = " + distanceTraveled.x);
        // face left
        if (distanceTraveled.x < 0f)
        {
            currTargeted.transform.rotation = new Quaternion(0f, 180f, 0f, 1f);
        }
        // face right
        else if (distanceTraveled.x > 0f)
        {
            currTargeted.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
        }

        currTargeted.GetComponent<Character>().movLeft = (int)(currTargeted.GetComponent<Character>().movLeft - Mathf.Abs(distanceTraveled.x));
        currTargeted.GetComponent<Character>().movLeft = (int)(currTargeted.GetComponent<Character>().movLeft - Mathf.Abs(distanceTraveled.y));

        //Debug.Log("moveUsedX: " + Mathf.Abs(distanceTraveled.x));
        //Debug.Log("moveUsedY: " + Mathf.Abs(distanceTraveled.y));
        //Debug.Log("moveLeft: " + moveLeft);
        updateCharInfo();
    }

    void targetEnemy(int i)
    {
        Debug.Log("Clicked enemy");
        //Debug.Log("i: " + i);
        //Debug.Log("playerUnit @ " + i + " is " + playerUnits[i].transform.name);
        currTargeted = enemyController.enemyUnits[i];
        isTargetEnemy = true;
        //Debug.Log("currTargeted is " + currTargeted.name);

        currTargeted.transform.GetChild(0).gameObject.SetActive(true);
        //currTargeted.transform.GetChild(1).gameObject.SetActive(true);

        charInfoPanel.gameObject.SetActive(true);
        updateCharInfo();
    }

    bool inMovementRange(Vector3Int mousePos)
    {
        Vector3 distanceTraveled = mousePos - currTargeted.transform.position;
        if (Mathf.Abs(distanceTraveled.x) + Mathf.Abs(distanceTraveled.y) <= currTargeted.GetComponent<Character>().movLeft)
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

    public void resetAllMove()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            playerUnits[i].GetComponent<Character>().resetMove();
        }
    }

    public void updateCharInfo()
    {
        charInfoPanel.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "Name: " + currTargeted.GetComponent<Character>().charName;
        charInfoPanel.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = "STR: " + currTargeted.GetComponent<Character>().STR;
        charInfoPanel.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = "MAG: " + currTargeted.GetComponent<Character>().MAG;
        charInfoPanel.transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().text = "DEF: " + currTargeted.GetComponent<Character>().DEF;
        charInfoPanel.transform.GetChild(5).GetComponent<TMPro.TextMeshProUGUI>().text = "RES: " + currTargeted.GetComponent<Character>().RES;
        charInfoPanel.transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>().text = "SPD: " + currTargeted.GetComponent<Character>().SPD;

        if (isTargetEnemy == false)
            charInfoPanel.transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().text = "MOV: " 
                + currTargeted.GetComponent<Character>().MOV + "(" + currTargeted.GetComponent<Character>().movLeft + " Left)";
        else
            charInfoPanel.transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().text = "MOV: "
                + currTargeted.GetComponent<Character>().MOV;

    }
}
