
function showLoading() {
    var $loadingParent = $('#loadingParent');
    $loadingParent.css("left", ($(document).width() / 2 - $loadingParent.width() / 2) + 50);

    $loadingParent.css("top", ($(window).height() / 2 - $loadingParent.height() / 2) + "px");
    //$loadingParent.css("top", "150px");

    $loadingParent.show();
}

function hideLoading() {
    $('#loadingParent').hide();
}

function blockUI() {
    $.blockUI({
        message: '',
        overlayCSS: {
            cursor: 'default',
            backgroundColor: "transparent"
        },
        baseZ: 20000
    });
}

function unblockUI() {
    $.unblockUI();
}