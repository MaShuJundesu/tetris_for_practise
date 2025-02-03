using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class tetromino : MonoBehaviour
{
    private float fallspeed;
    float fall = 0;

    public bool allowRotation=true;//允許旋轉
    public bool limitRotation=false;//限制轉兩個方向
    public AudioClip movesound;
    public AudioClip rotatesound;
    public AudioClip ClearLineSound;
    private AudioSource audioSource;

    private float continuousVerticalSpeed = 0.05f;
    private float continuousHorizontalSpeed =0.1f;
    private float buttonDownWaitMax = 0.2f;

    private float DownhorizontalTimer = 0;
    private float horizontalTimer = 0;
    private float DownverticalTimer = 0;
    private float verticalTimer = 0;

    private bool moveTimerLock = false;
    private bool DownTimerLock = false;

    void Start()
    {
        audioSource=GetComponent<AudioSource>();
        fallspeed = GameObject.Find("Grid").GetComponent<Game>().fallspeed;
        Debug.Log("目前速度"+fallspeed);
    }


    // Update is called once per frame
    void Update()
    {
        tetromoveInput();
        //Debug.Log(Time.time);
    }
    void PlayTheSound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }
    void tetromoveInput()
    {

        if (Input.GetKeyUp(KeyCode.LeftArrow)|| Input.GetKeyUp(KeyCode.RightArrow))//|| || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) 
        {
            horizontalTimer = 0;
            verticalTimer = 0;
            moveTimerLock =false;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            DownhorizontalTimer = 0;
            DownverticalTimer = 0;
            DownTimerLock = false;
        }
        //FindObjectOfType<Game>().UpdateGrid(this);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveLeft();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveRight();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
             
        }
        if (Input.GetKey(KeyCode.DownArrow)|| Time.time-fall>=fallspeed)
        {
           moveDown();
        }
    }
    void moveLeft() {
        if (moveTimerLock)
        {
            if (verticalTimer < buttonDownWaitMax)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
            horizontalTimer = 0;
        }
        if (!moveTimerLock)
        {
            moveTimerLock = true;
        }
        PlayTheSound(movesound);
        transform.position += new Vector3(-1, 0, 0);
        if (ChenkIsValidPosition())
        {

        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
            FindObjectOfType<Game>().UpdateGrid(this);
        }
    }

    void moveRight() {
        if (moveTimerLock)
        {
            if (verticalTimer < buttonDownWaitMax)
            {
                verticalTimer += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
            horizontalTimer = 0;
        }
        if (!moveTimerLock)
        {
            moveTimerLock = true;
        }

        PlayTheSound(movesound);
        transform.position += new Vector3(1, 0, 0);
        if (ChenkIsValidPosition())
        {

        }//若碰觸邊界
        else
        {

            transform.position += new Vector3(-1, 0, 0);
            FindObjectOfType<Game>().UpdateGrid(this);
        }
    }


    void moveDown() {
        if (DownTimerLock)
        {
            if (DownverticalTimer < buttonDownWaitMax)
            {
                DownverticalTimer += Time.deltaTime;
                return;
            }

            if (DownhorizontalTimer < continuousVerticalSpeed)
            {
                DownhorizontalTimer += Time.deltaTime;
                return;
            }
            DownhorizontalTimer = 0;
        }
        if (!DownTimerLock)
        {
            DownTimerLock = true;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            PlayTheSound(movesound);
        }
        
        transform.position += new Vector3(0, -1, 0);

        fall = Time.time;
        if (ChenkIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
            }
        }
        else
        {

            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<Game>().DelRow();
            if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<Game>().GameOver();
            }

            FindObjectOfType<Game>().SpawnTetromino();
            enabled = false;
        }
    }
    void Rotate() {
        if (allowRotation)
        {

            transform.Rotate(0, 0, -90);
            PlayTheSound(rotatesound);

            if (ChenkIsValidPosition())
            {
                //Debug.Log("沒碰邊界");
                if (limitRotation && transform.eulerAngles.z >= 90)
                {

                    transform.Rotate(0, 0, 180);
                }
            }
            else
            {
                //Debug.Log("碰邊界");
                transform.Rotate(0, 0, 90);
                FindObjectOfType<Game>().UpdateGrid(this);

            }


        }
        else { }
    }
    bool ChenkIsValidPosition(){
        foreach (Transform mino in transform)
        {
            //位子矯正
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
            //確認是否碰到邊界
            if (FindObjectOfType<Game>().CheckInsideGrid(pos) == false){
                return false;
            }
            if(FindObjectOfType<Game>().GetTransfromAtGridPostion(pos) != null && FindObjectOfType<Game>().GetTransfromAtGridPostion(pos).parent != transform  ) {
                return false;
                
            }
        }
        return true;
    }
    
    /*float Adjustvolume(float volume)
    {
        foreach(AudioSource echo in transform)
        {
             echo.volume=volume ;
        }
        return volume;
    }*/
}
