using System;
using System.Collections.Generic;

namespace Demo1.Models;

public partial class Donvicungcap
{
    public string Madvcc { get; set; } = null!;

    public string Tendvcc { get; set; } = null!;

    public string? Email { get; set; }

    public string? Sdt { get; set; }

    public virtual ICollection<Loaiphanthuong> Loaiphanthuongs { get; set; } = new List<Loaiphanthuong>();
}
