using System.Linq;
using System.Text.RegularExpressions;

namespace SlugStar
{
    public class DefaultSlugAlgorithm
    {
        private readonly SlugAlgorithmOptions _defaultOptions;

        public DefaultSlugAlgorithm()
        {
            _defaultOptions = new SlugAlgorithmOptions(

                //lowercase the whole string
                str => str.ToLower(),

                //remove accents
                str => System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(str)),

                // invalid chars           
                str => Regex.Replace(str, @"[^a-z0-9\s-]", ""),

                // convert multiple spaces into one space   
                str => Regex.Replace(str, @"\s+", " ").Trim(),

                // hyphens   
                str => Regex.Replace(str, @"\s", "-")

            // cut and trim 
            //str => str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();

                );
        }

        public string Slug(string phrase)
        {
            return Slug(phrase, _defaultOptions);
        }

        public string Slug(string phrase, SlugAlgorithmOptions options)
        {
            return options.Manipulators.Aggregate(phrase, (current, manipulator) => manipulator(current));
        }
    }
}