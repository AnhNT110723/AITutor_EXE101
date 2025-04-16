
//xử lí sidebar với nave bar và popup

window.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const navbar = document.querySelector('.navbar');
    const bg_custom = document.querySelector('.bg-custom');
    const item_custom = document.querySelector('.item-custom');
    const right_btn = document.querySelector('.right-btn');
    const content = document.getElementById('content');
    const sidebarCollapse = document.getElementById('sidebarCollapse');
    const logoImg = document.querySelector('#sidebar .logo img'); // Lấy phần tử ảnh logo
    var video = document.querySelector('video');
    var list = document.querySelector('.overflow');
    var list_auto = document.querySelector('.overflow-auto');


    // Đảm bảo video đã được tải và chiều cao được xác định
    if (video != null) {
        video.addEventListener('loadedmetadata', function () {
            adjustListHeight();
        });
    }


    function adjustListHeight() {
        if (!video || !list || !list_auto) return;
        // Kiểm tra chiều cao của video
        var videoHeight = video.offsetHeight;

        // Kiểm tra nếu video có chiều cao hợp lệ
        if (videoHeight > 0) {
            if (list != null) {
                list.style.height = videoHeight + 'px';
            }
            if (list_auto != null) {
                list_auto.style.height = videoHeight + 'px';

            }
        } else {
            // Nếu chiều cao video không hợp lệ, đặt lại chiều cao của danh sách
            setTimeout(adjustListHeight, 100);  // Thử lại sau một thời gian ngắn
        }
    }

    // Hàm thay đổi logo
    function updateLogo() {
        if (sidebar.classList.contains('active')) {
            logoImg.src = 'Images/concoc.png'; // Logo mặc định khi sidebar active
            logoImg.classList.add('logo-active'); // Thêm class cho trạng thái active
            logoImg.classList.remove('logo-inactive'); // Xóa class cho trạng thái không active
        } else {
            logoImg.src = 'Images/FAI-1.png'; // Logo khi sidebar không active
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

    function updateLayoutOnResize() {
        if (window.innerWidth >= 1400) {
            sidebar.classList.contains('active')
                ? (sidebar.classList.add('active'),
                    (navbar.style.left = "80px"),
                    (navbar.style.width = "calc(100% - 80px)"),
                    (bg_custom.style.width = "calc(100% - 80px)"),
                    (item_custom.style.width = "calc(100% - 80px)"),
                    (right_btn.classList.add('active')),
                    (content.style.marginLeft = "80px"))

                : (sidebar.classList.remove('active'),
                    (navbar.style.left = "270px"),
                    (navbar.style.width = "calc(100% - 270px)"),
                    (bg_custom.style.width = "calc(100% - 270px)"),
                    (item_custom.style.width = "calc(100% - 270px)"),
                    (right_btn.classList.remove('active')),
                    (content.style.marginLeft = "270px")

                );
        } else if (window.innerWidth >= 991) {
            sidebar.classList.add('active');
            navbar.style.left = "80px";
            navbar.style.width = "calc(100% - 80px)";
            bg_custom.style.width = "calc(100% - 80px)";
            item_custom.style.width = "calc(100% - 80px)";
            right_btn.classList.add('active');
            content.style.marginLeft = "80px";
        } else {
            sidebar.classList.contains('active')
                ? (sidebar.classList.remove('active'),
                    (navbar.style.left = "0"),
                    (navbar.style.width = "100%"),
                    (bg_custom.style.width = "100%"),
                    (item_custom.style.width = "100%"),
                    (right_btn.classList.remove('active')),
                    (content.style.marginLeft = "0"))
                : (sidebar.classList.add('active'),
                    (navbar.style.left = "80px"),
                    (navbar.style.width = "calc(100% - 80px)"),
                    (bg_custom.style.width = "calc(100% - 80px)"),
                    (item_custom.style.width = "calc(100% - 80px)"),
                    (right_btn.classList.add('active')),
                    (content.style.marginLeft = "80px"));
        }
        updateLogo();
        adjustListHeight();
    }

    // Xử lý sự kiện click vào nút toggle sidebar
    sidebarCollapse.addEventListener('click', function () {
        sidebar.classList.toggle('active');

        sidebar.classList.toggle('active');
        const isActive = sidebar.classList.contains('active');
        const width = window.innerWidth;

        if (width >= 991) {
            navbar.style.left = isActive ? "80px" : "270px";
            navbar.style.width = isActive ? "calc(100% - 80px)" : "calc(100% - 270px)";
            if (bg_custom) bg_custom.style.width = isActive ? "calc(100% - 80px)" : "calc(100% - 270px)";
            if (item_custom) item_custom.style.width = isActive ? "calc(100% - 80px)" : "calc(100% - 270px)";
            if (right_btn) isActive ? (right_btn.classList.add('act')) : (right_btn.classList.remove('act')),
                content.style.marginLeft = isActive ? "80px" : "270px";
        } else {
            navbar.style.left = isActive ? "80px" : "0";
            navbar.style.width = isActive ? "calc(100% - 80px)" : "100%";
            if (bg_custom) bg_custom.style.width = isActive ? "calc(100% - 80px)" : "100%";
            if (item_custom) item_custom.style.width = isActive ? "calc(100% - 80px)" : "100%";
            if (right_btn) isActive ? (right_btn.classList.add('act')) : (right_btn.classList.remove('act')),
                content.style.marginLeft = isActive ? "80px" : "0";

        }
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
            updateLayoutOnResize();
            adjustListHeight();
        }, 120);
    });

    // Điều chỉnh lại chiều cao khi cửa sổ thay đổi kích thước
    window.addEventListener('resize', adjustListHeight);
    adjustListHeight();
});

