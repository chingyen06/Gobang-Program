using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class AIChess : MonoBehaviour
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

    //�¥մѫ��s
    public Button blackBtn;
    public Button whiteBtn;

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

    //�аO���a��ܪ��Ѥl�C��A��l��null ��ܪ��a�|����ܴѤl�C��
    public int? userPlay = null;

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

    Config config;

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

        //�K�[��ܶ¥մѫ��s����ť��
        //() => chooseColor(0)�O�@��lambda�B�⦡�A�O�@�ӰΦW���
        /*
          �]��AddListener�����u�������a�Ѽƪ���ơA��chooseColor()�O���a�Ѽƪ�
          �ҥH�ϥ�lambda�B�⦡�O��a�Ѽƪ���ƫʸ˦��@�Ӥ��a�Ѽƪ��ΦW���
          �䤤�� "=>" �Ÿ��Olambda�B��l�A��"����"���N��A�N�O���䪺()�O�ΦW��ƪ��Ѽ�
          �b�o��S���ѼơA����s�Q�I���ɡA�|����k�����choosseColor()
        */
        blackBtn.onClick.AddListener(() => chooseColor(0));
        whiteBtn.onClick.AddListener(() => chooseColor(1));

        config = GetComponent<Config>();

        if (config == null)
            config = gameObject.AddComponent<Config>();
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
        userPlay = null;
    }

    //��ܤ����C�⪺�Ѥl�����
    public void chooseColor(int color)
    {
        if (userPlay == null) {  //���a�|����ܴѤl�C��
            if (color == 0) {  //���a��ܶ´�
                userPlay = 0;  //�аO���a��´�
                flag = userPlay.Value;  //flag�|����0�A�N��´ѥ��U
            }
            else {  //���a��ܥպX
                userPlay = 1;  //�аO���a��պX
                flag = 0;  //AI�^�X�AAI�|���U�´�
                AIFirstMove();  //AI�U�Ĥ@�B�����
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying) {
            //�p�G�S����Ӥ�B�O���a�^�X�B�ƹ��I������
            if (winner == 0 && flag == userPlay && Input.GetMouseButtonDown(0)) {
                Vector3 PointPos = Input.mousePosition;  //������a��e�ƹ��I������m

                if (PlaceChess(PointPos, true)) {  //���a��m�Ѥl
                    flag = 1 - flag;  //�ܧ�^�X��

                    CheckWinFor();

                    if (isPlaying)  //�p�G�C�����b�i��(�S��Ĺ�a)
                        AITurn();  //��AI�U��
                }

            }
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
    private bool PlaceChess(Vector3 PointPos, bool isPlayerTurn)
    {
        float minDis = float.MaxValue;  //�̤p��m
        Vector2 closestPos = Vector2.zero;  //�̪񪺦�m
        int closestX = -1, closestY = -1;  //�̪��m��x�By�y�Ъ����ޭ�

        //�M���ѽL
        for (int i = 0; i < 15; i++) {
            for (int j = 0; j < 15; j++) {
                float dist = Distance(PointPos, chessPos[i][j]);

                if (isPlayerTurn) {  //���a�^�X
                    if (dist < minGridDis / 2 && chessState[i][j] == 0) {
                        minDis = dist;
                        closestPos = chessPos[i][j];
                        closestX = i;
                        closestY = j;
                    }
                }
                else {  //AI�^�X�ɡA�������̪񪺪Ŧ�m
                    if (dist < minDis) {
                        minDis = dist;
                        closestPos = chessPos[i][j];
                        closestX = i;
                        closestY = j;
                    }
                }
            }
        }

        if (closestX != -1 && closestY != -1 && chessState[closestX][closestY] == 0) {
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
            //ToArray()�ഫ���r���}�C�����A
            //new string��r���}�C�ഫ���r�� "XO  O"
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

    //AI�U�Ĥ@�B�Ѫ����
    private void AIFirstMove()
    {
        int x = 7, y = 7;  //�N��b�ѽL����������m

        chessState[x][y] = 1;  //AI���ⳣ���b�ѽL(7, 7)��m�U�´�

        flag = 1 - flag;  //�ܧ�^�X�A�����a�U��
    }

    //�p��ѽL�W�C�@�渨�l���A���`��
    //�u���ݤ�V
    private int Value(List<List<int>> board, List<(string, List<string>)> tempList, List<Dictionary<string, List<Tuple<string, int>>>> valueModel, char chr)
    {
        int score = 0;  //��l�Ƥ��Ƭ�0

        foreach (var row in board)  //�M���ѽL
        {
            //��M���쪺���e���Ʀr�ন�������Ѥl�r��
            string listStr = new string(row.Select(c => c == 1 ? 'X' : (c == -1 ? 'O' : ' ')).ToArray());
            
            //�p�G�Ӧ椺�e�̪��ؼдѤl�ƶq�֩�2�ӡA�N���L�o�@��A�~��M���U�@��
            if (listStr.Count(c => c == chr) < 2)
                continue;

            //i�ΨӹM��listStr���_�l��m
            for (int i = 0; i < listStr.Length - 5; i++) {
                //�ΨӼȮɦs�ثe�M�����椤�ѧO�쪺�ѫ�
                List<(int, (string, Tuple<string, int>))> temp = new List<(int, (string, Tuple<string, int>))>();
                
                for (int j = 5; j < 12; j++) {
                    //�p�G�W�X������סA�h���X�o�h�j��
                    if (i + j > listStr.Length)
                        break;

                    //�ΨӦslistStr�I�����l�r��
                    string s = listStr.Substring(i, j);

                    //��s.Count(c => c == chr)�j��5�ɡA�ڭ̥u�|����5
                    int sNum = Math.Min(s.Count(c => c == chr), 5);

                    if (sNum < 2)
                        continue;

                    foreach (var valueGroup in valueModel)  //��ܳv�ӹM��valueModel�����C�Ӧr��
                    {
                        foreach (var item in valueGroup)  //��ܹM��valueGroup�r�夤���C��Key-Value��
                        {
                            /* itemValue�OList<Tuple<string, int>>>
                               shape�OList<Tuple<string, int>>>�����C��Tuple<string, int>*/
                            foreach (var shape in item.Value)
                            {
                                if (s == shape.Item1) {  //�p�G���ǰt���ѫ�
                                    //��_�l��m�����ޡB�ѫ��N���BTuple<string, int>�[��temp��
                                    temp.Add((i, (item.Key, shape)));
                                    break;  //���X�o�h�j��
                                }
                            }
                        }
                    }
                }
            
                if (temp.Count > 0) {  //�p�Gtemp�����šA�N�����ǰt���ѫ�
                    //���temp�����o���̰������ơA�o�̳��O����e�M���쪺��
                    int MaxScore = temp.Max(item => item.Item2.Item2.Item2);

                    //��캡���Ĥ@�ӳ̰��o�����ѫ�
                    var MaxShape = temp.First(item => item.Item2.Item2.Item2 == MaxScore);
                    tempList.Add((MaxShape.Item2.Item1, new List<string> { MaxShape.Item2.Item2.Item1 }));
                    
                    score += MaxScore;  //�N�C�檺�̰����[��score
                }
            }
        }

        return score;
    }

    //�p��ѽL�W��B�ݡB���סB�ϱפ�V���l���A
    private int ValueAll(List<List<int>> board, List<(string, List<string>)> tempList, List<Dictionary<string, List<Tuple<string, int>>>> valueModel, int color)
    {
        char chr = (color == 1) ? 'X' : 'O';

        //��CheckWinAll�����סB�ϱ׳B�z�覡�@��
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
            }
        }

        //����|�Ӥ�V���o��
        int a = Value(board, tempList, valueModel, chr);
        int b = Value(transpose(board), tempList, valueModel, chr);
        int c = Value(boardC, tempList, valueModel, chr);
        int d = Value(boardD, tempList, valueModel, chr);

        return a + b + c + d;
    }
    
    //�ھ��`�o������m�����
    private (int, int, int) ValueChess(List<List<int>> board, int color)
    {
        int opponentColor = (color == 1) ? -1 : 1;  //�]�m��⪺�Ѥl�C��

        //�Ы��{�ɦC��A�ΨӦsAI�M��⪺�{�ɼƾ�
        List<(string, List<string>)> temp_list_ai = new List<(string, List<string>)>();
        List<(string, List<string>)> temp_list_opponent = new List<(string, List<string>)>();

        //�p���e����������
        //���l�e�AAI���o��
        int scoreAI = ValueAll(board, temp_list_ai, color == 1 ? config.valueModelX : config.valueModelO, color);

        //���l�e�A��⪺�o��
        int scoreOpponent = ValueAll(board, temp_list_opponent, color == 1 ? config.valueModelO : config.valueModelX, opponentColor);

        //�ΨӰO���̨θ��l��m
        (int, int) bestMoveAI = (0, 0);  //AI�̨θ��l��m���y��
        (int, int) bestMoveOpponent = (0, 0);  //���̨θ��l��m�y��
        (int, int) bestMoveOverall = (0, 0);  //��X�Ҽ{�Ҧ��]���᪺�̨θ��l��m�y��

        //AI�b�Ҧ��i�઺���l��m���A����o���̰�����
        int bestScoreAI = 0;

        //���b�Ҧ��i�઺���l��m���A����o���̰�����
        int bestScoreOpponent = 0;

        //��ܦb�Ҧ��i�઺���l��m���A�p��X���̰���X�o���t
        int bestScoreDiff = 0;

        //����ѽL�W�w�g���Ѥl���d��
        (int minX, int maxX, int minY, int maxY) = GetChessRange(board);

        //�X�i�j�M�d��
        //�b�w���Ѥl���d���¦�W�V�~�X�i2��A�����W�X�ѽL���
        int startX = Math.Max(0, minX - 2);
        int endX = Math.Min(14, maxX + 2);
        int startY = Math.Max(0, minY - 2);
        int endY = Math.Min(14, maxY + 2);

        //�M���X�i�᪺�j���d�򤺪��C�Ӧ�m
        for (int x = startX; x <= endX; x++) {
            for (int y = startY; y <= endY; y++) {
                //�ΨӼȮɦs�b�����Y�ӯS�w���l��m�ɲ��ͪ��ƾ�
                //�O���b�����L�{���ѧO�X���ѫ��N���P�ѫ��r��
                List<(string, List<string>)> tp_list_ai = new List<(string, List<string>)>();
                List<(string, List<string>)> tp_list_opponent = new List<(string, List<string>)>();

                if (board[x][y] != 0)  //�p�G�Ӧ�m���Ѥl�A�N���L
                    continue;

                //����AI���l
                board[x][y] = color;
                
                //AI�b�o�Ӧ�m���l�᪺����
                int scoreA = ValueAll(board, tp_list_ai, color == 1 ? config.valueModelX : config.valueModelO, color);

                //AI�b�o�Ӧ�m���l���⪺����
                int scoreAO = ValueAll(board, tp_list_opponent, color == 1 ? config.valueModelO : config.valueModelX, opponentColor);

                if (scoreA > bestScoreAI) {
                    bestMoveAI = (x, y);
                    bestScoreAI = scoreA;
                }

                //������⸨�l
                board[x][y] = opponentColor;

                //���b�o�Ӧ�m���l�᪺����
                int scoreO = ValueAll(board, tp_list_opponent, color == 1 ? config.valueModelO : config.valueModelX, opponentColor);

                if (scoreO > bestScoreOpponent) {
                    bestMoveOpponent = (x, y);
                    bestScoreOpponent = scoreO;
                }

                board[x][y] = 0;  //��_�ѽL

                //�p���X�o��
                //scoreDiff�N�O��AI���Q������
                int scoreDiff = (int) (1.1 * (scoreA - scoreAI) + scoreOpponent - scoreAO + scoreO - scoreAO);

                if (scoreDiff > bestScoreDiff) {
                    bestMoveOverall = (x, y);
                    bestScoreDiff = scoreDiff;
                }
            }
        }

        //�ھڤ��P���p��̨ܳθ��l
        if (bestScoreAI >= 1000)  //�p�GAI�ઽ����ӡA�N��ܳo�Ӧ�m��AI���̨Φ�m���l
            return (bestMoveAI.Item1, bestMoveAI.Item2, bestScoreAI);
        else if (bestScoreOpponent >= 1000)  //�p�G���ઽ����ӡA�N��ܳo�Ӧ�m���׹�⪺���l
            return (bestMoveOpponent.Item1, bestMoveOpponent.Item2, scoreOpponent);  //(bestMoveOpponent.Item2, bestMoveOpponent.Item2, scoreOpponent)
        else  //�H�W��س��S���A�N��ܺ�X�o���̰������l
            return (bestMoveOverall.Item1, bestMoveOverall.Item2, bestScoreDiff);
    }

    //�o��ѽL�W�w�g����m�Ѥl���d����
    private (int, int, int, int) GetChessRange(List<List<int>> board)
    {
        int minX = 14, maxX = 0, minY = 14, maxY = 0;

        //�M���ѽL
        for (int x = 0; x < 15; x++) {
            for (int y = 0; y < 15; y++) {
                if (board[x][y] != 0) {  //�ѽL���Ӧ�m�W����m�Ѥl
                    if (x < minX)
                        minX = x;

                    if (x > maxX)
                        maxX = x;

                    if (y < minY)
                        minY = y;

                    if (y > maxY)
                        maxY = y;
                }
            }
        }

        return (minX, maxX, minY, maxY);
    }

    //AI�U�Ѫ��B�J
    private void AITurn()
    {
        //����̨θ��l��m
        (int x, int y, int score) = ValueChess(chessState, userPlay == 1 ? 1 : -1);

        Vector3 bestMove = chessPos[x][y];

        PlaceChess(bestMove, false);  //�եΩ�m�Ѥl�����

        flag = 1 - flag;  //�ܧ�^�X��

        CheckWinFor();  //�ˬd�ӧQ����
    }
}