using System.Text.RegularExpressions;

namespace Utility
{
    public class Slug
    {
        public static string GenerateSlug(string text)
        {
            //SlugHelper slugHelper = new();
            //string slug = slugHelper.GenerateSlug(text);
            //return slug;
            for (int i = 32; i < 48; i++)
            {
                text = text.Replace(((char)i).ToString(), " ").ToLower().Trim();

            }
            text = text.Replace(".", "-");

            text = text.Replace(" ", "-");

            text = text.Replace(",", "-");

            text = text.Replace(";", "-");

            text = text.Replace(":", "-");

            Regex regex = new(@"\p{IsCombiningDiacriticalMarks}+");

            string strFormD = text.Normalize(System.Text.NormalizationForm.FormD);

            return regex.Replace(strFormD, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}
