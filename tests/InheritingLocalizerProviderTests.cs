using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace triaxis.AspNetCore.Localization.Inheritance.Tests
{
    [TestClass]
    public class InheritingLocalizerProviderTests
    {
        class A {}
        class B : A {}

        IStringLocalizerFactory factory;
        InheritingLocalizerProvider provider;

        [TestInitialize]
        public void Init()
        {
            factory = new ResourceManagerStringLocalizerFactory(
                new OptionsWrapper<LocalizationOptions>(
                    new LocalizationOptions { ResourcesPath = "Resources" }
                ),
                new NullLoggerFactory()
            );
            provider = new InheritingLocalizerProvider(typeof(B), factory);
        }

        [TestMethod]
        public void Sanity()
        {
            Assert.AreEqual("A in A", provider["A"]);
            Assert.AreEqual("B in B", provider["B"]);
            Assert.AreEqual("B in A", provider.Parent["B"]);
            Assert.AreEqual("C in Shared", provider["C"]);
        }
    }
}
