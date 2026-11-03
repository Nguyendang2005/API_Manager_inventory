# 📦 SmartWarehouse API

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13-239120?style=for-the-badge&logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver)](https://www.microsoft.com/en-us/sql-server)
[![EF Core](https://img.shields.io/badge/EF%20Core-10-512BD4?style=for-the-badge&logo=dotnet)](https://docs.microsoft.com/en-us/ef/core/)
[![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=for-the-badge&logo=jsonwebtokens)](https://jwt.io/)
[![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=for-the-badge&logo=swagger)](https://swagger.io/)

**Hệ thống quản lý kho hàng thông minh** — RESTful API xây dựng theo kiến trúc **Clean Architecture**, hỗ trợ quản lý nhập xuất kho, tồn kho sản phẩm, phân quyền JWT và báo cáo thống kê.

</div>

---

## 📋 Mục lục

- [Tính năng nổi bật](#-tính-năng-nổi-bật)
- [Kiến trúc hệ thống](#-kiến-trúc-hệ-thống)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Cài đặt & Chạy](#-cài-đặt--chạy)
- [Cấu hình](#-cấu-hình)
- [API Endpoints](#-api-endpoints)
- [Phân quyền](#-phân-quyền)
- [Domain Model](#-domain-model)
- [Unit Tests](#-unit-tests)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)

---

## ✨ Tính năng nổi bật

| Tính năng | Mô tả |
|-----------|-------|
| 🏭 **Quản lý kho hàng** | CRUD kho, kiểm tra mã kho duy nhất, quản lý vị trí lưu trữ |
| 🚚 **Nhà cung cấp** | CRUD nhà cung cấp, tìm kiếm theo tên, validate mã số thuế / email / phone |
| 📦 **Sản phẩm** | CRUD sản phẩm, kiểm tra SKU duy nhất, theo dõi tồn kho real-time |
| 📥 **Phiếu nhập kho** | Tạo phiếu (Draft), xác nhận & tự động cộng tồn kho, báo cáo tháng |
| 📤 **Phiếu xuất kho** | Tạo phiếu, trừ kho nguyên tử (Atomic), duyệt phiếu phân quyền Manager |
| 🔐 **Bảo mật JWT** | Bearer Token, Role-Based Access Control (`WarehouseStaff`, `WarehouseManager`) |
| 📑 **Phân trang** | Hỗ trợ `pageIndex` / `pageSize` cho danh sách phiếu xuất |
| 🔍 **X-Correlation-ID** | Tracing request xuyên suốt toàn bộ pipeline |
| 🛡️ **Global Exception** | RFC 7807 `ProblemDetails` — xử lý lỗi chuẩn hóa toàn cục |
| 📊 **Swagger UI** | Giao diện tương tác đầy đủ với Bearer Auth modal |

---

## 🏗️ Kiến trúc hệ thống

Dự án được xây dựng theo **Clean Architecture** với 4 layer tách biệt hoàn toàn:

```
┌─────────────────────────────────────────────────┐
│              WebAPI Layer                        │
│  Controllers · Middlewares · Extensions          │
├─────────────────────────────────────────────────┤
│            Application Layer                    │
│  Services · DTOs · Contracts · Common           │
├─────────────────────────────────────────────────┤
│            Infrastructure Layer                  │
│  EF Core · Repositories · UnitOfWork · DbContext│
├─────────────────────────────────────────────────┤
│              Domain Layer                        │
│  Entities · Enums · Domain Exceptions           │
└─────────────────────────────────────────────────┘
```

**Luồng dữ liệu:**

```
HTTP Request → Controller → IService → IRepository → DbContext → SQL Server
                   ↓
            GlobalExceptionMiddleware (RFC 7807 ProblemDetails)
                   ↓
            RequestCorrelationMiddleware (X-Correlation-ID)
```

---

## 📁 Cấu trúc dự án

```
SmartWarehouse/
├── SmartWarehouse.Domain/              # Core business entities & rules
│   ├── Entities/
│   │   ├── Category.cs
│   │   ├── Product.cs
│   │   ├── Supplier.cs
│   │   ├── Warehouse.cs
│   │   ├── WarehouseLocation.cs
│   │   ├── InventoryReceipt.cs         # Phiếu nhập kho
│   │   ├── InventoryReceiptDetail.cs
│   │   ├── InventoryIssue.cs           # Phiếu xuất kho
│   │   └── InventoryIssueDetail.cs
│   ├── Enums/
│   │   ├── ReceiptStatus.cs            # Draft | Confirmed | Cancelled
│   │   ├── IssueStatus.cs              # Draft | Approved | Dispatched
│   │   └── StorageType.cs
│   └── Exceptions/
│       ├── DomainException.cs
│       ├── DuplicateSkuException.cs
│       └── InsufficientStockException.cs
│
├── SmartWarehouse.Application/         # Use cases & business logic
│   ├── Contracts/
│   │   ├── Persistence/                # Repository interfaces
│   │   └── Services/                   # Service interfaces
│   ├── DTOs/                           # Data Transfer Objects
│   │   ├── Auth/ · Product/ · Supplier/
│   │   ├── Warehouse/ · InventoryReceipt/ · InventoryIssue/
│   │   └── Common/
│   ├── Service/                        # Service implementations
│   │   ├── AuthService.cs
│   │   ├── ProductService.cs
│   │   ├── SupplierService.cs
│   │   ├── WarehouseService.cs
│   │   ├── InventoryReceiptService.cs
│   │   └── InventoryIssueService.cs
│   └── Common/
│       ├── PageResult.cs
│       └── PaginationParams.cs
│
├── SmartWarehouse.Infrastructure/      # Data access & persistence
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── Configurations/             # EF Core Fluent API configs
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   ├── ProductRepository.cs
│   │   ├── InventoryReceiptRepository.cs
│   │   ├── InventoryIssueRepository.cs
│   │   ├── SupplierRepository.cs
│   │   ├── WarehouseRepository.cs
│   │   └── UnitOfWork.cs
│   └── Migrations/
│
├── SmartWarehouse.WebAPI/              # Presentation layer
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProductsController.cs
│   │   ├── SuppliersController.cs
│   │   ├── WarehousesController.cs
│   │   ├── InventoryReceiptsController.cs
│   │   └── InventoryIssuesController.cs
│   ├── Middlewares/
│   │   ├── GlobalExceptionMiddleware.cs
│   │   └── RequestCorrelationMiddleware.cs
│   ├── Extensions/
│   │   ├── ServiceCollectionExtensions.cs  # JWT & Authorization
│   │   └── SwaggerConfigurationExtensions.cs
│   └── Program.cs
│
└── SmartWarehouse.UnitTests/           # xUnit unit tests
    ├── AuthServiceTests.cs
    ├── WarehouseServiceTests.cs
    ├── SupplierServiceTests.cs
    └── InventoryIssueServiceTests.cs
```

---

## ⚙️ Yêu cầu hệ thống

| Thành phần | Phiên bản |
|------------|-----------|
| .NET SDK | 10.0+ |
| SQL Server | 2019+ (hoặc LocalDB) |
| Visual Studio | 2022+ (hoặc VS Code) |
| EF Core Tools | `dotnet-ef` |

---

## 🚀 Cài đặt & Chạy

### 1. Clone repository

```bash
git clone https://github.com/Nguyendang2005/API_Manager_inventory.git
cd API_Manager_inventory
```

### 2. Cấu hình Connection String

Mở file `SmartWarehouse.WebAPI/appsettings.json` và cập nhật:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<TEN_SERVER>;Database=SmartWarehouseDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Chạy Migration & tạo database

```bash
cd SmartWarehouse.WebAPI
dotnet ef database update --project ../SmartWarehouse.Infrastructure
```

### 4. Chạy ứng dụng

```bash
dotnet run --project SmartWarehouse.WebAPI
```

### 5. Mở Swagger UI

Truy cập: **[https://localhost:7090/swagger](https://localhost:7090/swagger)**

---

## 🔧 Cấu hình

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DANG;Database=SmartWarehouseDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "SmartWarehouse_Secret_Key_For_JWT_Authentication_2026_Secure_Token!",
    "Issuer": "SmartWarehouse.API",
    "Audience": "SmartWarehouse.Client"
  }
}
```

> **⚠️ Lưu ý bảo mật:** Không commit `Jwt:Key` thật lên Git. Dùng `dotnet user-secrets` hoặc biến môi trường cho production.

---

## 📡 API Endpoints

### 🔐 Authentication

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `POST` | `/api/v1/auth/login` | Đăng nhập lấy Bearer Token | ❌ |

**Demo accounts:**
```
manager / Manager123!   → Role: WarehouseManager
staff   / Staff123!     → Role: WarehouseStaff
```

---

### 🏭 Warehouses — `/api/v1/warehouses`

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/api/v1/warehouses` | Lấy danh sách kho đang hoạt động | ❌ |
| `GET` | `/api/v1/warehouses/{id}` | Lấy chi tiết kho theo ID | ❌ |
| `POST` | `/api/v1/warehouses` | Tạo kho mới (kiểm tra mã kho duy nhất) | ❌ |
| `PUT` | `/api/v1/warehouses/{id}` | Cập nhật thông tin kho | ❌ |
| `DELETE` | `/api/v1/warehouses/{id}` | Xóa kho | ❌ |

---

### 🚚 Suppliers — `/api/v1/suppliers`

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/api/v1/suppliers` | Danh sách nhà cung cấp | ❌ |
| `GET` | `/api/v1/suppliers?search={name}` | Tìm kiếm theo tên | ❌ |
| `GET` | `/api/v1/suppliers/{id}` | Chi tiết nhà cung cấp | ❌ |
| `POST` | `/api/v1/suppliers` | Thêm nhà cung cấp mới | ❌ |
| `PUT` | `/api/v1/suppliers/{id}` | Cập nhật nhà cung cấp | ❌ |
| `DELETE` | `/api/v1/suppliers/{id}` | Xóa nhà cung cấp | ❌ |

---

### 📦 Products — `/api/v1/products`

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/api/v1/products` | Danh sách sản phẩm đang hoạt động | ❌ |
| `GET` | `/api/v1/products/{id}` | Chi tiết sản phẩm | ❌ |
| `POST` | `/api/v1/products` | Tạo sản phẩm (kiểm tra SKU trùng) | ❌ |
| `PUT` | `/api/v1/products/{id}` | Cập nhật sản phẩm | ❌ |
| `DELETE` | `/api/v1/products/{id}` | Xóa sản phẩm | ❌ |

---

### 📥 Inventory Receipts (Phiếu nhập kho) — `/api/v1/inventoryreceipts`

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/api/v1/inventoryreceipts?warehouseId={id}` | Danh sách phiếu nhập theo kho | ❌ |
| `GET` | `/api/v1/inventoryreceipts/{id}` | Chi tiết phiếu nhập (Eager loading) | ❌ |
| `GET` | `/api/v1/inventoryreceipts/summary?warehouseId={id}&year={y}&month={m}` | Báo cáo nhập kho theo tháng | ❌ |
| `POST` | `/api/v1/inventoryreceipts` | Tạo phiếu nhập (trạng thái Draft) | ❌ |
| `POST` | `/api/v1/inventoryreceipts/{id}/confirm` | Xác nhận phiếu → tự động cộng tồn kho | ❌ |

**Workflow phiếu nhập:**
```
Draft → [Confirm] → Confirmed
             ↓
     StockQuantity += quantity (per product)
```

---

### 📤 Inventory Issues (Phiếu xuất kho) — `/api/v1/inventory-issues`

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/api/v1/inventory-issues?pageIndex={p}&pageSize={s}` | Danh sách phiếu xuất (phân trang) | ❌ |
| `GET` | `/api/v1/inventory-issues/{id}` | Chi tiết phiếu xuất | ❌ |
| `POST` | `/api/v1/inventory-issues` | Tạo phiếu xuất + trừ tồn kho nguyên tử | 🔐 Staff/Manager |
| `PUT` | `/api/v1/inventory-issues/{id}/approve` | Duyệt phiếu xuất | 🔐 Manager only |

**Workflow phiếu xuất:**
```
Draft → [Create - Staff/Manager] → Draft
      → [Approve - Manager only] → Approved
                  ↓
       StockQuantity -= quantity (Atomic via UnitOfWork)
       InsufficientStockException nếu không đủ hàng
```

---

## 🔐 Phân quyền

| Role | Quyền hạn |
|------|-----------|
| `WarehouseStaff` | Tạo phiếu xuất kho |
| `WarehouseManager` | Tạo phiếu xuất + **Duyệt phiếu** |

**Cách sử dụng Bearer Token trong Swagger:**
1. Gọi `POST /api/v1/auth/login` với credentials demo
2. Copy `accessToken` từ response
3. Click **Authorize** (🔒) trên Swagger UI
4. Nhập: `Bearer <token>`

---

## 🗃️ Domain Model

```
Category ──< Product >──< InventoryReceiptDetail >── InventoryReceipt >── Warehouse
                    └──< InventoryIssueDetail    >── InventoryIssue   ─┘
                                                                        └── Supplier (qua Receipt)
Warehouse ──< WarehouseLocation
```

**Entities chính:**

| Entity | Mô tả |
|--------|-------|
| `Warehouse` | Kho hàng (Code, Name, Address, Locations) |
| `WarehouseLocation` | Vị trí lưu trữ trong kho (StorageType) |
| `Supplier` | Nhà cung cấp (Code, TaxCode, Email, Phone) |
| `Category` | Danh mục sản phẩm |
| `Product` | Sản phẩm (SKU, Unit, UnitPrice, StockQuantity) |
| `InventoryReceipt` | Phiếu nhập kho — trạng thái: `Draft → Confirmed → Cancelled` |
| `InventoryIssue` | Phiếu xuất kho — trạng thái: `Draft → Approved → Dispatched` |

---

## 🧪 Unit Tests

```bash
cd SmartWarehouse.UnitTests
dotnet test
```

Các test case bao phủ:

| Test File | Mô tả |
|-----------|-------|
| `AuthServiceTests.cs` | Xác thực đăng nhập, tạo JWT token |
| `WarehouseServiceTests.cs` | CRUD kho, kiểm tra mã trùng |
| `SupplierServiceTests.cs` | CRUD nhà cung cấp |
| `InventoryIssueServiceTests.cs` | Tạo phiếu xuất, trừ tồn kho, `InsufficientStockException` |

---

## 🛠️ Công nghệ sử dụng

| Công nghệ | Mục đích |
|-----------|----------|
| **ASP.NET Core 10** | Web API framework |
| **Entity Framework Core 10** | ORM — Code First, Fluent API |
| **SQL Server** | Cơ sở dữ liệu chính |
| **JWT Bearer** | Xác thực & phân quyền |
| **Swashbuckle / Swagger** | API documentation & testing UI |
| **xUnit** | Unit testing framework |
| **Moq** | Mocking trong unit tests |
| **Clean Architecture** | Tách biệt concern theo layer |
| **Repository + UnitOfWork Pattern** | Quản lý data access |
| **RFC 7807 ProblemDetails** | Chuẩn hóa error response |

---

## 🤝 Đóng góp

1. Fork repository
2. Tạo branch: `git checkout -b feature/ten-tinh-nang`
3. Commit: `git commit -m "feat: mô tả thay đổi"`
4. Push: `git push origin feature/ten-tinh-nang`
5. Mở Pull Request

---

<div align="center">

Made with ❤️ by **Nguyendang2005**

</div>
