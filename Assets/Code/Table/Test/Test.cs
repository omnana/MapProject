using System.Collections.Generic;

namespace Omnana
{
    public class TestModel : ITableModel
    {
        /// id
        public int Id { get; set; }
        /// 测试1
        public int Test1 { get; set; }
        /// 测试2
        public float Test2 { get; set; }
        /// 测试3
        public int[] Test3 { get; set; }
        /// 测试4
        public int[][] Test4 { get; set; }
        /// 测试5
        public string Test5 { get; set; }

        public object Key()
        {
            return Id;
        }
    }

    public class TestModelMgr : TableManager<TestModel>
    {
        public override string TableName()
        {
            return "Test.csv";
        }
        public override void InitModel(TestModel model, Dictionary<string, string> cellMap)
        {
            /// id;
            if (cellMap["Id"] != null)
                model.Id = FieldParser.IntParser(cellMap["Id"]);
            /// 测试1;
            if (cellMap["Test1"] != null)
                model.Test1 = FieldParser.IntParser(cellMap["Test1"]);
            /// 测试2;
            if (cellMap["Test2"] != null)
                model.Test2 = FieldParser.FloatParser(cellMap["Test2"]);
            /// 测试3;
            if (cellMap["Test3"] != null)
                model.Test3 = FieldParser.IntArrayParser(cellMap["Test3"]);
            /// 测试4;
            if (cellMap["Test4"] != null)
                model.Test4 = FieldParser.IntArraysParser(cellMap["Test4"]);
            /// 测试5;
            if (cellMap["Test5"] != null)
                model.Test5 = cellMap["Test5"];
        }
    }
}
