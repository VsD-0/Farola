using System;
using System.Collections.Generic;

namespace Farola.Database.Models;

public partial class Statement
{
    public int Id { get; set; }

    public int Professional { get; set; }

    public int Client { get; set; }

    public int StatusId { get; set; }

    public DateTime DateAdded { get; set; }

    public virtual User ClientNavigation { get; set; } = null!;

    public virtual User ProfessionalNavigation { get; set; } = null!;

    public virtual StatementStatus Status { get; set; } = null!;
}
