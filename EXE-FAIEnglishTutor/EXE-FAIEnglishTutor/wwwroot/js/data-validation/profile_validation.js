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
        dateError.textContent = document.getElementById('dateErrorEmpty').textContent;
        dateInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra ngày trong tương lai
    if (selectedDate > today) {
        dateError.textContent = document.getElementById('dateErrorFuture').textContent;
        dateInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra tuổi tối thiểu (5 tuổi)
    if (finalAge < 5) {
        dateError.textContent = document.getElementById('dateErrorMinAge').textContent;
        dateInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra tuổi tối đa (100 tuổi)
    if (finalAge > 100) {
        dateError.textContent = document.getElementById('dateErrorMaxAge').textContent;
        dateInput.classList.add('is-invalid');
        return false;
    }

    // Nếu mọi thứ OK
    dateInput.classList.add('is-valid');
    return true;
}

// Validate Phone
function validatePhone1() {
    const phoneError = document.getElementById("phoneError");
    const phoneInput = document.getElementById("phoneNumber");

    if (!iti) {
        console.error("intlTelInput chưa được khởi tạo!");
        return;
    }

    const phoneNumber = iti.getNumber();

    if (!phoneNumber || !iti.isValidNumber()) {
        phoneError.textContent = document.getElementById('phoneErrorInvalid').textContent;
        phoneInput.classList.remove("is-valid");
        phoneInput.classList.add("is-invalid");
    } else {
        phoneError.textContent = "";
        phoneInput.classList.remove("is-invalid");
        phoneInput.classList.add("is-valid");
    }
}

// Validate Password
function validatePassword() {
    const passwordInput = document.getElementById('password');
    const passwordError = document.getElementById('passwordError');
    const password = passwordInput.value;

    // Reset classes
    passwordInput.classList.remove('is-valid', 'is-invalid');
    passwordError.textContent = '';

    // Kiểm tra mật khẩu trống
    if (!password) {
        passwordError.textContent = document.getElementById('passwordErrorEmpty').textContent;
        passwordInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra độ dài tối thiểu
    if (password.length < 6) {
        passwordError.textContent = document.getElementById('passwordErrorMinLength').textContent;
        passwordInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra độ dài tối đa
    if (password.length > 20) {
        passwordError.textContent = document.getElementById('passwordErrorMaxLength').textContent;
        passwordInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra ký tự đặc biệt
    if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
        passwordError.textContent = document.getElementById('passwordErrorSpecialChar').textContent;
        passwordInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra số
    if (!/\d/.test(password)) {
        passwordError.textContent = document.getElementById('passwordErrorNumber').textContent;
        passwordInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra chữ in hoa
    if (!/[A-Z]/.test(password)) {
        passwordError.textContent = document.getElementById('passwordErrorUppercase').textContent;
        passwordInput.classList.add('is-invalid');
        return false;
    }

    // Kiểm tra chữ thường
    if (!/[a-z]/.test(password)) {
        passwordError.textContent = document.getElementById('passwordErrorLowercase').textContent;
        passwordInput.classList.add('is-invalid');
        return false;
    }

    // Nếu mọi thứ OK
    passwordInput.classList.add('is-valid');
    return true;
}

// Validate Confirm Password
function validateConfirmPassword() {
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('re_pass');
    const confirmPasswordError = document.getElementById('confirmPasswordError');

    // Reset classes
    confirmPasswordInput.classList.remove('is-valid', 'is-invalid');
    confirmPasswordError.textContent = '';

    if (confirmPasswordInput.value !== passwordInput.value) {
        confirmPasswordError.textContent = document.getElementById('confirmPasswordError').textContent;
        confirmPasswordInput.classList.add('is-invalid');
        return false;
    }

    confirmPasswordInput.classList.add('is-valid');
    return true;
}

