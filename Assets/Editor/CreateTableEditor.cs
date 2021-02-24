using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateTableEditor : Editor
{
    private static string ScriptFoldPath = Application.dataPath + "/Code/ModelData/";

    private static string ConfigFoldPath = Application.dataPath + "/Art/Configs";

    private static string _modeTemplateContent;

    private static List<string> ScripteNameList = new List<string>();

    [@MenuItem("Tools/Model解析工具/解析所有配置,会覆盖已存在脚本")]
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

                if (fileName.EndsWith(".json"))
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
        tableName = tableName.Substring(0, tableName.Length - 5); // 去掉".json"

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
            sb.AppendLine("    public int Id { get; set; }");
            foreach (var header in headers)
            {
                if (!header.Equals("Id"))
                {
                    sb.AppendLine(string.Format("    public string {0} ", header) + " { get; set; }");
                }
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
            sb.AppendLine(string.Format("    public override void InitModel({0}Model model, Dictionary<string, object> cellMap)", tableName));
            sb.AppendLine("    {");
            sb.AppendLine("        if (cellMap[\"Id\"] != null)");
            sb.AppendLine("            model.Id = int.Parse(cellMap[\"Id\"].ToString());");
            foreach (var header in headers)
            {
                if (!header.Equals("Id"))
                {
                    sb.AppendLine(string.Format("        if (cellMap[\"{0}\"] != null)", header));
                    sb.AppendLine(string.Format("            model.{0} = cellMap[\"{1}\"].ToString();", header, header));
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");

            sw.Write(sb.ToString());
            sw.Close();
            fs.Close();
        }
        Debug.Log("生成数据模型代码成功:" + tableName);
    }
}
