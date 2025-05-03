// Hàm tạo slug (nếu không dùng thư viện)
// Hàm createSlug hỗ trợ cả tiếng Việt và tiếng Anh
function createSlug(text) {
    // Bảng ánh xạ cho ký tự tiếng Việt
    const vietnameseMap = {
        'á': 'a', 'à': 'a', 'ả': 'a', 'ã': 'a', 'ạ': 'a',
        'ă': 'a', 'ắ': 'a', 'ằ': 'a', 'ẳ': 'a', 'ẵ': 'a', 'ặ': 'a',
        'â': 'a', 'ấ': 'a', 'ầ': 'a', 'ẩ': 'a', 'ẫ': 'a', 'ậ': 'a',
        'é': 'e', 'è': 'e', 'ẻ': 'e', 'ẽ': 'e', 'ẹ': 'e',
        'ê': 'e', 'ế': 'e', 'ề': 'e', 'ể': 'e', 'ễ': 'e', 'ệ': 'e',
        'í': 'i', 'ì': 'i', 'ỉ': 'i', 'ĩ': 'i', 'ị': 'i',
        'ó': 'o', 'ò': 'o', 'ỏ': 'o', 'õ': 'o', 'ọ': 'o',
        'ô': 'o', 'ố': 'o', 'ồ': 'o', 'ổ': 'o', 'ỗ': 'o', 'ộ': 'o',
        'ơ': 'o', 'ớ': 'o', 'ờ': 'o', 'ở': 'o', 'ỡ': 'o', 'ợ': 'o',
        'ú': 'u', 'ù': 'u', 'ủ': 'u', 'ũ': 'u', 'ụ': 'u',
        'ư': 'u', 'ứ': 'u', 'ừ': 'u', 'ử': 'u', 'ữ': 'u', 'ự': 'u',
        'ý': 'y', 'ỳ': 'y', 'ỷ': 'y', 'ỹ': 'y', 'ỵ': 'y',
        'đ': 'd'
    };

    return text
        .toLowerCase() // Chuyển thành chữ thường
        .replace(/[áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđ]/g, match => vietnameseMap[match] || match) // Xử lý ký tự tiếng Việt
        .replace(/[^a-z0-9\s-]/g, '') // Loại bỏ ký tự đặc biệt (giữ a-z, 0-9, khoảng trắng, dấu -)
        .trim() // Xóa khoảng trắng đầu cuối
        .replace(/\s+/g, '-') // Thay khoảng trắng bằng dấu gạch nối
        .replace(/-+/g, '-'); // Loại bỏ nhiều dấu gạch nối liên tiếp
}

function formatTitle(slug) {
    return slug
        .split('-') // Tách thành mảng: test-dau-vao-1 -> ['test', 'dau', 'vao', '1']
        .map(word => word.charAt(0).toUpperCase() + word.slice(1)) // Viết hoa chữ cái đầu
        .join(' '); // Nối lại bằng khoảng trắng: Test Dau Vao 1
}

