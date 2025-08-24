using System;
using System.Collections.Generic;

namespace Demo1.Models;

public partial class Dangkysukien
{
    public string Madk { get; set; } = null!;

    public string? Mask { get; set; }

    public string? Mand { get; set; }

    public DateTime Thoigiandangky { get; set; }

    public string? Xacnhanthamgia { get; set; }

    public virtual Nguoidung? MandNavigation { get; set; }

    public virtual Sukien? MaskNavigation { get; set; }
}
