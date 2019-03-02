/* eslint-disable eqeqeq, no-constant-condition */




function CrGetINVIDFromElem(item) {
    var inlenth = $(item).attr('id').split('_');
    var invstr = inlenth[inlenth.length - 1]
    return invstr
}
function initCreditFormINV(_invID) {
    _CRinvoiceId = _invID
    refreshCreditFormINV(_CRinvoiceId);
    showFranchiseesForLineNumberINV($("#select_LineNo_" + _CRinvoiceId + " option:first").val(), _CRinvoiceId);
    toggleCreditUnitColumnsINV(_CRinvoiceId);
    toggleApplyAmtFieldINV(_CRinvoiceId);

    applyMaskCurrency("#invAmt" + "_" + _CRinvoiceId)
    //applyMaskCurrency("#creditAmt")
    //applyMaskCurrency("#newBalance")
    //applyMaskCurrency("#oldBalance")


    $(document).on("change", "#creditAmt" + "_" + _CRinvoiceId, function () {
        _CRinvoiceId = CrGetINVIDFromElem(this);
        setCreditAmtINV($("#creditAmt" + "_" + _CRinvoiceId).val(), this.id, _CRinvoiceId);
    });

    $(document).on("change", "#form_applycreditform_" + _CRinvoiceId + " .credit-line-item-amt-field," + "#form_applycreditform_" + _CRinvoiceId + " .credit-line-item-total-field", function () {
        _CRinvoiceId = CrGetINVIDFromElem(this);

        var applyAMTp = 0;
        var applyAMTCRp = 0;
        updateLineItemDataINV(this.id, _CRinvoiceId);
        checkAutoApplyINV(this, _CRinvoiceId);
        updateFranchiseeDataINV(_CRinvoiceId);

        if ($("#numLineItems" + "_" + _CRinvoiceId).val() == 1) {
            $("#creditAmt" + "_" + _CRinvoiceId).val(formatCurrency(Math.max(0, removeFormatting($("#item0_creditAmt" + "_" + _CRinvoiceId).val()))));
        } else {
            var amtType = getApplyAmtTypeINV(_CRinvoiceId);
            applyAMTp = 0;
            applyAMTCRp = 0;
            for (var i = 0; i < parseInt($('#hdfInvoiceDetailItemsCount' + "_" + _CRinvoiceId).val()); i++) {

                if (amtType == 'total') {
                    applyAMTp = applyAMTp + parseFloat($('#item' + i + '_total' + "_" + _CRinvoiceId).val())
                } else {
                    applyAMTp = applyAMTp + parseFloat($('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).val())
                }
                applyAMTCRp = applyAMTCRp + parseFloat($('#item' + i + '_total' + "_" + _CRinvoiceId).val())

            }
            $("#requestCreditAmt_" + _CRinvoiceId).val(formatCurrency(applyAMTp));
            $("#creditAmt" + "_" + _CRinvoiceId).val(formatCurrency(Math.max(0, applyAMTCRp)));
        }

        updateSummaryDataINV(_CRinvoiceId);

        if ($("#numLineItems" + "_" + _CRinvoiceId).val() == 1) {
            $("#creditAmt" + "_" + _CRinvoiceId).val(formatCurrency(Math.max(0, removeFormatting($("#item0_creditAmt" + "_" + _CRinvoiceId).val()))));
        } else {
            applyAMTCRp = 0;
            for (var j = 0; j < parseInt($('#hdfInvoiceDetailItemsCount' + "_" + _CRinvoiceId).val()); j++) {
                applyAMTCRp = applyAMTCRp + parseFloat($('#item' + j + '_creditAmt' + "_" + _CRinvoiceId).val())
            }
            $("#creditAmt" + "_" + _CRinvoiceId).val(formatCurrency(Math.max(0, applyAMTCRp)));
        }
    });

    $(document).on("change", "#form_applycreditform_" + _CRinvoiceId + " .credit-franchisee-field", function () {
        _CRinvoiceId = CrGetINVIDFromElem(this);
        updateFranchiseeData(_CRinvoiceId);
    });

    $(document).on("change", "#select_LineNo" + "_" + _CRinvoiceId, function () {
        _CRinvoiceId = CrGetINVIDFromElem(this);
        showFranchiseesForLineNumberINV(this.value, _CRinvoiceId);
    });

    $(document).on("change", "#requestCreditAmt" + "_" + _CRinvoiceId, function () {
        _CRinvoiceId = CrGetINVIDFromElem(this);
        var amtType = getApplyAmtTypeINV(_CRinvoiceId);

        if (removeFormatting($("#requestCreditAmt" + "_" + _CRinvoiceId).val()) > removeFormatting($("#oldBalance" + "_" + _CRinvoiceId).val())) {
            alert("Request Credit Amount should be less than or equal to " + $("#oldBalance" + "_" + _CRinvoiceId).val() + ".");

        } else {
            $("#creditAmt" + "_" + _CRinvoiceId).val(formatCurrency(Math.max(0, removeFormatting($("#requestCreditAmt" + "_" + _CRinvoiceId).val()))));


            if (parseInt($('#hdfInvoiceDetailItemsCount' + "_" + _CRinvoiceId).val()) > 1) {

                var _requestCreditAmt = parseFloat($('#requestCreditAmt' + "_" + _CRinvoiceId).val());
                if ($('#invoiceCreditIsUpdate' + "_" + _CRinvoiceId).val() != '1') {
                    for (var i = 0; i < parseInt($('#hdfInvoiceDetailItemsCount' + "_" + _invID).val()); i++) {
                        if (amtType == 'total') {
                            $('#item' + i + '_total' + "_" + _CRinvoiceId).val(_requestCreditAmt)
                            $('#item' + i + '_total' + "_" + _CRinvoiceId).trigger("change");

                            _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_total').val())

                        } else {
                            $('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).val(_requestCreditAmt)
                            $('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).trigger("change");
                            _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).val())
                        }
                    }
                }


            } else {
                if (amtType == 'total') {
                    $('#item0_total' + "_" + _CRinvoiceId).val($('#requestCreditAmt' + "_" + _CRinvoiceId).val())
                    $('#item0_total' + "_" + _CRinvoiceId).trigger("change");
                } else {
                    $('#item0_creditAmt' + "_" + _CRinvoiceId).val($('#requestCreditAmt' + "_" + _CRinvoiceId).val())
                    $('#item0_creditAmt' + "_" + _CRinvoiceId).trigger("change");
                }
            }
        }
    });

    $(document).on("change", "#chkAutoApply" + "_" + _CRinvoiceId, function () {
        _CRinvoiceId = CrGetINVIDFromElem(this);
        var elem = $(this);
        if ($("#chkAutoApply" + "_" + _CRinvoiceId).prop("checked") == false && parseInt($('#hdfInvoiceFranchiseeItemsCount' + "_" + _CRinvoiceId).val()) > 1) {
            $("#form_applycreditform_" + _CRinvoiceId + " .credit-franchisee-field").prop('readonly', false);
        } else {
            $("#form_applycreditform_" + _CRinvoiceId + " .credit-franchisee-field").prop('readonly', true);
        }
        if ($("#chkAutoApply" + "_" + _CRinvoiceId).prop("checked") && $(elem).hasClass("credit-line-item-autoapply")) {
            handleAutoApply($(elem).attr('name').split('_')[0], _CRinvoiceId);
        }
    });


   


    //$(document).on("change", "#form_applycreditform_" + _CRinvoiceId + " input[type=radio][name=rdCreditUnit_" + _CRinvoiceId + "]", function () {
    //    _CRinvoiceId = CrGetINVIDFromElem(this);

    //    toggleCreditUnitColumns(_CRinvoiceId);
    //    var amtType = getApplyAmtTypeINV(_CRinvoiceId);
    //    if (parseInt($('#hdfInvoiceDetailItemsCount' + "_" + _CRinvoiceId).val()) > 1) {
    //        var _requestCreditAmt = parseFloat($('#requestCreditAmt' + "_" + _CRinvoiceId).val());
    //        if ($('#invoiceCreditIsUpdate' + "_" + _CRinvoiceId).val() != '1') {
    //            for (var i = 0; i < parseInt($('#hdfInvoiceDetailItemsCount').val()); i++) {
    //                if (amtType == 'total') {
    //                    $('#item' + i + '_total' + "_" + _CRinvoiceId).val(_requestCreditAmt)
    //                    $('#item' + i + '_total' + "_" + _CRinvoiceId).trigger("change");

    //                    _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_total' + "_" + _CRinvoiceId).val())

    //                } else {
    //                    $('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).val(_requestCreditAmt)
    //                    $('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).trigger("change");
    //                    _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).val())
    //                }
    //            }
    //        }
    //    } else {
    //        if (amtType == 'total') {
    //            $('#item0_total' + "_" + _CRinvoiceId).val(removeFormatting($('#requestCreditAmt' + "_" + _CRinvoiceId).val()))
    //            $('#item0_total' + "_" + _CRinvoiceId).trigger("change");
    //        } else {
    //            $('#item0_creditAmt' + "_" + _CRinvoiceId).val(removeFormatting($('#requestCreditAmt' + "_" + _CRinvoiceId).val()))
    //            $('#item0_creditAmt' + "_" + _CRinvoiceId).trigger("change");
    //        }
    //    }

    //});
}


function getrdCreditUnitChangeINV(_CRinvoiceId) {
    

    toggleCreditUnitColumnsINV(_CRinvoiceId);
    var amtType = getApplyAmtTypeINV(_CRinvoiceId);
    if (parseInt($('#hdfInvoiceDetailItemsCount' + "_" + _CRinvoiceId).val()) > 1) {
        var _requestCreditAmt = parseFloat($('#requestCreditAmt' + "_" + _CRinvoiceId).val());
        if ($('#invoiceCreditIsUpdate' + "_" + _CRinvoiceId).val() != '1') {
            for (var i = 0; i < parseInt($('#hdfInvoiceDetailItemsCount').val()); i++) {
                if (amtType == 'total') {
                    $('#item' + i + '_total' + "_" + _CRinvoiceId).val(_requestCreditAmt)
                    $('#item' + i + '_total' + "_" + _CRinvoiceId).trigger("change");

                    _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_total' + "_" + _CRinvoiceId).val())

                } else {
                    $('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).val(_requestCreditAmt)
                    $('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).trigger("change");
                    _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).val())
                }
            }
        }
    } else {
        if (amtType == 'total') {
            $('#item0_total' + "_" + _CRinvoiceId).val(removeFormatting($('#requestCreditAmt' + "_" + _CRinvoiceId).val()))
            $('#item0_total' + "_" + _CRinvoiceId).trigger("change");
        } else {
            $('#item0_creditAmt' + "_" + _CRinvoiceId).val(removeFormatting($('#requestCreditAmt' + "_" + _CRinvoiceId).val()))
            $('#item0_creditAmt' + "_" + _CRinvoiceId).trigger("change");
        }
    }
}

function getApplyAmtTypeINV(_CRinvoiceId) {
    debugger
    return $("input[type='radio'][name=rdApplyAmtType_" + _CRinvoiceId+"]:checked").val();
}

function toggleApplyAmtFieldINV(_CRinvoiceId) {
    var amtType = getApplyAmtTypeINV(_CRinvoiceId);
    debugger
    $("#form_applycreditform_" + _CRinvoiceId + " .credit-line-item-amt-field").prop('readonly', true);
    $("#form_applycreditform_" + _CRinvoiceId + " .credit-line-item-total-field").prop('readonly', true);

    if (parseInt($('#hdfInvoiceDetailItemsCount' + "_" + _CRinvoiceId).val()) > 1) {
        if (amtType == 'total')
            $("#form_applycreditform_" + _CRinvoiceId + " .credit-line-item-total-field").prop('readonly', false);
        else
            $("#form_applycreditform_" + _CRinvoiceId + " .credit-line-item-amt-field").prop('readonly', false);
    }





    if (parseInt($('#hdfInvoiceDetailItemsCount' + "_" + _CRinvoiceId).val()) > 1) {
        var _requestCreditAmt = parseFloat($('#requestCreditAmt' + "_" + _CRinvoiceId).val());
        if ($('#invoiceCreditIsUpdate').val() != '1') {
            for (var i = 0; i < parseInt($('#hdfInvoiceDetailItemsCount' + "_" + _CRinvoiceId).val()); i++) {
                if (amtType == 'total') {
                    $('#item' + i + '_total' + "_" + _CRinvoiceId).val(_requestCreditAmt)
                    $('#item' + i + '_total' + "_" + _CRinvoiceId).trigger("change");

                    _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_total' + "_" + _CRinvoiceId).val())

                } else {
                    $('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).val(_requestCreditAmt)
                    $('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).trigger("change");
                    _requestCreditAmt = _requestCreditAmt - parseFloat($('#item' + i + '_creditAmt' + "_" + _CRinvoiceId).val())
                }
            }
        }
    } else {
        if (amtType == 'total') {
            $('#item0_total' + "_" + _CRinvoiceId).val(removeFormatting($('#requestCreditAmt' + "_" + _CRinvoiceId).val()))
            $('#item0_total' + "_" + _CRinvoiceId).trigger("change");
        } else {
            $('#item0_creditAmt' + "_" + _CRinvoiceId).val(removeFormatting($('#requestCreditAmt' + "_" + _CRinvoiceId).val()))
            $('#item0_creditAmt' + "_" + _CRinvoiceId).trigger("change");
        }
    }
}

function checkAutoApplyINV(elem, _CRinvoiceId) {
    debugger
    if ($("#chkAutoApply" + "_" + _CRinvoiceId).prop("checked") && $(elem).hasClass("credit-line-item-autoapply")) {
        handleAutoApplyINV($(elem).attr('name').split('_')[0], _CRinvoiceId);
        debugger
        if ($("#chkAutoApply" + "_" + _CRinvoiceId).prop("checked") == false && parseInt($('#hdfInvoiceFranchiseeItemsCount').val()) > 1) {
            $("#form_applycreditform_" + _CRinvoiceId + " .credit-franchisee-field").prop('readonly', false);
        } else {
            $("#form_applycreditform_" + _CRinvoiceId + " .credit-franchisee-field").prop('readonly', true);
        }

    }
}


function handleAutoApplyINV(item, _CRinvoiceId) {
    debugger
    var totalAmt = 0;
    var creditAmt = 0;
    //var creditAmt = parseFloatSafe($("#" + item + "_total").val());

    $("#form_applycreditform_" + _CRinvoiceId + " .credit-line-item-total-field").each(function () {
        creditAmt += parseFloatSafe($(this).val());

    });

    //if (parseInt($('#hdfInvoiceDetailItemsCount').val()) > 1) {
    //    creditAmt = parseFloatSafe($("#" + item + "_total").val());
    //} else {
    //    creditAmt = creditAmt < parseFloat($('#requestCreditAmt').val()) ? parseFloat($('#requestCreditAmt').val()) : creditAmt;
    //}


    $("#form_applycreditform_" + _CRinvoiceId + " .franchisee-row-item").each(function () {
        //if (this.value == item) {
        var key = $(this).prev().val();
        var prefix = "#bp" + key;
        totalAmt += parseFloatSafe($(prefix + "_oldBalance" + "_" + _CRinvoiceId).val());
        //}
    });

    if (totalAmt == 0)
        return;

    $("#form_applycreditform_" + _CRinvoiceId + " .franchisee-row-item").each(function () {
        //if (this.value == item) {

        var key = $(this).prev().val();
        var prefix = "#bp" + key;
        var amt = parseFloatSafe($(prefix + "_oldBalance" + "_" + _CRinvoiceId).val());
        var portion = amt / totalAmt;
        var franchiseeCreditPct = portion * 100.00;
        var franchiseeCreditAmt = portion * creditAmt;



        $(prefix + "_creditAmt" + "_" + _CRinvoiceId).val(franchiseeCreditAmt.toFixed(2));
        $(prefix + "_creditPct" + "_" + _CRinvoiceId).val(franchiseeCreditPct.toFixed(2));
        //}
    });
}

function setCreditAmtINV(amtm, Id, _CRinvoiceId) {
    debugger;
    if (!validateCreditAmountINV(_CRinvoiceId))
        amtm = getMaxCreditINV(_CRinvoiceId);

    $("#creditAmt" + "_" + _CRinvoiceId).val(formatCurrency(amtm));
    $("#requestCreditAmt" + "_" + _CRinvoiceId).val(amtm);

    updateSummaryDataINV(_CRinvoiceId);

    //if ($("#numLineItems").val() == 1)
    //    $("#item0_creditAmt").val($("#creditAmt").val());

    updateLineItemDataINV(Id, _CRinvoiceId);
    checkAutoApplyINV(Id, _CRinvoiceId);
    //checkAutoApply($("#item0_creditAmt"));
    updateFranchiseeDataINV(_CRinvoiceId);

    $("#requestCreditAmt" + "_" + _CRinvoiceId).trigger("change");

}

function refreshCreditFormINV(_CRinvoiceId) {
    updateSummaryDataINV(_CRinvoiceId);
    updateLineItemDataINV("", _CRinvoiceId);
    updateFranchiseeDataINV(_CRinvoiceId);
}

function updateSummaryDataINV(_CRinvoiceId) {
    var oldBalance = parseFloatSafe(removeFormatting($("#oldBalance" + "_" + _CRinvoiceId).val()));
    var creditAmt = parseFloatSafe(removeFormatting($("#creditAmt" + "_" + _CRinvoiceId).val()));
    $("#creditAmt" + "_" + _CRinvoiceId).val(formatCurrency(creditAmt));
    $("#newBalance" + "_" + _CRinvoiceId).val(formatCurrency(oldBalance - creditAmt));
}

function updateLineItemDataINV(Id, _CRinvoiceId) {

    var itemCnt = 0;

    while (true) {

        var prefix = "#item" + itemCnt;

        if ($(prefix + "_oldBalance" + "_" + _CRinvoiceId).length) {
            var item_taxRate = $(prefix + "_taxRate" + "_" + _CRinvoiceId).val();
            var item_oldBalance = $(prefix + "_oldBalance" + "_" + _CRinvoiceId).val();

            var item_creditAmt = 0;
            var item_tax = 0;
            var item_total = 0;

            var applyAmtType = getApplyAmtTypeINV(_CRinvoiceId);

            if (applyAmtType == 'amt') {
                item_creditAmt = parseFloatSafe(removeFormatting($(prefix + "_creditAmt" + "_" + _CRinvoiceId).val()));
                item_creditAmt = Math.max(0, Math.min(item_creditAmt, item_oldBalance));
                item_tax = (item_creditAmt * item_taxRate).toFixed(2);
                item_total = (item_creditAmt - item_tax).toFixed(2);
            }
            else if (applyAmtType == 'total') {
                item_total = parseFloatSafe(removeFormatting($(prefix + "_total" + "_" + _CRinvoiceId).val()));
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

            $(prefix + "_creditAmt" + "_" + _CRinvoiceId).val(item_creditAmt);
            $(prefix + "_tax" + "_" + _CRinvoiceId).val(item_tax);
            $(prefix + "_total" + "_" + _CRinvoiceId).val(item_total);
            $(prefix + "_newBalance" + "_" + _CRinvoiceId).val((item_oldBalance - item_creditAmt).toFixed(2));

            itemCnt++;
        }
        else
            break;
    }
}

function updateFranchiseeDataINV(_CRinvoiceId) {
    UpdateDateValidationINV(_CRinvoiceId);
    var creditUnit = $("#form_applycreditform_" + _CRinvoiceId + " input[type=radio][name=rdCreditUnit_" + _CRinvoiceId+"]:checked").val();
    $("#form_applycreditform_" + _CRinvoiceId + " .franchisee-row-key").each(function () {

        var prefix = "#bp" + this.value;
        var item = $(prefix + "_item" + "_" + _CRinvoiceId).val();
        var creditAmt = parseFloatSafe($("#" + item + "_total" + "_" + _CRinvoiceId).val());


        if (parseInt($('#hdfInvoiceDetailItemsCount' + "_" + _CRinvoiceId).val()) > 1) {
            creditAmt = parseFloatSafe($("#" + item + "_total" + "_" + _CRinvoiceId).val());
        } else {
            creditAmt = creditAmt < parseFloat($('#requestCreditAmt' + "_" + _CRinvoiceId).val()) ? parseFloat($('#requestCreditAmt' + "_" + _CRinvoiceId).val()) : creditAmt;
        }

        var oldBalance = $(prefix + "_oldBalance" + "_" + _CRinvoiceId).val();
        var franchiseeCreditAmt = parseFloatSafe($(prefix + "_creditAmt" + "_" + _CRinvoiceId).val());
        var franchiseeCreditPct = parseFloatSafe($(prefix + "_creditPct" + "_" + _CRinvoiceId).val());
        if (creditUnit == "amt") {
            //$(prefix + "_creditAmt").val(franchiseeCreditAmt.toFixed(2));
            var pct = (franchiseeCreditAmt * 100 / creditAmt);
            $(prefix + "_creditPct" + "_" + _CRinvoiceId).val(pct.toFixed(2));
        }
        else if (creditUnit == "pct") {
            $(prefix + "_creditAmt").val((franchiseeCreditPct * creditAmt / 100.00).toFixed(2));
        }
        franchiseeCreditAmt = $(prefix + "_creditAmt" + "_" + _CRinvoiceId).val();
        franchiseeCreditAmt = Math.max(0, Math.min(franchiseeCreditAmt, oldBalance));

        $(prefix + "_creditAmt" + "_" + _CRinvoiceId).val(franchiseeCreditAmt.toFixed(2));
        $(prefix + "_newBalance" + "_" + _CRinvoiceId).val((oldBalance - franchiseeCreditAmt).toFixed(2));
    });
}

function showFranchiseesForLineNumberINV(lineNo, _CRinvoiceId) {
    $("#form_applycreditform_" + _CRinvoiceId + " .franchisee-row" + "_" + _CRinvoiceId).hide();
    $("#form_applycreditform_" + _CRinvoiceId + " .franchisee-row-line" + lineNo + "_" + _CRinvoiceId).show();
}

function toggleCreditUnitColumnsINV(_CRinvoiceId) {
    var credUnit = $("#form_applycreditform_" + _CRinvoiceId + " input[type=radio][name=rdCreditUnit_" + _CRinvoiceId+"]:checked").val();
    $("#form_applycreditform_" + _CRinvoiceId + " .franchisee-amt-col").hide();
    $("#form_applycreditform_" + _CRinvoiceId + " .franchisee-pct-col").hide();
    $("#form_applycreditform_" + _CRinvoiceId + " .franchisee-" + credUnit + "-col").show();
}

function isExtraCreditINV(_CRinvoiceId) {
    return $("#isExtraCredit" + "_" + _CRinvoiceId).val() == "True";
}

function getMaxCreditINV(_CRinvoiceId) {
    var maxCredit = 0;
    if (isExtraCreditINV(_CRinvoiceId)) {
        maxCredit = removeFormatting($("#invAmt" + "_" + _CRinvoiceId).val());
    }
    else {
        maxCredit = removeFormatting($("#oldBalance" + "_" + _CRinvoiceId).val());
    }
    return maxCredit;
}

function validateCreditAmountINV(_CRinvoiceId) {

    var maxCredit = getMaxCreditINV(_CRinvoiceId);
    var maxType = "";
    if (isExtraCreditINV(_CRinvoiceId)) {
        maxType = "invoice amount";
    }
    else {
        maxType = "current balance";
    }


    if (removeFormatting($("#creditAmt" + "_" + _CRinvoiceId).val()) > maxCredit) {
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