using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateTableEditor : Editor
{
    private static string ScriptFoldPath = Application.dataPath + "/Code/ModelData/";

    private static string ConfigFoldPath = Application.dataPath + "/Art/Csv";

    private static string _modeTemplateContent;

    private static List<string> ScripteNameList = new List<string>();

    private static Dictionary<string, string> FieldTypeDic = new Dictionary<string, string>()
    {
        {"string", "            model.{0} = cellMap[\"{1}\"];" },
        {"int", "            model.{0} = FieldParser.IntParser(cellMap[\"{1}\"]);" },
        {"float", "            model.{0} = FieldParser.FloatParser(cellMap[\"{1}\"]);" },
        {"long", "            model.{0} = FieldParser.LongParser(cellMap[\"{1}\"]);" },
        {"double", "            model.{0} = FieldParser.DoubleParser(cellMap[\"{1}\"]);" },
        {"int[]", "            model.{0} = FieldParser.IntArrayParser(cellMap[\"{1}\"]);" },
        {"int[][]", "            model.{0} = FieldParser.IntArraysParser(cellMap[\"{1}\"]);" },
        {"float[]", "            model.{0} = FieldParser.FloatArrayParser(cellMap[\"{1}\"]);" },
        {"float[][]", "            model.{0} = FieldParser.FloatArraysParser(cellMap[\"{1}\"]);" },
        {"long[]", "            model.{0} = FieldParser.LongArrayParser(cellMap[\"{1}\"]);" },
        {"long[][]", "            model.{0} = FieldParser.LongArraysParser(cellMap[\"{1}\"]);" },
        {"double[]", "            model.{0} = FieldParser.DoubleArrayParser(cellMap[\"{1}\"]);" },
        {"double[][]", "            model.{0} = FieldParser.DoubleArraysParser(cellMap[\"{1}\"]);" },
    };

    [@MenuItem("Tools/Csv/解析所有配置,会覆盖已存在脚本")]
    public static void Parse()
    {
        var fullPath = ConfigFoldPath;

        ScripteNameList.Clear();

        Debug.Log("开始解析ModelData");

        if (Directory.Exists(fullPath))
        {
            var direction = new DirectoryInfo(fullPath);

            var files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (var i = 0; i < files.Length; ++i)
            {
                var fileName = files[i].Name;

                if (fileName.EndsWith(".csv"))
                {
                    EditorUtility.DisplayProgressBar("解析Json", "解析中...", i / (float)files.Length);

                    GenerateModelFile(files[i].Name);
                }
            }
        }

        EditorUtility.ClearProgressBar();
    }

    private static void GenerateModelFile(string tableName)
    {
        tableName = tableName.Substring(0, tableName.Length - 4); // 去掉".csv"

        if (string.IsNullOrEmpty(tableName))
        {
            Debug.LogError("脚本名不能为空");
            return;
        }

        var scriptFoldPath = ScriptFoldPath + tableName;

        if (!Directory.Exists(scriptFoldPath)) Directory.CreateDirectory(scriptFoldPath);

        string codeFullName = scriptFoldPath + "/" + tableName + ".cs";

        using (FileStream fs = new FileStream(codeFullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))

        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
        {
            var headers = TableParser.GetTableHeaders(ConfigFoldPath + "/" + tableName);
            var sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("");
            sb.AppendLine(string.Format("public class {0}Model : ITableModel", tableName));
            sb.AppendLine("{");
            for (var i = 0; i < headers[0].Count; i++)
            {
                sb.AppendLine(string.Format("    /// {0}", headers[2][i]));
                sb.AppendLine(string.Format("    public {0} {1} ", headers[1][i], headers[0][i]) + " { get; set; }");
            }
            sb.AppendLine("    public object Key()");
            sb.AppendLine("    {");
            sb.AppendLine("        return Id;");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine(string.Format("public class {0}ModelMgr : TableManager<{1}Model, {2}ModelMgr>", tableName, tableName, tableName));
            sb.AppendLine("{");
            sb.AppendLine("    public override string TableName()");
            sb.AppendLine("    {");
            sb.AppendLine(string.Format("        return \"{0}\";", tableName));
            sb.AppendLine("    }");
            sb.AppendLine(string.Format("    public override void InitModel({0}Model model, Dictionary<string, string> cellMap)", tableName));
            sb.AppendLine("    {");
            for (var i = 0; i < headers[0].Count; i++)
            {
                sb.AppendLine(string.Format("        /// {0};", headers[2][i]));
                sb.AppendLine(string.Format("        if (cellMap[\"{0}\"] != null)", headers[0][i]));
                sb.AppendLine(string.Format(FieldTypeDic[headers[1][i]], headers[0][i], headers[0][i]));
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");

            sw.Write(sb);
            sw.Close();
            fs.Close();
        }
        object arr = new int[3];

        var d = (int[])arr;

        Debug.Log("生成数据模型代码成功:" + tableName);
    }
}
