$(document).ready(function () {

    setup_form();
    $("SELECT").chosen({ no_results_text: "No results matched: " });
    $("INPUT[type=checkbox]").prettyradiocheckbox();
    $("INPUT[type=radio]").prettyradiocheckbox();

    $("INPUT[type=datetime]").datepicker({ dateFormat: 'dd MM yy' });


    $("SELECT#SKU_VendorId").change(function () {
        //$("SELECT#SKU_PrivateKeyId").append('<option>ss</option>').trigger("liszt:updated");
    });



});




function setup_form() {
    form_focusFirstField();
    $("INPUT[type=number]").prettyNumeric();
}

function form_focusFirstField() {
    $("INPUT:visible:first").focus();
}

jQuery.fn.prettyNumeric = function (options) {
    var settings = jQuery.extend({ debug: false }, options || {});
    $(this).each(function (i) {
        var id = $(this).attr('id');

        $(this).keyup(function () { this.value = this.value.replace(/[^0-9\.]/g, ''); });

        $(this).wrap('<div class="prettynumeric" id="prettynumeric_' + id + '" />');
        $(this).after('<div class="prettynumeric_btn inc">+</div><div class="prettynumeric_btn dec">-</div>');

    });

    $("DIV.prettynumeric_btn").each(function () {
        $(this).click(function () {
            var $b = $(this);
            var $p = $b.parent().find("INPUT")
            var ov = parseFloat($p.val());
            ov = ov > 0 ? ov : 0;
            $p.val($b.hasClass('inc') ? ov + 1 : (ov >= 1) ? ov - 1 : 0);
        });
    });
}
