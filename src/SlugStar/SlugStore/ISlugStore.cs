namespace SlugStar.SlugStore
{
    public interface ISlugStore
    {
        bool Exists(string text);

        void Store(Slug slug);
    }
}