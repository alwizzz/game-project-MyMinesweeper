using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    public int rowSize;
    public int colSize;
    public int totalBomb;

    public Sprite[] NeighbourSprite = new Sprite[10];
    public GameObject gameBoxObj;
    Box gameBox;
    public Vector3 gameBoardInitialPoint;
    public GameObject SpawnPointObj;
    Vector3 gameBoardSpawnPoint;
    public float boxWidth = 1.28f; //pixel
    public int maxNormalRow = 6;
    public int maxNormalCol = 14;
    float headerDeficit = 1.3f;

    Box[, ] board; 

    public int openedCount;
    public int flagCount;

    public GameObject endPanel;
    Text endText;
    public Text flagText;
    public Text timerText;

    public bool timerActived;
    public float currentTime;

    public Button gameButton;
    Image gameButtonImage;
    public Sprite GameOn;
    public Sprite GameWin;
    public Sprite GameLose;

    public GameObject sceneLoaderObj;
    SceneLoader sceneLoader;
    


    // Start is called before the first frame update
    void Start()
    {
        gameBoardInitialPoint = transform.position;
        SpawnPointObj.transform.position = gameBoardInitialPoint;
        GamePreSet();

        gameButtonImage = gameButton.GetComponent<Image>();
        gameBox = gameBoxObj.GetComponent<Box>();
        endText = endPanel.GetComponentInChildren<Text>();
        sceneLoader = sceneLoaderObj.GetComponent<SceneLoader>();

        GameSet();
    }

    void Update()
    {
        flagText.text = "Flag: " + flagCount;
        if( timerActived ){
            currentTime += Time.deltaTime;
            timerText.text = "Timer: " + (int)currentTime;
        }
    }

    private void GamePreSet()
    {
        int op = SceneLoader.difficulty;
        if( op==0 ){
            op = 3; //default
            Debug.Log("difficulty: 0, default difficulty: " + op);
        }
        switch ( op ){
            case 1:
                Debug.Log("difficulty: 1");
                rowSize = 3; colSize = 3; totalBomb = 2;
                break;
            case 2:
                Debug.Log("difficulty: 2");
                rowSize = 5; colSize = 5; totalBomb = 5;
                break;
            case 3:
                Debug.Log("difficulty: 3");
                rowSize = 6; colSize = 14; totalBomb = 14   ;
                break;
        }

        //urus gameBoardSpawn, ASUMSI NORMAL ROW/COL GABERUBAH2
        float spawnX, spawnY;
        if( rowSize%2==1 ){ //ganjil
            spawnY = ((float)(maxNormalRow/2 - rowSize/2) + 1.5f) * boxWidth - headerDeficit;

        } else { //genap
            spawnY = (float)(maxNormalRow/2 - rowSize/2 + 1) * boxWidth;
            //Debug.LogError( "result: " + ((maxNormalRow/2)) );
        }
        spawnY -= boxWidth;

        if( colSize%2==1 ){ //ganjil
            spawnX = ((float)(maxNormalCol/2 - colSize/2) + 0.5f) * boxWidth;
        } else { //genap
            spawnX = (float)(maxNormalCol/2 - colSize/2 + 1) * boxWidth;
        }
        spawnX -= boxWidth;
        
        SpawnPointObj.transform.position = new Vector3(  gameBoardInitialPoint.x + (spawnX), 
                                                        gameBoardInitialPoint.y - (spawnY),
                                                        gameBoardInitialPoint.z);
                                                        
        gameBoardSpawnPoint = SpawnPointObj.transform.position;

        
        Debug.LogWarning(  "------------------------------------------------------\n" +
                    "rowSize, colSize = " + rowSize + " " + colSize + "\n" +
                    "spawnX, spawnY = " + spawnX + " " + spawnY + "\n" +
                    "INITIALPOINT = " + gameBoardInitialPoint + "\n" + 
                    "SPAWNPOINT = " + gameBoardSpawnPoint);
        

    }

    public void GameSet()
    {
        timerActived = false;
        currentTime = 0;
        timerText.text = "Timer: 0";

        openedCount = 0;
        flagCount = totalBomb;
        board = CreateGameBoard();

        gameButtonImage.sprite = GameOn;
        endPanel.SetActive(false);

    }

    public Box[,] CreateGameBoard()
    {
        board = new Box[rowSize, colSize];
        for( int i=0; i<rowSize; i++ ){
            for( int j=0; j<colSize; j++ ){
                Vector3 boxPosition = new Vector3(j*boxWidth, -i*boxWidth, gameBoardInitialPoint.z);
                board[i,j] = Instantiate(gameBox, gameBoardSpawnPoint + boxPosition, Quaternion.identity);
                board[i,j].isBomb = false;
                board[i,j].rowIndex = i;
                board[i,j].colIndex = j;
                board[i,j].state = BoxState.CLOSED;
                board[i,j].transform.SetParent(this.transform);
            }
        }
        Debug.Log("GameBoard created");

        //generating bomb
        int totalBombCounter = totalBomb;
        while( totalBombCounter > 0 ){
            int i = UnityEngine.Random.Range(0, rowSize);
            int j = UnityEngine.Random.Range(0, colSize);
            if( !board[i,j].isBomb ){
                board[i,j].isBomb = true;
                totalBombCounter--;
            }
        }

        //neighbour status
        for( int i=0; i<rowSize; i++ ){
            for( int j=0; j<colSize; j++ ){
                Debug.Log("CHECKING "+i+j+" ------------------" );
                int counter = 0;
                if( board[i,j].isBomb ){
                    counter = 9;
                    Debug.Log("is a bomb");
                } else {
                    int iPlus = i+1, iMin = i-1, jPlus = j+1, jMin = j-1;
                    //check 1 2 3
                    if( iMin >= 0 ){
                        if( jMin >= 0 ){
                            if( board[iMin, jMin].isBomb ){ counter++; } //1
                            Debug.Log("check 1, counter: "+counter);
                        }

                        if( board[iMin, j].isBomb ){ counter++; } //2
                        Debug.Log("check 2, counter: "+counter);

                        if( jPlus < colSize ){
                            if( board[iMin, jPlus].isBomb ){ counter++; } //3
                            Debug.Log("check 3, counter: "+counter);
                        }
                    }
                    //check 4
                    if( jMin >= 0 ){
                        if( board[i, jMin].isBomb ){ counter++; } //4
                        Debug.Log("check 4, counter: "+counter);
                    }
                    //check 6
                    if( jPlus < colSize ){
                        if( board[i, jPlus].isBomb ){ counter++; } //6
                        Debug.Log("check 6, counter: "+counter);
                    }
                    //check 7 8 9
                    if( iPlus < rowSize ){
                        if( jMin >= 0 ){
                            if( board[iPlus, jMin].isBomb ){ counter++; } //7
                            Debug.Log("check 7, counter: "+counter);
                        }
                        if( board[iPlus, j].isBomb ){ counter++; } //8
                        Debug.Log("check 8, counter: "+counter);
                        if( jPlus < colSize ){
                            if( board[iPlus, jPlus].isBomb ){ counter++; } //9
                            Debug.Log("check 9, counter: "+counter);
                        }
                    }

                    
                }
                board[i,j].neighbour = counter;
                board[i,j].setOpenedSprite( NeighbourSprite[ counter ] );  
                Debug.Log("board[" + i + "," + j + "] neighbour status is " + board[i,j].neighbour);

            }
        }
        return board;

    }

    public void OpenTheBox(int i, int j, bool isRevealing = false)
    {
        //Debug.Log("currentTime: "+ currentTime);
        if( board[i,j].isBomb ){
            board[i,j].boxRenderer.sprite = board[i,j].bomb_clicked_sprite;
            board[i,j].state = BoxState.OPENED;
            if( !isRevealing ){ 
                GameFinished(false);
            }
        } else {
            board[i,j].boxRenderer.sprite = board[i,j].opened_sprite;
            board[i,j].state = BoxState.OPENED;
            openedCount++;
            if( openedCount == 1){ timerActived = true; } 
            if( openedCount == (rowSize*colSize) - totalBomb && !isRevealing){
                GameFinished(true);
            }
        }
    }

    public void FlagTheBox(int i, int j)
    {
        board[i,j].boxRenderer.sprite = board[i,j].flagged_sprite;
        board[i,j].state = BoxState.FLAGGED;
        flagCount--;
    }

    public void UnflagTheBox(int i, int j)
    {
        board[i,j].boxRenderer.sprite = board[i,j].closed_sprite;
        board[i,j].state = BoxState.CLOSED;
        flagCount++;
    }

    public void OpenTheBlankBox(int i, int j)
    {
        OpenTheBox(i,j);

        int iPlus = i+1, iMin = i-1, jPlus = j+1, jMin = j-1;
        //check 1 2 3
        if( iMin >= 0 ){
            if( jMin >= 0 ){
                if( board[iMin, jMin].state == BoxState.CLOSED ){ 
                    if( board[iMin, jMin].neighbour == 0) {
                        OpenTheBlankBox(iMin, jMin);
                    } else {
                        OpenTheBox(iMin, jMin);
                    }  
                } //1
            }

            if( board[iMin, j].state == BoxState.CLOSED ){ 
                if( board[iMin, j].neighbour == 0) {
                    OpenTheBlankBox(iMin, j);
                } else {
                    OpenTheBox(iMin, j);
                } 
            } //2

            if( jPlus < colSize ){
                if( board[iMin, jPlus].state == BoxState.CLOSED ){ 
                    if( board[iMin, jPlus].neighbour == 0) {
                        OpenTheBlankBox(iMin, jPlus);
                    } else {
                        OpenTheBox(iMin, jPlus);
                    } 
                } //3
            }
        }
        //check 4
                
        if( jMin >= 0 ){
            if( board[i, jMin].state == BoxState.CLOSED ){ 
                if( board[i, jMin].neighbour == 0) {
                    OpenTheBlankBox(i, jMin);
                } else {
                    OpenTheBox(i, jMin);
                } 

            } //4
        }
        //check 6
        if( jPlus < colSize ){
            if( board[i, jPlus].state == BoxState.CLOSED ){ 
                if( board[i, jPlus].neighbour == 0) {
                    OpenTheBlankBox(i, jPlus);
                } else {
                    OpenTheBox(i, jPlus);
                } 

            } //6
        }
        //check 7 8 9
        if( iPlus < rowSize ){
            if( jMin >= 0 ){
                if( board[iPlus, jMin].state == BoxState.CLOSED ){ 
                    if( board[iPlus, jMin].neighbour == 0) {
                        OpenTheBlankBox(iPlus, jMin);
                    } else {
                        OpenTheBox(iPlus, jMin);
                    } 
                } //7
            }
            if( board[iPlus, j].state == BoxState.CLOSED ){ 
                if( board[iPlus, j].neighbour == 0) {
                    OpenTheBlankBox(iPlus, j);
                } else {
                    OpenTheBox(iPlus, j);
                }
            } //8
            if( jPlus < colSize ){
                if( board[iPlus, jPlus].state == BoxState.CLOSED ){ 
                    if( board[iPlus, jPlus].neighbour == 0) {
                        OpenTheBlankBox(iPlus, jPlus);
                    } else {
                        OpenTheBox(iPlus, jPlus);
                    } 
                } //9
            }
        }
        
    }

    public void GameFinished( bool isWon )
    {
        timerActived = false;
        //endPanel.SetActive(true);
        if( isWon ){
            Debug.Log("YOU WIN");
            endText.text = "YOU WIN!";
            gameButtonImage.sprite = GameWin;
        } else {
            RevealGameBoard();
            Debug.Log("YOU LOSE");
            endText.text = "YOU LOSE!";
            gameButtonImage.sprite = GameLose;
        }

    }

    private void RevealGameBoard()
    {
        for( int i=0; i<rowSize; i++ ){
            for( int j=0; j<colSize; j++ ){
                if( board[i,j].state == BoxState.CLOSED ){
                    OpenTheBox(i, j, true);
                } else if( board[i,j].state == BoxState.FLAGGED ){
                    if( !board[i,j].isBomb ){
                        board[i,j].boxRenderer.sprite = board[i,j].flagged_wrong_sprite;
                    }

                }
            }
        }
    }

    public void OnGameReset()
    {
        Debug.Log("GameButton clicked");
        DestroyGameBoard();
        GameSet();
    }

    public void DestroyGameBoard()
    {
        for( int i=0; i<rowSize; i++ ){
            for( int j=0; j<colSize; j++ ){
                Destroy( board[i,j].gameObject );
            }
        }
    }

    public void OnXButton()
    {
        DestroyGameBoard();
        sceneLoader.OnBackToMenuButton();
    }




    

    
}
