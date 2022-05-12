using AprsPersistenceService.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APRS.Tests
{
    [TestClass]
    public class LatLongParserTests
    {
        [TestMethod]
        public void TestConvertLongitude()
        {
            string LongitudeString = "W 085 33.3300";
            float result = LatLongParser.ConvertLongitude(LongitudeString);

            //Assert.AreEqual();
        }
    }
}
