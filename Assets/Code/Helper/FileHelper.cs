using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FileHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    public static void WriteTxt(string path, string content)
    {
        StreamWriter sw;
        FileInfo fi = new FileInfo(path);
        sw = fi.CreateText();
        sw.WriteLine(content);
        sw.Close();
        sw.Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadTxt(string path)
    {
        if (!File.Exists(path))
        {
            return string.Empty;
        }

        StreamReader sr = new StreamReader(path);
        var content = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();
        return content;
    }


    /// <summary>
    /// 加密文件文本
    /// </summary>
    /// <param name="conten">文本内容</param>
    /// <param name="key">加密key</param>
    public static string StrEncryption(string conten, string ckey)
    {
        char[] arrContent = conten.ToCharArray();
        char[] arrKey = ckey.ToCharArray();
        for (int i = 0; i < arrContent.Length; i++)
        {
            arrContent[i] ^= arrKey[i % arrKey.Length];
        }
        return new string(arrContent);
    }

    /// <summary>
    /// 解密文本文件
    /// </summary>
    /// <param name="arrstr">文本文件内容</param>
    /// <param name="ckey">解密key</param>
    /// <returns></returns>
    public static string ReStrEncryption(string conten, string ckey)
    {
        char[] arrContent = conten.ToCharArray();
        char[] arrkey = ckey.ToCharArray();
        for (int i = 0; i < arrContent.Length; i++)
        {
            arrContent[i] ^= arrkey[i % arrkey.Length];
        }

        return new string(arrContent);
    }

    ///序列化
    public static byte[] Serialize(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();

        MemoryStream stream = new MemoryStream();

        bf.Serialize(stream, obj);

        byte[] datas = stream.ToArray();

        stream.Dispose();

        return datas;
    }

    /// 反序列化
    public static object Deserialize(byte[] datas)
    {
        BinaryFormatter bf = new BinaryFormatter();

        MemoryStream stream = new MemoryStream(datas, 0, datas.Length);

        object obj = bf.Deserialize(stream);

        stream.Dispose();

        return obj;
    }
}
