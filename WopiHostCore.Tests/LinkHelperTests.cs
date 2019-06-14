using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WopiHostCore.Helpers;

namespace WopiHostCore.Tests
{
    [TestClass]
    public class LinkHelperTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration = new Mock<IConfiguration>();

        [TestMethod]
        public void Generate_Word_Zone()
        {
            //_mockConfiguration.Setup(x => x)
            WopiAppHelper linkHelper = new WopiAppHelper("Discovery.xml", true, _mockConfiguration.Object);

            var obj = linkHelper.GetZone("Word");

            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public void Get_Word_Doc_Link()
        {
            _mockConfiguration.SetupGet(r => r["appHmacKey"]).Returns("XFrJrnwjShiyXFuKjqlD/yTfXkUHEOGn/TUmUyiZwaPqFMiyx1nkieNDkMwhRux/VPu16oPuDyg03EHKttLsgA==");
            WopiAppHelper linkHelper = new WopiAppHelper("Discovery.xml", true, _mockConfiguration.Object);

            var wopiHostLink = "http://wopi2.com/api/wopi/files/test.docx";

            var obj = linkHelper.GetDocumentLink("Word", "docx", wopiHostLink, "tbd");

            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void Get_Excel_Doc_Link()
        {
            WopiAppHelper linkHelper = new WopiAppHelper("Discovery.xml", false, _mockConfiguration.Object);

            var wopiHostLink = "http://wopi2.com/api/wopi/files/test.xls";

            var obj = linkHelper.GetDocumentLink("Excel", "xls", wopiHostLink, "tbd");

            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void Get_ExcelX_Doc_Link()
        {
            WopiAppHelper linkHelper = new WopiAppHelper("Discovery.xml", false, _mockConfiguration.Object);

            var wopiHostLink = "http://wopi2.com/api/wopi/files/test.xlsx";

            var obj = linkHelper.GetDocumentLink("Excel", "xlsx", wopiHostLink, "tbd");

            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void TestTest()
        {
            _mockConfiguration.SetupGet(r => r["appHmacKey"]).Returns("XFrJrnwjShiyXFuKjqlD/yTfXkUHEOGn/TUmUyiZwaPqFMiyx1nkieNDkMwhRux/VPu16oPuDyg03EHKttLsgA==");
            WopiAppHelper linkHelper = new WopiAppHelper("Discovery.xml", true, _mockConfiguration.Object);
            var wopiHostLink = "http://wopi2.com/api/wopi/files/test.docx";
            linkHelper.GetDocumentLink(wopiHostLink);
        }
    }
}
