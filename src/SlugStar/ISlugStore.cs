namespace SlugStar
{
    public interface ISlugStore
    {
        bool Exists(string text);

        void Store(string text);
    }
}