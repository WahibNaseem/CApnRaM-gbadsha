if (typeof JK === "undefined") {
    JK = {};
}

if (typeof JK.CRM === "undefined") {
    JK.CRM = (function () { return {} })();
}

/**
 * Mask the input of common type data.
 */
JK.CRM.CRMFormInputMask = (function() {
    return {
        handleDateInputMask: function (parentSelector) {
            if ($(parentSelector).length === 0) return;
            $(parentSelector).inputmask("mm/dd/yyyy", {});   
        },
        handleNumericInputMask: function (parentSelector) {
            $(parentSelector).inputmask("numeric", {
                negative: false,
                scale: 2,
                rightAlign: false,
                autoUnmask: true,
                removeMaskOnSubmit: true,
            });
        },
        handleCurrencyInputMask: function (parentSelector) {
            $(parentSelector).inputmask("numeric", {
                decimal: ".",
                negative: false,
                scale: 2,
                groupSeparator: ",",
                digits: 2,
                autoGroup: true,
                prefix: "$",
                rightAlign: false,
                autoUnmask: true,
                removeMaskOnSubmit: true,
            });
        },
        handlePhoneInputMask: function (parentSelector) {
            if ($(parentSelector).length === 0) return;
            $(parentSelector).inputmask("mask", {
                "mask": "(999) 999-9999",
                autoUnmask: true,
                removeMaskOnSubmit: true,
            });
        }
    }
})();