
document.addEventListener('DOMContentLoaded', () => {
    const avatarEditBtn = document.getElementById('avatarEditBtn');
    const avatarModal = document.getElementById('avatarModal');


    // Hiển thị popup
    avatarEditBtn.addEventListener('click', () => {
        $(avatarModal).modal('show'); // Sử dụng Bootstrap modal
    });


});

let iti;




const phoneInput = document.querySelector("#phoneNumber");
const countryInput = document.querySelector("#countryCode");
let hasUserInput = false; // Thêm biến để track user input

// Khởi tạo intlTelInput với CDN utils
iti = intlTelInput(phoneInput, {
    initialCountry: "vn",
    preferredCountries: ["vn", "us", "gb"],
    separateDialCode: true,
    allowDropdown: true,
    utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/21.0.2/js/utils.js"
});

// Khi trang load, cập nhật hidden input với CountryCode ban đầu
const initialCountryData = iti.getSelectedCountryData();
if (countryInput) {
    countryInput.value = initialCountryData.iso2.toUpperCase();
}

// Lắng nghe sự kiện thay đổi quốc gia
phoneInput.addEventListener("countrychange", function () {
    const countryData = iti.getSelectedCountryData();
    if (countryInput) {
        countryInput.value = countryData.iso2.toUpperCase();
    }
    validatePhone1();

});

// Set lại quốc gia theo ViewBag
const regionCode = "@ViewBag.RegionCode";
if (regionCode) {
    iti.setCountry(regionCode);
}

// Gắn sự kiện oninput để kiểm tra số điện thoại
phoneInput.addEventListener("input", function () {
    hasUserInput = true; // Đánh dấu đã có tương tác
    validatePhone1();
});

// Reset validation state khi load trang
phoneInput.classList.remove("is-valid", "is-invalid");

// Hàm kiểm tra tính hợp lệ số điện thoại
function validatePhone1() {
    const phoneError = document.getElementById("phoneError");
    const phoneInput = document.getElementById("phoneNumber");

    if (!iti) {
        console.error("intlTelInput chưa được khởi tạo!");
        return;
    }

    const phoneNumber = iti.getNumber();

    if (!phoneNumber || !iti.isValidNumber()) {
        phoneError.textContent = "Số điện thoại không hợp lệ";
        phoneInput.classList.remove("is-valid");
        phoneInput.classList.add("is-invalid");
    } else {
        phoneError.textContent = "";
        phoneInput.classList.remove("is-invalid");
        phoneInput.classList.add("is-valid");
    }
}

// Validate Date
function validateDate() {
    const dateInput = document.getElementById('birthYear');
    const dateError = document.getElementById('dateError');
    const selectedDate = new Date(dateInput.value);
    const today = new Date();

    // Tính tuổi
    const age = today.getFullYear() - selectedDate.getFullYear();
    const monthDiff = today.getMonth() - selectedDate.getMonth();
    const dayDiff = today.getDate() - selectedDate.getDate();
    const finalAge = monthDiff < 0 || (monthDiff === 0 && dayDiff < 0) ? age - 1 : age;

    // Reset classes
    dateInput.classList.remove('is-valid', 'is-invalid');
    dateError.textContent = '';

    // Kiểm tra ngày hợp lệ
    if (!dateInput.value) {
        dateError.textContent = 'Vui lòng chọn ngày sinh';
        dateInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra ngày trong tương lai
    if (selectedDate > today) {
        dateError.textContent = 'Ngày sinh không thể trong tương lai';
        dateInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra tuổi tối thiểu (18 tuổi)
    if (finalAge < 5) {
        dateError.textContent = 'Bạn phải đủ 5 tuổi';
        dateInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra tuổi tối đa (100 tuổi)
    if (finalAge > 100) {
        dateError.textContent = 'Ngày sinh không hợp lệ';
        dateInput.classList.add('is-invalid');
        return false;
    }

    // Nếu mọi thứ OK
    dateInput.classList.add('is-valid');
    return true;
}


function validatePassword() {
    const password = document.getElementById("password").value;
    const passwordError = document.getElementById("passwordError");
    const passwordInput = document.getElementById("password");
    const regex = /^(?=.*[0-9])(?=.*[!@#$%^&*(){}\[\]_\-+=~`|:;\"'<>,.\/?])(?=.*[a-z])(?=.*[A-Z]).{8,}$/;

    if (!regex.test(password)) {
        passwordError.textContent = "Password must have (A-Z), (a-z), (0-9), a special char, and be 8+ chars.";
        passwordInput.classList.remove("is-valid");
        passwordInput.classList.add("is-invalid");
    } else {
        passwordError.textContent = "";
        passwordInput.classList.remove("is-invalid");
        passwordInput.classList.add("is-valid");
    }
}

function validateConfirmPassword() {
    const password = document.getElementById("password").value;
    const confirmPassword = document.getElementById("re_pass").value;

    const confirmPasswordError = document.getElementById("confirmPasswordError");
    const confirmPasswordInput = document.getElementById("re_pass");
    const regex = /^(?=.*[0-9])(?=.*[!@#$%^&*(){}\[\]_\-+=~`|:;\"'<>,.\/?])(?=.*[a-z])(?=.*[A-Z]).{8,}$/;

    if (!regex.test(confirmPassword)) {
        confirmPasswordError.textContent = "Password must have (A-Z), (a-z), (0-9), a special char, and be 8+ chars.";
        confirmPasswordInput.classList.remove("is-valid");
        confirmPasswordInput.classList.add("is-invalid");
    } else if (confirmPassword !== password) {
        confirmPasswordError.textContent = "Passwords do not match.";
        confirmPasswordInput.classList.remove("is-valid");
        confirmPasswordInput.classList.add("is-invalid");
    } else {
        confirmPasswordError.textContent = "";
        confirmPasswordInput.classList.remove("is-invalid");
        confirmPasswordInput.classList.add("is-valid");
    }
}

// fetch("https:provinces.open-api.vn/api/p/")
//     .then(res => res.json())
//     .then(data => {
//         const select = document.getElementById('provinceSelect');
//         data.forEach(province => {
//             const option = document.createElement('option');
//             option.value = province.code;
//             option.textContent = province.name;
//             select.appendChild(option);
//         });
//     });
// Sidebar toggle functionality
$(document).ready(function () {

    // Handle mobile menu toggle
    $('.btn-dark.d-inline-block.d-lg-none.ml-auto').on('click', function () {
        var navbar = $('#navbarSupportedContent');
        if (navbar.hasClass('show')) {
            navbar.removeClass('show');
        } else {
            navbar.addClass('show');
        }
    });

});


document.addEventListener('DOMContentLoaded', function () {
    const select = document.getElementById('provinceSelect');
    const modelProvinceValue = parseInt("@Model.Province", 10);
    console.log("Contry code: ", modelProvinceValue);
    // Function to apply initial styles
    function applyInitialStyles() {
        const selectedOption = select.options[select.selectedIndex];
        if (selectedOption && selectedOption.value) {
            selectedOption.style.backgroundColor = '#ff8c00';
            selectedOption.style.color = 'white';
        }
    }

    // Fetch provinces and set initial value
    fetch("https:provinces.open-api.vn/api/p/")
        .then(res => res.json())
        .then(data => {
            data.forEach(province => {
                const option = document.createElement('option');
                option.value = province.code;
                option.textContent = province.name;
                // Set selected if matches model value
                if (province.code === modelProvinceValue) {
                    option.selected = true;
                }
                select.appendChild(option);
            });
            // Apply styles after options are added and selection is set
            applyInitialStyles();
        });

    // Add animation when opening the dropdown
    select.addEventListener('mousedown', function () {
        if (this.size === 1) {
            this.size = 10;
            this.style.position = 'absolute';
            this.style.zIndex = 1000;
        }
    });

    // Close dropdown when clicking outside
    document.addEventListener('click', function (e) {
        if (!select.contains(e.target)) {
            select.size = 1;
            select.style.position = 'relative';
        }
    });

    // Handle option selection
    select.addEventListener('change', function () {
        this.size = 1;
        this.style.position = 'relative';

        // Reset all options
        Array.from(this.options).forEach(opt => {
            opt.style.backgroundColor = '';
            opt.style.color = '';
        });

        // Style selected option
        const selectedOption = this.options[this.selectedIndex];
        if (selectedOption) {
            selectedOption.style.backgroundColor = '#ff8c00';
            selectedOption.style.color = 'white';
        }
    });

    // Prevent immediate closing when clicking on select
    select.addEventListener('click', function (e) {
        if (this.size === 1) {
            e.preventDefault();
        }
    });

    // Style options on hover
    select.addEventListener('mouseover', function (e) {
        if (e.target.tagName === 'OPTION' && !e.target.selected) {
            e.target.style.backgroundColor = '#ff8c00';
            e.target.style.color = 'white';
        }
    });

    // Reset hover styles
    select.addEventListener('mouseout', function (e) {
        if (e.target.tagName === 'OPTION' && !e.target.selected) {
            e.target.style.backgroundColor = '';
            e.target.style.color = '';
        }
    });
});


//Xử lý ảnh trong profile
// Xử lý ảnh trong profile
document.addEventListener('DOMContentLoaded', () => {
    const avatarEditBtn = document.getElementById('avatarEditBtn');
    const avatarModal = document.getElementById('avatarModal');
    const uploadAvatar = document.getElementById('uploadAvatar');
    const hiddenAvatar = document.getElementById('hiddenAvatar');
    const hiddenAvatarStr = document.getElementById('hiddenAvatarStr');
    const filePreview = document.getElementById('filePreview');
    const titleModalImg = document.getElementById('titleModalImg');
    const avatarPreview = document.getElementById('avatarPreview');
    const saveAvatarBtn = document.getElementById('saveAvatarBtn');
    const closeBtn = document.getElementById('closeBtn');
    const avatarDisplay = document.getElementById('avatarDisplay');
    const defaultAvatars = document.querySelectorAll('.avatar-option');
    let selectedAvatar = null;
    let isSaved = false; // Biến để kiểm tra trạng thái lưu

    // Hàm xóa dấu ~ khỏi đường dẫn
    const normalizePath = (path) => {
        if (path && path.startsWith('~')) {
            return path.substring(1);
        }
        return path;
    };

    // Debug: Kiểm tra các phần tử
    console.log('avatarEditBtn:', avatarEditBtn);
    console.log('avatarModal:', avatarModal);
    console.log('uploadAvatar:', uploadAvatar);
    console.log('hiddenAvatar:', hiddenAvatar);
    console.log('hiddenAvatarStr:', hiddenAvatarStr);
    console.log('saveAvatarBtn:', saveAvatarBtn);

    // Hiển thị modal
    if (avatarEditBtn && avatarModal) {
        avatarEditBtn.addEventListener('click', () => {
            console.log('Avatar edit button clicked');
            isSaved = false; // Reset trạng thái khi mở modal
            $(avatarModal).modal('show');
        });
    }

    // Xử lý chọn ảnh mặc định
    if (defaultAvatars && defaultAvatars.length > 0) {
        defaultAvatars.forEach((img) => {
            img.addEventListener('click', () => {
                console.log('Default avatar selected:', img.dataset.url);
                // Đặt viền cho ảnh được chọn
                defaultAvatars.forEach((item) => item.parentElement.style.border = '1px solid #ddd');
                img.parentElement.style.border = '2px solid #007bff';

                // Cập nhật preview
                avatarPreview.src = img.src;
                avatarPreview.style.display = 'block';
                titleModalImg.style.display = 'none';

                // Lưu đường dẫn (xóa ~) và xóa file
                selectedAvatar = normalizePath(img.dataset.url);
                console.log('Normalized path:', selectedAvatar);
                uploadAvatar.value = '';
                hiddenAvatar.value = '';
                hiddenAvatarStr.value = selectedAvatar;
                console.log('hiddenAvatarStr set to:', hiddenAvatarStr.value);
            });
        });
    }

    // Xử lý chọn ảnh từ máy
    if (uploadAvatar && filePreview && avatarPreview) {
        uploadAvatar.addEventListener('change', (event) => {
            const file = event.target.files[0];
            console.log('File selected:', file ? file.name : 'No file');
            if (file) {
                // Kiểm tra định dạng và kích thước
                const validTypes = ['image/jpeg', 'image/png', 'image/gif'];
                if (!validTypes.includes(file.type)) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Vui lòng chọn file ảnh (JPEG, PNG, GIF)!',
                    });
                    uploadAvatar.value = '';
                    return;
                }
                if (file.size > 5 * 1024 * 1024) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Kích thước ảnh không được vượt quá 5MB!',
                    });
                    uploadAvatar.value = '';
                    return;
                }

                const reader = new FileReader();
                reader.onload = function (e) {
                    avatarPreview.src = e.target.result;
                    avatarPreview.style.display = 'block';
                    titleModalImg.style.display = 'none';
                    selectedAvatar = e.target.result;

                    // Chuyển file sang hiddenAvatar
                    const dataTransfer = new DataTransfer();
                    dataTransfer.items.add(file);
                    hiddenAvatar.files = dataTransfer.files;
                    hiddenAvatarStr.value = ''; // Xóa AvatarStr
                    console.log('File copied to hiddenAvatar:', hiddenAvatar.files[0]?.name);
                    console.log('hiddenAvatarStr cleared:', hiddenAvatarStr.value);
                };
                reader.readAsDataURL(file);

                // Xóa viền ảnh mặc định
                defaultAvatars.forEach((item) => item.parentElement.style.border = '1px solid #ddd');
            }
        });
    }

    // Lưu ảnh
    if (saveAvatarBtn) {
        saveAvatarBtn.addEventListener('click', () => {
            console.log('Save avatar button clicked, selectedAvatar:', selectedAvatar);
            if (selectedAvatar) {
                // Cập nhật ảnh hiển thị
                if (avatarDisplay) {
                    avatarDisplay.src = normalizePath(selectedAvatar);
                    console.log('avatarDisplay updated to:', avatarDisplay.src);
                }
                // Đánh dấu đã lưu
                isSaved = true;
                // Đóng modal
                const modal = bootstrap.Modal.getInstance(avatarModal);
                if (modal) {
                    modal.hide();
                }
            } else {
                Swal.fire({
                    icon: 'warning',
                    title: 'Chưa chọn ảnh',
                    text: 'Vui lòng chọn một ảnh mặc định hoặc tải lên ảnh mới!',
                });
            }
        });
    }

    // Reset khi đóng modal
    if (avatarModal) {
        avatarModal.addEventListener('hidden.bs.modal', () => {
            console.log('Modal closed, isSaved:', isSaved);
            if (!isSaved) {
                // Chỉ reset nếu chưa nhấn "Lưu"
                console.log('Resetting...');
                avatarPreview.src = '';
                avatarPreview.style.display = 'none';
                titleModalImg.style.display = 'block';
                uploadAvatar.value = '';
                hiddenAvatar.value = '';
                hiddenAvatarStr.value = '';
                selectedAvatar = null;
                defaultAvatars.forEach((item) => item.parentElement.style.border = '1px solid #ddd');
                console.log('Reset complete: hiddenAvatar:', hiddenAvatar.files.length, 'hiddenAvatarStr:', hiddenAvatarStr.value);
            }
        });
    }

    // Debug form submit
    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', (e) => {
            console.log('Form submitting...');
            console.log('hiddenAvatar files:', hiddenAvatar.files.length, hiddenAvatar.files[0]?.name);
            console.log('hiddenAvatarStr:', hiddenAvatarStr.value);
        });
    }
});


