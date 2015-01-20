using NUnit.Framework;

namespace SolrNet.Cloud.Tests
{
    public class StartupTests
    {
        [SetUp]
        public void Setup() {
            Startup.Init<FakeEntity>(new FakeProvider());
        }

        [Test]
        public void ShouldResolveBasicOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrBasicOperations<FakeEntity>>(),
                "Should resolve basic operations from startup container");
        }

        [Test]
        public void ShouldResolveBasicReadOnlyOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrBasicReadOnlyOperations<FakeEntity>>(),
                "Should resolve basic read only operations from startup container");
        }

        [Test]
        public void ShouldResolveOperationsFromStartupContainer() {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrOperations<FakeEntity>>(),
                "Should resolve operations from startup container");
        }

        [Test]
        public void ShouldResolveOperationsProviderFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrOperationsProvider>(),
                "Should resolve operations provider from startup container");
        }

        [Test]
        public void ShouldResolveReadOnlyOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrReadOnlyOperations<FakeEntity>>(),
                "Should resolve read only operations from startup container");
        }
    }
}