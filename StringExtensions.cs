using System;
using System.Text;

namespace Sunaba.Core;

public static class StringExtensions
{
    public static string ToSnakeCaseSnb(this string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (text.Length < 2)
        {
            return text.ToLowerInvariant();
        }

        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));

        for (int i = 1; i < text.Length; ++i)
        {
            char c = text[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
    
    public static string ToSnakeCaseWrapper(this string str)
    {
        return StringExtensions.ToSnakeCaseSnb(str);
    }

    public static string ToUpperSnakeCase(this string str)
    {
        var snakeCase = str.ToSnakeCaseSnb();
        return snakeCase.ToUpperInvariant();
    }
}