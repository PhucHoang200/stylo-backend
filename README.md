# 👗 STYLO – Backend API

Backend API trung tâm cho hệ thống **Stylo – Fashion Store Management System**.
Dịch vụ này chịu trách nhiệm xử lý toàn bộ nghiệp vụ cốt lõi, quản lý dữ liệu
và điều phối giao tiếp giữa ứng dụng frontend và các AI services.

---

## 📂 Related Repositories

Hệ thống Stylo bao gồm nhiều repository độc lập:

* 🔹 **Frontend (Flutter Mobile App)**
  👉 [https://github.com/PhucHoang200/stylo-frontend.git](https://github.com/PhucHoang200/stylo-frontend.git)

* 🔹 **AI Image Search Service (CNN)**
  👉 [https://github.com/PhucHoang200/image-search-service.git](https://github.com/PhucHoang200/image-search-service.git)

* 🔹 **AI Recommendation Service**
  👉 [https://github.com/PhucHoang200/recommend-service-fastapi.git](https://github.com/PhucHoang200/recommend-service-fastapi.git)

> Backend API (repo này) đóng vai trò trung tâm và kết nối tất cả các thành phần trên.

---

## 📦 System Overview

Hệ thống Stylo được xây dựng theo kiến trúc đa tầng, gồm các thành phần:

* **Mobile App (Flutter)**: giao diện người dùng
* **Backend API (repo này)**: xử lý nghiệp vụ và điều phối hệ thống
* **AI Image Search Service**: tìm kiếm sản phẩm bằng hình ảnh
* **AI Recommendation Service**: gợi ý sản phẩm thông minh

Backend API đóng vai trò **trung tâm**, cung cấp API cho frontend
và làm cầu nối giao tiếp với các AI services.

---

## 🧱 Architecture

```text
Flutter Mobile App
        ↓
ASP.NET Core Web API
        ↓
FastAPI AI Services
```

---

## 🎯 Responsibilities

* Quản lý người dùng và xác thực (JWT)
* Phân quyền truy cập hệ thống
* Quản lý sản phẩm, danh mục và tồn kho
* Quản lý đơn hàng
* Cung cấp RESTful API cho frontend
* Giao tiếp và tích hợp các AI services
* Chuẩn hóa dữ liệu đầu vào và đầu ra

---

## 🛠 Tech Stack

* **Framework**: ASP.NET Core Web API
* **Language**: C#
* **Database**: SQL Server
* **ORM**: Entity Framework Core
* **Authentication**: JWT
* **API Style**: RESTful API

---

## 🚀 Run Locally

### Prerequisites

* .NET SDK
* SQL Server

### Setup & Run

```bash
dotnet restore
dotnet run
```

Sau khi chạy thành công, backend API sẽ sẵn sàng để frontend và các service khác kết nối.

---

## 🔐 Environment Variables

| Name                  | Description                           |
| --------------------- | ------------------------------------- |
| DB_CONNECTION         | Chuỗi kết nối SQL Server              |
| JWT_SECRET            | Secret key dùng để ký và xác thực JWT |
| AI_IMAGE_SEARCH_URL   | URL của AI Image Search Service       |
| AI_RECOMMENDATION_URL | URL của AI Recommendation Service     |

---

## 🔌 External Services Integration

### AI Image Search Service

* **Method**: `POST`
* **Endpoint**: `/search-by-image`
* **Purpose**: Tìm kiếm sản phẩm tương tự dựa trên hình ảnh đầu vào

### AI Recommendation Service

* **Method**: `GET`
* **Endpoint**: `/recommend/{product_id}`
* **Purpose**: Gợi ý sản phẩm tương tự dựa trên thuộc tính sản phẩm

Backend chịu trách nhiệm gửi request, nhận response
và chuẩn hóa dữ liệu trước khi trả về cho frontend.

---

## 📄 API Design

* RESTful API
* JSON format cho request/response
* Có thể tích hợp Swagger/OpenAPI để tài liệu hóa API

---

## 📁 Project Structure (tham khảo)

```text
src/
 ├─ Controllers/
 ├─ Services/
 ├─ Models/
 ├─ DTOs/
 ├─ Data/
 └─ Program.cs
```
