using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Board board;
    [SerializeField] private float cameraOffset;
    [SerializeField] private float aspectRatio;//en boy orani
    [SerializeField] private float padding;
    [SerializeField] private float yOffset;


    private void Awake()
    {
        board=FindObjectOfType<Board>();
    }
    void Start()
    {
        if(board != null)
        {
            RepositionCamera(board.width-1,board.height-1);
        }
    }

    private void RepositionCamera(float x,float y)
    {
        Vector3 tempPosition = new Vector3(x / 2, y / 2 + yOffset, cameraOffset);
        transform.position = tempPosition;
        if (board.width>=board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;

        }
        else
        {
            Camera.main.orthographicSize = (board.height / 2 + padding);

        }
    }
}
