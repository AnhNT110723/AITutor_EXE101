(function($) {

	"use strict";

	var fullHeight = function() {

		$('.js-fullheight').css('height', $(window).height());
		$(window).resize(function(){
			$('.js-fullheight').css('height', $(window).height());
		});

	};
	fullHeight();

	$('#sidebarCollapse').on('click', function () {
      $('#sidebar').toggleClass('active');
  });

})(jQuery);



//xử lí sidebar với nave bar
document.addEventListener('DOMContentLoaded', function () {
  const sidebar = document.getElementById('sidebar');
  const navbar = document.querySelector('.navbar');
  const content = document.getElementById('content');
  const sidebarCollapse = document.getElementById('sidebarCollapse');
  const logoImg = document.querySelector('#sidebar .logo img'); // Lấy phần tử ảnh logo

    // Hàm thay đổi logo
	function updateLogo() {
		if (sidebar.classList.contains('active')) {
		  logoImg.src = 'static/images/concoc.png'; // Logo mặc định khi sidebar active
		  logoImg.classList.add('logo-active'); // Thêm class cho trạng thái active
          logoImg.classList.remove('logo-inactive'); // Xóa class cho trạng thái không active
		} else {
		  logoImg.src = 'static/images/FAI.png'; // Logo khi sidebar không active
		  logoImg.classList.add('logo-inactive'); // Thêm class cho trạng thái không active
          logoImg.classList.remove('logo-active'); // Xóa class cho trạng thái active
		}
	  }


  // Hàm cập nhật giao diện khi thay đổi kích thước màn hình
function updateLayoutOnResize() {
    if (window.innerWidth >= 1400) {
        sidebar.classList.contains('active')
            ? (sidebar.classList.remove('active'),
              (navbar.style.left = "270px"),
              (navbar.style.width = "calc(100% - 270px)"),
              (content.style.marginLeft = "270px"))
            : (sidebar.classList.add('active'),
              (navbar.style.left = "80px"),
              (navbar.style.width = "calc(100% - 80px)"),
              (content.style.marginLeft = "80px"));
    } else if (window.innerWidth >= 991) {
        sidebar.classList.add('active');
        navbar.style.left = "80px";
        navbar.style.width = "calc(100% - 80px)";
        content.style.marginLeft = "80px";
    } else {
        sidebar.classList.contains('active')
            ? (sidebar.classList.remove('active'),
              (navbar.style.left = "0"),
              (navbar.style.width = "100%"),
              (content.style.marginLeft = "0"))
            : (sidebar.classList.add('active'),
              (navbar.style.left = "80px"),
              (navbar.style.width = "calc(100% - 80px)"),
              (content.style.marginLeft = "80px"));
    }
    updateLogo();
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
      content.style.marginLeft = isActive ? "80px" : "270px";
    } else {
      navbar.style.left = isActive ? "80px" : "0";
      navbar.style.width = isActive ? "calc(100% - 80px)" : "100%";
      content.style.marginLeft = isActive ? "80px" : "0";
    }
    updateLogo();
  });

  // Lắng nghe sự kiện thay đổi kích thước màn hình
  // window.addEventListener('resize', updateLayoutOnResize);
  let resizeTimeout;

  window.addEventListener('resize', function () {
	if (resizeTimeout) clearTimeout(resizeTimeout);

	// Chỉ gọi updateLayoutOnResize sau khi resize dừng 200ms
	resizeTimeout = setTimeout(() => {
	  updateLayoutOnResize();
	  updateLogo();
	}, 120);
  });

});



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



//Xử lý ảnh trong profile

    document.addEventListener('DOMContentLoaded', () => {
        const defaultAvatars = document.querySelectorAll('.default-avatar img');
        const uploadAvatar = document.getElementById('uploadAvatar');
        const filePreview = document.getElementById('filePreview');
        const saveAvatarBtn = document.getElementById('saveAvatarBtn');
        let selectedAvatar = null;
    
        // Xử lý chọn ảnh mặc định
        defaultAvatars.forEach((img) => {
            img.addEventListener('click', () => {
                // Đặt viền cho ảnh được chọn
                defaultAvatars.forEach((item) => item.parentElement.style.borderColor = '#ddd');
                img.parentElement.style.borderColor = '#007bff';
    
                // Xóa preview nếu có và lưu đường dẫn ảnh được chọn
                filePreview.innerHTML = '<span>Tải ảnh lên từ thiết bị</span>';
                selectedAvatar = img.src;
            });
        });
    
        // Xử lý chọn ảnh từ máy
        uploadAvatar.addEventListener('change', (event) => {
            const file = event.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    filePreview.innerHTML = `<img src="${e.target.result}" alt="Ảnh tải lên">`;
                    selectedAvatar = e.target.result; // Gán ảnh tải lên làm ảnh được chọn
                };
                reader.readAsDataURL(file);
            }
        });
    
        // Lưu ảnh
        saveAvatarBtn.addEventListener('click', () => {
            if (selectedAvatar) {
                document.querySelector('.avatar-change img').src = selectedAvatar; // Thay đổi ảnh đại diện
                $('#avatarModal').modal('hide'); // Ẩn modal
            } else {
                alert('Vui lòng chọn một ảnh đại diện.');
            }
        });
    });
 


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
              searchCourse : 'searchCourse'

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
          fetch('/SRC/static/data/languages.json')
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



  //set uo head phone for each contries

     // Initialize intl-tel-input
     const phoneInput = document.querySelector("#phone");
     const iti = window.intlTelInput(phoneInput, {
         initialCountry: "vn",
         preferredCountries: ["vn", "us", "gb"],
         separateDialCode: true,
         allowDropdown: true, // Bật dropdown
         utilsScript: "static/intl-tel-input/js/utils.js", // Chỉ dùng nếu offline
         
     });


     // Function to handle submit
     function submitPhone() {
         if (iti.isValidNumber()) {
             const fullNumber = iti.getNumber(); // Get the full international number
             document.getElementById("result").innerText = `Số điện thoại đã lưu: ${fullNumber}`;
         } else {
             alert("Số điện thoại không hợp lệ. Vui lòng kiểm tra lại!");
         }
     }
