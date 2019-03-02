/// <reference path="CallBacks.ts" />

module JKApi {

    export interface IAjaxService {
        getAjax(url: string, data: any, callBacks: any);
        postAjax(url: string, data: any, callBacks: any);
    }

    export class JQueryAjaxService implements IAjaxService {
        getAjax: (url: string, data: any, callB) => void;
        postAjax: (url: string, data: any, callBacks: any) => void;

        constructor(jQuery: any) {
            const self = this;
            var performAjaxRequest = (url: string, data: any, callbacks: Callbacks, type: string) => {
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

            self.getAjax = (url: string, data: any, callbacks: Callbacks) => {
                performAjaxRequest(url, data, callbacks, "GET");
            };

            self.postAjax = (url: string, data: any, callbacks: Callbacks) => {
                performAjaxRequest(url, data, callbacks, "POST");
            };
        }
    }
}



