/*=============== SHOW MENU ===============*/
const navMenu = document.getElementById('nav-menu'),
      navToggle = document.getElementById('nav-toggle'),
      navClose = document.getElementById('nav-close');

/* Menu show */
navToggle.addEventListener('click', () =>{
   navMenu.classList.add('show-menu');
});

/* Menu hidden */
navClose.addEventListener('click', () =>{
   navMenu.classList.remove('show-menu');
});

/*=============== SEARCH ===============*/
const search = document.getElementById('search'),
      searchBtn = document.getElementById('search-btn'),
      searchClose = document.getElementById('search-close');

/* Search show */
searchBtn.addEventListener('click', () =>{
   search.classList.add('show-search');
});

/* Search hidden */
searchClose.addEventListener('click', () =>{
   search.classList.remove('show-search');
});

/*=============== LOGIN ===============*/
const login = document.getElementById('login'),
      loginBtn = document.getElementById('login-btn'),
      loginClose = document.getElementById('login-close');

/* Login show */
loginBtn.addEventListener('click', () =>{
   login.classList.add('show-login');
});

/* Login hidden */
loginClose.addEventListener('click', () =>{
   login.classList.remove('show-login');
});

const togglePassword = document.getElementById('togglePassword'),
      passwordInput = document.getElementById('password');

/* Toggle password visibility */
togglePassword.addEventListener('click', () => {
   // Check if the password is currently hidden
   const isPasswordHidden = passwordInput.getAttribute('type') === 'password';

   // Toggle the password input type
   passwordInput.setAttribute('type', isPasswordHidden ? 'text' : 'password');

   // Update the eye icon accordingly
   if (isPasswordHidden) {
       // Password is now visible, show 'eye-slash' icon
       togglePassword.classList.remove('fa-eye');
       togglePassword.classList.add('fa-eye-slash');
   } else {
       // Password is now hidden, show 'eye' icon
       togglePassword.classList.remove('fa-eye-slash');
       togglePassword.classList.add('fa-eye');
   }
});

