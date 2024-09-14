"use strict"
function openChatPopup(recepientId) {
    $('#description').val('');
    $('#chatPopup').data('RecepientId', recepientId)
    $('#chatPopup').modal('show');
}
function sendMessage() {
    // showLoader();
    let tutorId = $('#chatPopup').data('RecepientId');
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
$(document).ready(function () {
    $('.approve-btn').click(function () {
        var tutorId = $(this).data('tutor-id');
        approveTutor(tutorId);
    });

    $('.unapprove-btn').click(function () {
        var tutorId = $(this).data('tutor-id');
        unapproveTutor(tutorId);
    });
    $('.blockTutor').click(function () {
        var tutorId = $(this).data('tutor-id');
        BlockTutor(tutorId);
    });
    $('.unblockTutor').click(function () {
        var tutorId = $(this).data('tutor-id');
        UnblockTutor(tutorId);
    });

    $('.blockStudent').click(function () {
        var studentId = $(this).data('student-id');
        BlockStudent(studentId);
    });
    $('.unblockStudent').click(function () {
        var studentId = $(this).data('student-id');
        UnblockStudent(studentId);
    });

    function approveTutor(tutorId) {
        $.ajax({
            type: "POST",
            url: "/admin/dashboard/approve",
            data: { tutorId: tutorId },
            success: function (data) {
                console.log("Tutor approved successfully.");
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.error("Error approving tutor:", error);
                // Optionally show an error message
            }
        });
    }

    function unapproveTutor(tutorId) {
        $.ajax({
            type: "POST",
            url: "/admin/dashboard/unapprove",
            data: { tutorId: tutorId },
            success: function (data) {
                console.log("Tutor unapproved successfully.");
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.error("Error unapproving tutor:", error);
                // Optionally show an error message
            }
        });
    }
    function BlockTutor(tutorId) {
        $.ajax({
            type: "POST",
            url: "/admin/dashboard/BlockTutor",
            data: { tutorId: tutorId },
            success: function (data) {
                console.log("User Locked successfully.");
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.error("Error Locking the user:", error);
                // Optionally show an error message
            }
        });
    }
    function UnblockTutor(tutorId) {
        $.ajax({
            type: "POST",
            url: "/admin/dashboard/UnblockTutor",
            data: { tutorId: tutorId },
            success: function (data) {
                console.log("User Locked successfully.");
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.error("Error Locking the user:", error);
                // Optionally show an error message
            }
        });
    }
    function BlockStudent(studentId) {
        debugger
        $.ajax({
            type: "POST",
            url: "/admin/dashboard/BlockStudent",
            data: { studentId: studentId },
            success: function (data) {
                console.log("User Locked successfully.");
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.error("Error Locking the user:", error);
                // Optionally show an error message
            }
        });
    }
    function UnblockStudent(studentId) {
        $.ajax({
            type: "POST",
            url: "/admin/dashboard/UnBlockStudent",
            data: { studentId: studentId },
            success: function (data) {
                console.log("User Locked successfully.");
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.error("Error Locking the user:", error);
                // Optionally show an error message
            }
        });
    }

    function ApproveSubject(subjectId) {
        $.ajax({
            type: "POST",
            url: "/admin/dashboard/ApproveSubject",
            data: { subjectId: subjectId },
            success: function (data) {
                console.log("User Locked successfully.");
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.error("Error Locking the user:", error);
                // Optionally show an error message
            }
        });
    }
});