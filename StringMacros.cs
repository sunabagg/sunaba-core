
using System.Collections.Generic;

namespace Sunaba.Core;

public static class StringMacros
{
    public static Dictionary<string, string> Macros = new Dictionary<string, string>
    {
        {"$DOLLAR_SIGN", "$"},
        //{"$WINDOW_WIDTH", DisplayServer.WindowGetSize().X.ToString()},
        //{"$WINDOW_HEIGHT", DisplayServer.WindowGetSize().Y.ToString()},
        {"$FILL_BOTTOM_RIGHT", "1"},
        {"$FILL_TOP_LEFT", "0"},
    };

    public static void Add(string key, string value)
    {
        key = key.ToUpperSnakeCase();

        if (!key.StartsWith("$"))
        {
            key = "$" + key.ToUpperSnakeCase();
        }

        Macros[key] = value;
    }

    public static void Remove(string key)
    {
        key = key.ToUpperSnakeCase();

        if (!key.StartsWith("$"))
        {
            key = "$" + key;
        }

        Macros.Remove(key);
    }

    public static string ParseMacros(this string s, IoInterface assetIo = null)
    {
        foreach (var macro in Macros)
        {
            if (s.Contains(macro.Key))
            {
                s = s.Replace(macro.Key, macro.Value);
            }
        }

        if (s.Contains("$ASSET(") && assetIo != null)
        {
            var start = s.IndexOf("$ASSET(");
            var end = s.IndexOf(")", start);
            var asset = s.Substring(start + 7, end - start - 7);
            s = s.Replace("$ASSET(" + asset + ")", assetIo.LoadText(asset));
        }

        return s;
    }

    public static int ParseMacrosToInt(this string str, IoInterface assetIo = null)
    {
        return int.Parse(str.ParseMacros(assetIo));
    }

    public static float ParseMacrosToFloat(this string str, IoInterface assetIo = null)
    {
        return float.Parse(str.ParseMacros(assetIo));
    }

    public static long ParseMacrosToLong(this string str, IoInterface assetIo = null)
    {
        return long.Parse(str.ParseMacros(assetIo));
    }

    public static double ParseMacrosToDouble(this string str, IoInterface assetIo = null)
    {
        return double.Parse(str.ParseMacros(assetIo));
    }


}