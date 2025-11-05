using EmployeeManagement.Domain.Models;


namespace EmployeeManagement.Tests.Domain
{
    [TestFixture]
    public class EmployeeTests
    {
        [Test]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var email = "john.doe@example.com";
            var jobTitle = "Developer";

            // Act
            var employee = new Employee(firstName, lastName, email, jobTitle);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(employee.FirstName, Is.EqualTo(firstName));
                Assert.That(employee.LastName, Is.EqualTo(lastName));
                Assert.That(employee.Email, Is.EqualTo(email));
                Assert.That(employee.JobTitle, Is.EqualTo(jobTitle));
                Assert.That(employee.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(employee.CreatedAt, Is.Not.EqualTo(DateTimeOffset.MinValue));
            });
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenEmailIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => new Employee("John", "Doe", "", "Developer"));
        }

        [Test]
        public void UpdateJobTitle_ShouldUpdateValue()
        {
            // Arrange
            var employee = new Employee("John", "Doe", "john@example.com", "Developer");

            // Act
            employee.UpdateJobTitle("Senior Developer");

            // Assert
            Assert.That(employee.JobTitle, Is.EqualTo("Senior Developer"));
        }
    }
}
