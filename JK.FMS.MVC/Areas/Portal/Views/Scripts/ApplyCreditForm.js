/* eslint-disable eqeqeq, no-constant-condition */

$(document).on("change", "#creditAmt", function () {

    if (!validateCreditAmount())
        $("#creditAmt").val(formatCurrency(getMaxCredit()));

    updateSummaryData();

    if ($("#numLineItems").val() == 1)
        $("#item0_creditAmt").val($("#creditAmt").val());

    updateLineItemData();
    checkAutoApply($("#item0_creditAmt"));
    updateFranchiseeData();
});

$(document).on("change", ".credit-line-item-amt-field, .credit-line-item-total-field", function () {

    updateLineItemData();
    checkAutoApply(this);
    updateFranchiseeData();

    if ($("#numLineItems").val() == 1)
        $("#creditAmt").val(formatCurrency(Math.max(0, removeFormatting($("#item0_creditAmt").val()))));

    updateSummaryData();
});

$(document).on("change", ".credit-franchisee-field", function () {
    updateFranchiseeData();
});

$(document).on("change", "#select_LineNo", function () {
    showFranchiseesForLineNumber(this.value);
});

$(document).on("change", "input[type=radio][name=rdCreditUnit]", function () {
    toggleCreditUnitColumns();
});

function initCreditForm() {
    refreshCreditForm();
    showFranchiseesForLineNumber(1);
    toggleCreditUnitColumns();
    toggleApplyAmtField();
    
    applyMaskCurrency("#invAmt")
    //applyMaskCurrency("#creditAmt")
    //applyMaskCurrency("#newBalance")
    //applyMaskCurrency("#oldBalance")


}

function getApplyAmtType() {
    return $("input[type='radio'][name='rdApplyAmtType']:checked").val();
}

function toggleApplyAmtField() {
    var amtType = getApplyAmtType();

    $(".credit-line-item-amt-field").prop('disabled', true);
    $(".credit-line-item-total-field").prop('disabled', true);

    if (amtType == 'total')
        $(".credit-line-item-total-field").prop('disabled', false);
    else
        $(".credit-line-item-amt-field").prop('disabled', false);
}

function checkAutoApply(elem) {
    if ($("#chkAutoApply").prop("checked") && $(elem).hasClass("credit-line-item-autoapply"))
        handleAutoApply($(elem).attr('name').split('_')[0]);
}

function handleAutoApply(item) {

    var totalAmt = 0;

    var creditAmt = parseFloatSafe($("#" + item + "_total").val());

    $(".franchisee-row-item").each(function () {
        if (this.value == item) {
            var key = $(this).prev().val();
            var prefix = "#bp" + key;
            totalAmt += parseFloatSafe($(prefix + "_oldBalance").val());
        }
    });

    if (totalAmt == 0)
        return;

    $(".franchisee-row-item").each(function () {
        if (this.value == item) {
            var key = $(this).prev().val();
            var prefix = "#bp" + key;
            var amt = parseFloatSafe($(prefix + "_oldBalance").val());
            var portion = amt / totalAmt;
            var franchiseeCreditPct = portion * 100.00;
            var franchiseeCreditAmt = portion * creditAmt;

            $(prefix + "_creditAmt").val(franchiseeCreditAmt.toFixed(2));
            $(prefix + "_creditPct").val(franchiseeCreditPct.toFixed(2));
        }
    });
}

function refreshCreditForm() {
    updateSummaryData();
    updateLineItemData();
    updateFranchiseeData();
}

function updateSummaryData() {
    var oldBalance = parseFloatSafe(removeFormatting($("#oldBalance").val()));
    var creditAmt = parseFloatSafe(removeFormatting($("#creditAmt").val()));
    $("#creditAmt").val(formatCurrency(creditAmt));
    $("#newBalance").val(formatCurrency(oldBalance - creditAmt));
}

function updateLineItemData() {

    var itemCnt = 0;
    while (true) {
        var prefix = "#item" + itemCnt;
        if ($(prefix + "_oldBalance").length) {
            var item_taxRate = $(prefix + "_taxRate").val();
            var item_oldBalance = $(prefix + "_oldBalance").val();

            var item_creditAmt = 0;
            var item_tax = 0;
            var item_total = 0;

            var applyAmtType = getApplyAmtType();

            if (applyAmtType == 'amt') {
                item_creditAmt = parseFloatSafe(removeFormatting($(prefix + "_creditAmt").val()));
                item_creditAmt = Math.max(0, Math.min(item_creditAmt, item_oldBalance));
                item_tax = (item_creditAmt * item_taxRate).toFixed(2);
                item_total = (item_creditAmt - item_tax).toFixed(2);
            }
            else if (applyAmtType == 'total') {
                item_total = parseFloatSafe(removeFormatting($(prefix + "_total").val()));
                item_total = Math.max(0, item_total);
                item_creditAmt = (item_total * (1.0 / (1.0 - parseFloatSafe(item_taxRate)))).toFixed(2);
                item_tax = (item_creditAmt - item_total).toFixed(2);
            }

            // one last check if amt type was total
            if (item_creditAmt > item_oldBalance) {
                item_creditAmt = item_oldBalance;
                item_tax = (item_creditAmt * item_taxRate).toFixed(2);
                item_total = (item_creditAmt - item_tax).toFixed(2);
            }

            $(prefix + "_creditAmt").val(item_creditAmt);
            $(prefix + "_tax").val(item_tax);
            $(prefix + "_total").val(item_total);
            $(prefix + "_newBalance").val((item_oldBalance - item_creditAmt).toFixed(2));

            itemCnt++;
        }
        else
            break;
    }
}

function updateFranchiseeData() {
    var creditUnit = $("input[type=radio][name=rdCreditUnit]:checked").val();
    $(".franchisee-row-key").each(function () {
        var prefix = "#bp" + this.value;
        var item = $(prefix + "_item").val();
        var creditAmt = parseFloatSafe($("#" + item + "_total").val());
        var oldBalance = $(prefix + "_oldBalance").val();
        var franchiseeCreditAmt = parseFloatSafe($(prefix + "_creditAmt").val());
        var franchiseeCreditPct = parseFloatSafe($(prefix + "_creditPct").val());
        if (creditUnit == "amt") {
            //$(prefix + "_creditAmt").val(franchiseeCreditAmt.toFixed(2));
            var pct = (franchiseeCreditAmt * 100 / creditAmt);
            $(prefix + "_creditPct").val(pct.toFixed(2));
        }
        else if (creditUnit == "pct") {
            $(prefix + "_creditAmt").val((franchiseeCreditPct * creditAmt / 100.00).toFixed(2));
        }
        franchiseeCreditAmt = $(prefix + "_creditAmt").val();
        franchiseeCreditAmt = Math.max(0, Math.min(franchiseeCreditAmt, oldBalance));

        $(prefix + "_creditAmt").val(franchiseeCreditAmt.toFixed(2));
        $(prefix + "_newBalance").val((oldBalance - franchiseeCreditAmt).toFixed(2));
    });
}

function showFranchiseesForLineNumber(lineNo) {
    $(".franchisee-row").hide();
    $(".franchisee-row-line" + lineNo).show();
}

function toggleCreditUnitColumns(creditUnit) {
    var credUnit = $("input[type=radio][name=rdCreditUnit]:checked").val();
    $(".franchisee-amt-col").hide();
    $(".franchisee-pct-col").hide();
    $(".franchisee-" + credUnit + "-col").show();
}

function isExtraCredit() {
    return $("#isExtraCredit").val() == "True";
}

function getMaxCredit() {
    var maxCredit = 0;
    if (isExtraCredit()) {
        maxCredit = removeFormatting($("#invAmt").val());
    }
    else {
        maxCredit = removeFormatting($("#oldBalance").val());
    }
    return maxCredit;
}

function validateCreditAmount() {

    var maxCredit = getMaxCredit();
    var maxType = "";
    if (isExtraCredit()) {
        maxType = "invoice amount";
    }
    else {
        maxType = "current balance";
    }

    if (removeFormatting($("#creditAmt").val()) > maxCredit) {
        alert("Credit amount should be less than or equal to " + maxType + ".");
        return false;
    }

    return true;
}

function formatCurrency(total) {
    return total;
    //var neg = false;
    //if (total < 0) {
    //    neg = true;
    //    total = Math.abs(total);
    //}
    //return (neg ? "-" : '') +'$ '+ parseFloatSafe(total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();

   
}

function removeFormatting(i) {
   
    return typeof i === 'string' ?
        i.replace(/[\$,]/g, '') * 1 :
        typeof i === 'number' ?
            i : 0;
}

function parseFloatSafe(val) {
    return parseFloat(val) || 0;
}

function applyMaskCurrency(id) {
    $(id).inputmask("currency", {
        alias: 'currency',
        prefix: '$ ',
        digits: 2,
        autoUnmask: true,
        removeMaskOnSubmit: true,
        unmaskAsNumber: true,
        allowPlus: false,
        allowMinus: true,
        autoGroup: true,
        positionCaretOnTab: false,
        positionCaretOnClick: "select",
        groupSeparator: ",",
    });
}