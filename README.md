# CheckDiskUsageOfApp

Khi thu thập dữ liệu sử dụng đĩa từ một quá trình (process), chúng ta thường thu thập thông tin về số lượng byte mà quá trình đó đọc và ghi từ và tới các thiết bị lưu trữ như ổ đĩa cứng hoặc ổ đĩa trạng thái rắn (SSD). Dữ liệu này thường được thu thập dưới dạng:

- IO Read Bytes/sec: Số byte mà quá trình đọc từ các thiết bị vào mỗi giây.
- IO Write Bytes/sec: Số byte mà quá trình ghi ra các thiết bị mỗi giây.
- IO Data Bytes/sec: Tổng số byte mà quá trình truyền I/O (tức là cả đọc và ghi) mỗi giây. Đây là đơn vị mà PerformanceCounter trong project đang theo dõi.

Đơn vị của thông số thu thập là bytes/giây, và đây là một đơn vị tiêu chuẩn trong đo lường hiệu suất I/O vì nó cho thấy tốc độ dữ liệu di chuyển qua lại giữa quá trình và thiết bị lưu trữ. Sở dĩ chúng ta chọn đơn vị này để thu thập dữ liệu vì:

1. Khả năng Đo Lường Hiệu Suất: Bytes/giây cung cấp thông tin cụ thể về quá trình sử dụng đĩa. Nó cho chúng ta biết được định lượng chính xác dữ liệu được xử lý trong một đơn vị thời gian.

2. Phân Tích Sự Chênh Lệch: Khi thu thập thông tin này qua thời gian, chúng ta có thể phân tích để xác định sự chênh lệch và tìm những bất thường. Điều này giúp xác định các mô hình hành vi của ứng dụng, như khi nào nó cần nhiều tài nguyên I/O và liệu có ổn định không.

3. Dự đoán và Mở Rộng: Dữ liệu có được giúp trong việc dự đoán khi nào cần mở rộng hoặc nâng cấp hệ thống lưu trữ và ngăn chặn ùn tắc I/O, từ đó cải thiện hiệu suất tổng thể của ứng dụng.

Tóm lại, việc thu thập dữ liệu I/O bằng bytes/giây giúp theo dõi và đánh giá hiệu suất sử dụng ổ đĩa, là chìa khóa để quản lý tài nguyên hệ thống một cách thông minh và hiệu quả.
