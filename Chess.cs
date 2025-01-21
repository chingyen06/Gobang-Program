using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Chess : MonoBehaviour
{
    //�ѽL���I
    public GameObject LeftTop;
    public GameObject RightTop;
    public GameObject LeftBottom;
    public GameObject RightBottom;

    //���sUI
    public GameObject buttonPanel;
    
    //�D��v��
    public Camera cam;

    //�¥մѹϤ����z
    public Texture2D black;
    public Texture2D white;

    //�¥դ�ӧQ�Ϥ�
    public Texture2D blackWin;
    public Texture2D whiteWin;

    //���s�}�l���s
    public Button restartBtn;

    //�|�����I���ù���m
    Vector3 LTPos;
    Vector3 RTPos;
    Vector3 LBPos;
    Vector3 RBPos;

    //�x�s�ѽL�W�C�ӥ��I����m
    List<List<Vector2>> chessPos;

    //�w�q�ѽL���檺�e�סB����
    float gridWidth = 1;
    float gridHeight = 1;
    //�b����e�׻P���פ������p��
    float minGridDis;

    //�x�s�ѽL�W�����l���A
    List<List<int>> chessState;

    //�P�_���l��
    int flag = 0;

    //��l�Ƭ�0�A��ܵL��Ӥ�
    int winner = 0;

    //�N��C���O�_���b�i�椤
    bool isPlaying = true;

    // Start is called before the first frame update
    void Start()
    {
        //�p�����I�b�ù��W����m
        LTPos = cam.WorldToScreenPoint(LeftTop.transform.position);
        RTPos = cam.WorldToScreenPoint(RightTop.transform.position);
        LBPos = cam.WorldToScreenPoint(LeftBottom.transform.position);
        RBPos = cam.WorldToScreenPoint(RightBottom.transform.position);

        //�p��ѽL���檺�e�סB����
        gridWidth = (RTPos.x - LTPos.x) / 14;
        gridHeight = (LTPos.y - LBPos.y) / 14;

        //�����p�����涡�Z�A�T�O����O����Ϊ�
        /*�@�Φp�P���`��
          if (gridWidth < gridHeight)
            minGridDis = gridWidth;
          else
            minGridDis = gridHeight;
        */
        minGridDis = gridWidth < gridHeight ? gridWidth : gridHeight;

        //��l�Ƭ� 15 �� 15 ���G���C��
        chessPos = new List<List<Vector2>>();
        chessState = new List<List<int>>();
        for (int i = 0; i < 15; i++) {
            chessPos.Add(new List<Vector2>());
            chessState.Add(new List<int>());

            for (int j = 0; j < 15; j++) {
                chessPos[i].Add(Vector2.zero);
                chessState[i].Add(0);
            }
        }

        //�p��ѽL�W�i�H���l����m
        for (int i = 0; i < 15; i++)
            for (int j = 0; j < 15; j++)
                chessPos[i][j] = new Vector2(LBPos.x + gridWidth * i, LBPos.y + gridHeight * j);

        //�K�[���s�}�l���s����ť��
        restartBtn.onClick.AddListener(Restart);
    }

    //���s�}�l���s�����
    private void Restart()
    {
        for (int i = 0; i < 15; i++)
            for (int j = 0; j < 15; j++)
                chessState[i][j] = 0;  //�N�ѽL���l���A�]�m����

        isPlaying = true;  //�C���~��i��
        winner = 0;  //��Ӥ�]��0�A�N��L��Ӥ�
        flag = 0;  //�N��@�}�l���O�´ѥ��U
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PointPos;
        if (Input.GetMouseButtonDown(0)) {
            PointPos = Input.mousePosition;

            if (isPlaying && PlaceChess(PointPos))
                flag = 1 - flag;

            CheckWinFor();
        }
    }

    void OnGUI()
    {
        for (int i = 0; i < 15; i++) {
            for (int j = 0; j < 15; j++) {
                if (chessState[i][j] == 1)  //ø�s�´�
                    GUI.DrawTexture(new Rect(chessPos[i][j].x - gridWidth / 2, Screen.height - chessPos[i][j].y - gridHeight / 2, gridWidth, gridHeight), black);
                
                if (chessState[i][j] == -1)  //ø�s�մ�
                    GUI.DrawTexture(new Rect(chessPos[i][j].x - gridWidth / 2, Screen.height - chessPos[i][j].y - gridHeight / 2, gridWidth, gridHeight), white);
            }
        }

        //�����Ӥ誺�Ϥ�
        if (winner == 1)  //��ܶ´���Ӫ��Ϥ�
            GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.15f, Screen.width * 0.2f, Screen.height * 0.25f), blackWin);
        if (winner == -1)  //��ܥմ���Ӫ��Ϥ�
            GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.15f, Screen.width * 0.2f, Screen.height * 0.25f), whiteWin);
    }

    //�p����I���ڴX���o�Z��
    private float Distance(Vector3 mPos, Vector2 gridPos)
    {
        return Mathf.Sqrt(Mathf.Pow(mPos.x - gridPos.x, 2) + Mathf.Pow(mPos.y - gridPos.y, 2));
    }

    //�ھڪ��a�I����m�A�M��̪񪺴ѽL��m�U��
    private bool PlaceChess(Vector3 PointPos)
    {
        float minDis = float.MaxValue;  //�̤p��m
        Vector2 closestPos = Vector2.zero;  //�̪񪺦�m
        int closestX = -1, closestY = -1;  //�̪��m��x�By�y�Ъ����ޭ�

        //�M���ѽL
        for (int i = 0; i < 15; i++) {
            for (int j = 0; j < 15; j++) {
                float dist = Distance(PointPos, chessPos[i][j]);

                if (dist < minGridDis / 2 && chessState[i][j] == 0) {
                    minDis = dist;
                    closestPos = chessPos[i][j];
                    closestX = i;
                    closestY = j;
                }
            }
        }

        if (closestX != -1 && closestY != -1) {
            chessState[closestX][closestY] = flag == 0 ? 1 : -1;
            return true;  //���l���\
        }

        return false;  //���l����
    }

    //�ˬd���l�s�@�_����Ө��
    private int CheckWin(List<List<int>> board)
    {
        foreach(var boardList in board) {
            //���]boardList=[1,-1,0,0,1]�A�ϥ�Select��Ǧ^�r���ǦC['X','O',' ', ' ', 'X']
            //ToArray()��r���ǦC�ഫ���r�� "XO  O"
            string boardRow = new string(boardList.Select(i => i == 1 ? 'X' : (i == -1 ? 'O' : ' ')).ToArray());

            if (boardRow.Contains("XXXXX"))  //�´����
                return 1;
            else if (boardRow.Contains("OOOOO"))  //�մ����
                return 0;
        }

        return -1;  //�L��Ӥ�
    }

    private List<int> checkWinAll(List<List<int>> board)
    {
        //�Ыؤ@�Ӧs���סB�ϱפ�V���G���C��
        List<List<int>> boardC = new List<List<int>>();  //�ϱ�
        List<List<int>> boardD = new List<List<int>>();  //����

        for (int i = 0; i < 29; i++) {
            //���O��ܥ]�t29�ӪŦC���G���C��
            boardC.Add(new List<int>());
            boardD.Add(new List<int>());
        }

        for (int i = 0; i < 15; i++) {
            for (int j = 0; j < 15; j++) {
                boardC[i + j].Add(board[i][j]);
                boardD[i - j + 14].Add(board[i][j]);
                string str = "BoardC[i + j]:";
                Debug.Log($"i: {i}, j: {j}, {str} boardC[{i + j}]: {string.Join(", ", boardC[i + j])}");
            }
        }

        Debug.Log("�̫�զX�����G");
        Debug.Log("======================================");

        for (int i = 0; i < boardC.Count; i++) {
            string str = $"boardC[{i}]: ";

            foreach (var item in boardC[i])
                    str += item + " ";
            Debug.Log(str);
        }

        Debug.Log("======================================");

        for (int i = 0; i < board.Count; i++) {
            string str = $"board[{i}]: ";

            foreach (var item in board[i])
                str += item + " ";
            Debug.Log(str);
        }

        return new List<int>
        {
            CheckWin(board),
            CheckWin(transpose(board)),
            CheckWin(boardC),
            CheckWin(boardD)
        };
    }

    private void CheckWinFor()
    {
        List<int> result = checkWinAll(chessState);  //����ѽL�ثe���l���A���ӧQ���p

        if (result.Contains(0)) {
            Debug.Log("�մ����");
            winner = -1;
            isPlaying = false;  //�N��C������
        }
        else if (result.Contains(1)) {
            Debug.Log("�´����");
            winner = 1;
            isPlaying = false;  //�N��C������
        }
    }

    //�N�C����m�����
    private List<List<int>> transpose (List<List<int>> board)
    {
        int rowMatrix = board.Count;  //���o��ӤG���C��̤@���C���ƶq
        int colMatrix = board[0].Count;  //���o�@���C�������ƶq
        
        //�Ыؤ@����m�᪺�G���C��
        List<List<int>> transposed = new List<List<int>>();
        
        for (int i = 0; i < colMatrix; i++) {
            List<int> newRow = new List<int>();
            
            for (int j = 0; j < rowMatrix; j++)
                newRow.Add(board[j][i]);

            transposed.Add(newRow);
        }

        return transposed;
    }
}