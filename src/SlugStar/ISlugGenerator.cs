namespace SlugStar
{
    public interface ISlugGenerator
    {
        string GenerateSlug(string text, string[] uniquifiers = null);
    }
}