using System.Linq;
using System.Text.RegularExpressions;

namespace SlugStar.SlugAlgorithm
{
    public class DefaultSlugAlgorithm : ISlugAlgorithm
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

                );
        }

        public string Slug(string phrase)
        {
            return Slug(phrase, _defaultOptions);
        }

        public string Slug(string phrase, SlugAlgorithmOptions options)
        {
            var result = phrase;
            foreach (var manipulator in options.Manipulators)
                result = manipulator(result);
            return result;
        }
    }
}