using System;
using System.Collections.Generic;

namespace Demo1.Models;

public partial class Ghinhan
{
    public string Magn { get; set; } = null!;

    public string? Mand { get; set; }

    public string? Mask { get; set; }

    public virtual Nguoidung? MandNavigation { get; set; }

    public virtual Sukien? MaskNavigation { get; set; }
}
