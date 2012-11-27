//Depending on Radiobutton disable (or perhaps hide) the create new purchasing and owning customer
//Partial view. If only disabled please take care or clearing any validationmessages upon disable.

//Also set disabled state on load
$(function () {
    $('#NewOwningCustomer :input').attr('disabled', true);
});

function EnableNewPurchasingCustomer() {
    $('#NewPurchasingCustomer :input').removeAttr('disabled');
}

function DisableNewPurchasingCustomer() {
    $('#NewPurchasingCustomer :input').attr('disabled', true);
}

function EnableNewOwningCustomer() {
    $('#NewOwningCustomer :input').removeAttr('disabled');
}

function DisableNewOwningCustomer() {
    $('#NewOwningCustomer :input').attr('disabled', true);
}

var visible = false;

function ToggleNewOwningCustomer() {
    if (visible == false) {
        $('#OwningCustomer').slideDown(200, function () {
            $('#NewOwningCustomer :input').removeAttr('disabled');
            $('#NewOwningCustomer SELECT').trigger("liszt:updated");
        });
        visible = true;
    } else {
        $('#OwningCustomer').slideUp(200, function () {
            $('#NewOwningCustomer :input').attr('disabled', true);
            $('#NewOwningCustomer SELECT').trigger("liszt:updated");
        });
        visible = false;
    }

}