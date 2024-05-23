using System.Security.Cryptography;
using System.Text;

namespace SwissPension.Trial.Tree.Extensions;

public static class StringExtensions
{
    public static string GetChecksum(this string str)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(str));

        var hexString = string.Concat(hash.Select(b => b.ToString("X2")));

        return hexString;
    }

    public static string PascalCaseToSnakeCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        if (str.Length <= 3)
            return str.ToLower();

        var result = new StringBuilder();
        result.Append(char.ToLower(str[0]));

        for (var i = 1; i < str.Length; i++)
            if (char.IsUpper(str[i]))
            {
                if (char.IsLower(str[i - 1]) || (i < str.Length - 1 && char.IsLower(str[i + 1])))
                    result.Append('_');

                result.Append(char.ToLower(str[i]));
            }
            else
            {
                result.Append(str[i]);
            }

        return result.ToString();
    }
}