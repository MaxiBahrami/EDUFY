"use strict"
function addToLike(tutorId) {
    $.ajax({
        url: '/Like/addlike',
        type: 'POST',
        data: { tutorId: tutorId },
        contentype: "json",
        success: function (response) {
            if (response.status == true) {
                window.location.reload();
            }
            else {
                showToast('error', response.message)
            }
        },
        error: function (xhr, status, error) {
            if (xhr.status === 401) {
                // Redirect to login page
                window.location.href = '/identity/account/login';
            } else {
                console.error("Error uploading image: " + error);
            }
        }
    });
}
function deleteLike(wishListId) {
    $.ajax({
        url: '/Like/DeleteLike',
        type: 'POST',
        data: { wishListId: wishListId },
        contentype: "json",
        success: function (response) {
            window.location.reload();
        },
        error: function (xhr, status, error) {
            if (xhr.status === 401) {
                // Redirect to login page
                window.location.href = '/identity/account/login';
            } else {
                console.error("Error uploading image: " + error);
            }
        }
    });
}
