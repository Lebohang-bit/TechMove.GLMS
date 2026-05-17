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
            // Arrange
            string fileName = "contract.pdf";
            string expectedExtension = ".pdf";

            // Act
            string actualExtension = Path.GetExtension(fileName).ToLower();
            bool isValid = actualExtension == expectedExtension;

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidatePDFFile_WithExeExtension_ReturnsFalse()
        {
            // Arrange
            string fileName = "malware.exe";
            string expectedExtension = ".pdf";

            // Act
            string actualExtension = Path.GetExtension(fileName).ToLower();
            bool isValid = actualExtension == expectedExtension;

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidatePDFFile_WithJpgExtension_ReturnsFalse()
        {
            // Arrange
            string fileName = "image.jpg";
            string expectedExtension = ".pdf";

            // Act
            string actualExtension = Path.GetExtension(fileName).ToLower();
            bool isValid = actualExtension == expectedExtension;

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidatePDFFile_WithPngExtension_ReturnsFalse()
        {
            // Arrange
            string fileName = "screenshot.png";
            string expectedExtension = ".pdf";

            // Act
            string actualExtension = Path.GetExtension(fileName).ToLower();
            bool isValid = actualExtension == expectedExtension;

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidatePDFFile_WithDocxExtension_ReturnsFalse()
        {
            // Arrange
            string fileName = "document.docx";
            string expectedExtension = ".pdf";

            // Act
            string actualExtension = Path.GetExtension(fileName).ToLower();
            bool isValid = actualExtension == expectedExtension;

            // Assert
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
            // Act
            string extension = Path.GetExtension(fileName).ToLower();
            bool isValid = extension == ".pdf";

            // Assert
            Assert.Equal(expectedIsValid, isValid);
        }
    }
}
