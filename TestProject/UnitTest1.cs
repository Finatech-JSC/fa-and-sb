using MicroBase.Share.Extensions;

namespace TestProject
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EncryptString_Test()
        {
            var str = "Generate Random Number in Min to Max Range";

            var encrypt = str.Encrypt();

            Assert.Pass();
        }
    }
}