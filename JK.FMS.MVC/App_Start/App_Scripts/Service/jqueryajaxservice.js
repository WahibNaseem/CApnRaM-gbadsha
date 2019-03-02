/// <reference path="CallBacks.ts" />
var JKApi;
(function (JKApi) {
    var JQueryAjaxService = /** @class */ (function () {
        function JQueryAjaxService(jQuery) {
            var self = this;
            var performAjaxRequest = function (url, data, callbacks, type) {
                jQuery.ajax({
                    url: url,
                    data: data,
                    type: type,
                    success: callbacks.success,
                    error: callbacks.error,
                    traditional: true,
                    cache: false
                });
            };
            self.getAjax = function (url, data, callbacks) {
                performAjaxRequest(url, data, callbacks, "GET");
            };
            self.postAjax = function (url, data, callbacks) {
                performAjaxRequest(url, data, callbacks, "POST");
            };
        }
        return JQueryAjaxService;
    }());
    JKApi.JQueryAjaxService = JQueryAjaxService;
})(JKApi || (JKApi = {}));
//# sourceMappingURL=jqueryajaxservice.js.map