# 🎉 UIT EVENT – Phần mềm Quản lý Sự kiện

> Ứng dụng hỗ trợ quản lý và tập trung hóa thông tin sự kiện trong trường Đại học Công nghệ Thông tin – ĐHQG TP.HCM.  
> Giúp sinh viên dễ dàng theo dõi, đăng ký và tham gia các sự kiện từ nhiều khoa trong trường.

---

## 📌 Giới thiệu

Hiện nay, các khoa trong trường UIT thường đăng tải sự kiện qua fanpage riêng, nhưng sinh viên ít quan tâm nên thông tin dễ bị bỏ lỡ.  
**UIT EVENT** ra đời để giải quyết vấn đề này bằng cách:

- Tập trung tất cả sự kiện của các khoa vào **một ứng dụng duy nhất**.  
- Cho phép sinh viên dễ dàng đăng ký, theo dõi và nhận nhắc nhở sự kiện.  
- Hỗ trợ các khoa và admin quản lý sự kiện hiệu quả, nhanh chóng.  

---

## 🚀 Tính năng chính

- 👥 **Phân quyền người dùng**: Admin, Dean (khoa), User (sinh viên).  
- 📅 **Quản lý sự kiện**: Tạo, sửa, xóa, phê duyệt sự kiện.  
- 🔔 **Đăng ký sự kiện**: Sinh viên đăng ký, theo dõi và nhận thông báo nhắc nhở.  
- 📊 **Thống kê & báo cáo**: Quản lý danh sách người tham gia, thống kê hiệu quả.  
- 👤 **Quản lý người dùng**: Thêm, chỉnh sửa, phân quyền.  

---

## 🛠 Công nghệ sử dụng

- **Ngôn ngữ**: `C#`, `SQL`, `XAML`  
- **Framework**: `WPF`, `Entity Framework`, `Material Design Library`  
- **CSDL**: `SQL Server`  
- **Môi trường phát triển**: `Visual Studio`, `SQL Server Management Studio`  
- **Thiết kế giao diện**: `Figma`, `Draw.io`  
- **Quản lý mã nguồn**: `GitHub`  

---

## 🏗 Kiến trúc hệ thống

- Áp dụng mô hình **MVVM** (Model – View – ViewModel).  
- Hệ thống được chia thành 3 lớp chính:
  - **Frontend (UI)**: Giao diện WPF, trực quan và thân thiện.  
  - **Backend (Logic)**: Xử lý luồng nghiệp vụ, phân quyền và quản lý sự kiện.  
  - **Database**: SQL Server lưu trữ thông tin người dùng, sự kiện, khoa, đăng ký.  

---

## 📊 Mô hình CSDL (ERD)

- **NGUOIDUNG** (User)  
- **SUKIEN** (Event)  
- **KHOA** (Faculty)  
- **DANGKYSUKIEN** (Event Registration)  

Quan hệ chính:  
- 1 khoa – nhiều người dùng.  
- 1 người dùng – nhiều sự kiện (thông qua bảng đăng ký).  
- 1 sự kiện – nhiều người đăng ký.  

---

## 🔄 Luồng xử lý

1. Người dùng đăng ký tài khoản → đăng nhập.  
2. Admin phân quyền (User → Dean).  
3. Dean tạo sự kiện → Admin phê duyệt.  
4. Sinh viên đăng ký tham gia sự kiện.  
5. Dean xác nhận sinh viên tham gia.  
6. Sinh viên nhận thông báo & tham gia sự kiện.  

---

## 📈 Phân tích SWOT

- **Điểm mạnh**: Giao diện trực quan, tập trung thông tin sự kiện.  
- **Điểm yếu**: Khả năng mở rộng còn hạn chế.  
- **Cơ hội**: Đáp ứng nhu cầu kết nối thông tin trong toàn trường.  
- **Thách thức**: Bảo mật dữ liệu người dùng.  

---

## ⭐ Kết luận

Dự án **UIT EVENT** đã đạt được:  
- Giải quyết hiệu quả bài toán quản lý sự kiện trong toàn trường.  
- Giao diện thân thiện, hỗ trợ đa dạng nghiệp vụ.  
- Tăng cường kết nối giữa sinh viên và khoa.  

👉 Hướng phát triển: Mở rộng quy mô, tích hợp mobile app và tính năng nhắc lịch thông minh.  

---

Dự án này mình clone lại từ tài khoản github thứ 2 của mình là: https://github.com/Khanginyoureyes

---
✨ Nếu bạn thấy dự án hữu ích, hãy **Star ⭐ repository** để ủng hộ nhóm nhé!
