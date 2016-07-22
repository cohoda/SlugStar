using FakeItEasy;
using NUnit.Framework;
using SlugStar.SlugAlgorithm;
using SlugStar.SlugStore;

namespace SlugStar.Tests
{
    public class SlugGenerator_UsingOptions_Tests
    {


        private ISlugStore _fakeSlugStore;
        private ISlugAlgorithm _fakeSlugAlgorithm;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _fakeSlugStore = A.Fake<ISlugStore>();
            _fakeSlugAlgorithm = A.Fake<ISlugAlgorithm>();

        }

        [Test]
        public void GenerateSlug_Appends_number_to_text_if_original_text_slugged_exists_Starting_at_supplied_seed_value()
        {
            //Arrange
            var slugGenerator = new SlugGenerator(_fakeSlugStore, _fakeSlugAlgorithm,
                new SlugGeneratorOptions { IterationSeedValue = 3 }); //start at 3

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            A.CallTo(() => _fakeSlugStore.Exists("some-text"))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text 3"))
                .Returns("some-text-3");

            //Act
            var result = slugGenerator.GenerateSlug("Some text");

            //Assert
            Assert.That(result, Is.EqualTo("some-text-3"));

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text 1"))
                .MustNotHaveHappened();

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text 2"))
                .MustNotHaveHappened();
        }


    }
}