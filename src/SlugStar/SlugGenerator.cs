using System.Linq;
using SlugStar.SlugAlgorithm;
using SlugStar.SlugStore;

namespace SlugStar
{
    public class SlugGenerator
    {
        private readonly ISlugStore _slugStore;
        private readonly ISlugAlgorithm _slugAlgorithm;

        public SlugGenerator(ISlugStore slugStore, ISlugAlgorithm slugAlgorithm)
        {
            _slugStore = slugStore;
            _slugAlgorithm = slugAlgorithm;
        }

        public string GenerateSlug(string text, string[] uniquifiers = null)
        {
            var initialSlug = _slugAlgorithm.Slug(text);

            //we might get lucky on first try
            if (!_slugStore.Exists(initialSlug))
            {
                _slugStore.Store(new Slug(initialSlug));
                return initialSlug;
            }

            //if we've got uniquifiers, iterate them
            if (uniquifiers != null && uniquifiers.Any())
            {
                var slugWithUniquifier = string.Empty;

                var uniquifierIterationIndex = 0;

                while (slugWithUniquifier == string.Empty && uniquifierIterationIndex < uniquifiers.Length)
                {
                    //get the uniquifier by index
                    var uniquifier = uniquifiers.ElementAt(uniquifierIterationIndex);

                    slugWithUniquifier = GenerateSlugWithAppendageAndCheckIfExists(text, uniquifier);

                    uniquifierIterationIndex++;
                }

                //we couldn't generate a unique slug with any of the uniquifiers supplied
                //so now use the first uniquifier in the list, and append an incremental number until we find the unique
                if (string.IsNullOrEmpty(slugWithUniquifier))
                    return GenerateAndStoreSlugWithNumberAppendage(text + " " + uniquifiers.First());

                _slugStore.Store(new Slug(slugWithUniquifier));

                return slugWithUniquifier;
            }
            //no uniquifiers so add number on the end until we find a unique value
            return GenerateAndStoreSlugWithNumberAppendage(text);
        }

        private string GenerateAndStoreSlugWithNumberAppendage(string text)
        {
            var slugWithNumber = string.Empty;
            var number = 1;

            while (slugWithNumber == string.Empty)
            {
                slugWithNumber = GenerateSlugWithAppendageAndCheckIfExists(text, number.ToString());
                number++;
            }

            _slugStore.Store(new Slug(slugWithNumber));

            return slugWithNumber;
        }

        private string GenerateSlugWithAppendageAndCheckIfExists(string text, string appendage)
        {
            var textWithAppendage = text + " " + appendage;
            var slugWithAppendage = _slugAlgorithm.Slug(textWithAppendage);

            return !_slugStore.Exists(slugWithAppendage)
                ? slugWithAppendage : string.Empty;
        }
    }
}