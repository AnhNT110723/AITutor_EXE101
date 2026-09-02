/**
 * Hàm tải ảnh dùng chung (Có thể tái sử dụng cho bất kỳ thẻ img nào)
 * @param {string} imageUrl - Đường dẫn URL hoặc Base64 của ảnh
 * @param {string} fileName - Tên file sẽ được lưu
 */
function downloadImage(imageUrl, fileName) {
    fetch(imageUrl)
        .then(response => response.blob())
        .then(blob => {
            // Tạo một URL giả (Blob URL) để tải file
            const blobUrl = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = blobUrl;
            a.download = fileName || 'download.png';
            document.body.appendChild(a);
            a.click();
            // Dọn dẹp sau khi tải xong
            window.URL.revokeObjectURL(blobUrl);
            a.remove();
        })
        .catch(err => {
            console.error('Lỗi khi tải ảnh (có thể do CORS), đang sử dụng fallback:', err);
            // Fallback: Mở ảnh sang tab mới để người dùng tự lưu (nhấn chuột phải -> Save as)
            const a = document.createElement('a');
            a.href = imageUrl;
            a.download = fileName || 'download.png';
            a.target = '_blank';
            document.body.appendChild(a);
            a.click();
            a.remove();
        });
}
