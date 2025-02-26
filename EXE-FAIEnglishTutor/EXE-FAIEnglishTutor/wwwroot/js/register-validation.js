

function toggleSubmitButton() {

    // Kiểm tra điều kiện checkbox và các trường hợp lệ
    const emailError = document.getElementById("emailError").textContent;
    const phoneError = document.getElementById("phoneError").textContent;
    const passwordError = document.getElementById("passwordError").textContent;
    const confirmPasswordError = document.getElementById("confirmPasswordError").textContent;

    const submitButton = document.getElementById("submitButton");
    if (emailError === "" && phoneError === "" && passwordError === "" && confirmPasswordError === "") {
        submitButton.disabled = false;
    } else {
        submitButton.disabled = true;
    }


}
// Gọi lại `toggleSubmitButton` trong từng hàm kiểm tra
function validateEmail() {
    const email = document.getElementById("email").value;
    const emailError = document.getElementById("emailError");
    const emailInput = document.getElementById("email");
    const emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;

    if (!emailPattern.test(email)) {
        emailError.textContent = "Please enter a valid email address.";
        emailInput.classList.remove("is-valid");
        emailInput.classList.add("is-invalid");
    } else {
        emailError.textContent = ""; // Clear error if no frontend error
        emailInput.classList.remove("is-invalid");
        emailInput.classList.add("is-valid");
    }

    toggleSubmitButton();
}

function validatePhone() {

    const phoneError = document.getElementById("phoneError");
    const phoneInput = document.getElementById("phoneNumber");

    const iti = window.intlTelInputGlobals.getInstance(phoneInput); // Lấy instance của intlTelInput

    // Lấy số điện thoại đã được định dạng theo chuẩn E.164
    const phoneNumber = iti.getNumber();

    if (!phoneNumber || !iti.isValidNumber()) {
        phoneError.textContent = "Invalid phone number format.";
        phoneInput.classList.remove("is-valid");
        phoneInput.classList.add("is-invalid");
    } else {
        phoneError.textContent = ""; // Clear error if no frontend error
        phoneInput.classList.remove("is-invalid");
        phoneInput.classList.add("is-valid");
        const countryData = iti.getSelectedCountryData();
        console.log("Country Code:", countryData.dialCode); // Mã quốc gia (ví dụ: +84)
        console.log("Country Name:", countryData.name); // Tên quốc gia (ví dụ: Vietnam)
        console.log("Formatted Number:", phoneNumber); // Số điện thoại đã định dạng
    }
    // Lấy thông tin quốc gia đã chọn

    toggleSubmitButton();
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

    toggleSubmitButton();
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

    toggleSubmitButton();
}