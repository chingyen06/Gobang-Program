using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Chess : MonoBehaviour
{
    //棋盤錨點
    public GameObject LeftTop;
    public GameObject RightTop;
    public GameObject LeftBottom;
    public GameObject RightBottom;

    //按鈕UI
    public GameObject buttonPanel;
    
    //主攝影機
    public Camera cam;

    //黑白棋圖片紋理
    public Texture2D black;
    public Texture2D white;

    //黑白方勝利圖片
    public Texture2D blackWin;
    public Texture2D whiteWin;

    //重新開始按鈕
    public Button restartBtn;

    //四個錨點的螢幕位置
    Vector3 LTPos;
    Vector3 RTPos;
    Vector3 LBPos;
    Vector3 RBPos;

    //儲存棋盤上每個交點的位置
    List<List<Vector2>> chessPos;

    //定義棋盤網格的寬度、高度
    float gridWidth = 1;
    float gridHeight = 1;
    //在網格寬度與高度中取較小值
    float minGridDis;

    //儲存棋盤上的落子狀態
    List<List<int>> chessState;

    //判斷落子方
    int flag = 0;

    //初始化為0，表示無獲勝方
    int winner = 0;

    //代表遊戲是否正在進行中
    bool isPlaying = true;

    // Start is called before the first frame update
    void Start()
    {
        //計算錨點在螢幕上的位置
        LTPos = cam.WorldToScreenPoint(LeftTop.transform.position);
        RTPos = cam.WorldToScreenPoint(RightTop.transform.position);
        LBPos = cam.WorldToScreenPoint(LeftBottom.transform.position);
        RBPos = cam.WorldToScreenPoint(RightBottom.transform.position);

        //計算棋盤網格的寬度、高度
        gridWidth = (RTPos.x - LTPos.x) / 14;
        gridHeight = (LTPos.y - LBPos.y) / 14;

        //取較小的網格間距，確保網格是正方形的
        /*作用如同此注釋
          if (gridWidth < gridHeight)
            minGridDis = gridWidth;
          else
            minGridDis = gridHeight;
        */
        minGridDis = gridWidth < gridHeight ? gridWidth : gridHeight;

        //初始化為 15 × 15 的二維列表
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

        //計算棋盤上可以落子的位置
        for (int i = 0; i < 15; i++)
            for (int j = 0; j < 15; j++)
                chessPos[i][j] = new Vector2(LBPos.x + gridWidth * i, LBPos.y + gridHeight * j);

        //添加重新開始按鈕的監聽器
        restartBtn.onClick.AddListener(Restart);
    }

    //重新開始按鈕的函數
    private void Restart()
    {
        for (int i = 0; i < 15; i++)
            for (int j = 0; j < 15; j++)
                chessState[i][j] = 0;  //將棋盤落子狀態設置為空

        isPlaying = true;  //遊戲繼續進行
        winner = 0;  //獲勝方設為0，代表無獲勝方
        flag = 0;  //代表一開始都是黑棋先下
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
                if (chessState[i][j] == 1)  //繪製黑棋
                    GUI.DrawTexture(new Rect(chessPos[i][j].x - gridWidth / 2, Screen.height - chessPos[i][j].y - gridHeight / 2, gridWidth, gridHeight), black);
                
                if (chessState[i][j] == -1)  //繪製白棋
                    GUI.DrawTexture(new Rect(chessPos[i][j].x - gridWidth / 2, Screen.height - chessPos[i][j].y - gridHeight / 2, gridWidth, gridHeight), white);
            }
        }

        //顯示獲勝方的圖片
        if (winner == 1)  //顯示黑棋獲勝的圖片
            GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.15f, Screen.width * 0.2f, Screen.height * 0.25f), blackWin);
        if (winner == -1)  //顯示白棋獲勝的圖片
            GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.15f, Screen.width * 0.2f, Screen.height * 0.25f), whiteWin);
    }

    //計算兩點的歐幾里得距離
    private float Distance(Vector3 mPos, Vector2 gridPos)
    {
        return Mathf.Sqrt(Mathf.Pow(mPos.x - gridPos.x, 2) + Mathf.Pow(mPos.y - gridPos.y, 2));
    }

    //根據玩家點擊位置，尋找最近的棋盤位置下棋
    private bool PlaceChess(Vector3 PointPos)
    {
        float minDis = float.MaxValue;  //最小位置
        Vector2 closestPos = Vector2.zero;  //最近的位置
        int closestX = -1, closestY = -1;  //最近位置的x、y座標的索引值

        //遍歷棋盤
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
            return true;  //落子成功
        }

        return false;  //落子失敗
    }

    //檢查五子連一起的獲勝函數
    private int CheckWin(List<List<int>> board)
    {
        foreach(var boardList in board) {
            //假設boardList=[1,-1,0,0,1]，使用Select後傳回字元序列['X','O',' ', ' ', 'X']
            //ToArray()把字元序列轉換成字串 "XO  O"
            string boardRow = new string(boardList.Select(i => i == 1 ? 'X' : (i == -1 ? 'O' : ' ')).ToArray());

            if (boardRow.Contains("XXXXX"))  //黑棋獲勝
                return 1;
            else if (boardRow.Contains("OOOOO"))  //白棋獲勝
                return 0;
        }

        return -1;  //無獲勝方
    }

    private List<int> checkWinAll(List<List<int>> board)
    {
        //創建一個存正斜、反斜方向的二維列表
        List<List<int>> boardC = new List<List<int>>();  //反斜
        List<List<int>> boardD = new List<List<int>>();  //正斜

        for (int i = 0; i < 29; i++) {
            //分別表示包含29個空列表的二維列表
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

        Debug.Log("最後組合完結果");
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
        List<int> result = checkWinAll(chessState);  //獲取棋盤目前落子狀態的勝利狀況

        if (result.Contains(0)) {
            Debug.Log("白棋獲勝");
            winner = -1;
            isPlaying = false;  //代表遊戲停止
        }
        else if (result.Contains(1)) {
            Debug.Log("黑棋獲勝");
            winner = 1;
            isPlaying = false;  //代表遊戲停止
        }
    }

    //將列表轉置的函數
    private List<List<int>> transpose (List<List<int>> board)
    {
        int rowMatrix = board.Count;  //取得整個二維列表裡一維列表的數量
        int colMatrix = board[0].Count;  //取得一維列表的元素數量
        
        //創建一個轉置後的二維列表
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