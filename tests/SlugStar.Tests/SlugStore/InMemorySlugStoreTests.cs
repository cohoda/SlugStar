using System.Collections.Concurrent;
using System.Linq;
using NUnit.Framework;
using SlugStar.SlugStore;

namespace SlugStar.Tests.SlugStore
{
    [TestFixture]
    public class InMemorySlugStoreTests
    {
        private InMemorySlugStore _inMemorySlugStore;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }

        [SetUp]
        public void BeforeEachTest()
        {
            _inMemorySlugStore = new InMemorySlugStore();
            InMemorySlugStore.Cache = new ConcurrentDictionary<string, Slug>();
        }

        [Test]
        public void Store_Adds_slug_value_to_collection()
        {
            _inMemorySlugStore.Store(new Slug("Store_Adds_slug_value_to_collection"));

            var result = InMemorySlugStore.Cache.ContainsKey("Store_Adds_slug_value_to_collection");

            Assert.That(result, Is.True);
        }

        [Test]
        public void Exists_Returns_true_if_slug_value_exists()
        {
            InMemorySlugStore.Cache.TryAdd("Exists_Returns_true_if_slug_value_exists", new Slug("Exists_Returns_true_if_slug_value_exists"));

            var result = _inMemorySlugStore.Exists("Exists_Returns_true_if_slug_value_exists");

            Assert.That(result, Is.True);
        }


        [Test]
        public void Exists_Returns_false_if_slug_value_does_not_exist()
        {
            var result = _inMemorySlugStore.Exists("Exists_Returns_false_if_slug_value_does_not_exist");

            Assert.That(result, Is.False);
        }
    }
}
