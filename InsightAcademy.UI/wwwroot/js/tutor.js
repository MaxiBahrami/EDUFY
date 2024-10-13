"use strict"
function searchTutor() {
    debugger
    let subject = $('#selectv8').val();
    let search = $('#search').val();
    localStorage.setItem('CategoryId', subject);
    window.location.href = '/tutor/list?category=' + subject + "&subject=" + search
}
function refreshPage(event) {
    // Prevent the default action of the event
    event.preventDefault();
    window.location.href = '/tutor/list'
}
function addMoreText() {
    var dots = document.getElementById("dots");
    var moreText = document.getElementById("more");
    var btnText = document.getElementById("myBtn");

    if (dots.style.display === "none") {
        dots.style.display = "inline";
        btnText.innerHTML = "Read more";
        moreText.style.display = "none";
    } else {
        dots.style.display = "none";
        btnText.innerHTML = "Read less";
        moreText.style.display = "inline";
    }
}
function filter(type) {
    showLoader();
    var urlParams = new URLSearchParams(window.location.search);
    var subjectValue = urlParams.get('category');
    let categoryId = localStorage.getItem('CategoryId');
    if (categoryId) {
        subjectValue = categoryId;
    }
    else {
        localStorage.setItem('CategoryId', subjectValue);
    }

    let location = $('#location').val();
    let order = $('#order').val();
    let minPrice = $('#tu-min-value').val();
    let maxPrice = $('#tu-max-value').val();
    let education = $('#selectv7').val();

    let service;
    let photo;
    let gender;
    let Rating;
    let isLocal;

    if ($('#local-tutor').is(':checked')) {
        isLocal = true;
    }
    if ($('#online').is(':checked')) {
        service = $('#online').val();
    }
    if ($('#photo').is(':checked')) {
        photo = $('#photo').val();
    }
    if ($('#gender').is(':checked')) {
        gender = $('#gender').val();
    }
    if ($('#female').is(':checked')) {
        gender = $('#female').val();
    }

    if ($('#rate').is(':checked')) {
        Rating = $('#rate').val();
    }
    if ($('#rate4').is(':checked')) {
        Rating = $('#rate4').val();
    }
    if ($('#rate3').is(':checked')) {
        Rating = $('#rate3').val();
    }
    if ($('#rate2a').is(':checked')) {
        Rating = $('#rate2a').val();
    }
    if ($('#rate1a').is(':checked')) {
        Rating = $('#rate1a').val();
    }
    var selectedSubjects = $('.subject:checked').map(function () {
        return $(this).val();
    }).get();
    var selectedRating = $('.rating:checked').map(function () {
        return $(this).val();
    }).get();
    //if (selectedSubjects.length <= 0) {
    //    selectedSubjects.push(subjectValue)
    //}
    console.log(selectedSubjects);
    console.log(selectedRating);

    //let price = $('#price').val();
    $.ajax({
        url: '/tutor/Filter',
        type: 'POST',
        data: { Education: education, Order: order, Location: location, SelectedRating: selectedRating, SelectedCategories: selectedSubjects, MinPrice: minPrice, MaxPrice: maxPrice, service: service, SubjectIds: selectedSubjects, photo: photo, Gender: gender, Rating: Rating, FilterType: type, IsLocal: isLocal },
        contentype: "json",
        dataType: "json",
        success: function (response) {
            console.log(response);
            $('#tutor').empty();
            $('#tutor').html(response.partialView);
            $('#tutor-count').text(response.totalCount);
        },
        error: function (xhr, status, error) {
            console.error("Error uploading image: " + error);
        }, complete: function () {
            hideLoader();
        }
    });
}

//function gridView() {
//    $.ajax({
//        url: '/tutor/TutorGridView',
//        type: 'GET',
//        contentype: "json",
//        dataType: "html",
//        success: function (response) {
//            console.log(response);
//            $('#list-view').remove();
//            $('#grid').html(response);
//        },
//        error: function (xhr, status, error) {
//            console.error("Error uploading image: " + error);
//        }, complete: function () {
//            hideLoader();
//        }
//    });
//}
function showRatingBreakDown(id, avgRating) {
    showLoader();
    $.ajax({
        url: '/tutor/GetTutorRatingBreakDown?tutorId=' + id,
        type: 'GET',
        dataType: "json",
        success: function (response) {
            console.log(response);
            $('#five-star').text(response.fiveStarCount)
            $('#four-star').text(response.fourStarCount)
            $('#three-star').text(response.threeStarCount)
            $('#two-star').text(response.twoStarCount)
            $('#one-star').text(response.oneStarCount)

            $('#five-star-progress').css('width', response.fiveStarCount + "%")
            $('#four-star-progress').css('width', response.fourStarCount + "%")
            $('#three-star-progress').css('width', response.threeStarCount + "%")
            $('#two-star-progress').css('width', response.twoStarCount + "%")
            $('#one-star-progress').css('width', response.oneStarCount + "%")

            $('#avg-rating').text(avgRating + " out of 5");

            $('#ratingBreakdown').modal('show');
        },
        error: function (xhr, status, error) {
            console.error(error);
        }, complete: function () {
            hideLoader();
        }
    });
}
function editSubject(id) {
    $.ajax({
        url: '/tutor/GetSubjectById',
        type: 'Get',
        data: { id: id },
        success: function (response) {
            console.log(response);
            let tagsArray = response.tags.split(',');
            $('#subject-form').css('display', 'block');
            $('#selectv8').val(response.subjectId);
            $('#selectv9').val(response.categoryId);
            $('#selectv8').trigger('change');
            $('#selectv9').trigger('change');
            /*$('#Tags').addTag(response.tags);*/
            tagsArray.forEach(function (tag) {
                $('#Tags').addTag(tag.trim()); // Trim whitespace from each tag
            });
            $('#Id').val(response.id);

            $('html, body').animate({ scrollTop: $('#subject-form').offset().top }, 'fast');
        },
        error: function (xhr, status, error) {
            console.error("Error uploading image: " + error);
        }
    });
}
function formatDate(dateString) {
    var dateObject = new Date(dateString);
    var formattedDate = dateObject.toISOString().slice(0, 10);
    return formattedDate;
}
function onChange(checkbox) {
    var endDateInput = document.getElementById("EndDate");
    if (checkbox.checked) {
        endDateInput.disabled = true;
    } else {
        endDateInput.disabled = false;
    }
}
function deleteSubject(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'You will not be able to recover this item!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'Post',
                url: '/Tutor/DeleteSubject',
                data: { id: id },

                success: function (response) {
                    window.location.reload();
                },
                error: function (error) {
                    console.log(JSON.stringify(error));
                    // Handle the error
                }
            });
        }
    });
}
function handleFileUploadChange(input) {
    showLoader();

    var files = input.files;
    var formData = new FormData();

    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        if (!file.type.startsWith('image/')) {
            alert('Please select only image files.');
            return;
        }
        formData.append('mediaFiles', file);
    }

    $.ajax({
        url: '/Tutor/UploadMedia',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.status == true) {
                window.location.reload();
            }
            else {
                showToast("error", response.message);
            }
        },
        error: function (error) {
            console.error('Error uploading files:', error);
        },
        complete: function () {
            hideLoader();
        }
    });
}

function saveVideo() {
    let videoUrl = $('#url').val();
    if (videoUrl) {
        $.ajax({
            type: 'Post',
            url: '/Tutor/SaveVideo',
            data: JSON.stringify({ url: videoUrl }),
            contentType: 'application/json',
            success: function (response) {
                /*  $('#row_' + id).remove();*/
                window.location.reload();
            },
            error: function (error) {
                console.log(JSON.stringify(error));
                // Handle the error
            }
        });
    }
    else {
        alert('Please add video url')
    }
}

function deletePicture(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'You will not be able to recover this item!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Tutor/DeletePicture',
                method: 'POST',
                data: { id: id },
                success: function (response) {
                    $('#media-list li').each(function () {
                        if ($(this).find('img').data('id') === id) {
                            $(this).remove();
                        }
                    });

                    window.location.reload();
                },
                error: function (xhr, status, error) {
                    alert("An error occurred while deleting the picture.");
                    console.error(error);
                }
            });
        }
    });
}