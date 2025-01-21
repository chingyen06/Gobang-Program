using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Config : MonoBehaviour
{
    //�w�q�s��´Ѵѫ����r��
    public Dictionary<string, Dictionary<string, Tuple<string, int>>> valueModelXTest;

    //�w�q�s��մѴѫ����r��
    public Dictionary<string, Dictionary<string, Tuple<string, int>>> valueModelOTest;

    //�w�q�ഫ�᪺�ѫ��C��
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
                    {"5", Tuple.Create("XXXXX", 1000)}  //�s���ѫ�
                }
            },
            {"4", new Dictionary<string, Tuple<string, int>>
                {
                    //�j�}���Ѥl�ƶq�O0
                    {"4p_0", Tuple.Create(" XXXX ", 400)},  //���|
                    {"4_0_1", Tuple.Create(" XXXX", 100)},
                    {"4_0_2", Tuple.Create("OXXXX ", 100)},

                    //�j�}���Ѥl�ƶq�O1
                    {"4_1_1", Tuple.Create("  XXX X", 120)},
                    {"4_1_2", Tuple.Create("XXX X", 120)},
                    
                    //�j�}���Ѥl�ƶq�O2
                    {"4_2_1", Tuple.Create("XX XX", 100)}
                }
            },
            {"3", new Dictionary<string, Tuple<string, int>>
                {
                    //�j�}���Ѥl�ƶq�O0
                    {"3p_0", Tuple.Create("  XXX  ", 60)},  //���T
                    {"3p_0_1", Tuple.Create("  XXX ", 30)},  //���T
                    {"3_0_1", Tuple.Create("OXXX  ", 30)},
                    {"3_0_2", Tuple.Create("O XXX O", 30)},
                    
                    //�j�}���Ѥl�ƶq�O1
                    {"3p_1_0", Tuple.Create("XX X", 30)},
                    {"3_1_1", Tuple.Create("OXX X ", 30)},
                    {"3_1_2", Tuple.Create("XX  X ", 30)},

                    //�j�}���Ѥl�ƶq�O2
                    {"3_2_0", Tuple.Create("OX XX ", 30)},

                    {"3_3_1", Tuple.Create("X X X", 30)}
                }
            },
            {"2", new Dictionary<string, Tuple<string, int>>
                {
                    //�j�}���Ѥl�ƶq�O0
                    {"2_0_1", Tuple.Create("   XX   ", 8)},
                    {"2_0_2", Tuple.Create("OXX   ", 4)},
                    {"2_0_3", Tuple.Create("O XX  O", 4)},

                    //�j�}���Ѥl�ƶq�O1
                    {"2_1_0", Tuple.Create("  X X  ", 8)},
                    {"2_1_1", Tuple.Create("X  X", 8)},
                    {"2_1_2", Tuple.Create("OX X  ", 4)},
                    {"2_1_3", Tuple.Create("O X X O  ", 4)},

                    //�j�}���Ѥl�ƶq�O2
                    {"2_2_0", Tuple.Create("OX  X ", 4)}
                }
            }
        };

        valueModelOTest = new Dictionary<string, Dictionary<string, Tuple<string, int>>>();

        foreach (var outer in valueModelXTest)  //�M���Ĥ@�h�r��
        {
            var outerKey = outer.Key;  //�o��Ĥ@�h�r�媺Key
            var innerDic = outer.Value;  //�o��Ĥ@�h�r�媺�ȡA�]�N�O�ĤG�h�r��

            foreach (var inner in innerDic)  //�M���ĤG�h�r��
            {
                var innerKey = inner.Key;  //�o��ĤG�h�r�媺Key�A�]�N�O�ѫ��s��
                var tupleValue = inner.Value;  //�o��ĤG�h�r�媺�ȡA�]�N�O�@��<�ѫ��r��, int����>��Tuple

                //��Tuple���Ĥ@�Ӥ�������'X'�ഫ��'O'�A'O'�ഫ��'X'
                var newString = new string(tupleValue.Item1.Select(c => c == 'X' ? 'O' : c == 'O' ? 'X' : c).ToArray());

                var newTuple = Tuple.Create(newString, tupleValue.Item2);  //�o��@�Ӵ����᪺�sTuple

                if (!valueModelOTest.ContainsKey(outerKey)) {  //�ˬdvalueModelOTest�O�_�]�t��e���Ĥ@�h�r�媺Key
                    //�p�G���]�t�A�h�Ыؤ@�ӷs�����h(�ĤG�h)�r��A�òK�[��valueModelOTest��
                    valueModelOTest[outerKey] = new Dictionary<string, Tuple<string, int>>();
                }

                //���Ĥ@�h�r�媺Value(�]�O�ĤG�h�r��)�A�]���n�A���ĤG�h�r�媺Value�~�|������Tuple<�ѫ��r��, int����>
                valueModelOTest[outerKey][innerKey] = newTuple;
            }
        }
    }

    //�N�r���ഫ��List���A�����
    private List<Dictionary<string, List<Tuple<string, int>>>> TransformModel(Dictionary<string, Dictionary<string, Tuple<string, int>>> model)
    {
        var transformedList = new List<Dictionary<string, List<Tuple<string, int>>>>();

        foreach (var outer in model)  //�M���~�h�r��
        {
            var innerDic = outer.Value;  //����~�h�r�媺Value�A�]�N�O���h�r��
            var transformedDic = new Dictionary<string, List<Tuple<string, int>>>();  //��l���ഫ�᪺�r��

            foreach (var inner in innerDic)  //�M�����h�r��
            {
                //�@�ӷs���C��A�]�t�@�Ӥ����O���h�r�媺Value�A�]�N�O�@��Tuple<string, int>
                var tupleList = new List<Tuple<string, int>> { inner.Value };

                //�NtupleList�K�[��transformedDic�r�媺Value�A�]�N�OTuple<string, int>
                transformedDic[inner.Key] = tupleList;
            }

            //�ഫ�᪺�r��K�[��transformedList�A�]�N�O���ADictionary<string, List<Tuple<string, int>>>
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