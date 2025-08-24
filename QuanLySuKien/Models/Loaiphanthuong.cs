using System;
using System.Collections.Generic;

namespace Demo1.Models;

public partial class Loaiphanthuong
{
    public string Malpt { get; set; } = null!;

    public string Tenlpt { get; set; } = null!;

    public string? Madvcc { get; set; }

    public string Mota { get; set; } = null!;

    public virtual ICollection<Ctpt> Ctpts { get; set; } = new List<Ctpt>();

    public virtual Donvicungcap? MadvccNavigation { get; set; }
}
