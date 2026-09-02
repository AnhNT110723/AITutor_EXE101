//xử lí sidebar với nave bar và popup

window.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const navbar = document.querySelector('.navbar');
    const content = document.getElementById('content');
    const sidebarCollapse = document.getElementById('sidebarCollapse');
    const logoImg = document.querySelector('#sidebar .logo img'); // Lấy phần tử ảnh logo


    // Hàm thay đổi logo
    function updateLogo() {
        if (sidebar.classList.contains('active')) {
            logoImg.src = '/images/concoc.png'; // Logo mặc định khi sidebar active
            logoImg.classList.add('logo-active'); // Thêm class cho trạng thái active
            logoImg.classList.remove('logo-inactive'); // Xóa class cho trạng thái không active
        } else {
            logoImg.src = '/images/FAI-1.png'; // Logo khi sidebar không active
            logoImg.classList.add('logo-inactive'); // Thêm class cho trạng thái không active
            logoImg.classList.remove('logo-active'); // Xóa class cho trạng thái active
        }
    }


    // Xử lý sự kiện click vào nút toggle sidebar
    if (sidebarCollapse) {
        sidebarCollapse.addEventListener('click', function () {
            updateLogo();
        });
    }

});

// Thiết lập trạng thái mặc định khi load trang trên mobile
window.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    if (sidebar && window.innerWidth < 991) {
        sidebar.classList.remove('active');
        const logoImg = document.querySelector('#sidebar .logo img');
        if (logoImg) {
            logoImg.src = '/images/FAI-1.png';
            logoImg.classList.add('logo-inactive');
            logoImg.classList.remove('logo-active');
        }
    }
});
