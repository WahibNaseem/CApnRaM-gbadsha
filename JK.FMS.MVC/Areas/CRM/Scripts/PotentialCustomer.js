function getLeadDetail (id, flg) {
    if (!(isNaN(id))) {
        BindStage(id);
        $("#divBelow").css("display", "block");
        $("#lead_title").css('display', 'block');
        //var tblPotential = $('#PotentialLeadList').DataTable();

        $("#lead_title_id").html(id);
        $('#divcustomerList').hide();
        $(".checkboxcontactinfo_accountworkbook").prop("checked", false);
        $(".checkboxcontactinfo_BidSheetbook").prop("checked", false);
        $(".checkboxcontactinfo_Cleaningbook").prop("checked", false);
        /*Call the Call log table function to get initialize*/
      

        //Make a request  
        var URL = "/CRM/CustomerSales/GetPotentialDetail?accountId=" + id;
        //$.blockUI();
        $.ajax({
            url: URL,
            type: "GET",
            dataType: "json",
            cache: false,
            success: function (data) {
                $("#hdfAssigneeId").val(data.assigneeId);
                $("#hdCustomerId").val(data.result.CRM_AccountCustomerDetailId);
                $("#hdStageStatusId").val(data.result.StageStatus);
                //BindStage(data.result.CRM_AccountCustomerDetailId, data.result.CRM_AccountId, data.result.StageStatus, data)
                setDetailData(data);
                //setupForm();
                updateContactU(data);
                
                //$.unblockUI();
            },
            error: function (error) {
                JK.CRM.CRMApp.hideActivity("#addlead_modalview_footer");
                console.log(error.message);
                //$.unblockUI();
            }
        });
        if (flg == '1') {
            callLoginitTable(id);
        }
    }
}

function BindStage(accountid) {
    $.blockUI();
    var sURL = "/CRM/CustomerSales/PartialPotentialLoadPerf?accountId=" + accountid;
    $.ajax({
        type: "GET",
        url: sURL,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#' + lastDivstagestatus).html(data);
            setupForm();
            //updateContactU(data1);
            $.unblockUI();
        },
        error: function () {
            alert("Content load failed.");
            $.unblockUI();
        }
    });

}