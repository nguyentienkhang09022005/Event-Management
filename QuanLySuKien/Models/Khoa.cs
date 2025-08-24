using System;
using System.Collections.Generic;

namespace Demo1.Models;

public partial class Khoa
{
    public string Makhoa { get; set; } = null!;

    public string Tenkhoa { get; set; } = null!;

    public virtual ICollection<Nguoidung> Nguoidungs { get; set; } = new List<Nguoidung>();

    public virtual ICollection<Sukien> Sukiens { get; set; } = new List<Sukien>();
}
