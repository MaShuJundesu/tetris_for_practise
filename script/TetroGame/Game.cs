using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


//bug 名單 LineScore數值錯誤或錯位  mino降到底後機率出現網格錯位
public class Game : MonoBehaviour
{
    public float fallspeed = 1.0f;
    public static int GridWidth = 10;
    public static int GridHeight = 20;
    public static Transform[,] grid = new Transform[GridWidth, GridHeight];
    public int[] LineScore = new int[]{40, 100, 300, 1200};
    private int howmanyLineInThisTrun = 0;
    private int currentScore = 0;
    private int levelBar = 0;
    private int NowLevel = 1;
    public Text hub_Score;

    private GameObject NextMino;
    private GameObject previewTetroomino;

    private bool GameStarted=false;
    private Vector2 viewTetromino=new Vector2(-6f,15f);
    // Start is called before the first frame update
    void Start()
    {
        SpawnTetromino();
        Debug.Log(LineScore[0]);
        Debug.Log(LineScore[1]);
        Debug.Log(LineScore[2]);
        Debug.Log(LineScore[3]);
    }
    private void Update()
    {
        UpdateScoreToUI();
    }
    void UpdateLevelAndSpeed()
    {
        if (levelBar > 10)
        {
            levelBar -= 10;
            NowLevel += 1;
            Debug.Log("SpeedUp");
            fallspeed-=0.1f;//10關後變為0後
        }
        
        
    }


    public void UpdateScore()
    {
        if (howmanyLineInThisTrun > 0)
        {
            levelBar += howmanyLineInThisTrun;
            UpdateLevelAndSpeed();
            currentScore+= LineScore[howmanyLineInThisTrun - 1];
            
            howmanyLineInThisTrun = 0;
            

        }
    }
    public void UpdateScoreToUI()
    {
        hub_Score.text = "Score:"+currentScore.ToString();
    }
    public bool CheckIsAboveGrid(tetromino tetromino)
    {
        for (int x = 0; x < GridWidth; x++) {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);
                if(pos.y> GridHeight-1) 
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool SreachFullLineAt(int y)
    {
        for (int x = 0; x < GridWidth; x++)
        {
            if (grid[x, y] == null)
            {
                UpdateScore();
                return false;
               
            }
        }
        howmanyLineInThisTrun++;
        return true;
    }
    public void DelminoAt(int y)
    {
        for (int x = 0; x < GridWidth; x++)
        {
            Destroy(grid[x,y].gameObject);
            grid[x, y]=null;
        }
    }
    
    public void MoveRowDown(int y)
    {
        for (int x=0;x<GridWidth;x++) {
            if (grid[x, y] != null)
            {
                grid[x, y-1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position +=new Vector3(0,-1,0);
            }
        }
    }
    public void MoveAllLineDown(int y)
    {
        for (int i = y; i < GridHeight; i++) {
            MoveRowDown(i);

        }
    }
    public void DelRow()
    {
        for (int i = 0; i < GridHeight; i++)
        {
            if (SreachFullLineAt(i))
            {
                DelminoAt(i);
                MoveAllLineDown(i+1);
                i--;
            }
        }
    }
    public void UpdateGrid(tetromino tetromino)
    {
        for (int y = 0; y < GridHeight; y++) {
            for (int x = 0; x < GridWidth; x++) { 
                if(grid[x, y] !=null){
                    
                    if(grid[x, y].parent== tetromino.transform)
                    {
                        
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach(Transform mino in tetromino.transform)
        {
            
            Vector2 pos= Round(mino.position);
            if (pos.y < GridHeight)
            {
                //Debug.Log(mino.position);
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransfromAtGridPostion(Vector2 pos)
    {
        if (pos.y > GridHeight - 1)
        {
            return null;
        }
        else {
            return grid[(int)pos.x, (int)pos.y];
        }
    }
   
    public bool CheckInsideGrid(Vector2 pos)
    {   //確認邊界
        return ((int)pos.x>=0 && (int)pos.x<GridWidth && (int)pos.y>=0);
    }
    
    public Vector2 Round(Vector2 pos)
    {   //防止位移
        return new Vector2(Mathf.Round(pos.x),Mathf.Round(pos.y));
    }
    string RendomTetromino(){
        int rendomino=Random.Range(1,8);
        string minoName= "item/I";

        switch (rendomino)
        {
            case 1:
                minoName = "item/I";
                break;
            case 2:
                minoName = "item/J";
                break;
            case 3:
                minoName = "item/L";
                break;
            case 4:
                minoName = "item/O";
                break;
            case 5:
                minoName = "item/S";
                break;
            case 6:
                minoName = "item/T";
                break;
            case 7:
                minoName = "item/Z";
                break;
        }
        return minoName;
    }
    
    public void SpawnTetromino()
    {
        if (!GameStarted)
        {
            GameStarted = true;
            GameObject NextTromino = (GameObject)Instantiate(Resources.Load(RendomTetromino()), new Vector2(4f, 20f), Quaternion.identity);
            NextMino = (GameObject)Instantiate(Resources.Load(RendomTetromino()), viewTetromino, Quaternion.identity);
            NextMino.GetComponent<tetromino>().enabled = false; 
        }
        else {
            NextMino.transform.position = new Vector2(5f, 20f);
            NextMino.GetComponent<tetromino>().enabled = true;
            previewTetroomino = NextMino;

            NextMino = (GameObject)Instantiate(Resources.Load(RendomTetromino()), viewTetromino, Quaternion.identity);
            NextMino.GetComponent<tetromino>().enabled = false;

        }

        
    }
    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

}
