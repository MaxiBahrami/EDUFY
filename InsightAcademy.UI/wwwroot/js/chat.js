"use strict"

//$(document).ready(function () {
//    $('#description').on('input', function () {
//        var message = $(this).val().trim();
//        var wordCount = message.split(/\s+/).filter(function (word) {
//            return word.length > 0;
//        }).length;

//        if (wordCount > 10) {
//            $('#btnSendMessage').prop('disabled', false);
//        } else {
//            $('#btnSendMessage').prop('disabled', true);
//        }
//    });
//});
function sendMessage() {
    showLoader();
    let tutorId = $('#chatPopup').data('TutorId');
    let message = $('#description').val()

    let chat = {};
    chat.Message = message;
    chat.RecipientId = tutorId;

    $.ajax({
        url: '/Chat/SendMessage',
        type: 'POST',
        data: chat,
        contentype: "json",
        success: function (response) {
            if (response.status == true) {
                showToast('success', 'message sent');
                $('#chatPopup').modal('hide');
            }
            else {
                window.location.href = '/identity/account/login';
            }
        },
        error: function (xhr, status, error) {
            if (xhr.status === 401) {
                // Redirect to login page
                window.location.href = '/identity/account/login';
            } else {
                console.error("Error uploading image: " + error);
            }
        },
        complete: function () {
            hideLoader();
        }
    });
}

function openChatPopup(id, image, name) {
    alert('ok')
    $('#description').val('');
    $('#description').val('Hi ' + name + "!");

    $('#chatPopup').modal('show');
    $('#chatPopup').data('TutorId', id);

    $("#tutor-image").prop('src', "/" + image)
    $("#userName").text("Contact " + name)
}