using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    // we need a pointer to both Player and Enemy controller. Must be connected via unity editor. 
    public GameObject playerController = null;
    public GameObject enemyController = null;

    // enum for whose turn it is currently, the players or the enemies.
    public enum turnMode { PlayerTurn, EnemyTurn };
    private turnMode currTurnMode;

    // enum for what the game is currently doing/displayhing, a menu, the map, or a battle. 
    public enum gameMode { MenuMode, MapMode, BattleMode };
    private gameMode currGameMode;

    // Must be connected via unity editor
    public TMPro.TMP_Text turnModeTXT = null;

    // doesnt actually need to be connected here, BUT this button does need this for its onPress
    //public Button endTurnButton = null; 

    // Must be connected via unity editor
    public Grid currGrid = null;
    public Tilemap currTilemap = null;
    public Tile hoverTile = null;

    // defaulted to this so the hover doesnt overwrite an existing tile on first deselection
    private Vector3Int previousMousePos = new Vector3Int(0, 0, -999);

    // set these ones 
    public float tileX = 1;
    public float tileY = 1;

    // Start is called before the first frame update
    void Start()
    {
        changeTurn(turnMode.PlayerTurn);
        changeMode(gameMode.MapMode);

        updateTurnText();
    }

    // Update is called once per frame
    void Update()
    {
        // Map mode only
        if (currGameMode == gameMode.MapMode)
        {

            // mouse hover over effect
            Vector3Int mousePos = GetMousePosition();

            // adjust pos to z -5, see below
            mousePos = new Vector3Int(mousePos.x, mousePos.y, -6);

            if (!mousePos.Equals(previousMousePos))
            {
                // select layer is z: -6 (cause I randomly decided)
                Vector3Int mousePosZHover = new Vector3Int(mousePos.x, mousePos.y, -6);
                currTilemap.SetTile(previousMousePos, null); // Remove old hoverTile
                currTilemap.SetTile(mousePosZHover, hoverTile);
                previousMousePos = mousePos;
            }

        }
    }

    // for hovering effect
    Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return currGrid.WorldToCell(mouseWorldPos);
    }

    public void changeTurn(turnMode newTurn)
    {
        turnMode prevTurnMode = currTurnMode;
        currTurnMode = newTurn;

        // make sure to update turn text as well
        updateTurnText();

        if (currTurnMode == newTurn)
        {
            Debug.Log("turnMode changed from " + prevTurnMode + " to " + newTurn);
           
            // if player turn
            if (currTurnMode == turnMode.PlayerTurn)
            {
                playerController.GetComponent<PlayerController>().resetMove();
                playerController.GetComponent<PlayerController>().ourTurn = true; 
            }
            // if enemy turn
            else
            {
                playerController.GetComponent<PlayerController>().ourTurn = false;
                enemyController.GetComponent<EnemyController>().enemyTurn();
            }
        }
        else
        {
            Debug.Log("!!! Failed to change turnMode from " + prevTurnMode + " to " + newTurn);
            
        }
    }

    public void changeMode(gameMode newMode)
    {
        gameMode prevGameMode = currGameMode; 
        currGameMode = newMode;

        if (currGameMode == newMode)
        {
            Debug.Log("gameMode changed from " + prevGameMode + " to " + newMode);
        }
        else
        {
            Debug.Log("!!! Failed to change gameMode from " + prevGameMode + " to " + newMode);
        }
    }

    public void updateTurnText()
    {
        if (currTurnMode == turnMode.PlayerTurn)
        {
            turnModeTXT.text = "Player Turn";    
        }
        else 
        {
            turnModeTXT.text = "Enemy Turn";
        }
    }

    public void endTurnButtonPressed()
    {
        if (currTurnMode == turnMode.PlayerTurn)
            changeTurn(turnMode.EnemyTurn);
        else
            Debug.Log("!!! The end turn button was pressed BUT it isnt currently the players turn");
    }
}
