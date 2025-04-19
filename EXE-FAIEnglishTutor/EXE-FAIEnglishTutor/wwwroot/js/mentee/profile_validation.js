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

function validateOldPassword() {
    const password = document.getElementById("OldPassword").value;
    const passwordError = document.getElementById("OldPasswordError");
    const passwordInput = document.getElementById("OldPassword");
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
