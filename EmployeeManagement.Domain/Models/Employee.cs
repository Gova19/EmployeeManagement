using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Models;

public class Employee
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string JobTitle { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    // ctor for EF
    private Employee() { }

    public Employee(string firstName, string lastName, string email, string jobTitle)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email required", nameof(email));
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        JobTitle = jobTitle;
    }

    public void UpdateJobTitle(string jobTitle)
    {
        JobTitle = jobTitle;
    }
}
