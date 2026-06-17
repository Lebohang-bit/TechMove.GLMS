using System.IO;
using System.Linq;
using Xunit;

namespace TechMove.GLMS.Tests
{
    public class FileValidationTests
    {
        [Fact]
        public void ValidatePDFFile_WithPDFExtension_ReturnsTrue()
        {
            
            string fileName = "contract.pdf";
            string expectedExtension = ".pdf";

            
            string actualExtension = Path.GetExtension(fileName).ToLower();
            bool isValid = actualExtension == expectedExtension;

            
            Assert.True(isValid);
        }

        [Fact]
        public void ValidatePDFFile_WithExeExtension_ReturnsFalse()
        {
            
            string fileName = "malware.exe";
            string expectedExtension = ".pdf";

           
            string actualExtension = Path.GetExtension(fileName).ToLower();
            bool isValid = actualExtension == expectedExtension;

            
            Assert.False(isValid);
        }

        [Fact]
        public void ValidatePDFFile_WithJpgExtension_ReturnsFalse()
        {
            
            string fileName = "image.jpg";
            string expectedExtension = ".pdf";

            
            string actualExtension = Path.GetExtension(fileName).ToLower();
            bool isValid = actualExtension == expectedExtension;

            
            Assert.False(isValid);
        }

        [Fact]
        public void ValidatePDFFile_WithPngExtension_ReturnsFalse()
        {
            
            string fileName = "screenshot.png";
            string expectedExtension = ".pdf";

            
            string actualExtension = Path.GetExtension(fileName).ToLower();
            bool isValid = actualExtension == expectedExtension;

            
            Assert.False(isValid);
        }

        [Fact]
        public void ValidatePDFFile_WithDocxExtension_ReturnsFalse()
        {
            
            string fileName = "document.docx";
            string expectedExtension = ".pdf";

            
            string actualExtension = Path.GetExtension(fileName).ToLower();
            bool isValid = actualExtension == expectedExtension;

            
            Assert.False(isValid);
        }

        [Theory]
        [InlineData("contract.pdf", true)]
        [InlineData("agreement.PDF", true)]
        [InlineData("file.exe", false)]
        [InlineData("image.jpg", false)]
        [InlineData("document.png", false)]
        [InlineData("virus.exe", false)]
        [InlineData("terms.docx", false)]
        public void FileExtensionValidation_VariousFiles_ReturnsExpectedResult(string fileName, bool expectedIsValid)
        {
            
            string extension = Path.GetExtension(fileName).ToLower();
            bool isValid = extension == ".pdf";

            
            Assert.Equal(expectedIsValid, isValid);
        }
    }
}
