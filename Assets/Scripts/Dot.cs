using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int Column;
    public int Row;
    private int previousColumn;
    private int previousRow;
    public bool IsMatched = false;
    public bool IsColumnBomb;
    public bool IsRowBomb;
    public bool IsColorBomb;
    public bool IsAdjacentBomb;

    private FindMatches findMatches;
    public GameObject OtherDot;
    public GameObject ColorBomb;
    public GameObject AdjacentMarker;

    private EndGame endGame;
    private Board board;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;

    [SerializeField] private GameObject rowArrow;
    [SerializeField] private GameObject columnArrow;
    [SerializeField] public int targetX;
    [SerializeField] private int targetY;
    public float swipeAngle = 0f;
    [SerializeField] private float swipeResist = 1f;

    private void Start()
    {
        IsColumnBomb = false;
        IsRowBomb = false;
        IsColorBomb = false;
        IsAdjacentBomb = false;

        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        endGame = FindObjectOfType<EndGame>();
    }

    private void Update()
    {
        targetX = Column;
        targetY = Row;
       //noktanýn y koordinati ile hedef y koordinati arasýndaki fark 
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPos = new Vector2(targetX, transform.position.y);
            //noktanýn pos hedef pos güncelleme islemi
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);

            if (board.AllDots[Column, Row] != this.gameObject) 
            {
                board.AllDots[Column, Row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //Hedef pos ulasildiysa pozisyonu ayarla
            tempPos = new Vector2(targetX, transform.position.y); 
            transform.position = tempPos;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);

            if (board.AllDots[Column, Row] != this.gameObject)
            {
                board.AllDots[Column, Row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }
    private void OnMouseOver() // Fare imlecinin GameObject'in uzerinde oldugunda
    {
        if (Input.GetMouseButtonDown(1))
        {
            IsAdjacentBomb = true;
            GameObject marker = Instantiate(AdjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
    }
    public IEnumerator CheckMove()
    {
        if (IsColorBomb)
        {
            findMatches.MatchPiecesOfColor(OtherDot.tag);
            IsMatched = true; //eslestirme = true
        }
        else if (OtherDot.GetComponent<Dot>().IsColorBomb)
        {
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            OtherDot.GetComponent<Dot>().IsMatched = true;
        }
        yield return new WaitForSeconds(.5f);

        if (OtherDot != null)
        {
            if (!IsMatched && !OtherDot.GetComponent<Dot>().IsMatched)
            {
                OtherDot.GetComponent<Dot>().Row = Row;
                OtherDot.GetComponent<Dot>().Column = Column;
                Row = previousRow; //Bu noktanin satirini önceki satira ayarlamak
                Column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }
            else
            {
                if (endGame!=null)
                {
                    if (endGame.requirement.gameType==GameType.Moves)
                    {
                        endGame.DecreaseCounterValue(); //oyuncunun kalan hareket hakkini 1 azaltir
                    }
                }
                board.DestroyMatches();//eslesen parcalar yok edilir
            }
        }
    }
    private void OnMouseDown() //fare tiklanmasi basladiginda
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Debug.Log(firstTouchPos);
        }
    }
    private void OnMouseUp() //fare tiklanmasi sonlandýgýnda
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
    public void CalculateAngle() //kaydirma acisi
    {
        if (Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist) //kaydirma hareketi >  kaydýrma direncinden (swipeResist)
        {
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI; //radyan'i dereceye cevirme islemi
                                                                                                                             // Debug.Log(swipeAngle);
            MovePieces();// parca hareketi

            board.currentDot = this;
        }
        else
        { board.currentState = GameState.move;}
    }
    private void MovePiecesActual(Vector2 direction)
    {
        OtherDot = board.AllDots[Column + (int)direction.x, Row + (int)direction.y];
        previousRow = Row;
        previousColumn = Column;
        if(OtherDot !=null)
        {
            OtherDot.GetComponent<Dot>().Column += -1 * (int)direction.x;
            OtherDot.GetComponent<Dot>().Row += -1 * (int)direction.y;
            Column += (int)direction.x;
            Row += (int)direction.y;
            StartCoroutine(CheckMove());
        }
        else
        {
            board.currentState = GameState.move;
        }
      
    }
    private void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && Column < board.width - 1) // right
        {
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && Row < board.height - 1) // up
        {
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && Column > 0) // left
        {
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && Row > 0) // down
        {
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        }
    }
    void FindMatches()
    {
        if (Column > 0 && Column < board.width - 1)
        {
            GameObject leftDot = board.AllDots[Column - 1, Row];
            GameObject rightDot = board.AllDots[Column + 1, Row];

            if (leftDot != null && rightDot != null)
            {

                if (leftDot.tag == this.gameObject.tag && rightDot.tag == this.gameObject.tag)
                {
                    leftDot.GetComponent<Dot>().IsMatched = true;
                    rightDot.GetComponent<Dot>().IsMatched = true;
                    IsMatched = true;

                }
            }
        }
        if (Row > 0 && Row < board.height - 1)
        {
            GameObject upDot = board.AllDots[Column, Row + 1];
            GameObject downDot = board.AllDots[Column, Row - 1];

            if (upDot != null && downDot != null)
            {

                if (upDot.tag == this.gameObject.tag && downDot.tag == this.gameObject.tag)
                {
                    upDot.GetComponent<Dot>().IsMatched = true;
                    downDot.GetComponent<Dot>().IsMatched = true;
                    IsMatched = true;

                }

            }
        }
    }
    public void MakeRowBomb()
    {
            IsRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
    }
    public void MakeColumnBomb()
    {   
            IsColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;  
    }
    public void MakeColorBomb()
    {
            IsColorBomb = true;
            GameObject color = Instantiate(ColorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
            this.gameObject.tag = "Color";
    }
    public void MakeAdjacentBomb()
    {
            IsAdjacentBomb = true;
            GameObject marker = Instantiate(AdjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
    }
}
