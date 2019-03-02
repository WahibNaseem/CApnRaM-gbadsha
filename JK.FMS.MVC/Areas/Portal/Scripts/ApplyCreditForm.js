/* eslint-disable eqeqeq, no-constant-condition */

$(document).on("change", "#creditAmt", function () {

    setCreditAmt($("#creditAmt").val(), this.id);

});

$(document).on("change", "#form_applycreditform .credit-line-item-amt-field,#form_applycreditform .credit-line-item-total-field", function () {
    
    updateLineItemData(this.id);
    checkAutoApply(this);
    updateFranchiseeData();

    if ($("#numLineItems").val() == 1) {
        $("#creditAmt").val(formatCurrency(Math.max(0, removeFormatting($("#item0_creditAmt").val()))));
    } else {
        var amtType = getApplyAmtType();
        var applyAMTp = 0;
        var applyAMTCRp = 0;
        for (var i = 0; i < parseInt($('#hdfInvoiceDetailItemsCount').val()); i++) {
            
            if (amtType == 'total') {                
                applyAMTp = applyAMTp + parseFloat($('#item' + i + '_total').val())                
            } else {                
                applyAMTp = applyAMTp + parseFloat($('#item' + i + '_creditAmt').val())
            }
            applyAMTCRp = applyAMTCRp + parseFloat($('#item' + i + '_creditAmt').val())
        }
        $("#requestCreditAmt").val(formatCurrency(applyAMTp));
        $("#creditAmt").val(formatCurrency(Math.max(0, applyAMTCRp)));
    }

    updateSummaryData();

    if ($("#numLineItems").val() == 1) {
        $("#creditAmt").val(formatCurrency(Math.max(0, removeFormatting($("#item0_creditAmt").val()))));
    } else {                
        applyAMTCRp = 0;
        for (var i = 0; i < parseInt($('#hdfInvoiceDetailItemsCount').val()); i++) {
            applyAMTCRp = applyAMTCRp + parseFloat($('#item' + i + '_creditAmt').val())
        }        
        $("#creditAmt").val(formatCurrency(Math.max(0, applyAMTCRp)));
    }
});

$(document).on("change", "#form_applycreditform .credit-franchisee-field", function () {
    updateFranchiseeData();
});

$(document).on("change", "#select_LineNo", function () {
    debugger
    showFranchiseesForLineNumber(this.value);
});

$(document).on("change", "#requestCreditAmt", function () {
    var amtType = getApplyAmtType();
    
    if (removeFormatting($("#requestCreditAmt").val()) > removeFormatting($("#oldBalance").val())) {
        alert("Request Credit Amount should be less than or equal to " + $("#oldBalance").val() + ".");
        
    } else {
        $("#creditAmt").val(formatCurrency(Math.max(0, removeFormatting($("#requestCreditAmt").val()))));


        if (parseInt($('#hdfInvoiceDetailItemsCount').val()) > 1) {

            var _requestCreditAmt = parseFloat($('#requestCreditAmt').val());
            if ($('#invoiceCreditIsUpdate').val() != '1') {
                for (var i = 0; i < parseInt($('#hdfInvoiceDetailItemsCount').val()); i++) {
                    if (amtType == 'total') {
                        $('#item' + i + '_total').val(_requestCreditAmt)
                        $('#item' + i + '_total').trigger("change");

                        _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_total').val())

                    } else {
                        $('#item' + i + '_creditAmt').val(_requestCreditAmt)
                        $('#item' + i + '_creditAmt').trigger("change");
                        _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_creditAmt').val())
                    } 
                }
            }

            
        } else {
            if (amtType == 'total') {
                $('#item0_total').val($('#requestCreditAmt').val())
                $('#item0_total').trigger("change");
            } else {
                $('#item0_creditAmt').val($('#requestCreditAmt').val())
                $('#item0_creditAmt').trigger("change");
            }
        }
    }    
});

function initCreditForm() {
    
    refreshCreditForm();
    showFranchiseesForLineNumber($("#select_LineNo option:first").val());
    toggleCreditUnitColumns();
    toggleApplyAmtField();

    applyMaskCurrency("#invAmt")
    //applyMaskCurrency("#creditAmt")
    //applyMaskCurrency("#newBalance")
    //applyMaskCurrency("#oldBalance")


}
$(document).on("change", "#chkAutoApply", function () {
    debugger
    var elem = $(this);
    debugger
    if ($("#chkAutoApply").prop("checked") == false && parseInt($('#hdfInvoiceFranchiseeItemsCount').val()) > 1) {
        $("#form_applycreditform .credit-franchisee-field").prop('readonly', false);
    } else {
        $("#form_applycreditform .credit-franchisee-field").prop('readonly', true);
    }
    if ($("#chkAutoApply").prop("checked") && $(elem).hasClass("credit-line-item-autoapply")) {
        handleAutoApply($(elem).attr('name').split('_')[0]);
    }
});


$(document).on("change", "#form_applycreditform input[type=radio][name=rdCreditUnit]", function () {
    toggleCreditUnitColumns();
    var amtType = getApplyAmtType();
    if (parseInt($('#hdfInvoiceDetailItemsCount').val()) > 1) {
        var _requestCreditAmt = parseFloat($('#requestCreditAmt').val());
        if ($('#invoiceCreditIsUpdate').val() != '1') {
            for (var i = 0; i < parseInt($('#hdfInvoiceDetailItemsCount').val()); i++) {
                if (amtType == 'total') {
                    $('#item' + i + '_total').val(_requestCreditAmt)
                    $('#item' + i + '_total').trigger("change");

                    _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_total').val())

                } else {
                    $('#item' + i + '_creditAmt').val(_requestCreditAmt)
                    $('#item' + i + '_creditAmt').trigger("change");
                    _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_creditAmt').val())
                }
            }
        }
    } else {
        if (amtType == 'total') {
            $('#item0_total').val(removeFormatting($('#requestCreditAmt').val()))
            $('#item0_total').trigger("change");
        } else {
            $('#item0_creditAmt').val(removeFormatting($('#requestCreditAmt').val()))
            $('#item0_creditAmt').trigger("change");
        }
    }

});

function getApplyAmtType() {
    return $("#form_applycreditform input[type='radio'][name='rdApplyAmtType']:checked").val();
}

function toggleApplyAmtField() {
    var amtType = getApplyAmtType();



    
    $("#form_applycreditform .credit-line-item-amt-field").prop('readonly', true);
    $("#form_applycreditform .credit-line-item-total-field").prop('readonly', true);

    if (parseInt($('#hdfInvoiceDetailItemsCount').val()) > 1) {
        if (amtType == 'total')
            $("#form_applycreditform .credit-line-item-total-field").prop('readonly', false);
        else
            $("#form_applycreditform .credit-line-item-amt-field").prop('readonly', false);
    }

   

    
    
    if (parseInt($('#hdfInvoiceDetailItemsCount').val()) > 1) {
        var _requestCreditAmt = parseFloat($('#requestCreditAmt').val());
        if ($('#invoiceCreditIsUpdate').val() != '1') {
            for (var i = 0; i < parseInt($('#hdfInvoiceDetailItemsCount').val()); i++) {
                if (amtType == 'total') {
                    $('#item' + i + '_total').val(_requestCreditAmt)
                    $('#item' + i + '_total').trigger("change");

                    _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_total').val())

                } else {
                    $('#item' + i + '_creditAmt').val(_requestCreditAmt)
                    $('#item' + i + '_creditAmt').trigger("change");
                    _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_creditAmt').val())
                }
            }
        }
    } else {
        if (amtType == 'total') {
            $('#item0_total').val(removeFormatting($('#requestCreditAmt').val()))
            $('#item0_total').trigger("change");
        } else {
            $('#item0_creditAmt').val(removeFormatting($('#requestCreditAmt').val()))
            $('#item0_creditAmt').trigger("change");
        }
    }

    
}

function checkAutoApply(elem) {
    debugger
    if ($("#chkAutoApply").prop("checked") && $(elem).hasClass("credit-line-item-autoapply")) {
        handleAutoApply($(elem).attr('name').split('_')[0]);
        debugger
        if ($("#chkAutoApply").prop("checked") == false && parseInt($('#hdfInvoiceFranchiseeItemsCount').val()) > 1) {
            $("#form_applycreditform .credit-franchisee-field").prop('readonly', false);
        } else {
            $("#form_applycreditform .credit-franchisee-field").prop('readonly', true);
        }

    }
}


function handleAutoApply(item) {
    debugger
    var totalAmt = 0;
    var creditAmt = 0;
    //var creditAmt = parseFloatSafe($("#" + item + "_total").val());

    $("#form_applycreditform .credit-line-item-total-field").each(function () {
        creditAmt += parseFloatSafe($(this).val());
        
    });

    //if (parseInt($('#hdfInvoiceDetailItemsCount').val()) > 1) {
    //    creditAmt = parseFloatSafe($("#" + item + "_total").val());
    //} else {
    //    creditAmt = creditAmt < parseFloat($('#requestCreditAmt').val()) ? parseFloat($('#requestCreditAmt').val()) : creditAmt;
    //}


    $("#form_applycreditform .franchisee-row-item").each(function () {
        //if (this.value == item) {
            var key = $(this).prev().val();
            var prefix = "#bp" + key;
            totalAmt += parseFloatSafe($(prefix + "_oldBalance").val());
        //}
    });

    if (totalAmt == 0)
        return;

    $("#form_applycreditform .franchisee-row-item").each(function () {
        //if (this.value == item) {
        
            var key = $(this).prev().val();
            var prefix = "#bp" + key;
            var amt = parseFloatSafe($(prefix + "_oldBalance").val());
            var portion = amt / totalAmt;
            var franchiseeCreditPct = portion * 100.00;
            var franchiseeCreditAmt = portion * creditAmt;

            

            $(prefix + "_creditAmt").val(franchiseeCreditAmt.toFixed(2));
            $(prefix + "_creditPct").val(franchiseeCreditPct.toFixed(2));
        //}
    });
}

function setCreditAmt(amtm, Id) {
    debugger;
    if (!validateCreditAmount())
        amtm = getMaxCredit();

    $("#creditAmt").val(formatCurrency(amtm));
    $("#requestCreditAmt").val(amtm);
   
    updateSummaryData();

    //if ($("#numLineItems").val() == 1)
    //    $("#item0_creditAmt").val($("#creditAmt").val());

    updateLineItemData(Id);
    checkAutoApply(Id);
    //checkAutoApply($("#item0_creditAmt"));
    updateFranchiseeData();

    $("#requestCreditAmt").trigger("change");

}
function refreshCreditForm() {
    updateSummaryData();
    updateLineItemData("");
    updateFranchiseeData();
}

function updateSummaryData() {
    var oldBalance = parseFloatSafe(removeFormatting($("#oldBalance").val()));
    var creditAmt = parseFloatSafe(removeFormatting($("#creditAmt").val()));
    $("#creditAmt").val(formatCurrency(creditAmt));
    $("#newBalance").val(formatCurrency(oldBalance - creditAmt));
}

function updateLineItemData(Id) {
   
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
    UpdateDateValidation();
    var creditUnit = $("#form_applycreditform input[type=radio][name=rdCreditUnit]:checked").val();
    $("#form_applycreditform .franchisee-row-key").each(function () {
        
        var prefix = "#bp" + this.value;
        var item = $(prefix + "_item").val();
        var creditAmt = parseFloatSafe($("#" + item + "_total").val());
        

        if (parseInt($('#hdfInvoiceDetailItemsCount').val()) > 1) {
            creditAmt = parseFloatSafe($("#" + item + "_total").val());
        } else {
            creditAmt = creditAmt < parseFloat($('#requestCreditAmt').val()) ? parseFloat($('#requestCreditAmt').val()) : creditAmt;
        }

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
    $("#form_applycreditform .franchisee-row").hide();
    $("#form_applycreditform .franchisee-row-line" + lineNo).show();
}

function toggleCreditUnitColumns(creditUnit) {
    var credUnit = $("#form_applycreditform input[type=radio][name=rdCreditUnit]:checked").val();
    $("#form_applycreditform .franchisee-amt-col").hide();
    $("#form_applycreditform .franchisee-pct-col").hide();
    $("#form_applycreditform .franchisee-" + credUnit + "-col").show();
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