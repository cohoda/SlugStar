namespace SlugStar.SlugStore
{
    public interface ISlugStore
    {
        bool Exists(string slug);

        void Store(Slug slug);
    }
}