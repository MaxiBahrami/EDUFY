"use strict"
function filterReviews(id) {
    let selectedRating = $('#ratingFilter').val();
    let selectedOrder = $('#sortOrder').val();
    $.ajax({
        url: '/Reviews/FilterReviews',
        type: 'POST',
        data: { rating: selectedRating, order: selectedOrder, tutorId: id },
        dataType: "html",
        success: function (response) {
            console.log(response);
            $('#reviews').empty();
            $('#reviews').html(response);
        },
        error: function (xhr, status, error) {
            if (xhr.status === 401) {
                // Redirect to login page
                window.location.href = '/identity/account/login';
            } else {
                console.error("Error uploading image: " + error);
            }
        }, complete: function () {
            hideLoader();
        }
    });
}
function sortReviews() {
}
function addReview(tutorId) {
    showLoader();
    let description = $('#tu-reviews-content').val()
    let rating = $('#rating').val();
    $.ajax({
        url: '/Student/AddReview',
        type: 'POST',
        data: { feedBack: description, rating: rating, tutorId: tutorId },
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
        }, complete: function () {
            hideLoader();
        }
    });
}