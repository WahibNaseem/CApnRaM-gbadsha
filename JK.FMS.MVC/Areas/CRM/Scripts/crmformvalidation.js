if (typeof JK === "undefined") {
    JK = {};
}

if (typeof JK.CRM === "undefined") {
    JK.CRM = (function () { return {} })();
}

/**
 * Validate the user inputs in the form type.
 */
JK.CRM.CRMFormValidation = (function () {

    var handleFormValidation = function (parentSelector) {
        var form = $(parentSelector);
        var errorCallback = $(".alert-danger", form);
        var successCallback = $(".alert-success", form);
        form.validate({
            errorElement: "span",   //default input error message container
            errorClass: "help-block help-block-error",  // default input error message class
            focusInvalid: false,  // do not focus the last invalid input
            ignore: "",   //validate all fields including form hidden input
            messages: {
               // // TODO: Localizable
               // phonenumber: {
               //     required: "Enter a valid phone number"
               // },
               // datepicker: {
               //     required:"Date and Time required"
               // },
               // service: {
               //  required: " "
               //// minlength: jQuery.validator.format("Please select  at least {0} types of Service")
               // },
               // membership: {
               //     required:" "
               // }
            },
            rules: {
                phonenumber: {
                    required: true,
                    phoneUS: true
                },
                name: {
                    minlength: 2,
                    required: true                                                  
                },
                number:{
                    required:true,
                    number:true
                },
                email: {
                    required: true,
                    email: true
                },
                options2:{
                    required:true
                },
                membership:{
                  required:true
                },
                datepicker: {
                    required:true
                },
                service: {
                    required: true,
                    minlength:1
                },
                select: {
                    required: !0
                }
            },
            invalidHandler: function (event, validator) {
                successCallback.hide();
                errorCallback.show();
                App.scrollTo(errorCallback, -200);
            },
            errorPlacement: function (error, element) {
                var cont = $(element).parent(".input-group");
                if (cont.size() > 0) {
                    cont.after(error);
                } else {
                    element.after(error);
                }
            },
            highlight: function (element) {
                $(element).closest(".form-group").addClass("has-error");
            },
            unhighlight: function (element) {
                $(element).closest(".form-group").removeClass("has-error");
            },
            success: function (label) {
                label.closest(".form-group").removeClass("has-error");
            },
            submitHandler: function (form) {
                successCallback.show();
                errorCallback.hide();
            }
        });
    }

    return {
        init: function (parentSelector) {
            if ($(parentSelector).length === 0) return;
            handleFormValidation(parentSelector);
        },
        reset: function (parentSelector) {
            if ($(parentSelector).length === 0) return;
            var form = $(parentSelector);
            var validator = form.validate();
            validator.resetForm();
            form.find(".form-group").removeClass("has-error");
        }
    };
})();