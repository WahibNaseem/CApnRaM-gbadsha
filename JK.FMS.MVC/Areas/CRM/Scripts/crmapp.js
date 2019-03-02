if (typeof JK === "undefined") {
    JK = {};
}

if (typeof JK.CRM === "undefined") {
    JK.CRM = (function () { return {} })();
}

/**
 * Core script to handle the entire CRM application.
**/
JK.CRM.CRMApp = (function () {

    return {
        showActivity: function(parentId) {
            App.blockUI({ animate: true, target: parentId, overlayColor: "#FFF" });
        },
       
        hideActivity: function(parentId) {
            App.unblockUI(parentId);
        }
    }

})();