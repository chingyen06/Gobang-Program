using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Config : MonoBehaviour
{
    //定義存放黑棋棋型的字典
    public Dictionary<string, Dictionary<string, Tuple<string, int>>> valueModelXTest;

    //定義存放白棋棋型的字典
    public Dictionary<string, Dictionary<string, Tuple<string, int>>> valueModelOTest;

    //定義轉換後的棋型列表
    public List<Dictionary<string, List<Tuple<string, int>>>> valueModelX { get; private set; }
    public List<Dictionary<string, List<Tuple<string, int>>>> valueModelO { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        InitializedValueModels();
        TransformValueModels();
    }

    void InitializedValueModels()
    {
        valueModelXTest = new Dictionary<string, Dictionary<string, Tuple<string, int>>>
        {
            {"5", new Dictionary<string, Tuple<string, int>>
                {
                    {"5", Tuple.Create("XXXXX", 1000)}  //連五棋型
                }
            },
            {"4", new Dictionary<string, Tuple<string, int>>
                {
                    //隔開的棋子數量是0
                    {"4p_0", Tuple.Create(" XXXX ", 400)},  //活四
                    {"4_0_1", Tuple.Create(" XXXX", 100)},
                    {"4_0_2", Tuple.Create("OXXXX ", 100)},

                    //隔開的棋子數量是1
                    {"4_1_1", Tuple.Create("  XXX X", 120)},
                    {"4_1_2", Tuple.Create("XXX X", 120)},
                    
                    //隔開的棋子數量是2
                    {"4_2_1", Tuple.Create("XX XX", 100)}
                }
            },
            {"3", new Dictionary<string, Tuple<string, int>>
                {
                    //隔開的棋子數量是0
                    {"3p_0", Tuple.Create("  XXX  ", 60)},  //活三
                    {"3p_0_1", Tuple.Create("  XXX ", 30)},  //活三
                    {"3_0_1", Tuple.Create("OXXX  ", 30)},
                    {"3_0_2", Tuple.Create("O XXX O", 30)},
                    
                    //隔開的棋子數量是1
                    {"3p_1_0", Tuple.Create("XX X", 30)},
                    {"3_1_1", Tuple.Create("OXX X ", 30)},
                    {"3_1_2", Tuple.Create("XX  X ", 30)},

                    //隔開的棋子數量是2
                    {"3_2_0", Tuple.Create("OX XX ", 30)},

                    {"3_3_1", Tuple.Create("X X X", 30)}
                }
            },
            {"2", new Dictionary<string, Tuple<string, int>>
                {
                    //隔開的棋子數量是0
                    {"2_0_1", Tuple.Create("   XX   ", 8)},
                    {"2_0_2", Tuple.Create("OXX   ", 4)},
                    {"2_0_3", Tuple.Create("O XX  O", 4)},

                    //隔開的棋子數量是1
                    {"2_1_0", Tuple.Create("  X X  ", 8)},
                    {"2_1_1", Tuple.Create("X  X", 8)},
                    {"2_1_2", Tuple.Create("OX X  ", 4)},
                    {"2_1_3", Tuple.Create("O X X O  ", 4)},

                    //隔開的棋子數量是2
                    {"2_2_0", Tuple.Create("OX  X ", 4)}
                }
            }
        };

        valueModelOTest = new Dictionary<string, Dictionary<string, Tuple<string, int>>>();

        foreach (var outer in valueModelXTest)  //遍歷第一層字典
        {
            var outerKey = outer.Key;  //得到第一層字典的Key
            var innerDic = outer.Value;  //得到第一層字典的值，也就是第二層字典

            foreach (var inner in innerDic)  //遍歷第二層字典
            {
                var innerKey = inner.Key;  //得到第二層字典的Key，也就是棋型編號
                var tupleValue = inner.Value;  //得到第二層字典的值，也就是一個<棋型字串, int分數>的Tuple

                //把Tuple的第一個元素中的'X'轉換成'O'，'O'轉換成'X'
                var newString = new string(tupleValue.Item1.Select(c => c == 'X' ? 'O' : c == 'O' ? 'X' : c).ToArray());

                var newTuple = Tuple.Create(newString, tupleValue.Item2);  //得到一個替換後的新Tuple

                if (!valueModelOTest.ContainsKey(outerKey)) {  //檢查valueModelOTest是否包含當前的第一層字典的Key
                    //如果不包含，則創建一個新的內層(第二層)字典，並添加到valueModelOTest中
                    valueModelOTest[outerKey] = new Dictionary<string, Tuple<string, int>>();
                }

                //取第一層字典的Value(也是第二層字典)，因此要再取第二層字典的Value才會對應到Tuple<棋型字串, int分數>
                valueModelOTest[outerKey][innerKey] = newTuple;
            }
        }
    }

    //將字典轉換成List型態的函數
    private List<Dictionary<string, List<Tuple<string, int>>>> TransformModel(Dictionary<string, Dictionary<string, Tuple<string, int>>> model)
    {
        var transformedList = new List<Dictionary<string, List<Tuple<string, int>>>>();

        foreach (var outer in model)  //遍歷外層字典
        {
            var innerDic = outer.Value;  //獲取外層字典的Value，也就是內層字典
            var transformedDic = new Dictionary<string, List<Tuple<string, int>>>();  //初始化轉換後的字典

            foreach (var inner in innerDic)  //遍歷內層字典
            {
                //一個新的列表，包含一個元素是內層字典的Value，也就是一個Tuple<string, int>
                var tupleList = new List<Tuple<string, int>> { inner.Value };

                //將tupleList添加到transformedDic字典的Value，也就是Tuple<string, int>
                transformedDic[inner.Key] = tupleList;
            }

            //轉換後的字典添加到transformedList，也就是型態Dictionary<string, List<Tuple<string, int>>>
            transformedList.Add(transformedDic);
        }

        return transformedList;
    }

    void TransformValueModels()
    {
        valueModelO = TransformModel(valueModelOTest);
        valueModelX = TransformModel(valueModelXTest);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}