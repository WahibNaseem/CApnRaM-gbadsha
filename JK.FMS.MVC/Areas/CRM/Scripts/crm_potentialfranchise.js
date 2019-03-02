
$(function () {
    $("#select_Type").change(function () {
        if ($(this).val() == "1") {
            $("#divOutcome").show();
            $("#divenddate").hide();
        } else {
            $("#divOutcome").hide();
        }
        if ($(this).val() == "3") {
            $("#divenddate").show();
        }
        if ($(this).val() == "2") {
            $("#divenddate").hide();
        }
    });

});

function ScheduleDate(jsonDate) {
    var newDate = new Date(parseInt(jsonDate.substring(6)));
    var options = {
        day: "numeric"
    };
    return newDate.toLocaleDateString("en-us", options);
}

function ScheduleMonth(jsonDate) {
    var newDate = new Date(parseInt(jsonDate.substring(6)));
    var options = {
        month: "short"
    };
    return newDate.toLocaleDateString("en-us", options);
}

function ScheduleTime(jsonDate) {
    var newDate = new Date(parseInt(jsonDate.substring(6)));
    var options = {
        hour: "2-digit", minute: "2-digit"
    };
    return newDate.toLocaleTimeString("en-us", options);
}

function formatJSONDate(jsonDate) {
    var newDate = new Date(parseInt(jsonDate.substring(6)));
    var options = {
        day: "numeric", month: "long", year: "numeric",
        hour: "2-digit", minute: "2-digit"
    };
    return newDate.toLocaleTimeString("en-us", options);
}



var resetAddActivityForm = function () {
    $("#select_Type").val("1");
    $("#select_OutCome").val("");
    $("#todo_startdate").val("");
    $("#todo_enddate").val("");
    $("#form_description").val("");

    $("#todo_startdate").css('border-color', '');
    $("#todo_starttime").css('border-color', '');
    $("#todo_enddate").css('border-color', '');
    $("#todo_endtime").css('border-color', '');
}

var resetAddScheduleForm = function () {
    $("#input_scheduletitle").val("");
    $("#input_scheduledescription").val("");
    $("#select_schedulestartdate").val("");
    $("#select_scheduleduration").val("");
}

var resetAddNoteForm = function () {
    $("#input_notetitle").val("");
    $("#input_notedescription").val("");
}

var resetStageClass = function () {
    $(".potential").removeClass("max-opp-stage-selected");
    $(".franchise-contract").removeClass("max-opp-stage-selected");
    $(".sign-agreement").removeClass("max-opp-stage-selected");
    $(".follow-up").removeClass("max-opp-stage-selected");

}

var saveNoteData = function () {
    $.blockUI();
    var noteForm = $("#addNote_form");
    if (!noteForm.valid()) {
        return;
    } else {
        var postData = {
            "CRM_AccountFranchiseDetailId": $("#accountfranchisedetailid").val(),
            "Title": $("#input_notetitle").val(),
            "Description": $("#input_notedescription").val()
        }

        $("cancel_addnote_button").prop("disabled", true);
        $("addnote_button").prop("disabled", true);
        //JK.CRM.CRMApp.showActivity("#addnote_modalview_footer");

        //Make a Ajax Request
        $.ajax({
            url: "/CRM/CRMPotentialFranchise/SaveFranchiseNote",
            type: "POST",
            data: postData,
            dataType: "json",
            cache: false,
            success: function (response) {
                $("#addNote_modalview").modal('hide');
                $("cancel_addnote_button").prop("disabled", false);
                $("addnote_button").prop("disabled", false);
                resetAddNoteForm();
                RefreshLead(response);
            },
            error: function (error) {
                console.log(error);
            }
        });
    }

}

var saveScheduleData = function () {
    var scheduleForm = $("#addSchedule_form");
    if (!scheduleForm.valid()) {
        return;
    } else {
        var postData = {
            "CRM_AccountFranchiseDetailId": $("#accountfranchisedetailid").val(),
            "Title": $("#input_scheduletitle").val(),
            "Description": $("#input_scheduledescription").val(),
            "StartDate": $("#select_schedulestartdate").val(),
            "Duration": $("#select_scheduleduration option:selected").val()
        }
        //Make a Ajax Request
        $("cancel_addschedule_button").prop("disabled", true);
        $("addschedule_button").prop("disabled", true);
        JK.CRM.CRMApp.showActivity("#addschedule_modalview_footer");

        $.ajax({
            url: "/CRM/CRMPotentialFranchise/SaveFranchiseScheduleSummary",
            type: "POST",
            data: postData,
            dataType: "json",
            cache: false,
            success: function (response) {
                RefreshLead(response);
            },
            error: function (error) {
                JK.CRM.CRMApp.hideActivity("#addschedule_modalview_footer");
                console.log(error.message);
            }
        });
    }

}

var saveActivityData = function () {
    var activityForm = $("#addActivity_form");
    if (!activityForm.valid()) {
        return;
    }
    
    else {
    var postData = {
        "CRM_AccountFranchiseDetailId": $("#accountfranchisedetailid").val(),
        "Note": $("#form_description").val(),
        "ActivityType": $("#select_Type option:selected").val(),
        "OutComeType": $("#select_OutCome option:selected").val(),
        "StartDate": $("#todo_startdate").val(),
        "StartTime": $("#todo_starttime").val(),
        "EndDate": $("#todo_enddate").val(),
        "EndTime": $("#todo_endtime").val(),
    };

    // Make a request
    $("#cancel_addactivity_button").prop("disabled", true);
    $("#addactivity_button").prop("disabled", true);
    JK.CRM.CRMApp.showActivity("#addActivity_modalview_footer");


    $.ajax({
        url: "/CRM/CRMPotentialFranchise/SaveFranchiseActivitySummary",
        type: "POST",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (response) {

            $("#addActivity_modalview").modal('hide');
            $("#cancel_addactivity_button").prop("disabled", false);
            $("#addactivity_button").prop("disabled", false);

            resetAddActivityForm();
            RefreshLead(response);
        },
        error: function (error) {
            JK.CRM.CRMApp.hideActivity("#addActivity_modalview_footer");
            console.log(error.message);
        }
    });
}
}

var saveDocument = function () {

    $.blockUI();
    var formdata = new FormData();
    formdata.append("CRM_AccountId", $("#accountid").val());
    formdata.append("CRM_AccountFranchiseDetailId", $("#accountfranchisedetailid").val());
    formdata.append("document", $('#fileDocument')[0].files[0]);
    formdata.append("Description", $("#input_filedescription").val());
    formdata.append("File_Title", $("#input_filename").val());

    $.ajax({
        url: "/CRM/CRMPotentialFranchise/SaveDocument",
        type: "POST",
        data: formdata,
        dataType: "json",
        cache: false,
        processData: false,
        contentType: false,
        success: function (response) {
            $("#uploaddocument_modalview").modal('hide');
            resetUploadform();
            RefreshLead(response);
            $.unblockUI();
        },
        error: function (error) {
            console.log(error);
        }
    });
    //  $("#uploaddocument_modalview").modal('hide');

}


var setupForm = function () {

    JK.CRM.CRMFormValidation.init("#form_contactleadgen");

    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_leadgenzipcode");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_leadgencontactphonenumber");

    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_contactphonenumber");


    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_budgetamount");

    /*Sign Agreement*/
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_terms");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_nopayments");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_planamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_ibamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_downpayment");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_interest");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_paymentamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_currentpayment");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_triggeramount");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".planamount_modal");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".ibamount_modal");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".downpayment_modal");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".interest_modal");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".paymentamount_modal");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".currentpayment_modal");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".triggeramount_modal");

    $('input.date-picker').datepicker({ Format: 'mm/dd/yy' });

    $('input.timepicker').timepicker({ timeFormat: "hh:mm p", dynamic: true });

    //Lead Generation Date And Time Events
    $("#input_leadgenstartdate").change(function () {
        $("#input_leadgenenddate").val($(this).val());
    });

    $("#input_leadgenstarttime").change(function () {
        $('#input_leadgenendtime').timepicker('setTime', incrementTimeByOne($(this).val()));
    });

    //Follow up Date And Time Events
    $("#input_followup_franchise_startdate").change(function () {
        $("#input_followup_franchise_enddate").val($(this).val());
    });

    $("#input_followup_franchise_starttime").change(function () {
        $('#input_followup_franchise_endtime').timepicker('setTime', incrementTimeByOne($(this).val()));
    });


    //$('#select_signingpurpose').change(function () {
    //    if ($(this).val() == "5" || $(this).val() == "2" || $(this).val() == "") {
    //        $("#div_franchise_reschedule").hide();
    //    }
    //    else {
    //        $("#div_franchise_reschedule").show();
    //    }
    //});

}

function incrementTimeByOne(timeStr) {
    var splitedTimeStr = timeStr.split(':');
    var hours = parseInt(splitedTimeStr[0]);
    var meridiem = splitedTimeStr[1].split(" ")[1];
    var minutes = splitedTimeStr[1].split(" ")[0];
    var nextHours = (hours + 1);
    var nextMeridiem;
    if (hours >= 11) {
        if (meridiem.toLowerCase() == "am") {
            nextMeridiem = "pm";
        } else if (meridiem.toLowerCase() == "pm") {
            nextMeridiem = "am";
        }
        if (nextHours > 12) {
            nextHours = nextHours - 12;
        }

    } else {
        nextMeridiem = meridiem;
    }
    return nextHours + ":" + minutes + " " + nextMeridiem;
}



function onClickFranchiseCRMSaveDoc() {

    var CRMAccFranchDetailId = $("#hdnCRMAccFranchDetailId").val();
    var CRMAccFranchDetailstage = $("#hdnCRMAccFranchDetailStage").val();
    if (CRMAccFranchDetailId != "") {

        if ($("#CRM_file_31").val() == "" && $("#hdndocFile_31").val() == "") {
            $('#errdoc_31').show();
            return false;
        }
        if ($("#CRM_file_32").val() == "" && $("#hdndocFile_32").val() == "") {
            $('#errdoc_32').show();
            return false;
        }
        //if ($("#CRM_file_4").val() == "" && $("#hdndocFile_4").val() == "") {
        //    $('#errdoc_4').show();
        //    return false;
        //}



        //if ($("#CRM_file_6").val() == "" && $("#hdndocFile_6").val() == "" && CRMAccCustDetailstage == 1) {
        //    $('#errdoc_6').show();
        //    return false;
        //}



        $('#errdoc_31').hide();
        $('#errdoc_32').hide();


        var strIds = $("#CRM_hdfFiletypeListIds").val();
        if (strIds != "") {
            var fileData = new FormData();

            var FiletypeIds = strIds.split(",");
            var selIds = "";
            for (var i = 0; i <= FiletypeIds.length; i++) {

                if (FiletypeIds[i] != "") {

                    var CRM_file = $("#CRM_file_" + FiletypeIds[i]);

                    if (CRM_file.length > 0 && CRM_file != null && CRM_file[0].files[0] != "" && CRM_file[0].files[0] != undefined) {

                        fileData.append("CRM_file_" + FiletypeIds[i], CRM_file[0].files[0]);
                        fileData.append("Document_" + FiletypeIds[i], FiletypeIds[i]);
                        if (selIds == "") {
                            selIds = FiletypeIds[i];
                        }
                        else {
                            selIds += "," + FiletypeIds[i];
                        }
                    }
                }
            }
            fileData.append("selIds", selIds);
            fileData.append("CRMAccFranchDetailId", CRMAccFranchDetailId);


            $.ajax({
                url: "/CRM/CRMPotentialFranchise/CRMSaveDocuments",
                type: "POST",
                contentType: false,
                processData: false,
                data: fileData,
                async: false,
                success: function (response) {
                    console.log(response);
                    refreshFranchiseDocuments(response);
                    //OpenCRMUploadDocumentPopup();
                    refreshFranchiseCRMDocumentDisplay(response.document);
                    //$(".clseditlnk").show();
                    $("#ModelFranchiseCRMUploadDocumentPopup").modal("hide");

                    $(".checkbox_questionaire").prop("checked", true);
                    $(".checkbox_acknowledgement").prop("checked", true);
                },
                error: function (error) {
                    console.log(error);
                }
            });
        }
    }
}


var refreshFranchiseCRMDocumentDisplay = function (doc) {

    $(".CRMFranchiseDocumentDisplay").html('');
    $.each(doc, function (index, value) {
        var filepath = $("#hdnsiteurl").val() + "/Areas/CRM/Documents/" + value.File_Name;
        if (value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'pdf') {
            $(".CRMFranchiseDocumentDisplay").append('<div class="col-md-3" style="margin-left: 30px;"><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_pdf.png" /></a></div>');
        }
        else if (value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'doc' || value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'docx') {
            $(".CRMFranchiseDocumentDisplay").append('<div class="col-md-3" ><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_Doc.png" /></a></div>');
        }
        else if (value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'xls' || value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'xlsx' || value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'xlsm') {
            $(".CRMFranchiseDocumentDisplay").append('<div class="col-md-3"><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_xls.png" /></a></div>');
        }
        else if (value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'txt') {
            $(".CRMFranchiseDocumentDisplay").append('<div class="col-md-3"><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_Text.png" /></a></div>');
        }
        else if (value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'jpg' || value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'png' || value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'jpeg') {
            $(".CRMFranchiseDocumentDisplay").append('<div class="col-md-3"><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_image.png" /></a></div>');
        }
        else {
            $(".CRMFranchiseDocumentDisplay").append('<div class="col-md-3"><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_defult.png" /></a></div>');
        }
    });
    $.unblockUI();
}

var refreshFranchiseDocuments = function (data) {

    //Empty the Document html
    $("#content_document").empty();
    if (data != null) {
        $.each(data.document, function (key, item) {

            var contentDocument = '<div class="mt-comment">';
            contentDocument += '<div class="mt-comment-img">';
            contentDocument += '<h3 class="mt-date-text text-center"></h3>';
            contentDocument += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
            contentDocument += '</div>';
            contentDocument += '<div class="mt-comment-body">';
            contentDocument += '<div class="mt-comment-info">';
            contentDocument += '<span class="mt-title">' + item.File_Title + '</span>';
            contentDocument += '</div>';
            contentDocument += '<div class="mt-comment-info">';
            contentDocument += '<span class="mt-scheduletime"><a href=/CRM/CustomerSales/DownloadDocument?id=' + item.CRM_DocumentId + '>Download</a></span>';
            contentDocument += '</div>';
            contentDocument += '<div class="mt-Note-text">' + item.Description + '</div>';
            contentDocument += '</div>';
            contentDocument += '</div>';
            contentDocument += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
            $("#content_document").append(contentDocument);
        });
    }
    $.unblockUI();

}

function RemoveCRMFranchiseDocument(Id, AccFranId) {
    $.ajax({
        url: "/CRM/CRMPotentialFranchise/RemoveCRM_Document?Id=" + Id + "&CRMAccFranId=" + AccFranId,
        type: "POST",
        contentType: false,
        processData: false,
        async: false,
        success: function (response) {
            OpenFranchiseCRMUploadDocumentPopup();
            refreshFranchiseCRMDocumentDisplay(response.document);
        },
        error: function (error) {
            console.log(error);
        }
    });
}



function OpenFranchiseCRMUploadDocumentPopup() {

    debugger;
    var franchiseStage = $("#hdfStageId").val();
    var aId = $("#hdfselectedaccountdetailid").val();
    if (Id = !"") {
        var sURL = "/CRM/CRMPotentialFranchise/CRMUploadDocumentPopup?id=" + aId + "&signagreement=" + franchiseStage;
        $.ajax({
            type: "GET",
            url: sURL,
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {

                console.log(data);
                $('#RenderFranchiseCRMUploadDocumentPopup').html(data);
                $("#ModelFranchiseCRMUploadDocumentPopup").modal({ backdrop: 'static' });
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
}

var setFranchiseDetailData = function (data) {


    $("#franchise_content_activity").empty();
    $("#franchise_content_todo").empty();
    $("#franchise_content_note").empty();
    $("#franchise_content_schedule").empty();
    $("#franchise_content_document").empty();


    if (data.franchisedetailvm != null) {

        if (data.franchisedetailvm["StageStatus"] == "31") { resetStageClass(); $(".leadgeneration").addClass("max-opp-stage-selected"); $(".potential").addClass("max-opp-stage-selected"); }
        if (data.franchisedetailvm["StageStatus"] == "28") { resetStageClass(); $(".leadgeneration").addClass("max-opp-stage-selected"); $(".potential").addClass("max-opp-stage-selected"); $(".franchise-contract").addClass("max-opp-stage-selected"); }
        if (data.franchisedetailvm["StageStatus"] == "29") { resetStageClass(); $(".leadgeneration").addClass("max-opp-stage-selected"); $(".potential").addClass("max-opp-stage-selected"); $(".franchise-contract").addClass("max-opp-stage-selected"); $(".follow-up").addClass("max-opp-stage-selected"); $(".sign-agreement").addClass("max-opp-stage-selected"); }
        if (data.franchisedetailvm["StageStatus"] == "24") { resetStageClass(); $(".leadgeneration").addClass("max-opp-stage-selected"); $(".potential").addClass("max-opp-stage-selected"); $(".franchise-contract").addClass("max-opp-stage-selected"); $(".follow-up").addClass("max-opp-stage-selected"); }
        if (data.franchisedetailvm["StageStatus"] == "32") { resetStageClass(); $(".leadgeneration").addClass("max-opp-stage-selected"); $(".potential").addClass("max-opp-stage-selected"); $(".franchise-contract").addClass("max-opp-stage-selected"); $(".follow-up").addClass("max-opp-stage-selected"); $(".sign-agreement").addClass("max-opp-stage-selected"); $(".soldtolegal").addClass("max-opp-stage-selected"); }
        if (data.franchisedetailvm != null) {
            BindFranchiseDetail(data.franchisedetailvm);
        }

        if (data.note.length > 0) {
            BindNote(data.note);
        }
        if (data.schedule.length > 0) {

        }
        if (data.activity.length > 0) {
            BindActivity(data.activity);
        }
        if (data.document.length > 0) {
            UpdateFranchiseLeadDocument(data.document);
        }
        if (data.initial != null) {
            UpdateInitialActivity(data.initial);
        }
        if (data.leadgen != null) {
            UpdatePotentialInquaryActivity(data.leadgen);
        }
        if (data.franchiseContract != null) {
            UpdateFranchiseContractActivity(data.franchiseContract);
        }
        if (data.franchiseFollowUp != null) {
            UpdateFranchiseFollowUpActivity(data.franchiseFollowUp);
        }

        getSchedules(data.franchisedetailvm["CRM_AccountFranchiseDetailId"]);

    }




}

var disabledFields = function () {


    $("#input_contactname").prop("disabled", true);
    $("#input_contactphonenumber").prop("disabled", true);
    $("#input_contactemail").prop("disabled", true);

    $("#input_franchisename").prop("disabled", true);
    $("#input_position").prop("disabled", true);
    $("#select_jobtype").prop("disabled", true);

    $("#input_addressline1").prop("disabled", true);
    $("#input_addressline2").prop("disabled", true);
    $("#input_city").prop("disabled", true);
    $("#input_county").prop("disabled", true);
    $("#select_state").prop("disabled", true);
    $("#input_homenumber").prop("disabled", true);
    $("#input_zipcode").prop("disabled", true);

}

var enabledFields = function () {


    $("#input_contactname").prop("disabled", false);
    $("#input_contactphonenumber").prop("disabled", false);
    $("#input_contactemail").prop("disabled", false);

    $("#input_franchisename").prop("disabled", false);
    $("#input_position").prop("disabled", false);
    $("#select_jobtype").prop("disabled", false);

    $("#input_addressline1").prop("disabled", false);
    $("#input_addressline2").prop("disabled", false);
    $("#input_city").prop("disabled", false);
    $("#input_county").prop("disabled", false);
    $("#select_state").prop("disabled", false);
    $("#input_homenumber").prop("disabled", false);
    $("#input_zipcode").prop("disabled", false);

}

var disabledDetail = function () {

    $("#select_stagestatus").prop("disabled", true);
    $("#select_providertype").prop("disabled", true);
    $("#select_providersource").prop("disabled", true);
    $("#input_budgetamount").prop("disabled", true);
}

var enabledDetail = function () {

    //$("#select_stagestatus").prop("disabled", false);
    $("#select_providertype").prop("disabled", false);
    $("#select_providersource").prop("disabled", false);
    $("#input_budgetamount").prop("disabled", false);
}

var EditData = function () {
    if ($("#editcontactinfo_button").text() != "Edit") {
        saveFranchiseContactInfo();
    }
    else {
        $("#editcontactinfo_button").text("Save");
        enabledFields();
        $("#cancelcontactinfo_button").show();
    }
}

var CancelBtn = function () {
    $("#cancelcontactinfo_button").hide();
    disabledFields();
    $("#editcontactinfo_button").text("Edit");
}

var btnPotential_Edit = function () {
    if ($("#btn_editpotential").text() != "Edit") {
        saveSummaryData();
    }
    else {

        $("#btn_editpotential").text("Save");
        enabledDetail();
        $("#btn_cancelpotential").show();
    }
}

var btnPotential_Cancel = function () {
    $("#btn_cancelpotential").hide();
    disabledDetail();
    $("#btn_editpotential").text("Edit");
}

var validateTodo = function () {

    var isValid = true;
    /*If the Select Type is Meeting*/
    if ($("#select_Type").val() == "3") {

        if ($("#todo_startdate").val().trim() == "") {
            $("#todo_startdate").css('border-color', 'red');
            $("#todo_starttime").css('border-color', 'red');
            if (isValid) { isValid = false; }
        }
        else {
            $("#todo_startdate").css('border-color', '');
            $("#todo_starttime").css('border-color', '');
        }

        if ($("#todo_enddate").val().trim() == "") {
            $("#todo_enddate").css('border-color', 'red');
            $("#todo_endtime").css('border-color', 'red');
            if (isValid) { isValid = false; }
        }
        else {
            $("#todo_enddate").css('border-color', '');
            $("#todo_endtime").css('border-color', '');
        }
    }
    else {

        if ($("#todo_startdate").val().trim() == "") {
            $("#todo_startdate").css('border-color', 'red');
            $("#todo_starttime").css('border-color', 'red');
            if (isValid) { isValid = false; }
        }
        else {
            $("#todo_startdate").css('border-color', '');
            $("#todo_starttime").css('border-color', '');
        }

    }

    if (isValid) {
        saveActivityData();
    }
}

var validateLeadGen = function () {
    var isValid = true;
    if ($("#input_leadgenname").val().trim() == "") { $("#input_leadgenname").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgenname").css('border-color', ''); }

    if ($("#input_leadgencity").val().trim() == "") { $("#input_leadgencity").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgencity").css('border-color', ''); }

    if ($("#input_leadgenaddress").val().trim() == "") { $("#input_leadgenaddress").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgenaddress").css('border-color', ''); }

    if ($("#input_leadgenstate").val().trim() == "") { $("#input_leadgenstate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgenstate").css('border-color', ''); }

    if ($("#input_leadgenzipcode").val().trim() == "") { $("#input_leadgenzipcode").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgenzipcode").css('border-color', ''); }

    if ($("#input_leadgencontactperson").val().trim() == "") { $("#input_leadgencontactperson").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgencontactperson").css('border-color', ''); }

    if ($("#input_leadgencontactphonenumber").val().trim() == "") { $("#input_leadgencontactphonenumber").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgencontactphonenumber").css('border-color', ''); }

    if ($("#input_leadgenstartdate").val().trim() == "") { $("#input_leadgenstartdate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgenstartdate").css('border-color', ''); }

    if ($("#input_leadgenstarttime").val().trim() == "") { $("#input_leadgenstarttime").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgenstarttime").css('border-color', ''); }


    if ($("#input_leadgenenddate").val().trim() == "") { $("#input_leadgenenddate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgenenddate").css('border-color', ''); }


    if ($("#input_leadgenendtime").val().trim() == "") { $("#input_leadgenendtime").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#input_leadgenendtime").css('border-color', ''); }


    if ($("#select_leadgencontactpurpose").val().trim() == "") { $("#select_leadgencontactpurpose").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    else { $("#select_leadgencontactpurpose").css('border-color', ''); }

    if (isValid) {



        validateScheduleAvailability(
            "#input_leadgenstartdate",
            "#input_leadgenstarttime",
            "#input_leadgenenddate",
            "#input_leadgenendtime",
            saveLeadGeneration);

    }

}

var validateFranchiseContract = function () {


    $(".errmsg").hide();

    var isValid = true;

    if (!($("#check_allprinciple").is(':checked'))) {
        $("#lbl_principleclose").css('color', 'red');
        $(window).scrollTop(10);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $("#lbl_principleclose").css('color', '');
    }
    if (!($("#radio_yessignagreement").is(':checked') || $("#radio_nosignagreement").is(':checked'))) {
        $("#lbl_signagreement").css('color', 'red');
        $(window).scrollTop(10);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $("#lbl_signagreement").css('color', '');
    }

    if ($("#select_companycreated").val().trim() == "") {
        $("#select_companycreated").css('border-color', 'red');
        $(window).scrollTop(10);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $("#select_companycreated").css('border-color', '');
    }
    if (!($("#radio_yesfranchiseapp").is(':checked') || $("#radio_nofranchiseapp").is(':checked'))) {
        $("#lbl_completefranchiseapp").css('color', 'red');
        $(window).scrollTop(10);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $("#lbl_completefranchiseapp").css('color', '');
    }


    if (!$("#checkbox_questionaire").is(":checked")) {
        $("#requiredDocumentsMsg").show();
        $(window).scrollTop(0);
        if (isValid == true) {
            isValid = false;
        }
    }
    if (!$("#checkbox_acknowledgement").is(":checked")) {
        $("#requiredDocumentsMsg").show();
        $(window).scrollTop(0);
        if (isValid == true) {
            isValid = false;
        }
    }


    if (isValid) {
        saveFranchiseContract();
    }
}

var valiadteFranchiseFollowup = function () {

    var isValid = true;
    /*Signing Schedule*/
    if ($("#input_signingschedule").val() == "") {
        $("#input_signingschedule").css('border-color', 'red');
        $(window).scrollTop(10);
        if (isValid) {
            isValid = false;
        }
    }
    else {
        $("#input_signingschedule").css('border-color', '');
    }

    /* Signing Schedule Purpose */
    if ($("#select_signingpurpose").val() == "") {
        $("#select_signingpurpose").css('border-color', 'red');
        $(window).scrollTop(10);
        if (isValid) {
            isValid = false;
        }
    }
    else {
        $("#select_signingpurpose").css('border-color', '');
    }


    /* Validate on Close dropdown value if contract signed or sale cycle close */
    if ($("#select_signingpurpose").val() != "" && ($("#select_signingpurpose").val() == "5" || $("#select_signingpurpose").val() == "2")) {


        /*Start Date Time*/
        if ($("#input_followup_franchise_startdate").val() == "") {
            $("#input_followup_franchise_startdate").css('border-color', 'red');
            $(window).scrollTop(10);
            if (isValid) {
                isValid = false;
            }
        }
        else {
            $("#input_followup_franchise_startdate").css('border-color', '');
        }

        /* End Date Time */
        if ($("#input_followup_franchise_enddate").val() == "") {
            $("#input_followup_franchise_enddate").css('border-color', 'red');
            $(window).scrollTop(10);
            if (isValid) {
                isValid = false;
            }
        }
        else {
            $("#input_followup_franchise_enddate").css('border-color', '');
        }
    }

    /* Available To Meet Purpose*/
    if ($("#select_followup_franchise_schedulepurpose").val() == "") {
        $("#select_followup_franchise_schedulepurpose").css('border-color', 'red');
        $(window).scrollTop(10);
        if (isValid) {
            isValid = false;
        }
    }
    else {
        $("#select_followup_franchise_schedulepurpose").css('border-color', '');
    }

    if (!$("#check_followup_additionalpurpose").is(":checked")) {
        $("#lbl_discloseadditionpurpose").css('color', 'red');
        $(window).scrollTop(10);
        if (isValid) {
            isValid = false;
        }
    }
    else {
        $("#lbl_discloseadditionpurpose").css('color', '');
    }


    if (!($("#followup_yes_creationconfirmed").is(':checked') || $("#followup_no_creationconfirmed").is(':checked'))) {
        $("#lbl_statuscreationconfirmed").css('color', 'red');
        $(window).scrollTop(10);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $("#lbl_statuscreationconfirmed").css('color', '');
    }


    if (!$("#check_followup_notifytraining").is(":checked")) {
        $("#lbl_notifynexttraning").css('color', 'red');
        $(window).scrollTop(10);
        if (isValid) {
            isValid = false;
        }
    }
    else {
        $("#lbl_notifynexttraning").css('color', '');
    }



    if (!($("#folowup_yeskeepactive").is(':checked') || $("#followup_nokeepactive").is(':checked'))) {
        $("#lbl_keepitactive").css('color', 'red');
        $(window).scrollTop(10);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $("#lbl_keepitactive").css('color', '');
    }

    if (isValid) {

        validateScheduleAvailability(
            "#input_followup_franchise_startdate",
            "#input_followup_franchise_starttime",
            "#input_followup_franchise_enddate",
            "#input_followup_franchise_endtime",
            saveFranchiseFollowup);

    }

}

var validateSignAgreement = function () {

    var isValid = true;

    /* Sign Franchise Agreement */
    if (!($("#radio_yes_signfranchiseagreement").is(":checked") || $("#radio_no_signfranchiseagreement").is(":checked"))) {
        if (isValid) { isValid = false; }
        $("#lbl_signfranchiseagreement").css('color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#lbl_signfranchiseagreement").css('color', '');
    }

    /* Guarantees Signed by All Non Officer/Partner Spouse */
    if (!($("#radio_yes_guarantees").is(":checked") || $("#radio_no_guarantees").is(":checked"))) {
        if (isValid) { isValid = false; }
        $("#lbl_guarantees").css('color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#lbl_guarantees").css('color', '');
    }

    /* Required documents uploaded */
    if (!($("#radio_yes_requireddocuments").is(":checked") || $("#radio_no_requireddocuments").is(":checked"))) {
        if (isValid) { isValid = false; }
        $("#lbl_requireddocuments").css('color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#lbl_requireddocuments").css('color', '');
    }

    /* To Legal for Background Check */
    if (!($("#radio_yes_backgroundcheck").is(":checked") || $("#radio_no_backgroundcheck").is(":checked"))) {
        if (isValid) { isValid = false; }
        $("#lbl_backgroundcheck").css('color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#lbl_backgroundcheck").css('color', '');
    }

    /* Date Sign */
    if ($("#input_datesign").val() == "") {
        if (isValid) { isValid = false; }
        $("#input_datesign").css('border-color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#input_datesign").css('border-color', '');
    }

    /* Terms (Yrs) */
    if ($("#input_terms").val() == "") {
        if (isValid) { isValid = false; }
        $("#input_terms").css('border-color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#input_terms").css('border-color', '');
    }

    /* Exp.Date */
    if ($("#input_expdate").val() == "") {
        if (isValid) { isValid = false; }
        $("#input_expdate").css('border-color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#input_expdate").css('border-color', '');
    }

    /* Plan Type */
    if ($("#input_plantype").val() == "") {
        if (isValid) { isValid = false; }
        $("#input_plantype").css('border-color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#input_plantype").css('border-color', '');
    }

    /* Plan Amount */
    if ($("#input_planamount").val() == "") {
        if (isValid) { isValid = false; }
        $("#input_planamount").css('border-color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#input_planamount").css('border-color', '');
    }





    /* No Of Payments */
    if ($("#input_nopayments").val() == "") {
        if (isValid) { isValid = false; }
        $("#input_nopayments").css('border-color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#input_nopayments").css('border-color', '');
    }

    /* Payment Start Date */
    if ($("#input_paymentstartdate").val() == "") {
        if (isValid) { isValid = false; }
        $("#input_paymentstartdate").css('border-color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#input_paymentstartdate").css('border-color', '');
    }

    /* Trigger Amt */
    if ($("#input_triggeramount").val() == "") {
        if (isValid) { isValid = false; }
        $("#input_triggeramount").css('border-color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#input_triggeramount").css('border-color', '');
    }

    /* Legal Obl.Start */
    if ($("#input_legaloblstart").val() == "") {
        if (isValid) { isValid = false; }
        $("#input_legaloblstart").css('border-color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#input_legaloblstart").css('border-color', '');
    }

    /* Legal Obl.Due */
    if ($("#input_legalobldue").val() == "") {
        if (isValid) { isValid = false; }
        $("#input_legalobldue").css('border-color', 'red');
        $(window).scrollTop(10);
    }
    else {
        $("#input_legalobldue").css('border-color', '');
    }

    if (isValid) {
        saveSignAgreement();
    }


}

var validateNextMeetingDateAgainstPrevious = function (isValid, dateElem, timeElem) {
    if (isValid == true) {
        var newStartDate;
        if (timeElem != null)
            newStartDate = moment($(dateElem).val() + " " + $(timeElem).val());
        else
            newStartDate = moment($(dateElem).val());
        if (!newStartDate.isValid())
            return isValid;

        var mostRecentDate = moment($("#hdnMostRecent").val());
        var isAfter = false;

        if (timeElem != null)
            isAfter = newStartDate > mostRecentDate;
        else
            isAfter = newStartDate.isSame(mostRecentDate, 'day') || newStartDate.isAfter(mostRecentDate, 'day');

        if (!isAfter) {
            $("#dateTooEarlyMsg").show();
            $(dateElem).css('border-color', 'red');
            if (timeElem != null)
                $(timeElem).css('border-color', 'red');
            $(window).scrollTop(0);
            isValid = false;
        }
        else {
            $("#dateTooEarlyMsg").hide();
            $(dateElem).css('border-color', '');
            if (timeElem != null)
                $(timeElem).css('border-color', '');
        }
    }
    return isValid;
}

var validateScheduleAvailability = function (startDateElem, startTimeElem, endDateElem, endTimeElem, saveFunc) {
    
    var startTime = moment($(startDateElem).val() + " " + $(startTimeElem).val());
    var endTime = moment($(endDateElem).val() + " " + $(endTimeElem).val());
    var userId = $("#hdfAssigneeId").val();

    $("#scheduleConflictMsg").hide();
    $(startDateElem).css('border-color', '');
    $(startTimeElem).css('border-color', '');
    $(endDateElem).css('border-color', '');
    $(endTimeElem).css('border-color', '');

    if (!startTime.isValid()) {
        $(startDateElem).css('border-color', 'red');
        $(startTimeElem).css('border-color', 'red');
    }
    if (!endTime.isValid()) {
        $(endDateElem).css('border-color', 'red');
        $(endTimeElem).css('border-color', 'red');
    }

    if (!startTime.isValid() || !endTime.isValid()) {
        console.log("I'm here")
        return;
    }
    console.log("I am here after validate")
    $.ajax({
        url: "/CRM/CustomerSales/ValidateScheduleAvailability?userId=" + userId + '&startDate=' + startTime.format("YYYY-MM-DD HH:mm:ss") + '&endDate=' + endTime.format("YYYY-MM-DD HH:mm:ss"),
        type: "GET",
        dataType: "json",
        cache: false,
        success: function (response) {

            if (response.result == true) {
                if (saveFunc != null)
                    saveFunc();
            }
            else {
                $(startDateElem).css('border-color', 'red');
                $(startTimeElem).css('border-color', 'red');
                $(endDateElem).css('border-color', 'red');
                $(endTimeElem).css('border-color', 'red');

                $("#scheduleConflictMsg").show();
            }

        },
        error: function (error) {


            console.log(error.message);
        }
    });
}

var saveLeadGeneration = function () {
    $.blockUI();

    var postData = {
        "CRM_AccountId": $("#accountid").val(),
        "CRM_AccountFranchiseDetailId": $("#accountfranchisedetailid").val(),
        "Name": $("#input_leadgenname").val(),
        "Address": $("#input_leadgenaddress").val(),
        "City": $("#input_leadgencity").val(),
        "State": $("#input_leadgenstate").val(),
        "ZipCode": $("#input_leadgenzipcode").val(),
        "ContactPerson": $("#input_leadgencontactperson").val(),
        "PhoneNumber": $("#input_leadgencontactphonenumber").val(),
        "StartDate": $("#input_leadgenstartdate").val(),
        "StartTime": $("#input_leadgenstarttime").val(),
        "EndDate": $("#input_leadgenenddate").val(),
        "EndTime": $("#input_leadgenendtime").val(),
        "PurposeId": $("#select_leadgencontactpurpose").val(),
        "Purpose": $("#select_leadgencontactpurpose").text(),
        "Note": $("#note_leadgen").val()
    }

    $("#saveleadgen_button").prop("disabled", true);
    JK.CRM.CRMApp.showActivity("#leadgen_action");
    $.ajax({
        url: "/CRM/CRMPotentialFranchise/SaveLeadGenerationData",
        type: "POST",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (response) {
            JK.CRM.CRMApp.hideActivity("#leadgen_action");
            $("#saveleadgen_button").prop("disabled", false);
            RefreshLead(response);
            $.unblockUI();
        },
        error: function (error) {
            JK.CRM.CRMApp.hideActivity("#leadgen_action");
            $("#saveleadgen_button").prop("disabled", false);
            console.log(error.message);
            $.unblockUI();
        }
    });
}

var saveFranchiseContract = function () {
    $.blockUI();

    var postData = {
        "CRM_AccountId": $("#accountid").val(),
        "CRM_AccountFranchiseDetailId": $("#accountfranchisedetailid").val(),
        "FranchiseQuestionaire": $("#checkbox_questionaire").is(':checked'),
        "DSignAcknowledgement": $("#checkbox_acknowledgement").is(':checked'),        
        "AllPrinciple": $("#check_allprinciple").is(':checked'),
        "SignAgreement": $("#radio_yessignagreement").is(':checked'),
        "CompanyCreated": $("#select_companycreated").val(),
        "FranchiseApp": $("#radio_yesfranchiseapp").is(':checked'),
        "Note": $("#note_franchisecontract").val()
    }

    $("#save_franchisecontract").prop('disabled', true);
    JK.CRM.CRMApp.showActivity("#franchisecontract_action");
    $.ajax({
        url: "/CRM/CRMPotentialFranchise/SaveFranchiseContractData",
        type: "POST",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (response) {
            JK.CRM.CRMApp.hideActivity("#franchisecontract_action");
            $("#save_franchisecontract").prop("disabled", false);
            RefreshLead();
            $.unblockUI();
        },
        error: function (error) {
            JK.CRM.CRMApp.hideActivity("#franchisecontract_action");
            $("#save_franchisecontract").prop("disabled", false);
            console.log(error.message);
            $.unblockUI();
        }
    });
}

var saveFranchiseFollowup = function () {

    $.blockUI();

    var postData = {
        "CRM_AccountId": $("#accountid").val(),
        "CRM_AccountFranchiseDetailId": $("#accountfranchisedetailid").val(),
        "SigningDate": $("#input_signingschedule").val(),
        "SigningTime": $("#input_signingtime").val(),
        "PurposeId": $("#select_signingpurpose").val(),
        "Purpose": $("#select_signingpurpose option:selected").text(),
        "StartDate": $("#input_followup_franchise_startdate").val(),
        "StartTime": $("#input_followup_franchise_starttime").val(),
        "EndDate": $("#input_followup_franchise_enddate").val(),
        "EndTime": $("#input_followup_franchise_endtime").val(),
        "FollowPurposeId": $("#select_followup_franchise_schedulepurpose").val(),
        "FollowPurpose": $("#select_followup_franchise_schedulepurpose option:selected").text(),
        "AdditionalPurpose": $("#check_followup_additionalpurpose").is(":checked"),
        "CreationConfirmed": $("#followup_yes_creationconfirmed").is(":checked"),
        "NotifyTraining": $("#check_followup_notifytraining").is(":checked"),
        "KeepItActive": $("#folowup_yeskeepactive").is(":checked"),
        "Note": $("#text_franchise_followup").val()
    }
    console.log(postData);

    $.ajax({
        url: "/CRM/CRMPotentialFranchise/SaveFranchiseFollowUp",
        type: "POST",
        dataType: "json",
        cache: false,
        data: postData,
        success: function (response) {
            RefreshLead();
            $.unblockUI();

        },
        error: function (error) {
            console.log(error);
        }

    });

}

var saveSignAgreement = function () {


    var postData = {
        "CRM_AccountId": $("#accountid").val(),
        "CRM_AccountFranchiseDetailId": $("#accountfranchisedetailid").val(),
        "SignFranchseAgreement": $("#radio_yes_signfranchiseagreement").is(":checked"),
        "GuaranteesSigned": $("#radio_yes_guarantees").is(":checked"),
        "RequiredDocument": $("#radio_yes_requireddocuments").is(":checked"),
        "LegalBackGround": $("#radio_yes_backgroundcheck").is(":checked"),
        "DateSign": $("#input_datesign").val(),
        "Term": $("#input_terms").val(),
        "ExpDate": $("#input_expdate").val(),
        "PlanType": $("#input_plantype").val(),
        "PlanAmount": $("#input_planamount").val(),
        "IBAmount": $("#input_ibamount").val(),
        "DownPayment": $("#input_downpayment").val(),
        "Interest": $("#input_interest").val(),
        "PaymentAmount": $("#input_paymentamount").val(),
        "NoPayments": $("#input_nopayments").val(),
        "CurrentPayment": $("#input_currentpayment").val(),
        "PaymentStartDate": $("#input_paymentstartdate").val(),
        "TriggerAmount": $("#input_triggeramount").val(),
        "LegalOblStart": $("#input_legaloblstart").val(),
        "LegalOblEnd": $("#input_legaloblend").val(),
        "LegalOblDue": $("#input_legalobldue").val(),
        "Note": $("#text_signagreement").val(),
    };

    $.ajax({
        url: "/CRM/CRMPotentialFranchise/SaveSignAgreement",
        type: "POST",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (response) {
            location.reload();
        },
        error: function (error) {
            alert("Contend load failed");
        }

    });
}


var saveFranchiseContactInfo = function () {

    var franchiseInfo = $("#form_referenceinfo");
    if (!franchiseInfo.valid()) {
        return;
    }

    $.blockUI();
    var postData = {
        "CRM_AccountId": $("#accountid").val(),
        "CRM_AccountFranchiseDetailId": $("#accountfranchisedetailid").val(),
        "ContactName": $("#input_contactname").val(),

        //  "LastName": $("#input_contactlastname").val(),
        "PhoneNumber": $("#input_contactphonenumber").val(),
        "EmailAddress": $("#input_contactemail").val(),

        "FranchiseName": $("#input_franchisename").val(),
        "Position": $("#input_position").val(),
        "JobType": $("#select_jobtype option:selected").val(),

        "Address1": $("#input_addressline1").val(),
        "Address2": $("#input_addressline2").val(),
        "City": $("#input_city").val(),

        "County": $("#input_county").val(),
        "State": $("#select_state option:selected").val(),
        "HomePhoneNumber": $("#input_homenumber").val(),

        "FaxNo": $("#input_faxnumber").val(),
        "ZipCode": $("#input_zipcode").val()
    };


    JK.CRM.CRMApp.showActivity("#contactinfo_action");
    $("#editcontactinfo_button").prop("disabled", true);
    $("#cancelcontactinfo_button").prop("disabled", true);

    $.ajax({
        url: "/CRM/CRMPotentialFranchise/SaveFranchiseContactInfo",
        type: "POST",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (response) {
            JK.CRM.CRMApp.hideActivity("#contactinfo_action");
            RefreshLead(response);
            $.unblockUI();

        },
        error: function (error) {
            JK.CRM.CRMApp.hideActivity("#contactinfo_action");
            $("#editcontactinfo_button").prop("disabled", false);
            $("#cancelcontactinfo_button").prop("disabled", false);
            $.unblockUI();
            console.log(error.message);
            $.unblockUI();
        }
    });
}

var saveSummaryData = function () {
    $.blockUI();
    var summaryForm = $("#form_summary");
    if (!summaryForm.valid()) {
        return;
    }
    else {
        var postData = {
            "CRM_AccountId": $("#accountid").val(),
            "CRM_AccountFranchiseDetailId": $("#accountfranchisedetailid").val(),
            "StageStatus": $("#select_stagestatus option:selected").val(),
            "ProviderType": $("#select_providertype option:selected").val(),
            "ProviderSource": $("#select_providersource option:selected").val(),
            "BudgetAmount": $("#input_budgetamount").val()
        };

        JK.CRM.CRMApp.showActivity("#summary_action");
        $("#btn_editpotential").prop("disabled", true);
        $("#btn_cancelpotential").prop("disabled", true);

        //Make a Ajax Request
        $.ajax({
            url: "/CRM/CRMPotentialFranchise/SaveFranchiseAccountSummary",
            type: "POST",
            data: postData,
            dataType: "json",
            cache: false,
            success: function (response) {
                JK.CRM.CRMApp.hideActivity("#summary_action");
                RefreshLead(response);
                $.unblockUI();
            },
            error: function (error) {
                JK.CRM.CRMApp.hideActivity("#summary_action");
                console.log(error.message);
                $.unblockUI();
            }
        });
    }
}

var enablecontent = function (stage) {

    if (stage == 27) {
        $("#contact_leadgen").show();


        //Follow up hide
        $("#contact_followup").hide();


        //Franchise Disclosure Hide
        $("#contact_frandis").hide();
    }
    if (stage == 24) {

        //Lead Generation Hide
        $("#contact_leadgen").hide();

        //Follow up show
        $("#contact_followup").show();

        //Franchise Disclosure hide
        $("#contact_frandis").hide();
    }
    if (stage == 28) {
        //Lead Generation Hide
        $("#contact_leadgen").hide();


        //Follow up show
        $("#contact_followup").hide();

        $("#contact_frandis").show();
    }
}

var disablecontent = function () {

    //Lead Generation
    $("#contact_leadgen").hide();
    $("#potentialdetail_leadgen").hide();
    $("#activity_leadgen").hide();
    $("#todo_leadgen").hide();
    $("#personalnote_leadgen").hide();
    $("#document_leadgen").hide();
    $("#schedule_leadgen").hide();

    //Franchise Disclosure
    $("#contact_frandis").hide();

    //Follow -up
    $("#contact_followup").hide();
}

function SendEmail() {
    var email = $("#input_contactemail").val();
    var mailto_link = 'mailto:' + email;
    window.open(mailto_link, 'emailWindow');
}

/*
   Button Event of Button
*/

//$("#addnote_button").click(function (e) {
//    e.preventDefault();
//    saveNoteData();

//});

$("#addschedule_button").click(function (e) {
    e.preventDefault();
    saveScheduleData();
});

$("#addactivity_button").click(function (e) {
    e.preventDefault();
    validateTodo();
});

$("#btnSubmitdocument").click(function (e) {
    e.preventDefault();
    saveDocument();
});

/* btn_franchisecontractdocupload */
$(document).on("click","#btn_franchisecontractdocupload", function (e) {    
    e.preventDefault();
    OpenFranchiseCRMUploadDocumentPopup();
});
/* btn_signagreementdocupload */
$(document).on("click","#btn_signagreementdocupload" ,function (e) {    
    e.preventDefault();
    OpenFranchiseCRMUploadDocumentPopup();
});




var getSchedules = function (accountFranchiseId) {


    $.ajax({
        url: "/CRM/CRMPotentialFranchise/GetPotentialLeadSchedule?accountFranchiseId=" + accountFranchiseId,
        type: "GET",
        dataType: "json",
        cache: false,
        success: function (response) {
            UpDateCalendar(response.calendar);
            BindSchedule(response);
            $("#hdnMostRecent").val(response.mostrecentDate);
        },
        error: function (error) {
            console.log(error);
        }

    });
}

var GetFranchisePotential = function () {
    $("#FranchisePotentialList").DataTable().ajax.reload();
}

//var refresh = function () {


//    $.blockUI();
//    getFranchiseDetail($('#hdfSelectedAccountId').val());
//    GetFranchisePotential();
//    $.unblockUI();
//}

var BindNote = function (note) {


    /*
     Note
     */
    $.each(note, function (key, item) {

        var contentNote = '<div class="mt-comment">';
        contentNote += '<div class="mt-comment-body">';
        contentNote += '<div class="mt-comment-info">';
        contentNote += '<span class="mt-title">' + item.Title + '</span>';
        contentNote += '</div>';
        contentNote += '<div class="mt-Note-text">' + item.Description + '</div>';
        contentNote += '</div>';
        contentNote += '</div>';
        contentNote += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#franchise_content_note").append(contentNote);
    });


    /*
      Hide the Note Modal with footer Progress Bar
    */

    JK.CRM.CRMApp.hideActivity("#addnote_modalview_footer");
    $("#addNote_modalview").modal('hide');


}

var BindSchedule = function (data) {

    /*
     empty the content of the schedule
     */
    $("#franchise_content_schedule").empty();
    if (data != null) {
        /**
          Schedule
        **/
        /*  var scheduleCalendar = $("#calender");
          scheduleCalendar.fullCalendar('destroy');
          scheduleCalendar.fullCalendar({
              header: {
                  left: 'title',
                  center: '',
                  right: 'prev , next , today , month , agendaWeek , agendaDay'
              },
              defaultView: 'month',
  
              events: data.calenderDates
          });*/

        $.each(data.schedule, function (key, item) {

            var contentSchedule = '<div class="mt-comment">';
            contentSchedule += '<div class="mt-comment-img">';
            contentSchedule += '<h3 class="mt-date-text text-center">' + ScheduleDate(item.StartDate) + '</h3>';
            contentSchedule += '<h5 class="mt-date-text text-center mt-schedule-monthSpace">' + ScheduleMonth(item.StartDate) + '</h5>';
            contentSchedule += '</div>';
            contentSchedule += '<div class="mt-comment-body">';
            contentSchedule += '<div class="mt-comment-info">';
            contentSchedule += '<span class="mt-title">' + item.Title + '</span>';
            contentSchedule += '</div>';
            contentSchedule += '<div class="mt-comment-info">';
            contentSchedule += '<span class="mt-scheduletime">' + ScheduleTime(item.StartDate) + ' - ' + ScheduleTime(item.EndDate) + '</span>';
            contentSchedule += '</div>';
            contentSchedule += '<div class="mt-Note-text">' + ((item.Description != "" && item.Description != null) ? item.Description : "") + '</div>';
            contentSchedule += '</div>';
            contentSchedule += '</div>';
            contentSchedule += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
            $("#franchise_content_schedule").append(contentSchedule);
        });

        JK.CRM.CRMApp.hideActivity("#addschedule_modalview_footer");
    }
}

var BindActivity = function (activity) {
    /** 
      Show the Activity
      **/

    $.each(activity, function (key, item) {
        var contentActivity = '<div class="mt-comment-body">';
        contentActivity += '<div class="mt-comment-info space-note">';
        if (item["ActivityType"] == "1")
            contentActivity += '<i class="fa fa-phone" style="float: left;"></i>';
        if (item["ActivityType"] == "2")
            contentActivity += '<i class="fa fa-envelope" style="float: left;"></i>';
        if (item["ActivityType"] == "3")
            contentActivity += '<i class="fa fa-calendar" style="float: left;"></i>';

        contentActivity += '<span class="mt-activitytitle">' + item["ActivityTypeName"] + '</span>';

        if (item["OutComeTypeName"] != null)
            contentActivity += '<span class="item-label badge-status">' + item["OutComeTypeName"] + '</span>';
        contentActivity += '<span class="mt-comment-date">' + formatJSONDate(item["TimeStamp"]) + '</span>';
        contentActivity += '</div>';
        contentActivity += '<div class="mt-Note-text ">' + item["Note"] + '</div>';
        contentActivity += '</div>';
        contentActivity += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#franchise_content_todo").append(contentActivity);
    });


    // $("#addActivity_modalview").modal('toggle');
    JK.CRM.CRMApp.hideActivity("#addActivity_modalview_footer");
}

var refreshFranchiseDocuments = function (data) {

    /**
    Show Documents
    **/
    //Empty the Document html
    $("#franchise_content_document").empty();
    if (data != null) {
        $.each(data.document, function (key, item) {

            var contentDocument = '<div class="mt-comment">';
            contentDocument += '<div class="mt-comment-img">';
            contentDocument += '<h3 class="mt-date-text text-center"></h3>';
            contentDocument += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
            contentDocument += '</div>';
            contentDocument += '<div class="mt-comment-body">';
            contentDocument += '<div class="mt-comment-info">';
            contentDocument += '<span class="mt-title">' + item.File_Title + '</span>';
            contentDocument += '</div>';
            contentDocument += '<div class="mt-comment-info">';
            contentDocument += '<span class="mt-scheduletime"><a href=/CRM/CRMPotentialFranchise/DownloadDocument?id=' + item.CRM_DocumentId + '>Download</a></span>';
            contentDocument += '</div>';
            contentDocument += '<div class="mt-Note-text">' + item.Description + '</div>';
            contentDocument += '</div>';
            contentDocument += '</div>';
            contentDocument += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
            $("#franchise_content_document").append(contentDocument);
        });
    }

}

var refreshFranchiseInfo = function (data) {


    $("#input_contactname").val(data.accountFranchise["ContactName"]);
    // $("#input_contactlastname").val(data.accountFranchise["LastName"]);
    $("#input_contactphonenumber").val(data.accountFranchise["PhoneNumber"]);
    $("#input_contactemail").val(data.accountFranchise["EmailAddress"]);

    $("#input_franchisename").val(data.accountFranchiseDetail["FranchiseeName"]);
    $("#input_position").val(data.accountFranchiseDetail["Position"]);
    $("#select_jobtype option:selected").val(data.accountFranchiseDetail["JkFull"]);

    $("#input_addressline1").val(data.accountFranchiseDetail["Address1"]);
    $("#input_addressline2").val(data.accountFranchiseDetail["Address2"]);
    $("#input_city").val(data.accountFranchiseDetail["City"]);

    $("#input_county").val(data.accountFranchiseDetail["County"]);
    $("#select_state option:selected").val(data.accountFranchiseDetail["State"]);
    $("#input_homenumber").val(data.accountFranchiseDetail["HomeNumber"]);

    $("#input_faxnumber").val(data.accountFranchiseDetail["FaxNumber"]);
    $("#input_zipcode").val(data.accountFranchiseDetail["ZipCode"]);

    /*
    Save btn text into Edit ,Cancel hide ,Disable field
    */
    $("#editcontactinfo_button").text("Edit");
    $("#cancelcontactinfo_button").hide();

    disabledFields();
    $("#editcontactinfo_button").prop("disabled", false);
    $("#cancelcontactinfo_button").prop("disabled", false);
}

var refreshFranchiseSummary = function (data) {
    /**
   * Lead Detail
   */
    $("#select_stagestatus").val(data.result["StageStatus"]);
    $("#select_providertype").val(data.result["ProviderType"]);
    $("#select_providersource").val(data.result["ProviderSource"]);
    $("#input_budgetamount").val(data.result["AmtToInvest"]);

    /*
   Save btn text into Edit ,Cancel hide ,Disable field
   */
    $("#editpotential_button").text("Edit");
    $("#cancelpotential_button").hide();

    disabledDetail();
    $("#editpotential_button").prop("disabled", false);
    $("#cancelpotential_button").prop("disabled", false);
}

var BindFranchiseDetail = function (franchiseDetail) {

    $("#accountid").val(franchiseDetail["CRM_AccountId"]);
    $("#input_contactname").val(franchiseDetail["ContactName"]);
    $("#input_contactphonenumber").val(franchiseDetail["PhoneNumber"]);
    $("#input_contactemail").val(franchiseDetail["EmailAddress"]);

    $("#input_franchisename").val(franchiseDetail["FranchiseeName"]);
    $("#input_position").val(franchiseDetail["Position"]);
    $("#select_jobtype").val(franchiseDetail["JkFull"]);

    $("#input_addressline1").val(franchiseDetail["Address1"]);
    $("#input_addressline2").val(franchiseDetail["Address2"]);
    $("#input_city").val(franchiseDetail["City"]);
    $("#input_county").val(franchiseDetail["County"]);
    $("#select_state").val(franchiseDetail["State"]);
    $("#input_homenumber").val(franchiseDetail["HomeNumber"]);
    $("#input_faxnumber").val(franchiseDetail["FaxNumber"]);
    $("#input_zipcode").val(franchiseDetail["ZipCode"]);

    $("#accountfranchisedetailid").val(franchiseDetail["CRM_AccountFranchiseDetailId"]);
    $("#select_stagestatus").val(franchiseDetail["StageStatus"]);
    $("#select_providertype").val(franchiseDetail["ProviderType"]);
    $("#select_providersource").val(franchiseDetail["ProviderSource"]);
    $("#input_budgetamount").val(franchiseDetail["AmtToInvest"]);

    /*Bind the hidden Ids*/
    $("#hdfSelectedAccountId").val(franchiseDetail.CRM_AccountId);
    $("#hdfselectedaccountdetailid").val(franchiseDetail.CRM_AccountFranchiseDetailId);
    $("#hdfStageId").val(franchiseDetail.StageStatusId);
}


var RefreshLead = function () {

    $.blockUI();
    getFranchiseDetail($('#hdfSelectedAccountId').val());
    GetFranchisePotential();
    $.unblockUI();
}

var UpDateCalendar = function (data, calendarSelector) {
   
    if (calendarSelector == null)
    {
        calendarSelector = "#calendar";
    }

    /*If Data is not null*/
    if (data != null) {

        var scheduleCalendar = $(calendarSelector);

        scheduleCalendar.fullCalendar('destroy');
        scheduleCalendar.fullCalendar({
            header: {
                left: 'title',
                center: '',
                right: 'prev,next,today,month,agendaWeek,agendaDay'
            },
            eventClick: function (calEvent, jsEvent, view) {
                $("#input_scheduletitle_detail").val(calEvent.title);
                $("#input_schedulelocation_detail").val(calEvent.Location);
                //  calEvent.IsAllDay == true ? $("#checkbox_alldayevent").prop("checked", true) : $("#checkbox_alldayevent").prop("checked", false);
                $("#input_scheduledescription_detail").val(calEvent.Description);

                $("#input_schedulestartdate_detail").val(calEvent.StartDate);
                $("#input_scheduletype_detail").val(calEvent.CRM_ScheduleTypeId);
                $("#input_scheduleenddate_detail").val(calEvent.EndDate);
                $("#input_schedulestarttime_detail").val(calEvent.StartTime);
                $("#input_scheduleendtime_detail").val(calEvent.EndTime);
                $("#detailschedule_modalview").modal('show');
            },

            defaultView: 'month',

            events: data

        });
    }
}




var UpdateInitialActivity = function (initial) {


    /*Set initial Data in Activity */


    var contentInitial = '<div class="mt-comment">';
    contentInitial += '<div class="mt-comment-img">';
    contentInitial += '<h3 class="mt-date-text text-center"></h3>';
    contentInitial += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
    contentInitial += '</div>';
    contentInitial += '<div class="mt-comment-body">';
    contentInitial += '<div class="mt-comment-info">';
    contentInitial += '<span class="mt-title">' + "Lead Generation" + '</span>';
    contentInitial += '<span class="mt-comment-date">' + formatJSONDate(initial["CreatedDate"]) + '</span>';
    contentInitial += '</div>';
    contentInitial += '<div class="mt-Note-text" style="margin-top:4px;">' + initial["Note"] + '</div>';
    contentInitial += '<div class="mt-comment-info">';
    contentInitial += '<span class="mt-comment-date" style="margin-top:8px;"><a onclick="OpenInitialPP(' + initial["CRM_AccountFranchiseDetailId"] + ')" data-toggle="modal">View Detail</a></span>';
    contentInitial += '</div>';
    contentInitial += '</div>';
    contentInitial += '</div>';
    contentInitial += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
    $("#franchise_content_activity").append(contentInitial);

}

var UpdatePotentialInquaryActivity = function (leadgen) {



    var contentInitial = '<div class="mt-comment">';
    contentInitial += '<div class="mt-comment-img">';
    contentInitial += '<h3 class="mt-date-text text-center"></h3>';
    contentInitial += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
    contentInitial += '</div>';
    contentInitial += '<div class="mt-comment-body">';
    contentInitial += '<div class="mt-comment-info">';
    contentInitial += '<span class="mt-title">' + "Potential/Inquary" + '</span>';
    contentInitial += '<span class="mt-comment-date">' + formatJSONDate(leadgen["CreatedDate"]) + '</span>';
    contentInitial += '</div>';
    contentInitial += '<div class="mt-Note-text" style="margin-top:4px;">' + leadgen["Note"] + '</div>';
    contentInitial += '<div class="mt-comment-info">';
    contentInitial += '<span class="mt-comment-date" style="margin-top:8px;"><a onclick="OpenPotentialInquaryPP(' + leadgen["CRM_AccountFranchiseDetailId"] + ')" data-toggle="modal">View Detail</a></span>';
    contentInitial += '</div>';
    contentInitial += '</div>';
    contentInitial += '</div>';
    contentInitial += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
    $("#franchise_content_activity").append(contentInitial);

}

var UpdateFranchiseContractActivity = function (franchisecontract) {


    var contentInitial = '<div class="mt-comment">';
    contentInitial += '<div class="mt-comment-img">';
    contentInitial += '<h3 class="mt-date-text text-center"></h3>';
    contentInitial += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
    contentInitial += '</div>';
    contentInitial += '<div class="mt-comment-body">';
    contentInitial += '<div class="mt-comment-info">';
    contentInitial += '<span class="mt-title">' + "Franchise Contract" + '</span>';
    contentInitial += '<span class="mt-comment-date">' + formatJSONDate(franchisecontract["CreatedDate"]) + '</span>';
    contentInitial += '</div>';
    contentInitial += '<div class="mt-Note-text" style="margin-top:4px;">' + franchisecontract["Notes"] + '</div>';
    contentInitial += '<div class="mt-comment-info">';
    contentInitial += '<span class="mt-comment-date" style="margin-top:8px;"><a onclick="OpenFrachiseContractPP(' + franchisecontract["CRM_AccountFranchiseDetailId"] + ')" data-toggle="modal">View Detail</a></span>';
    contentInitial += '</div>';
    contentInitial += '</div>';
    contentInitial += '</div>';
    contentInitial += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
    $("#franchise_content_activity").append(contentInitial);

}

var UpdateFranchiseFollowUpActivity = function (franchisefollowup) {


    var contentInitial = '<div class="mt-comment">';
    contentInitial += '<div class="mt-comment-img">';
    contentInitial += '<h3 class="mt-date-text text-center"></h3>';
    contentInitial += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
    contentInitial += '</div>';
    contentInitial += '<div class="mt-comment-body">';
    contentInitial += '<div class="mt-comment-info">';
    contentInitial += '<span class="mt-title"> Follow-Up </span>';
    contentInitial += '<span class="mt-comment-date">' + formatJSONDate(franchisefollowup["CreatedDate"]) + '</span>';
    contentInitial += '</div>';
    contentInitial += '<div class="mt-Note-text" style="margin-top:4px;">' + franchisefollowup["Note"] + '</div>';
    contentInitial += '<div class="mt-comment-info">';
    contentInitial += '<span class="mt-comment-date" style="margin-top:8px;"><a onclick="OpenFrachiseFollowUpPP(' + franchisefollowup["CRM_AccountFranchiseDetailId"] + ')" data-toggle="modal">View Detail</a></span>';
    contentInitial += '</div>';
    contentInitial += '</div>';
    contentInitial += '</div>';
    contentInitial += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
    $("#franchise_content_activity").append(contentInitial);
}

var UpdateFranchiseLeadDocument = function (document) {

    /*This check is use to  Display the document
     * icon on the franchise contract While Showing Detail*/
    if (document.length > 0) {
        refreshFranchiseCRMDocumentDisplay(document);
        $.each(document, function (key, item) {
            if (item.FileTypeListId == "31") {
                $("#checkbox_questionaire").prop("checked", true);
            }
            if (item.FileTypeListId == "32") {
                $("#checkbox_acknowledgement").prop("checked", true);}
        });
    }
    else {
        $("#CRMFranchiseDocumentDisplay").html('');
    }

    $.each(document, function (key, item) {

        var imgExt = item.File_Name.toString().substring(item.File_Name.toString().lastIndexOf('.') + 1);
        var imageFilename = 'if_defult.png';

        if (imgExt == "pdf") imageFilename = 'if_pdf.png';
        else if (imgExt == "doc" || imgExt == "docx") imageFilename = 'if_Doc.png';
        else if (imgExt == "xls" || imgExt == "xlsx" || imgExt == "xlsm") imageFilename = 'if_xls.png';
        else if (imgExt == "txt") imageFilename = 'if_Text.png';
        else if (imgExt == "jpg" || imgExt == "jpeg" || imgExt == "png") imageFilename = 'if_image.png';

        var contentDocument = '<div class="mt-comment">';
        contentDocument += '<div class="mt-comment-img">';
        contentDocument += '<img src="/Images/' + imageFilename + '" style="border-radius: 0% !important">';
        contentDocument += '<h3 class="mt-date-text text-center"></h3>';
        contentDocument += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
        contentDocument += '</div>';
        contentDocument += '<div class="mt-comment-body">';
        contentDocument += '<div class="mt-comment-info">';
        contentDocument += '<span class="mt-title">' + item.File_Title + '</span>';
        contentDocument += '</div>';
        contentDocument += '<div class="mt-comment-info">';
        contentDocument += '<span class="mt-scheduletime"><a target="_blank" href="/CRM/CustomerSales/DownloadDocument?id=' + item.CRM_DocumentId + '">Download</a></span>';
        contentDocument += '</div>';
        contentDocument += '<div class="mt-Note-text">' + item.Description + '</div>';
        contentDocument += '</div>';
        contentDocument += '</div>';
        contentDocument += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#franchise_content_document").append(contentDocument);
    });
}




