// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showLoader() {
    $('#loader').css('display', 'flex');
}
function hideLoader() {
    $('#loader').css('display', 'none');
}
function showToast(type, message) {
    $.toast({
        heading: type,
        text: message,
        icon: type
    });
}
function closePopup(id) {
    $('#' + id).modal('hide');
}
function togglePasswordVisibility(icon, inputId) {
    var passwordInput = document.getElementById(inputId);
    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        icon.querySelector('i').classList.remove('fa-eye-slash');
        icon.querySelector('i').classList.add('fa-eye');
    } else {
        passwordInput.type = "password";
        icon.querySelector('i').classList.remove('fa-eye');
        icon.querySelector('i').classList.add('fa-eye-slash');
    }
}
function openChatBox() {
    document.getElementById("chat-box").style.display = "block";
}

function closeChatBox() {
    document.getElementById("chat-box").style.display = "none";
}
function ConnectUser() {
    const connection = new signalR.HubConnectionBuilder().withUrl("/presence").build();
    connection.start()
        .then(() => {
            console.log("SignalR connection established.");
            // Connection is successfully established, you can now send messages
        })
        .catch((error) => {
            console.error("Error establishing SignalR connection:", error);
        });

    connection.on("UserIsOnline", function (name) {
        console.log("site.js" + name);
    });
}
function getCitiesByCountryId(id) {
    showLoader();
    $.ajax({
        url: '/admin/geo/GetCitiesById?id=' + id,
        type: 'GET',
        dataType: "json",
        success: function (response) {
         
            const dropdown = document.getElementById('ddlcity');
            dropdown.innerHTML = '<option value="">Select a city</option>';
            response.forEach(city => {
                const option = `<option value="${city.id}">${city.name}</option>`;
                dropdown.innerHTML += option;
            });
        },
        error: function (xhr, status, error) {
            console.error("Error uploading image: " + error);
        }, complete: function () {
            hideLoader();
        }
    });
}
$(document).ready(function () {
    $('.js-example-basic-single').select2();
});