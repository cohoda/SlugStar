using System;

namespace SlugStar
{
    public class Slug
    {
        public Slug(string value)
        {
            Value = value;
            Created = DateTime.Now;
        }

        public string Value { get; set; }

        public DateTime Created { get; set; }
    }
}