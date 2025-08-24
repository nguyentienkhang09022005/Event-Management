using System;
using System.Collections.Generic;

namespace Demo1.Models;

public partial class Ctpt
{
    public string Mapt { get; set; } = null!;

    public string? Malpt { get; set; }

    public short Soluong { get; set; }

    public string? Mask { get; set; }

    public virtual ICollection<Ghinhan> Ghinhans { get; set; } = new List<Ghinhan>();

    public virtual Loaiphanthuong? MalptNavigation { get; set; }

    public virtual Sukien? MaskNavigation { get; set; }
}
