(function ($) {

    "use strict";

    var fullHeight = function () {

        $('.js-fullheight').css('height', $(window).height());
        $(window).resize(function () {
            $('.js-fullheight').css('height', $(window).height());
        });

    };
    fullHeight();

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });

})(jQuery);

// popups

document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('registerModal');
    const overlay = document.getElementById('overlay');

    modal.addEventListener('show.bs.modal', function () {
        overlay.style.display = 'block'; // Hiển thị overlay
    });

    modal.addEventListener('hidden.bs.modal', function () {
        overlay.style.display = 'none'; // Ẩn overlay
    });
});



////Xử lý ảnh trong profile

//document.addEventListener('DOMContentLoaded', () => {
//    const defaultAvatars = document.querySelectorAll('.default-avatar img');
//    const uploadAvatar = document.getElementById('uploadAvatar');
//    const filePreview = document.getElementById('filePreview');
//    const saveAvatarBtn = document.getElementById('saveAvatarBtn');
//    let selectedAvatar = null;

//    // Xử lý chọn ảnh mặc định
//    if (defaultAvatars && defaultAvatars.length > 0) {
//        defaultAvatars.forEach((img) => {
//            img.addEventListener('click', () => {
//                // Đặt viền cho ảnh được chọn
//                defaultAvatars.forEach((item) => item.parentElement.style.borderColor = '#ddd');
//                img.parentElement.style.borderColor = '#007bff';

//                // Xóa preview nếu có và lưu đường dẫn ảnh được chọn
//                if (filePreview) {
//                    filePreview.innerHTML = '<span>Tải ảnh lên từ thiết bị</span>';
//                }
//                selectedAvatar = img.src;
//            });
//        });
//    }

//    // Xử lý chọn ảnh từ máy
//    if (uploadAvatar && filePreview) {
//        uploadAvatar.addEventListener('change', (event) => {
//            const file = event.target.files[0];
//            if (file) {
//                const reader = new FileReader();
//                reader.onload = function (e) {
//                    filePreview.innerHTML = `<img src="${e.target.result}" alt="Ảnh tải lên">`;
//                    selectedAvatar = e.target.result;
//                };
//                reader.readAsDataURL(file);
//            }
//        });
//    }

//    // Lưu ảnh
//    if (saveAvatarBtn) {
//        saveAvatarBtn.addEventListener('click', () => {
//            if (selectedAvatar) {
//                const avatarChange = document.querySelector('.avatar-change img');
//                if (avatarChange) {
//                    avatarChange.src = selectedAvatar;
//                }
//                const avatarModal = document.getElementById('avatarModal');
//                if (avatarModal) {
//                    const modal = bootstrap.Modal.getInstance(avatarModal);
//                    if (modal) {
//                        modal.hide();
//                    }
//                }
//            } else {
//                alert('Vui lòng chọn một ảnh đại diện.');
//            }
//        });
//    }
//});



// Đổi ngôn ngữ
document.addEventListener('DOMContentLoaded', () => {
    const languageDropdown = document.getElementById('languageDropdown');
    const languageItems = document.querySelectorAll('.dropdown-item');

    // Cập nhật nội dung trên trang
    const updatePageContent = (translations) => {
        const elementsToUpdate = {
            title: 'title',
            accountRe: 'accountRe',
            navHome: 'navHome',
            navAbout: 'navAbout',
            navPortfolio: 'navPortfolio',
            profileOption: 'profileOption',
            logoutOption: 'logoutOption',
            footerText: 'footerText',
            titleModalImg: 'titleModalImg',
            avatarModalLabel: 'avatarModalLabel',
            inputName: 'inputName',
            inputDate: 'inputDate',
            inputPhone: 'inputPhone',
            inputEmail: 'inputEmail',
            inputPassword: 'inputPassword',
            inputRepass: 'inputRepass',
            closeBtn: 'closeBtn',
            saveBtn: 'saveAvatarBtn',
            live: 'live',
            AI: 'AI',
            self: 'self',
            parentPage: 'parentPage',
            groupTalk: 'groupTalk',
            inputAddress: 'inputAddress',
            inputOldpass: 'inputOldpass',
            btnSaveChange: 'btnSaveChange',
            viewProfile: 'viewProfile',
            changePassword: 'changePassword',
            searchCourse: 'searchCourse',
            titleListCourse: 'titleListCourse'

        };

        Object.keys(elementsToUpdate).forEach((key) => {
            const element = document.getElementById(elementsToUpdate[key]);
            if (element) {
                element.textContent = translations[key] || element.textContent;
            } else {
                console.warn(`Element with ID '${elementsToUpdate[key]}' not found.`);
            }
        });
    };

    // Thay đổi ngôn ngữ
    const changeLanguage = (lang) => {
        localStorage.setItem('language', lang); // Lưu vào localStorage
        fetch('../data/languages.json')
            .then((res) => res.json())
            .then((data) => {
                if (data[lang]) {
                    updatePageContent(data[lang]);
                } else {
                    console.error(`No translations available for language: ${lang}`);
                }
            })
            .catch((err) => console.error('Error fetching language file:', err));
    };

    // Đặt ngôn ngữ mặc định
    const savedLanguage = localStorage.getItem('language') || 'en';
    changeLanguage(savedLanguage);

    // Lắng nghe sự kiện thay đổi ngôn ngữ
    languageItems.forEach((item) => {
        item.addEventListener('click', (e) => {
            const lang = e.target.getAttribute('data-lang');
            changeLanguage(lang);

            // Cập nhật dropdown
            const flagSrc = e.target.querySelector('img').src;
            const langText = e.target.textContent.trim();
            languageDropdown.innerHTML = `<img src="${flagSrc}" class="flag-icon" /> ${langText}`;
        });
    });
});



////set uo head phone for each contries

//// Initialize intl-tel-input
//const phoneInput = document.querySelector("#phone");
//const iti = window.intlTelInput(phoneInput, {
//    initialCountry: "vn",
//    preferredCountries: ["vn", "us", "gb"],
//    separateDialCode: true,
//    allowDropdown: true, // Bật dropdown
//    utilsScript: "static/intl-tel-input/js/utils.js", // Chỉ dùng nếu offline

//});


//// Function to handle submit
//function submitPhone() {
//    if (iti.isValidNumber()) {
//        const fullNumber = iti.getNumber(); // Get the full international number
//        document.getElementById("result").innerText = `Số điện thoại đã lưu: ${fullNumber}`;
//    } else {
//        alert("Số điện thoại không hợp lệ. Vui lòng kiểm tra lại!");
//    }
//}

