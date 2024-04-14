using System;
using System.Collections.Generic;

namespace Farola.Database.Models;

public partial class StatementStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Statement> Statements { get; set; } = new List<Statement>();
}
