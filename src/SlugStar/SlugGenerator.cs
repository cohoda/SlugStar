using System.Linq;
using SlugStar.SlugAlgorithm;
using SlugStar.SlugStore;

namespace SlugStar
{
    public class SlugGenerator : ISlugGenerator
    {
        private readonly ISlugStore _slugStore;
        private readonly ISlugAlgorithm _slugAlgorithm;
        private readonly SlugGeneratorOptions _slugGeneratorOptions;

        public SlugGenerator()
            : this(new SlugGeneratorOptions(), new InMemorySlugStore(), new DefaultSlugAlgorithm())
        { }

        public SlugGenerator(SlugGeneratorOptions slugGeneratorOptions)
            : this(slugGeneratorOptions, new InMemorySlugStore(), new DefaultSlugAlgorithm())
        { }

        public SlugGenerator(SlugGeneratorOptions slugGeneratorOptions, ISlugStore slugStore)
            : this(slugGeneratorOptions, slugStore, new DefaultSlugAlgorithm())
        { }

        public SlugGenerator(SlugGeneratorOptions slugGeneratorOptions, ISlugAlgorithm slugAlgorithm)
            : this(slugGeneratorOptions, new InMemorySlugStore(), slugAlgorithm)
        { }

        public SlugGenerator(ISlugStore slugStore, ISlugAlgorithm slugAlgorithm)
            : this(new SlugGeneratorOptions(), slugStore, slugAlgorithm)
        { }

        public SlugGenerator(SlugGeneratorOptions slugGeneratorOptions, ISlugStore slugStore, ISlugAlgorithm slugAlgorithm)
        {
            _slugStore = slugStore;
            _slugAlgorithm = slugAlgorithm;
            _slugGeneratorOptions = slugGeneratorOptions;
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
                string slugWithUniquifier = null;

                var uniquifierIterationIndex = 0;

                while (string.IsNullOrEmpty(slugWithUniquifier) && uniquifierIterationIndex < uniquifiers.Length)
                {
                    var uniquifier = uniquifiers.ElementAt(uniquifierIterationIndex);

                    slugWithUniquifier = _slugAlgorithm.Slug(text + " " + uniquifier);

                    if (_slugStore.Exists(slugWithUniquifier))
                        slugWithUniquifier = null;

                    uniquifierIterationIndex++;
                }

                //we couldn't generate a unique slug with any of the uniquifiers supplied
                //so now use the first uniquifier in the list, and append an incremental number until we find the unique
                if (string.IsNullOrEmpty(slugWithUniquifier))
                    return GenerateAndStoreSlugWithIncrementedNumberAppendage(text + " " + uniquifiers.First());

                _slugStore.Store(new Slug(slugWithUniquifier));

                return slugWithUniquifier;
            }
            //no uniquifiers so add number on the end until we find a unique value
            return GenerateAndStoreSlugWithIncrementedNumberAppendage(text);
        }

        public string GenerateAndStoreSlugWithIncrementedNumberAppendage(string text)
        {
            // this could be a bit slow if there's a load of slugs with the same precident:
            //slug-1
            //slug-2
            //slug-3
            //etc.... 

            string slugWithNumber = null;
            var number = _slugGeneratorOptions.IterationSeedValue ?? 1;

            while (slugWithNumber == null)
            {
                slugWithNumber = _slugAlgorithm.Slug(text + " " + number);

                if (_slugStore.Exists(slugWithNumber))
                    slugWithNumber = null;

                number++;
            }

            _slugStore.Store(new Slug(slugWithNumber));

            return slugWithNumber;
        }
    }
}