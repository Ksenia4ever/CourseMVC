using System;
using System.Collections.Generic;

namespace CourseDomain.Model;

public partial class AccountScourse 
{
    public int AccountId { get; set; }

    public int ScoreId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Score Score { get; set; } = null!;
}
