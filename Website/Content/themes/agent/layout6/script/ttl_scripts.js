
/* ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ============ SCRIPT =======================================
//////////////////////////////////////////////////////////////////////////////////////////////////////////// */

$(document).ready(function () {
    $('.lst-sport-match .itm-sport-match:not(.up-comming)').click(function () {
        $(this).toggleClass('highlight');
    })

    $('.lst-sport-match-2 .itm-sport-match .game-description .toggle-detail').click(function () {
        $(this).toggleClass('open');
        $(this).closest('.game-description').siblings('.game-detail').slideToggle('100');
    })
})

