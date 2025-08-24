using System;
using System.Collections.Generic;

namespace Demo1.Models;

public partial class Sukien
{
    public string Mask { get; set; } = null!;

    public string Tensk { get; set; } = null!;

    public string Dvtc { get; set; } = null!;

    public string Theloai { get; set; } = null!;

    public string Mota { get; set; } = null!;

    public string Mandb { get; set; } = null!;

    public int Soluongthamgia { get; set; }

    public string? Venue { get; set; }

    public string? Phanthuong { get; set; }

    public DateTime Ngaydongdangky { get; set; }

    public DateTime Ngaybatdau { get; set; }

    public DateTime Ngayketthuc { get; set; }

    public DateTime Ngaymodangky { get; set; }

    public byte[]? Imageevent { get; set; }

    public string? Nhataitro { get; set; }

    public int? Duyet { get; set; }

    public virtual ICollection<Dangkysukien> Dangkysukiens { get; set; } = new List<Dangkysukien>();

    public virtual Khoa DvtcNavigation { get; set; } = null!;

    public virtual Nguoidung MandbNavigation { get; set; } = null!;
}
