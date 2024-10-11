namespace DocScheduler.Utils.Test
{
    public class DateUtilsTests
    {
        [Fact]
        public void GetPreviousMonday_Returns_Correct_Date()
        {
            // Arrange
            DateTime inputDate = new DateTime(2024, 7, 5); // Wednesday, July 5th, 2024
            DateTime expectedDate = new DateTime(2024, 7, 1); // Previous Monday is July 1st, 2024 (Monday)

            // Act
            DateTime result = DateUtils.GetPreviousMonday(inputDate);

            // Assert
            Assert.Equal(expectedDate, result);
        }

        [Fact]
        public void GetNextMonday_Returns_Correct_Date()
        {
            // Arrange
            DateTime currentDate = new DateTime(2024, 7, 5); // Wednesday, July 5th, 2024
            DateTime expectedDate = new DateTime(2024, 7, 8); // Next Monday is July 8th, 2024 (Monday)

            // Act
            DateTime result = DateUtils.GetNextMonday(currentDate);

            // Assert
            Assert.Equal(expectedDate, result);
        }

        [Fact]
        public void GetFormattedDate_Returns_Correct_Format()
        {
            // Arrange
            DateTime inputDate = new DateTime(2024, 7, 5); // Wednesday, July 5th, 2024
            string expectedFormat = "20240705"; // Expected formatted string in "yyyyMMdd" format

            // Act
            string result = DateUtils.GetFormattedDate(inputDate);

            // Assert
            Assert.Equal(expectedFormat, result);
        }
    }
}