using EmployeeManagement.Domain.Models;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
namespace EmployeeManagement.Tests.Repositories
{
    [TestFixture]
    public class EmployeeRepositoryTests
    {
        private EmployeeDbContext _context;
        private EmployeeRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<EmployeeDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // isolated per test
                .Options;

            _context = new EmployeeDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new EmployeeRepository(_context);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddEmployee()
        {
            // Arrange
            var employee = new Employee("Alice", "Smith", "alice@example.com", "QA Engineer");

            // Act
            await _repository.AddAsync(employee);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _repository.GetByIdAsync(employee.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo("alice@example.com"));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllEmployees()
        {
            // Arrange
            _context.Employees.AddRange(
                new Employee("A", "B", "a@b.com", "Tester"),
                new Employee("C", "D", "c@d.com", "Dev")
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveEmployee()
        {
            // Arrange
            var employee = new Employee("Eve", "Doe", "eve@example.com", "Designer");
            await _repository.AddAsync(employee);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(employee.Id);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _repository.GetByIdAsync(employee.Id);
            Assert.That(result, Is.Null);
        }
    }
}
