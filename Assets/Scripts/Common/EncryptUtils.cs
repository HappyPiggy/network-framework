using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
/// <summary>
/// 加密解密的封装
/// </summary>
public class EncryptUtils
{

    /// <summary>
    /// aes加密
    /// </summary>
    /// <param name="src"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static byte[] AesEncrypt(byte[] src, byte[] key)
    {

        RijndaelManaged rm = new RijndaelManaged
        {
            Mode = CipherMode.CBC,
            Padding = PaddingMode.PKCS7
        };

        ICryptoTransform cTransform = rm.CreateEncryptor(key,key);
        byte[] resultArray = cTransform.TransformFinalBlock(src,0,src.Length);

        return resultArray;
    }
}