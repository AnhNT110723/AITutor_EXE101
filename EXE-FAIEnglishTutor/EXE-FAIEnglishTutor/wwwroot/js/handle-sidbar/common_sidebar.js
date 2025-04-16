
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
            logoImg.src = '/Images/concoc.png'; // Logo mặc định khi sidebar active
            logoImg.classList.add('logo-active'); // Thêm class cho trạng thái active
            logoImg.classList.remove('logo-inactive'); // Xóa class cho trạng thái không active
        } else {
            logoImg.src = '/Images/FAI-1.png'; // Logo khi sidebar không active
            logoImg.classList.add('logo-inactive'); // Thêm class cho trạng thái không active
            logoImg.classList.remove('logo-active'); // Xóa class cho trạng thái active
        }
    }


    // Hàm cập nhật giao diện khi thay đổi kích thước màn hình

    //function updateLayoutOnResize() {
    //    if (window.innerWidth >= 1400) {
    //        if (sidebar.classList.contains('active')) {
    //            sidebar.classList.add('active');
    //            navbar.style.left = "80px";
    //            navbar.style.width = "calc(100% - 80px)";
    //            if (bg_custom) bg_custom.style.width = "calc(100% - 80px)";
    //            if (item_custom) item_custom.style.width = "calc(100% - 80px)";
    //            if (right_btn) right_btn.classList.add('active');
    //            content.style.marginLeft = "80px";
    //        } else {
    //            sidebar.classList.remove('active');
    //            navbar.style.left = "270px";
    //            navbar.style.width = "calc(100% - 270px)";
    //            if (bg_custom) bg_custom.style.width = "calc(100% - 270px)";
    //            if (item_custom) item_custom.style.width = "calc(100% - 270px)";
    //            if (right_btn) right_btn.classList.remove('active');
    //            content.style.marginLeft = "270px";
    //        }

    //    } else if (window.innerWidth >= 991) {
    //        sidebar.classList.add('active');
    //        navbar.style.left = "80px";
    //        navbar.style.width = "calc(100% - 80px)";
    //        if (bg_custom) bg_custom.style.width = "calc(100% - 80px)";
    //        if (item_custom) item_custom.style.width = "calc(100% - 80px)";
    //        if (right_btn) right_btn.classList.add('active');
    //        content.style.marginLeft = "80px";
    //    } else {

    //        if (sidebar.classList.contains('active')) {
    //            sidebar.classList.remove('active');
    //            navbar.style.left = "0";
    //            navbar.style.width = "100%";
    //            if (bg_custom) bg_custom.style.width = "100%";
    //            if (item_custom) item_custom.style.width = "100%";
    //            if (right_btn) right_btn.classList.remove('active');
    //            content.style.marginLeft = "0";
    //        } else {
    //            sidebar.classList.add('active');
    //            navbar.style.left = "80px";
    //            navbar.style.width = "calc(100% - 80px)";
    //            if (bg_custom) bg_custom.style.width = "calc(100% - 80px)";
    //            if (item_custom) item_custom.style.width = "calc(100% - 80px)";
    //            if (right_btn) right_btn.classList.add('active');
    //            content.style.marginLeft = "80px";
    //        }
    //    }
    //    updateLogo();
    //    adjustListHeight();
    //}

    //function updateLayoutOnResize() {
    //    if (window.innerWidth >= 1400) {
    //        sidebar.classList.contains('active')
    //            ? (sidebar.classList.add('active'),
    //                (navbar.style.left = "80px"),
    //                (navbar.style.width = "calc(100% - 80px)"),
    //                (content.style.marginLeft = "80px"))

    //            : (sidebar.classList.remove('active'),
    //                (navbar.style.left = "270px"),
    //                (navbar.style.width = "calc(100% - 270px)"),
    //                (content.style.marginLeft = "270px")

    //            );
    //    } else if (window.innerWidth >= 991) {
    //        sidebar.classList.add('active');
    //        navbar.style.left = "80px";
    //        navbar.style.width = "calc(100% - 80px)";
    //        content.style.marginLeft = "80px";
    //    } else {
    //        sidebar.classList.contains('active')
    //            ? (sidebar.classList.remove('active'),
    //                (navbar.style.left = "0"),
    //                (navbar.style.width = "100%"),
    //                (content.style.marginLeft = "0"))
    //            : (sidebar.classList.add('active'),
    //                (navbar.style.left = "80px"),
    //                (navbar.style.width = "calc(100% - 80px)"),
    //                (content.style.marginLeft = "80px"));
    //    }
    //    updateLogo();

    //}

    // Xử lý sự kiện click vào nút toggle sidebar
    sidebarCollapse.addEventListener('click', function () {
        updateLogo();
        setTimeout(adjustListHeight, 300);

    });


    // Lắng nghe sự kiện thay đổi kích thước màn hình
    // window.addEventListener('resize', updateLayoutOnResize);
    let resizeTimeout;

    window.addEventListener('resize', function () {
        if (resizeTimeout) clearTimeout(resizeTimeout);

        // Chỉ gọi updateLayoutOnResize sau khi resize dừng 200ms
        resizeTimeout = setTimeout(() => {
            //updateLayoutOnResize();
            adjustListHeight();
        }, 120);
    });
});

