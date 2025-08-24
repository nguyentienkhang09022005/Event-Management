using System;
using System.Collections.Generic;

namespace Demo1.Models;

public partial class Nguoidung
{
    public string Mand { get; set; } = null!;

    public string Hoten { get; set; } = null!;

    public string Gioitinh { get; set; } = null!;

    public string? Makhoa { get; set; }

    public string? Passworduser { get; set; }

    public string? Email { get; set; }

    public string Sdt { get; set; } = null!;

    public string? Masvgv { get; set; }

    public int? Roleuser { get; set; }

    public byte[] Imageuser { get; set; } = null!;

    public virtual ICollection<Dangkysukien> Dangkysukiens { get; set; } = new List<Dangkysukien>();

    public virtual Khoa? MakhoaNavigation { get; set; }

    public virtual ICollection<Sukien> Sukiens { get; set; } = new List<Sukien>();
}
