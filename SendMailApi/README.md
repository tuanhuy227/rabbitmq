# Hướng dẫn sử dụng SendMail API

## Yêu cầu trước khi chạy
- .NET 8.0 SDK
- Docker (để chạy RabbitMQ)

## Cấu hình

### 1. Cài đặt RabbitMQ
Chạy RabbitMQ bằng Docker:
```bash
docker run -d -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### 2. Cấu hình appsettings.json
Cập nhật thông tin SMTP trong `appsettings.json`:
```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": "587",
  "UserName": "your-email@gmail.com",
  "Password": "your-app-password",
  "FromEmail": "your-email@gmail.com",
  "FromName": "SendMail App"
}
```

Nếu dùng Gmail:
- Bật 2-Step Verification
- Tạo App Password

## Chạy dự án
```bash
cd SendMailApi
dotnet run
```

## API Endpoint

### Đăng ký người dùng
- **URL**: `POST /api/auth/register`
- **Body**:
```json
{
  "fullName": "Nguyen Van A",
  "email": "nguyenvana@example.com",
  "password": "123456"
}
```

## Luồng hoạt động
1. Người dùng đăng ký
2. Dữ liệu được lưu vào SQLite database
3. Email thông báo được đưa vào hàng đợi RabbitMQ
4. Background service đọc từ hàng đợi và gửi email qua SMTP
