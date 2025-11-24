using System.Security.Cryptography;

namespace AlterTools.BatchExport.Utils;

public static class HashHelper
{
    /// <summary>
    ///     Returns string with MD5 Hash of given file
    /// </summary>
    public static string GetMd5Hash(this string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return null;
        if (!File.Exists(fileName)) return null;

        using MD5 md5 = MD5.Create();

        try
        {
            using FileStream stream = File.OpenRead(fileName);
            return Convert.ToBase64String(md5.ComputeHash(stream));
        }
        catch
        {
            return null;
        }
    }
}