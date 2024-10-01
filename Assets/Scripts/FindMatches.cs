using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    public void Start()
    {
        board = FindObjectOfType<Board>();

    }
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }


    private List<GameObject> IsAdjacentBomb(Dot dot1,Dot dot2,Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.IsAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.Column,dot1.Row));
        }
        if (dot2.IsAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.Column,dot2.Row));
        }
        if (dot3.IsAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.Column, dot3.Row));
        }
        return currentDots;

    }
    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.IsRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.Row));
        }
        if (dot2.IsRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.Row));
        }
        if (dot3.IsRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.Row));
        }
        return currentDots;
    }
    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.IsColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.Column));
        }
        if (dot2.IsColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.Column));
        }
        if (dot3.IsColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.Column));
        }
        return currentDots;
    }
    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().IsMatched = true;
    }
    private void GetNearByPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }
    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.AllDots[i, j];

                if (currentDot != null)
                {
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.AllDots[i - 1, j];
                        GameObject rightDot = board.AllDots[i + 1, j];


                        if (leftDot != null && rightDot != null)
                        {
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            Dot rightDotDot = rightDot.GetComponent<Dot>();
                            if (leftDot != null && rightDot != null)
                            {
                                if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                                {
                                    currentMatches = currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot)).ToList();
                                    currentMatches = currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot)).ToList();
                                    currentMatches = currentMatches.Union(IsAdjacentBomb(leftDotDot, currentDotDot, rightDotDot)).ToList();

                                    GetNearByPieces(leftDot, currentDot, rightDot);
                                }
                            }

                        }

                    }
                    if (j > 0 && j < board.height - 1)
                    {

                        GameObject upDot = board.AllDots[i, j + 1];
                        GameObject downDot = board.AllDots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {
                            Dot upDotDot = upDot.GetComponent<Dot>();
                            Dot downDotDot = downDot.GetComponent<Dot>();

                            if (upDot != null && downDot != null)
                            {
                                if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                                {
                                    //  Dot currentDotDot = currentDot.GetComponent<Dot>();

                                    currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));
                                    currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));
                                    currentMatches.Union(IsAdjacentBomb(upDotDot, currentDotDot, downDotDot));

                                    GetNearByPieces(upDot, currentDot, downDot);
                                }
                            }
                        }

                    }
                }
            }
        }
    }
    public void MatchPiecesOfColor(string Color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.AllDots[i, j] != null)
                {
                    if (board.AllDots[i, j].tag == Color)
                    {
                        board.AllDots[i, j].GetComponent<Dot>().IsMatched = true;
                    }
                }
            }

        }
    }

    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if (board.AllDots[i,j] !=null)
                    {
                        dots.Add(board.AllDots[i, j]);
                        board.AllDots[i, j].GetComponent<Dot>().IsMatched = true;
                    }
                   
                }
            }
        }
        return dots;
    }
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.AllDots[column, i] != null)
            {
                Dot dot = board.AllDots[column,i].GetComponent<Dot>();
                if (dot.IsRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }

                dots.Add(board.AllDots[column, i]);
                dot.IsMatched= true;
            }

        }
        return dots;
    }
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.AllDots[i, row] != null)
            {

                Dot dot = board.AllDots[i,row].GetComponent<Dot>();
                if (dot.IsColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                dots.Add(board.AllDots[i, row]);
                dot.IsMatched = true;
            }

        }
        return dots;
    }
    public void CheckBombs()
    {
        if (board.currentDot != null)
        {
            if (board.currentDot.IsMatched)
            {
                board.currentDot.IsMatched = false;
                /*
                int typeOfBomb = Random.Range(0, 100);
                if (typeOfBomb < 50)
                {
                    board.currentDot.MakeRowBomb();
                }
                else if (typeOfBomb >= 50)
                {
                    board.currentDot.MakeColumnBomb();
                }
                */
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)
                {
                    board.currentDot.MakeRowBomb();


                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            else if (board.currentDot.OtherDot != null)
            {
                Dot otherDot = board.currentDot.OtherDot.GetComponent<Dot>();
                if (otherDot.IsMatched)
                {
                    otherDot.IsMatched = false;
                    /*
                    int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        otherDot.MakeRowBomb();
                    }
                    else if (typeOfBomb >= 50)
                    {
                        otherDot.MakeColumnBomb();
                    } */
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)
                    {
                        otherDot.MakeRowBomb();


                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}
