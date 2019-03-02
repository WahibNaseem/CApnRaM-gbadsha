/// <reference path="../service/callbacks.ts" />
/// <reference path="../service/jqueryajaxservice.ts" />
/// <reference path="../../scripts/typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../scripts/typings/jquery/jquery.d.ts" />
var Customer;
(function (Customer) {
    var ajaxService = JKApi.JQueryAjaxService;
    var callBacks = JKApi.Callbacks;
    var ajax = new ajaxService($);
    var cb = new callBacks();
    var Maintenance = /** @class */ (function () {
        function Maintenance() {
        }
        Maintenance.prototype.saveCustomerInfo = function () {
            cb.success = function (response) {
                if (response.msg === "OK") {
                    alert("Customer has been saved.");
                }
                else {
                    alert(response.msg);
                }
            };
            var data = {
                mainAddressName: $("#txtMainAddressName").val(),
                mainAddress1: $("#txtMainAddress1").val(),
                mainAddress2: $("#txtMainAddress2").val(),
                mainAddressCity: $("#txtMainAddressCity").val(),
                mainAddressState: $("#ddlMainAddressState").val(),
                mainAddressZip: $("#txtMainAddressZip").val(),
                mainAddressPhone: $("#txtMainAddressMobilePhone").val(),
                mainAddressFax: $("#txtMainAddressFax").val(),
                billingAddressName: $("#txtBillingAddressName").val(),
                billingAddressAttention: $("#txtBillingAddressAttention").val(),
                billingAddress1: $("#txtBillingAddress1").val(),
                billingAddress2: $("#txtBillingAddress2").val(),
                billingAddressCity: $("#txtBillingAddressCity").val(),
                billingAddressState: $("#ddlBillingAddressState").val(),
                billingAddressZip: $("#txtBillingAddressZip").val(),
                contactInformationFirstName: $("#txtContactInfoFirstName").val(),
                contactInformationLastName: $("#txtContactInfoLastName").val(),
                contactInformationTitle: $("#txtContactInfoTitle").val(),
                contactInformationPhone: $("#txtContactInfoPhone").val(),
                contactInformationExt: $("#txtContactInfoExt").val(),
                contactInformationCell: $("#txtContactInfoCell").val(),
                contactInformationFax: $("#txtContactInfoFax").val(),
                contactInformationEmail: $("#txtContactInfoEmail").val(),
                billingContactInfoFirstName: $("#txtBillingContactInfoFirstName").val(),
                billingContactInfoLastName: $("#txtBillingContactInfoLastName").val(),
                billingContactInfoTitle: $("#txtBillingContactInfoTitle").val(),
                billingContactInfoPhone: $("#txtBillingContactInfoPhone").val(),
                billingContactInfoExt: $("#txtBillingContactInfoExt").val(),
                billingContactInfoCell: $("#txtBillingContactInfoCell").val(),
                billingContactInfoFax: $("#txtBillingContactInfoFax").val(),
                billingContactInfoEmail: $("#txtBillingContactInfoFax").val(),
                isNationalAccount: $("#chknationalaccount").is(":checked"),
                isReference: $("#chkreference").is(":checked"),
                isParentAccount: $("#chkparent").is(":checked")
            };
            ajax.postAjax($("#lnkSaveCustomer").val(), data, cb);
        };
        return Maintenance;
    }());
    Customer.Maintenance = Maintenance;
})(Customer || (Customer = {}));
var maintenance = new Customer.Maintenance();
//# sourceMappingURL=maintenance.js.map