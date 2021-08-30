using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxState { CLOSED, OPENED, FLAGGED }

public class Box : MonoBehaviour
{
    
    public BoxState state;
    public Sprite closed_sprite;
    public Sprite flagged_sprite;
    public Sprite flagged_wrong_sprite;
    //public Sprite bomb_sprite;
    public Sprite bomb_clicked_sprite;
    public Sprite opened_sprite;
    public GameObject gameBoardObj;
    GameBoard gameBoard;

    public SpriteRenderer boxRenderer;
    public bool isBomb;
    public int neighbour; //-1 if bomb
    public int rowIndex;
    public int colIndex;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = gameBoardObj.GetComponent<GameBoard>(); 
        boxRenderer = GetComponent<SpriteRenderer>();
        //state = BoxState.CLOSED; 
        boxRenderer.sprite = closed_sprite;
    }

    void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0)){ //leftclick
        
            if( state == BoxState.CLOSED ){
                if( neighbour == 0 ){
                    gameBoard.OpenTheBlankBox(rowIndex, colIndex);
                } else {
                    gameBoard.OpenTheBox(rowIndex, colIndex);
                }
                Debug.Log("Pressed left click on closed box of "+rowIndex+colIndex);
            }
            //updateSprite();
        }
        if(Input.GetMouseButtonDown(1)){ //leftclick
            if( state == BoxState.CLOSED ){
                gameBoard.FlagTheBox(rowIndex, colIndex);
                Debug.Log("Pressed right click on closed box of "+rowIndex+colIndex);
            } else if( state == BoxState.FLAGGED ){
                gameBoard.UnflagTheBox(rowIndex, colIndex);
                Debug.Log("Pressed right click on flagged box of "+rowIndex+colIndex);
            }
            //updateSprite();
        }
    }

    public void setOpenedSprite( Sprite sprite ){

        opened_sprite = sprite;
    }

}


