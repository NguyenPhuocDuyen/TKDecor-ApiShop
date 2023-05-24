using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utility
{
    public class Slug
    {
        public static string GenerateSlug(string title)
        {
            string slug = RemoveDiacritics(title);
            slug = Regex.Replace(slug, @"\s+", " "); // Chuyển nhiều khoảng trắng thành một khoảng trắng duy nhất
            slug = Regex.Replace(slug, @"[^a-zA-Z0-9\s]", ""); // Xóa các ký tự không hợp lệ
            slug = slug.ToLower(); // Chuyển tất cả thành chữ thường
            slug = slug.Replace(" ", "-"); // Thay thế khoảng trắng bằng dấu gạch ngang
            return slug;
        }

        private static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }
    }
}
