# Luồng Đăng Ký Người Dùng

## Biểu đồ luồng hoạt động

```mermaid
sequenceDiagram
    participant User as Người dùng
    participant API as Auth Controller
    participant UserService as User Service
    participant DB as Database (SQLite)
    participant RabbitMQ as RabbitMQ Queue
    participant Worker as Email Worker
    participant EmailService as SMTP Email Service

    User->>API: POST /api/auth/register<br/>(FullName, Email, Password)
    API->>UserService: RegisterAsync(RegisterDto)
    
    UserService->>DB: Kiểm tra email đã tồn tại?
    alt Email đã tồn tại
        DB-->>UserService: Trả về null
        UserService-->>API: Trả về false
        API-->>User: 400 Bad Request<br/>"Email đã tồn tại"
    else Email hợp lệ
        DB-->>UserService: Không tìm thấy
        UserService->>UserService: Băm mật khẩu (BCrypt)
        UserService->>DB: Lưu User vào DB
        DB-->>UserService: Lưu thành công
        
        UserService->>RabbitMQ: Gửi tin nhắn EmailMessageDto<br/>vào hàng đợi email_queue
        RabbitMQ-->>UserService: Tin nhắn đã thêm
        
        UserService-->>API: Trả về true
        API-->>User: 200 OK<br/>"Đăng ký thành công, vui lòng kiểm tra email"
    end

    Worker->>RabbitMQ: Lắng nghe hàng đợi email_queue
    RabbitMQ-->>Worker: Gửi tin nhắn EmailMessageDto
    
    Worker->>EmailService: SendEmailAsync(emailMessage)
    EmailService->>EmailService: Tạo MimeMessage
    EmailService->>SMTP Server: Kết nối SMTP và gửi email
    SMTP Server-->>EmailService: Email đã gửi
    EmailService-->>Worker: Hoàn thành
    
    Worker->>RabbitMQ: Xác nhận (BasicAck)
```

## Biểu đồ hệ thống

```mermaid
graph TB
    A[Người dùng] --> B[API Controller]
    B --> C[User Service]
    C --> D[Database<br/>SQLite]
    C --> E[RabbitMQ<br/>Queue]
    E --> F[Background<br/>Email Worker]
    F --> G[SMTP Email<br/>Service]
    G --> H[SMTP Server]
```

## Mô tả chi tiết các bước

### 1. Người dùng gửi yêu cầu đăng ký
- Gửi request POST đến `/api/auth/register`
- Body chứa: FullName, Email, Password

### 2. Controller xử lý yêu cầu
- `AuthController` nhận request và gọi `UserService.RegisterAsync()`

### 3. Kiểm tra và lưu vào database
- `UserService` kiểm tra xem email đã tồn tại trong DB chưa
- Nếu tồn tại: trả về lỗi
- Nếu chưa: băm mật khẩu bằng BCrypt và lưu User vào DB

### 4. Đưa email vào hàng đợi RabbitMQ
- `UserService` tạo `EmailMessageDto` với thông tin email chào mừng
- `RabbitMQService` gửi tin nhắn vào hàng đợi `email_queue`
- Tin nhắn được đánh dấu persistent để đảm bảo không mất dữ liệu

### 5. Background Worker xử lý hàng đợi
- `EmailWorker` chạy nền, lắng nghe hàng đợi `email_queue`
- Khi có tin nhắn mới, worker lấy tin nhắn ra và xử lý
- Worker tạo scope để lấy `EmailService` và gửi email

### 6. Gửi email qua SMTP
- `SmtpEmailService` sử dụng MailKit để kết nối SMTP server
- Tạo email HTML với nội dung chào mừng
- Gửi email đến địa chỉ người dùng
- Xác nhận gửi thành công và trả về worker

### 7. Hoàn thành
- Worker gửi `BasicAck` cho RabbitMQ để xóa tin nhắn khỏi hàng đợi
- Người dùng nhận được email chào mừng
