using System;
using System.Collections.Generic;

namespace Farola.Database.Models;

public partial class User
{
    public int Id { get; set; }

    public int Role { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Area { get; set; }

    public string? Information { get; set; }

    public int? Specialization { get; set; }

    public string? Photo { get; set; }

    public DateTime DateRegistration { get; set; }

    public string? Profession { get; set; }

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual Specialization? SpecializationNavigation { get; set; }

    public virtual ICollection<Statement> StatementClientNavigations { get; set; } = new List<Statement>();

    public virtual ICollection<Statement> StatementProfessionalNavigations { get; set; } = new List<Statement>();
}
