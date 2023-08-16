using NUnit.Framework;

namespace API.Tests
{
    public class RequestQueryBuilderTests
    {
        [SetUp]
        public void BeforeAll() => _builder = new RequestQueryBuilder();

        [Test]
        public void ShouldQuery()
        {
            var query = _builder.Build();
            Assert.That(query, Is.TypeOf<string>());
        }

        [Test]
        public void ShouldSetPage()
        {
            var query = _builder
                .SetPage(2)
                .Build();

            Assert.That(query, Is.EqualTo("?page=2"));
        }

        [Test]
        public void ShouldSetLimit()
        {
            var query = _builder
                .SetLimit(10)
                .Build();

            Assert.That(query, Is.EqualTo("?limit=10"));
        }

        [Test]
        public void ShouldSetOffset()
        {
            var query = _builder
                .SetOffset(7)
                .Build();

            Assert.That(query, Is.EqualTo("?offset=7"));
        }

        [Test]
        public void ShouldSetFields()
        {
            var query = _builder
                .SetFields("field1", "field2")
                .Build();

            Assert.That(query, Is.EqualTo("?fields=field1,field2"));
        }

        [Test]
        public void ShouldSetFilters()
        {
            var query = _builder
                .SetFilter("field1", Condition.Eq, "value1")
                .SetFilter("field2", Condition.In, new[] { 2, 1, 4 })
                .Build();

            Assert.That(query, Is.EqualTo("?filter=field1||$eq||value1&filter=field2||$in||2,1,4"));

            _builder.Reset();

            query = _builder
                .SetFilter("field1", Condition.Gte, 1.2)
                .SetFilter("field2", Condition.Ne, false)
                .Build();

            Assert.That(query, Is.EqualTo("?filter=field1||$gte||1.2&filter=field2||$ne||false"));
        }

        [Test]
        public void ShouldSetOrs()
        {
            var query = _builder
                .SetOr("field1", Condition.Eq, "value1")
                .SetOr("field2", Condition.In, new[] { 2, 1, 4 })
                .Build();

            Assert.That(query, Is.EqualTo("?or=field1||$eq||value1&or=field2||$in||2,1,4"));

            _builder.Reset();

            query = _builder
                .SetOr("field1", Condition.Gte, 1.2)
                .SetOr("field2", Condition.Ne, false)
                .Build();

            Assert.That(query, Is.EqualTo("?or=field1||$gte||1.2&or=field2||$ne||false"));
        }

        [Test]
        public void ShouldSetJoin()
        {
            var query = _builder
                .SetJoin("relation1")
                .SetJoin("relation2", "field1", "field2")
                .Build();

            Assert.That(query, Is.EqualTo("?join=relation1&join=relation2||field1,field2"));
        }

        [Test]
        public void ShouldSetOrder()
        {
            var query = _builder
                .SetSort("field1")
                .Build();

            Assert.That(query, Is.EqualTo("?sort=field1,ASC"));
        }

        [Test]
        public void ShouldCreateComplexQueries()
        {
            var query = _builder
                .SetPage(1)
                .SetLimit(2)
                .SetJoin("users", "id", "name")
                .SetFilter("users.imageUrl", Condition.NotNull)
                .SetSort("name")
                .Build();

            Assert.That(query,
                Is.EqualTo(
                    "?page=1&limit=2&join=users||id,name&filter=users.imageUrl||$notnull&sort=name,ASC"));

            _builder.Reset();

            query = _builder
                .SetLimit(2)
                .SetOffset(10)
                .SetJoin("users")
                .SetFilter("title", Condition.ContL, "A")
                .SetFilter("expectTime", Condition.Gte, 1)
                .SetSort("name", Order.Desc)
                .Build();

            Assert.That(query,
                Is.EqualTo(
                    "?limit=2&offset=10&join=users&filter=title||$contL||A&filter=expectTime||$gte||1&sort=name,DESC"));
        }

        private RequestQueryBuilder _builder;
    }
}