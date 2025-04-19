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

// Gắn sự kiện oninput để kiểm tra số điện thoại
phoneInput.addEventListener("input", function () {
    hasUserInput = true; // Đánh dấu đã có tương tác
    validatePhone1();
});

// Reset validation state khi load trang
phoneInput.classList.remove("is-valid", "is-invalid");



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







