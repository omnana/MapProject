using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MD5Helper
{
    private const int Md5ReadLen = 16 * 1024;       // 一次读取长度 16384 = 16 * kb


    public static string GetMD5HashFromFile(string fileName)
    {
        byte[] buffer = new byte[Md5ReadLen];

        int readLength = 0;//每次读取长度

        var output = new byte[Md5ReadLen];

        using (Stream inputStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0) // 计算MD5
                {
                    hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
                }

                //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)  
                hashAlgorithm.TransformFinalBlock(buffer, 0, 0);

                var retVal = hashAlgorithm.Hash;

                var sb = new System.Text.StringBuilder(32);

                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                hashAlgorithm.Clear();

                inputStream.Close();

                return sb.ToString();
            }
        }
    }

}
