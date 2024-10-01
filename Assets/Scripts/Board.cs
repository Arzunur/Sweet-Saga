using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}
public enum TileKind
{
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;

    public int width;
    public int height;


    public Dot currentDot;
    private FindMatches findMatches;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;

    public bool[,] BlankSpaces;
    public TileType[] boardLayout;
    private BackgroundTile[,] breakableTiles;



    public GameObject TilePrefab;
    public GameObject[] Dots;
    public GameObject[,] AllDots;
    public GameObject effect;

    public Vector3 scale = new Vector3(1f, 1f, 1f);

    [SerializeField] private int nullCount = 0;
    [SerializeField] private int offset;
    [SerializeField] private GameObject breakableTilePrefab;
    [SerializeField] private int basePiecesValue = 20; //doðru eþleþme 
    [SerializeField] private int streakValue = 20;//ekstra puan
    [SerializeField] private float refillDelay = .5f;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        soundManager = FindObjectOfType<SoundManager>();
        goalManager = FindObjectOfType<GoalManager>();
    }
    public void Start()
    {
        breakableTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        BlankSpaces = new bool[width, height]; //Bos alanlari temsil eden dizi 
        AllDots = new GameObject[width, height];
        SetUp();
    }
    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                BlankSpaces[boardLayout[i].x, boardLayout[i].y] = true;

            }
        }
    }
    public void GenerateBreakableTile()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    public void SetUp()
    {
        GenerateBlankSpaces();
        GenerateBreakableTile();
        for (int i = 0; i < width; i++)//x eks
        {
            for (int j = 0; j < height; j++) //y eks
            {
                if (!BlankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(TilePrefab, tilePosition, Quaternion.identity) as GameObject;// TilePrefab nesnesini oluþtur ve board'a ekle
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + "," + j + ")";
                    int dotToUse = Random.Range(0, Dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, Dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, Dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(Dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().Row = j;
                    dot.GetComponent<Dot>().Column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + "," + j + ")";
                    AllDots[i, j] = dot;
                }
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (AllDots[column - 1, row] != null && AllDots[column - 2, row] != null)
            {
                if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (AllDots[column, row - 1] != null && AllDots[column, row - 2] != null)
            {
                if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (AllDots[column, row - 1] != null && AllDots[column, row - 2] != null)
                {
                    if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (AllDots[column - 1, row] != null && AllDots[column - 2, row] != null)

                {
                    if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }

            }
        }
        return false;
    }
    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.Row == firstPiece.Row)
                {
                    numberHorizontal++;
                }
                if (dot.Column == firstPiece.Column)
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);
    }
    private void CheckToMakeBomb()
    {
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7){ 
            findMatches.CheckBombs();
        }

        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8) {
            if (ColumnOrRow()){ 

                if (currentDot != null) {
                    if (currentDot.IsMatched) {
                        if (!currentDot.IsColorBomb) {
                            currentDot.IsMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }else
                    {
                        if (currentDot.OtherDot != null)
                        {
                            Dot otherDot = currentDot.OtherDot.GetComponent<Dot>();
                            if (otherDot.IsMatched){
                                if (!otherDot.IsColorBomb) {
                                    otherDot.IsMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
            else 
            {
                if (currentDot != null)
                {
                    if (currentDot.IsMatched)
                    {
                        if (!currentDot.IsAdjacentBomb)
                        {
                            currentDot.IsMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.OtherDot != null)
                        {
                            Dot otherDot = currentDot.OtherDot.GetComponent<Dot>();
                            if (otherDot.IsMatched)
                            {
                                if (!otherDot.IsAdjacentBomb)
                                {
                                    otherDot.IsMatched = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (AllDots[column, row].GetComponent<Dot>().IsMatched)
        {
            if (findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBomb();
            }
            if (breakableTiles[column, row] != null) //does a tile need to break
            {
                breakableTiles[column, row].TakeDamge(1); //hasar vermek 1 
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }
            if (goalManager != null)
            {
                goalManager.CompareGoal(AllDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }
            if (soundManager != null)
            {
                soundManager.PlaySoundEffect();
            }
            GameObject effectInstance = Instantiate(effect, AllDots[column, row].transform.position, Quaternion.Euler(-90f, 0f, 0f));
            effectInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Destroy(AllDots[column, row]);
            scoreManager.Score(basePiecesValue * streakValue);
            AllDots[column, row] = null;
            Destroy(effectInstance, 0.1f);
        }
    }
    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (AllDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }
    private IEnumerator DecreaseRowCo()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!BlankSpaces[i, j] && AllDots[i, j] == null)
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (AllDots[i, k] != null)
                        {
                            AllDots[i, k].GetComponent<Dot>().Row = j;
                            AllDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoard());
    }
    private IEnumerator DecreaseRow()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (AllDots[i, j] == null)
                {
                    nullCount++;

                }
                else if (nullCount > 0)
                {
                    AllDots[i, j].GetComponent<Dot>().Row -= nullCount;
                    AllDots[i, j] = null;

                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoard());
    }
    private void RefillBoard() //yok olan objleri tekrar doldurmak
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (AllDots[i, j] == null && !BlankSpaces[i, j])
                {
                    Vector2 tempPos = new Vector2(i, j + offset);
                    int dotToUse = Random.Range(0, Dots.Length);
                    int maxIterations = 0;

                    //Burasý performans açýsýndan skýntýlý tekrar gözden geçir
                    while (MatchesAt(i, j, Dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, Dots.Length);
                    }
                    maxIterations = 0;

                    GameObject piece = Instantiate(Dots[dotToUse], tempPos, Quaternion.identity);
                    AllDots[i, j] = piece;
                    piece.GetComponent<Dot>().Row = j;
                    piece.GetComponent<Dot>().Column = i;

                }
            }

        }

    }
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (AllDots[i, j] != null)
                {
                    if (AllDots[i, j].GetComponent<Dot>().IsMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield return new WaitForSeconds(2 * refillDelay);

        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.5f);
        if (IsDeadLocked())
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            Debug.Log("FINISH");

        }
        //yield return new WaitForSeconds(refillDelay);
        currentState = GameState.move;
        streakValue = 1;

    }
    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        GameObject holder = AllDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        AllDots[column + (int)direction.x, row + (int)direction.y] = AllDots[column, row];
        AllDots[column, row] = holder;
    }
    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (AllDots[i, j] != null)
                {
                    if (i < width - 2)
                    {
                        if (AllDots[i + 1, j] != null && AllDots[i + 2, j] != null)
                        {
                            if (AllDots[i + 1, j].tag == AllDots[i, j].tag && AllDots[i + 2, j].tag == AllDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        if (AllDots[i, j + 1] != null && AllDots[i, j + 2] != null)
                        {
                            if (AllDots[i, j + 1].tag == AllDots[i, j].tag && AllDots[i, j + 2].tag == AllDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    private bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }
    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (AllDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }

                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
}
