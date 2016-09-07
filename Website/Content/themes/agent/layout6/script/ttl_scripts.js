
/* ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ============ SCRIPT =======================================
//////////////////////////////////////////////////////////////////////////////////////////////////////////// */

$(document).ready(function() {
    $('.lst-sport-match .itm-sport-match:not(.up-comming)').click(function() {
        $(this).toggleClass('highlight');
    });

    $('.lst-sport-match-2 .itm-sport-match .game-description .toggle-detail').click(function() {
        $(this).toggleClass('open');
        $(this).closest('.game-description').siblings('.game-detail').slideToggle('100');
    });

    
});

(function($) {
    // round robin selection
    roundRobinSelection();
})(jQuery);

// Print
function printContent(el) {
    var restorepage = document.body.innerHTML;
    var printcontent = document.getElementById(el).innerHTML;
    document.body.innerHTML = printcontent;

    window.print();
    document.body.innerHTML = restorepage;
}

// Select Round Robin
function roundRobinSelection() {
    $("body").delegate("#cblRoundRobinOptions input:checkbox", "click", function (event) {
        var isRoundRobin = $("#rdbRoundRobin").is(":checked");
        
        if (!isRoundRobin)
            return false;
    });
}
