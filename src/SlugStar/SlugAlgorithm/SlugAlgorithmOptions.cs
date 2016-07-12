using System;
using System.Collections.Generic;

namespace SlugStar.SlugAlgorithm
{
    public class SlugAlgorithmOptions
    {
        public SlugAlgorithmOptions(params Func<string, string>[] manipulators)
        {
            Manipulators = manipulators;
        }

        public IEnumerable<Func<string, string>> Manipulators { get; private set; }
    }
}