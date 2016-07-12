using System;
using FakeItEasy;
using NUnit.Framework;
using SlugStar.SlugAlgorithm;
using SlugStar.SlugStore;

namespace SlugStar.Tests
{
    [TestFixture]
    public class SlugGeneratorTests
    {
        private ISlugStore _fakeSlugStore;
        private ISlugAlgorithm _fakeSlugAlgorithm;

        private SlugGenerator _slugGenerator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _fakeSlugStore = A.Fake<ISlugStore>();
            _fakeSlugAlgorithm = A.Fake<ISlugAlgorithm>();

            _slugGenerator = new SlugGenerator(_fakeSlugStore, _fakeSlugAlgorithm);
        }

        [Test]
        public void GenerateSlug_slugs_given_text_initially()
        {
            var result = _slugGenerator.GenerateSlug("Some text");

            A.CallTo(() => _fakeSlugStore.Exists("Some text"))
            .MustHaveHappened();
        }

        [Test]
        public void GenerateSlug_checks_SlugStore_for_existing_slug()
        {
            var result = _slugGenerator.GenerateSlug("Some text");

            A.CallTo(() => _fakeSlugStore.Exists("Some text"))
            .MustHaveHappened();
        }

        [Test]
        public void GenerateSlug_stores_slug_and_returns_if_it_does_not_already_exist()
        {
            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            A.CallTo(() => _fakeSlugStore.Exists("some-text"))
                .Returns(false);

            var result = _slugGenerator.GenerateSlug("Some text");

            A.CallTo(() => _fakeSlugStore.Store("some-text"))
                .MustHaveHappened();

            Assert.That(result, Is.EqualTo("some-text"));
        }

        [Test]
        public void GenerateSlug_adds_uniquifier_and_re_checks_existence_before_returning()
        {
            //first
            A.CallTo(() => _fakeSlugStore.Exists("some-text"))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            //second
            A.CallTo(() => _fakeSlugStore.Exists("some-text-unique"))
                .Returns(false);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text unique"))
                .Returns("some-text-unique");

            var result = _slugGenerator.GenerateSlug("Some text", new[] { "unique" });

            A.CallTo(() => _fakeSlugStore.Store("some-text-unique"))
                .MustHaveHappened();

            Assert.That(result, Is.EqualTo("some-text-unique"));
        }


        [Test]
        public void GenerateSlug_iterates_uniquifiers_if_first_one_appended_to_slug_still_exists()
        {
            //first
            A.CallTo(() => _fakeSlugStore.Exists("some-text"))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            //second
            A.CallTo(() => _fakeSlugStore.Exists("some-text-uniqueone"))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniqueone"))
                .Returns("some-text-uniqueone");

            //third
            A.CallTo(() => _fakeSlugStore.Exists("some-text-uniquetwo"))
                .Returns(false);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniquetwo"))
                .Returns("some-text-uniquetwo");

            var result = _slugGenerator.GenerateSlug("Some text", new[] { "uniqueone", "uniquetwo" });

            A.CallTo(() => _fakeSlugStore.Store("some-text-uniquetwo"))
                .MustHaveHappened();

            Assert.That(result, Is.EqualTo("some-text-uniquetwo"));
        }


        [Test]
        public void GenerateSlug_appends_number_to_text_if_original_text_slugged_exists_without_supplying_uniquifiers()
        {
            //first
            A.CallTo(() => _fakeSlugStore.Exists("some-text"))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            //second
            A.CallTo(() => _fakeSlugStore.Exists("some-text-1"))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text 1"))
                .Returns("some-text-1");

            //third
            A.CallTo(() => _fakeSlugStore.Exists("some-text-2"))
                .Returns(false);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text 2"))
                .Returns("some-text-2");

            var result = _slugGenerator.GenerateSlug("Some text");

            A.CallTo(() => _fakeSlugStore.Store("some-text-2"))
                .MustHaveHappened();

            Assert.That(result, Is.EqualTo("some-text-2"));
        }


        [Test]
        public void GenerateSlug_appends_number_to_text_and_uniquifier_if_slugged_exists()
        {
            //first
            A.CallTo(() => _fakeSlugStore.Exists("some-text"))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            //second
            A.CallTo(() => _fakeSlugStore.Exists("some-text-uniqueone"))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniqueone"))
                .Returns("some-text-uniqueone");

            //third
            A.CallTo(() => _fakeSlugStore.Exists("some-text-uniquetwo"))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniquetwo"))
                .Returns("some-text-uniquetwo");

            //fourth
            A.CallTo(() => _fakeSlugStore.Exists("some-text-uniqueone-1"))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniqueone 1"))
                .Returns("some-text-uniqueone-1");

            //fifth
            A.CallTo(() => _fakeSlugStore.Exists("some-text-uniqueone-2"))
                .Returns(false);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniqueone 2"))
                .Returns("some-text-uniqueone-2");

            var result = _slugGenerator.GenerateSlug("Some text", new[] { "uniqueone", "uniquetwo" });

            A.CallTo(() => _fakeSlugStore.Store("some-text-uniqueone-2"))
                .MustHaveHappened();

            Assert.That(result, Is.EqualTo("some-text-uniqueone-2"));
        }
    }
}
