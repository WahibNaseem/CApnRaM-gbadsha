(function ($) {

    function AddObj() {
        var $this = this;

        function initilizeUser() {
            var formAccountngFeeRebateProcess = new Global.FormHelper($("#frm-Process-AccountingFeeRebate"), { updateTargetId: "validation-summary" })
        }

        $this.init = function () {
            initilizeObj();
        }
    }
    $(function () {
        var self = new AddObj();
        self.init();
    })

}(jQuery))