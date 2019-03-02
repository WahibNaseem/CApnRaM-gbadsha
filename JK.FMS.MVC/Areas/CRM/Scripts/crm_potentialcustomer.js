
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


//Follow up Reschedule
$(function () {
    $(".select_closetype").change(function () {
        if ($(this).val() == "1" || $(this).val() == "2" || $(this).val() == "5") {
            $(".div_reschedule").hide();

        } else {
            $(".div_reschedule").show();
        }
    });
});

/*
Contact info Document upload
*/




function myRemove(button) {
    var id = button.name;
    var totalfile = $("#contact_bidfileDocument")[0].files;
    var fileNameList = [];
    var RemoveFiles = [];
    $.each(totalfile, function (index, value) {
        var filename = value.name;
        var fileNameExt = filename;
        if (id.name.matches(value.name)) {
            RemoveFiles.push(fileNameExt);
        }
        else {
            fileNameList.push(fileNameExt);
        }

        $("#contact_listDiv").html('');
        $.each(fileNameList, function (index, value) {
            if (value.substr(value.lastIndexOf('.') + 1) == 'pdf') {
                $("#contact_listDiv").append('<div class="col-md-3"><img src="/Images/if_pdf.png" /><button class="btn" name=' + value + ' onclick="myRemove(this)" style="padding-top:0px;padding-bottom:0px;background: white;"><i class="fa fa-remove"></i></button></div>');
            }
            else if (value.substr(value.lastIndexOf('.') + 1) == 'doc' || value.substr(value.lastIndexOf('.') + 1) == 'docx') {
                $("#contact_listDiv").append('<div class="col-md-3"><img src="/Images/if_Doc.png" /><button class="btn" name=' + value + ' onclick="myRemove(this)" style="padding-top:0px;padding-bottom:0px;background: white;"><i class="fa fa-remove"></i></button></div>');
            }
            else if (value.substr(value.lastIndexOf('.') + 1) == 'xls' || value.substr(value.lastIndexOf('.') + 1) == 'xlsx' || value.substr(value.lastIndexOf('.') + 1) == 'xlsm') {
                $("#contact_listDiv").append('<div class="col-md-3"><img src="/Images/if_xls.png" /><button class="btn" name=' + value + ' onclick="myRemove(this)" style="padding-top:0px;padding-bottom:0px;background: white;"><i class="fa fa-remove"></i></button></div>');
            }
            else if (value.substr(value.lastIndexOf('.') + 1) == 'txt') {
                $("#contact_listDiv").append('<div class="col-md-3"><img src="/Images/if_Text.png" /><button class="btn" name=' + value + ' onclick="myRemove(this)" style="padding-top:0px;padding-bottom:0px;background: white;"><i class="fa fa-remove"></i></button></div>');
            }
            else if (value.substr(value.lastIndexOf('.') + 1) == 'jpg' || value.substr(value.lastIndexOf('.') + 1) == 'png' || value.substr(value.lastIndexOf('.') + 1) == 'jpeg') {
                $("#contact_listDiv").append('<div class="col-md-3"><img src="/Images/if_image.png" /><button class="btn" name=' + value + ' onclick="myRemove(this)" style="padding-top:0px;padding-bottom:0px;background: white;"><i class="fa fa-remove"></i></button></div>');
            }
            else {
                $("#contact_listDiv").append('<div class="col-md-3"><img src="/Images/if_defult.png" /><button class="btn" name=' + value + ' onclick="myRemove(this)" style="padding-top:0px;padding-bottom:0px;background: white;"><i class="fa fa-remove"></i></button></div>');
            }
        });
    });
}


$('#remoceallfile').click(function () {
    $("#contact_listDiv").html('');

})

var ContactDecisionHide = function () {
    $("#decision").hide();
    $("#potential_decision").hide();
    $("#activity_decision").hide();
    $("#todo_decision").hide();
    $("#personal_decision").hide();
    $("#document_decision").hide();
    $("#schedule_decision").hide();
}

var ContactDecisionShow = function () {

    $("#decision").show();
    $("#potential_decision").show();
    $("#activity_decision").show();
    $("#todo_decision").show();
    $("#personal_decision").show();
    $("#document_decision").show();
    $("#schedule_decision").show();
}

var setupForm = function () {
    JK.CRM.CRMFormValidation.init("#form_schedulestagedetail");

    JK.CRM.CRMFormValidation.init("#form_fvpotentialstagedetail");

    JK.CRM.CRMFormValidation.init("#form_fvschedulestagedetail");

    JK.CRM.CRMFormValidation.init("#form_bidstagedetail");

    JK.CRM.CRMFormValidation.init("#form_pdstagedetail");
    JK.CRM.CRMFormValidation.init("#form_pdpotentialdetail");




    JK.CRM.CRMFormValidation.init("#form_followstagedetail");
    JK.CRM.CRMFormValidation.init("#form_followpotentialstagedetail");

    JK.CRM.CRMFormValidation.init("#form_contractagreement");
    JK.CRM.CRMFormValidation.init("#form_signedagreement");
    JK.CRM.CRMFormValidation.init("#form_documentsalescrm");
    JK.CRM.CRMFormValidation.init("#form_accountplacement");



    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_contactphonenumber");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_companyphonenumber");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_companyfaxnumber");

    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_pdcontactphonecontactinfo");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_pdcontactphonepotential");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_pdcontactphoneactivity");

    JK.CRM.CRMFormInputMask.handlePhoneInputMask(".clsbiddingconphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_bidding_potential_contactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_bidding_activity_contactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_bidding_todo_contactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_bidding_note_contactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_bidding_document_contactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_bidding_schedule_contactphone");



    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_pdcontactphonetodo");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_pdcontactphonenote");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_pdcontactphonedocument");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_pdcontactphoneschedule");



    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_decisioncontactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_potential_decisioncontactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_activity_decisioncontactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_personalnote_decisioncontactphone");

    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_todo_decisioncontactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_schedule_decisioncontactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_document_decisioncontactphone");

    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_bidding_modal_contactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_modal_pdcontactphone");
    JK.CRM.CRMFormInputMask.handlePhoneInputMask("#input_modal_decisioncontactphone");


    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_measurefacility");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_potentialmeasurefacility");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_activitymeasurefacility");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_todomeasurefacility");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_notemeasurefacility");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_documentmeasurefacility");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_schedulemeasurefacility");

    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_numberoflocations");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_zipcode");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_numoffloors");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_potentialnumoffloors");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_activitynumoffloors");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_documentnumoffloors");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_notenumoffloors");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_schedulenumoffloor");

    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_cleantimes");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_potentialcleantimes");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_activitycleantimes");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_tdocleantimes");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_notecleantimes");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_documentcleantimes");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#input_schedulecleantimes");

    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_potential_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_activity_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_todo_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_note_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_document_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_schedule_cleantime");

    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_contractTerm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_potential_contractTerm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_activity_contractTerm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_todo_contractTerm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_note_contractTerm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_document_contractTerm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#close_schedule_contractTerm");

    JK.CRM.CRMFormInputMask.handleNumericInputMask(".sold_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_potential_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_activity_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_todo_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_note_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_document_cleantime");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_schedule_cleantime");

    JK.CRM.CRMFormInputMask.handleNumericInputMask(".sold_contractterm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_potential_contractterm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_activity_contractterm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_todo_contractterm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_note_contractterm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_document_contractterm");
    JK.CRM.CRMFormInputMask.handleNumericInputMask("#sold_schedule_contractterm");





    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_budgetamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_bidbudget");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".input_budgetamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_activitybudget");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_notebudget");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_documentbudget");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_schedulebudget");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_fvbudget");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".input_monthlyprice");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".input_includeicprice");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_potentialmonthlyprice");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_potentialicewprice");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_activitymonthlyprice");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_activityicewprice");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_notemonthlyprice");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_noteicewprice");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_documentmonthlyprice");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_documenticewprice");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_schedulemonthlyprice");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_scheduleicewprice");
    JK.CRM.CRMFormInputMask.handleDateInputMask("#input_callback");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_monthlypricemodal");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_icewpricemodal");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#input_budgetmodal");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_initialcleanamount");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_potential_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_potential_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_potential_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_potential_initialcleanamount");




    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_activity_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_activity_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_activity_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_activity_initialcleanamount");


    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_todo_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_todo_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_todo_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_todo_initialcleanamount");


    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_note_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_note_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_note_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_note_initialcleanamount");


    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_document_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_document_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_document_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_document_initialcleanamount");


    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_schedule_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_schedule_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_schedule_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#close_schedule_initialcleanamount");


    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".sold_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".sold_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".sold_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask(".sold_initialcleanamount");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_potential_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_potential_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_potential_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_potential_initialcleanamount");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_activity_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_activity_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_activity_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_activity_initialcleanamount");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_todo_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_todo_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_todo_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_todo_initialcleanamount");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_note_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_note_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_note_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_note_initialcleanamount");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_document_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_document_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_document_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_document_initialcleanamount");

    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_schedule_propamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_schedule_initialamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_schedule_contractamount");
    JK.CRM.CRMFormInputMask.handleCurrencyInputMask("#sold_schedule_initialcleanamount");









    /* No Decision Maker */

    $(".pd_yesdecisionmaker").change(function () {

        $(".pd_decision").hide();
    });

    $(".pd_nodecisionmaker").change(function () {

        $(".pd_decision").show();
    });



    $(function () {
        $("#select_Type").change(function () {
            if ($(this).val() == "1") {
                $("#divOutcome").show();
            } else {
                $("#divOutcome").hide();
            }
        });

    });

    $(function () {
        $("#select_potentialcleanfrequency").change(function () {
            if ($(this).val() == "1" || $(this).val() == "7") {
                $("#potentialweekdays").show();
            } else {
                //$("#contact_mon").rules("remove");
                $("#potentialweekdays").hide();
            }
        });
    });


    //Follow up Reschedule
    $(function () {
        $(".select_closetype").change(function () {
            if ($(this).val() == "1" || $(this).val() == "2" || $(this).val() == "5") {
                $(".div_reschedule").hide();

            } else {
                $(".div_reschedule").show();
            }
        });
    });


    $(".clssamecontact").click(function () {
        if ($(this).is(":checked")) {
            $(".clsmeasurecontact").val($("#input_contactname").val());
            $(".clsmeasurecontact").attr("readonly", "readonly");
            $(".clssamecontact").attr("Checked", true);
        }
        else {
            $(".clsmeasurecontact").val("");
            $(".clsmeasurecontact").removeAttr("readonly", "readonly");
            $(".clssamecontact").attr("Checked", false);
        }
    });

    $(".clssameContmeetingper").click(function () {
        if ($(this).is(":checked")) {
            $(".clsmeetwith").val($("#input_contactname").val());
            $(".clsconphone").val($("#input_contactphonenumber").val());
            $(".clsconemail").val($("#input_contactemail").val());


            $(".clsmeetwith").attr("readonly", "readonly");
            $(".clsconphone").attr("readonly", "readonly");
            $(".clsconemail").attr("readonly", "readonly");

            $(".clssameContmeetingper").prop("checked", true);
        }
        else {
            $(".clsmeetwith").val("");
            $(".clsconphone").val("");
            $(".clsconemail").val("");

            $(".clsmeetwith").removeAttr("readonly", "readonly");
            $(".clsconphone").removeAttr("readonly", "readonly");
            $(".clsconemail").removeAttr("readonly", "readonly");

            $(".clssameContmeetingper").prop("checked", false);
        }
    });

    $(".clssamebiddingmeetingper").click(function () {
        if ($(this).is(":checked")) {
            $(".clsbiddingconper").val($("#input_contactname").val());
            $(".clsbiddingconphone").val($("#input_contactphonenumber").val());
            $(".clsbiddingconemail").val($("#input_contactemail").val());


            $(".clsbiddingconper").attr("readonly", "readonly");
            $(".clsbiddingconphone").attr("readonly", "readonly");
            if ($("#input_bidding_contactemail").val() != "")
                $(".clsbiddingconemail").attr("readonly", "readonly");

            $(".clssamebiddingmeetingper").prop("checked", true);
        }
        else {
            $(".clsbiddingconper").val("");
            $(".clsbiddingconphone").val("");
            $(".clsbiddingconemail").val("");

            $(".clsbiddingconper").removeAttr("readonly", "readonly");
            $(".clsbiddingconphone").removeAttr("readonly", "readonly");
            $(".clsbiddingconemail").removeAttr("readonly", "readonly");

            $(".clssamebiddingmeetingper").prop("checked", false);
        }
    });








    /*Bidding Changes Events  */
    //timepicker & datepicker
    $('input.date-picker').datepicker({ Format: 'mm/dd/yy' });



    $('input.timepicker').timepicker();
    //$('#input_bidding_scheduleendtime').timepicker({ timeFormat: 'h:mm:ss p', dynamic: true });
    $("#input_bidding_schedulestartdate").change(function () {
        $("#input_bidding_scheduleenddate").val($(this).val());
    });


    $("#input_bidding_schedulestarttime").change(function () {
        $('#input_bidding_scheduleendtime').timepicker('setTime', incrementTimeByOne($(this).val()));
    });



    /*Pd Appointment Detail Changes Events  */
    $("#input_pdfollow_potential_schedulestartdate").change(function () {
        $("#input_pdfollow_potential_scheduleenddate").val($(this).val());
    });


    $("#input_pdfollow_potential_schedulestarttime").change(function () {
        $('#input_pdfollow_potential_scheduleendtime').timepicker('setTime', incrementTimeByOne($(this).val()));
    });

    $(".check_bidpresentedproposal").change(function () {

        $('.pd_followup_time_div').hide();

        if ($(this).val() == "1") {
            $('#select_pdpotentialovercome').val("");
            $("#select_pdpotentialovercome").removeAttr('disabled');
            $('.pd_overcome').show();
            $('.pd_present_no').hide();
        }
        else {
            $('#select_pdpotentialovercome').val("");
            $("#select_pdpotentialovercome").attr("disabled", "disabled");
            $('.pd_overcome').hide();
            $('.pd_present_no').show();
            $('.pd_selectovercome').hide();
        }

    });

    $("#select_pdpotentialovercome").change(function () {

        $('.pd_followup_time_div').hide();

        $("#select_pdpotentialovercome").css('border-color', '');
        $(".select_pdpurpose").css('border-color', '');

        if ($(this).val() == "1") {
            $(".pd_overcome_no").hide();
            $(".pd_overcome_yes").show();
        }
        else if ($(this).val() == "0") {
            $(".pd_overcome_yes").hide();
            $(".pd_overcome_no").show();
        }

    });

    $(".select_pdpurpose").change(function () {

        $(".select_pdpurpose").css('border-color', '');

        if ($(this).val() != "" && $(this).val() != "12") {
            $('.pd_followup_time').removeAttr("disabled");
            $('#input_pdfollow_potential_schedulestartdate').attr("disabled", "disabled");
            $('#input_pdfollow_potential_schedulestarttime').attr("disabled", "disabled");
            $('#input_pdfollow_potential_scheduleenddate').attr("disabled", "disabled");
            $('#input_pdfollow_potential_scheduleendtime').attr("disabled", "disabled");
            $('#text_pdpotential_note').attr("disabled", "disabled");
            $('.pd_followup_time_div').show();
        }
        else {
            $('.pd_followup_time').attr("disabled", "disabled");
            $('.pd_followup_time_div').hide();

            
        }

    });

    /*Follw-up Changes Events  */
    $("#input_followup_reschedule_startdate").change(function () {
        $("#input_followup_reschedule_enddate").val($(this).val());
    });


    $("#input_followup_reschedule_starttime").change(function () {
        $('#input_followup_reschedule_endtime').timepicker('setTime', incrementTimeByOne($(this).val()));
    });


    //$("#sold_signdate").change(function () {        
    //    alert(1);
    //    $("#sold_startdate").datepicker({minDate: new Date()});
    //});


    //var dateToday = new Date();
    //var dates = $("#sold_signdate, #sold_startdate").datepicker({
    //    defaultDate: "+1w",
    //    changeMonth: true,
    //    numberOfMonths: 3,
    //    minDate: dateToday,
    //    onSelect: function(selectedDate) {
    //        var option = this.id == "sold_signdate" ? "minDate" : "maxDate",
    //            instance = $(this).data("datepicker"),
    //            date = $.datepicker.parseDate(instance.settings.dateFormat || $.datepicker._defaults.dateFormat, selectedDate, instance.settings);
    //        dates.not(this).datepicker("option", option, date);
    //    }
    //});


    $('#potential_sold_btnDocumentUpload').click(function (e) {
        e.preventDefault();

        var aId = $("#hdfselectedaccountdetailid").val();
        if (Id = !"") {
            var sURL = "/CRM/CustomerSales/CRMUploadDocumentPopup?id=" + aId + "&sold=1";
            $.ajax({
                type: "GET",
                url: sURL,
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                success: function (data) {
                    $('#RenderCRMUploadDocumentPopup').html(data);
                    $("#ModelCRMUploadDocumentPopup").modal({ backdrop: 'static' });
                },
                error: function (err) {
                    console.log(err);
                }
            });
        }

    });



    $('#btnBiddingDetailSubmit').click(function (e) {
        e.preventDefault();
        $(".errmsg").hide();
        var isValid = true;
        if ($(".clsbiddingconper").val().trim() == "") { $(".clsbiddingconper").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".clsbiddingconper").css('border-color', ''); }
        if ($(".clsbiddingconphone").val().trim() == "") { $(".clsbiddingconphone").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".clsbiddingconphone").css('border-color', ''); }
        if ($(".clsbiddingconemail").val().trim() == "") { $(".clsbiddingconemail").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".clsbiddingconemail").css('border-color', ''); }
        if ($("#input_bidding_schedulestartdate").val().trim() == "") { $("#input_bidding_schedulestartdate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $("#input_bidding_schedulestartdate").css('border-color', ''); }
        if ($(".input_bid_starttime").val().trim() == "") { $(".input_bid_starttime").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".input_bid_starttime").css('border-color', ''); }
        if ($(".input_bid_enddate").val().trim() == "") { $(".input_bid_enddate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".input_bid_enddate").css('border-color', ''); }
        if ($(".input_bid_endtime").val().trim() == "") { $(".input_bid_endtime").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".input_bid_endtime").css('border-color', ''); }

        if ($(".select_bid_purpose option:selected").val().trim() == "") { $(".select_bid_purpose").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".select_bid_purpose").css('border-color', ''); }

        if ($(".input_monthlyprice").val().trim() == "") { $(".input_monthlyprice").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".input_monthlyprice").css('border-color', ''); }


        if ($(".select_priceapproved option:selected").val().trim() == "") { $(".select_priceapproved").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".select_priceapproved").css('border-color', ''); }

        if ($(".input_includeicprice").val().trim() == "") { $(".input_includeicprice").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".input_includeicprice").css('border-color', ''); }

        if (!$("#checkboxcontactinfo_accountworkbook").is(":checked")) {
            $("#requiredDocumentsMsg").show();
            $(window).scrollTop(0);
            if (isValid == true) {
                isValid = false;
            }
        }
        if (!$("#checkboxcontactinfo_BidSheetbook").is(":checked")) {
            $("#requiredDocumentsMsg").show();
            $(window).scrollTop(0);
            if (isValid == true) {
                isValid = false;
            }
        }
        if (!$("#checkboxcontactinfo_Cleaningbook").is(":checked")) {
            $("#requiredDocumentsMsg").show();
            $(window).scrollTop(0);
            if (isValid == true) {
                isValid = false;
            }
        }

        isValid = validateNextMeetingDateAgainstPrevious(isValid, "#input_bidding_schedulestartdate", "#input_bidding_schedulestarttime");

        if (isValid == true) {

            validateScheduleAvailability(
                "#input_bidding_schedulestartdate",
                "#input_bidding_schedulestarttime",
                "#input_bidding_scheduleenddate",
                "#input_bidding_scheduleendtime",
                saveBiddingSummaryData);
        }

    });


}

/*Convert Json Date into Date format */
function formatJSONDate(jsonDate) {
    if (jsonDate != null) {
        var newDate = new Date(parseInt(jsonDate.substring(6)));
        var options = {
            day: "numeric", month: "long", year: "numeric",
            hour: "2-digit", minute: "2-digit"
        };
        return newDate.toLocaleTimeString("en-us", options);
    }
    return null;
}

function SetJSONDate(jsonDate) {
    if (jsonDate != null) {
        var dtstr = jsonDate.substr(6);
        var dateString = dtstr.substring(0, dtstr.length - 2);
        var currentTime = new Date(parseInt(dateString));
        var date = moment.utc(currentTime).format("MM/DD/YYYY");
        return date;
    }
    return null;
}

/*Get the Time from Json Time*/
function SetJSONtime(jsonTime) {

    if (jsonTime != null) {
        var dtstr = jsonTime.substr(6);
        var dateString = dtstr.substring(0, dtstr.length - 2);
        var currentTime = new Date(parseInt(dateString));

        var Time = moment(currentTime).format("hh:mm A");
        return Time;
    }
    return null;
}

var getDate = function (jsonDate) {
    var newDate = new Date(parseInt(jsonDate.substring(6)));
    var options = {
        month: "numeric", day: "numeric", year: "numeric"
    }
    return newDate.toLocaleDateString("en-us", options);
}



function ScheduleDate(jsonDate) {
    var newDate = new Date(parseInt(jsonDate.substring(6)));
    var options = {
        day: "numeric"
    };
    return newDate.toLocaleDateString("en", options);
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
    return newDate.toLocaleTimeString("en", options);
}


var resetStageClass = function () {
    $(".fv-presentation").removeClass("max-opp-stage-selected");
    $(".initial-communication").removeClass("max-opp-stage-selected");
    $(".bidding").removeClass("max-opp-stage-selected");
    $(".pd-appointment").removeClass("max-opp-stage-selected");
    $(".follow-up").removeClass("max-opp-stage-selected");
    $(".close-potential").removeClass("max-opp-stage-selected");
    $(".sold").removeClass("max-opp-stage-selected");
}

var hideReport = function () {
    $("#report_initial").hide();
    $("#report_fv").hide();
    $("#report_bidding").hide();
    $("#report_pd").hide();
    $("#report_follow").hide();
}


/*Enable Fields of Contact Information */
var enabledfield = function () {

    $("#input_contactname").prop("disabled", false);
    $("#input_contacttitle").prop("disabled", false);
    $("#input_contactphonenumber").prop("disabled", false);
    $("#input_contactemail").prop("disabled", false);

    $("#input_companyname").prop("disabled", false);
    $("#select_industrytype").prop("disabled", false);
    $("#input_numberoflocations").prop("disabled", false);
    $("#input_companyphonenumber").prop("disabled", false);

    $("#input_companyfaxnumber").prop("disabled", false);
    $("#input_companyemail").prop("disabled", false);
    $("#input_companywebsite").prop("disabled", false);
    $("#input_addressline1").prop("disabled", false);

    $("#input_addressline2").prop("disabled", false);
    $("#input_city").prop("disabled", false);
    $("#input_county").prop("disabled", false);
    $("#select_state").prop("disabled", false);
    $("#input_zipcode").prop("disabled", false);

    $("#input_sqft").prop("disabled", false);
    $("#input_lineofbusiness").prop("disabled", false);
    $("#input_salesvolume").prop("disabled", false);

}

/*Disable Fields of Contact Information */
var disabledfield = function () {

    $("#input_contactname").prop("disabled", true);
    $("#input_contacttitle").prop("disabled", true);
    $("#input_contactphonenumber").prop("disabled", true);
    $("#input_contactemail").prop("disabled", true);

    $("#input_companyname").prop("disabled", true);
    $("#select_industrytype").prop("disabled", true);
    $("#input_numberoflocations").prop("disabled", true);
    $("#input_companyphonenumber").prop("disabled", true);

    $("#input_companyfaxnumber").prop("disabled", true);
    $("#input_companyemail").prop("disabled", true);
    $("#input_companywebsite").prop("disabled", true);
    $("#input_addressline1").prop("disabled", true);

    $("#input_addressline2").prop("disabled", true);
    $("#input_city").prop("disabled", true);
    $("#input_county").prop("disabled", true);
    $("#select_state").prop("disabled", true);
    $("#input_zipcode").prop("disabled", true);

    $("#input_sqft").prop("disabled", true);
    $("#input_lineofbusiness").prop("disabled", true);
    $("#input_salesvolume").prop("disabled", true);

}

var disabledPotentialDetailField = function () {
    $("#select_stagestatus").prop("disabled", true);
    $("#select_providertype").prop("disabled", true);
    $("#select_providersource").prop("disabled", true);
    $("#select_besttimetocall").prop("disabled", true);
    $("#input_callback").prop("disabled", true);
    $("#input_budgetamount").prop("disabled", true);
    $("#text_leadgeneralnote").prop("disabled", true);
    $("#select_assigne").prop("disabled", true);
    $("#select_reporterr").prop("disabled", true);
    $("#btn_calendar").prop("disabled", true);
}

var enabledPotentialDetailField = function () {
    //$("#select_stagestatus").prop("disabled", false);
    $("#select_providertype").prop("disabled", false);
    $("#select_providersource").prop("disabled", false);
    $("#select_besttimetocall").prop("disabled", false);
    $("#input_callback").prop("disabled", false);
    $("#input_budgetamount").prop("disabled", false);
    $("#text_leadgeneralnote").prop("disabled", false);
    $("#select_assigne").prop("disabled", false);
    $("#select_reporterr").prop("disabled", false);
    $("#btn_calendar").prop("disabled", false);
}

/*Update Contact Info click Event */
$("#contactinfo_edit_button").unbind("click").click(function (e) {
    e.preventDefault();
    if ($("#contactinfo_edit_button").text() != "Edit") {
        saveContactInfo();
    }
    if ($("#contactinfo_edit_button").text() == "Edit") {
        $("#contactinfo_edit_button").text("Save");
        enabledfield();
        $("#contactinfo_cancel_button").show();
        $("#contactinfo_cancel_button").prop("disabled", false);
    }
});

$("#contactinfo_cancel_button").unbind("click").click(function (e) {
    e.preventDefault();
    $("#contactinfo_cancel_button").hide();
    disabledfield();
    $("#contactinfo_edit_button").text("Edit");
});

function SendEmail() {
    var email = $("#input_contactemail").val();
    var mailto_link = 'mailto:' + email;
    window.open(mailto_link, 'emailWindow');
}

var enableContent = function (stageStatus) {

    if (stageStatus == 21) {

        //Fv Presentation
        $(".fvpresentationcontent").show();

        //Bidding
        $(".biddingcontent").hide();

        //Pd Appointment
        $(".pdappointmentcontent").hide();

        //Follow up
        $(".followupcontent").hide();

        //Sold
        $(".soldcontent").hide();

        //Close
        $(".closecontent").hide();
    }

    if (stageStatus == 22) {


        //Fv Presentation
        $(".fvpresentationcontent").hide();


        //Bidding
        $(".biddingcontent").show();


        //PdAppointment
        $(".pdappointmentcontent").hide();


        //Follow up
        $(".followupcontent").hide();



        //Sold
        $(".soldcontent").hide();


        //Close
        $(".closecontent").hide();

    }

    if (stageStatus == 23) {

        //Fv Presentation
        $(".fvpresentationcontent").hide();


        //Bidding
        $(".biddingcontent").hide();


        //Pd Appointment
        $(".pdappointmentcontent").show();


        //Follow Up
        $(".followupcontent").hide();



        //Sold
        $(".soldcontent").hide();


        //Close
        $(".closecontent").hide();


    }

    if (stageStatus == 24) {



        //Fv Presentation
        $(".fvpresentationcontent").hide();


        //Bidding
        $(".biddingcontent").hide();


        //Pd Appointment
        $(".pdappointmentcontent").hide();


        //Follow Up
        $(".followupcontent").show();


        //Sold
        $(".soldcontent").hide();


        //Close
        $(".closecontent").hide();

    }

    if (stageStatus == 25) {




        //Fv Presentation
        $(".fvpresentationcontent").hide();


        //Bidding
        $(".biddingcontent").hide();


        //Pd Appointment
        $(".pdappointmentcontent").hide();


        //Follow Up
        $(".followupcontent").hide();


        //Sold
        $(".soldcontent").hide();



        //Close
        $(".closecontent").show();

    }

    if (stageStatus == 30) {

        //Fv Presentation
        $(".fvpresentationcontent").hide();

        //Bidding
        $(".biddingcontent").hide();

        //Pd Appointment
        $(".pdappointmentcontent").hide();

        //Follow Up
        $(".followupcontent").hide();

        //Sold
        $(".soldcontent").show();

        //Close
        $(".closecontent").hide();
    }
}

var disableContent = function () {

    //Fv Presentation
    $(".fvpresentationcontent").hide();

    //Bidding
    $(".biddingcontent").hide();

    //Pd Appointment
    $(".pdappointmentcontent").hide();

    //Follow up
    $(".followupcontent").hide();

    //Sold
    $(".soldcontent").hide();

    //Close
    $(".closecontent").hide();
}

var disableLink = function () {

    $(".checkboxcontactinfo_accountworkbook").prop('checked', false);

    $(".checkboxcontactinfo_BidSheetbook").prop('checked', false);

    $(".checkboxcontactinfo_Cleaningbook").prop('checked', false);
}

var enableLink = function () {


    $("#checkboxpotentialdetail_accountworkbook").prop('checked', true);
    $("#checkboxpotentialdetail_accountworkbook").prop('disabled', true);

    $("#checkboxactivity_accountworkbook").prop('checked', true);
    $("#checkboxactivity_accountworkbook").prop('disabled', true);

    $("#checkboxtodo_accountworkbook").prop('checked', true);
    $("#checkboxtodo_accountworkbook").prop('disabled', true);

    $("#checkboxnote_accountworkbook").prop('checked', true);
    $("#checkboxnote_accountworkbook").prop('disabled', true);

    $("#checkboxdocument_accountworkbook").prop('checked', true);
    $("#checkboxdocument_accountworkbook").prop('disabled', true);

    $("#checkboxschedule_accountworkbook").prop('checked', true);
    $("#checkboxschedule_accountworkbook").prop('disabled', true);
}


$("#btnSubmitdocument").unbind("click").click(function (e) {
    e.preventDefault();
    hideDocModal();
});

$("#btnSubmitbiddocument").unbind("click").click(function (e) {
    e.preventDefault();
    hideBidDocModal();

});



function ContactValueKeyPress() {
    //var edValue = document.getElementById("input_monthlyprice");
    var s = $(".input_monthlyprice").val();
    // var s = edValue.value;

    if (s > 9999) {
        $(".select_priceapproved option[value='4']").remove();
        $(".select_priceapproved option[value='5']").remove();

        $('.select_priceapproved').append('<option value="4">President</option>');
        $('.select_priceapproved').append('<option value="5">Corporation VP</option>');

        $(".select_priceapproved option[value='1']").remove();
        $(".select_priceapproved option[value='2']").remove();
        $(".select_priceapproved option[value='3']").remove();
    }
    else {
        $(".select_priceapproved option[value='1']").remove();
        $(".select_priceapproved option[value='2']").remove();
        $(".select_priceapproved option[value='3']").remove();

        $('.select_priceapproved').append('<option value="1">Sales Manager</option>');
        $('.select_priceapproved').append('<option value="2">Region Director</option>');
        $('.select_priceapproved').append('<option value="3">Corporation VP</option>');

        $(".select_priceapproved option[value='4']").remove();
        $(".select_priceapproved option[value='5']").remove();
    }
}

/* Bid Price over 10K ?  */

$("#contactinfo_yes_bidover").change(function () {
    if ($(this).is(":checked")) {
        $("#select_priceapproved option[value='4']").remove();
        $("#select_priceapproved option[value='5']").remove();

        $('#select_priceapproved').append('<option value="4">President</option>');
        $('#select_priceapproved').append('<option value="5">Corporation VP</option>');

        $("#select_priceapproved option[value='1']").remove();
        $("#select_priceapproved option[value='2']").remove();
        $("#select_priceapproved option[value='3']").remove();
    }
});

$("#contactinfo_no_bidover").change(function () {
    if ($(this).is(":checked")) {
        $("#select_priceapproved option[value='1']").remove();
        $("#select_priceapproved option[value='2']").remove();
        $("#select_priceapproved option[value='3']").remove();

        $('#select_priceapproved').append('<option value="1">Sales Manager</option>');
        $('#select_priceapproved').append('<option value="2">Region Director</option>');
        $('#select_priceapproved').append('<option value="3">Corporation VP</option>');

        $("#select_priceapproved option[value='4']").remove();
        $("#select_priceapproved option[value='5']").remove();
    }
});

/*Save Schedule  */
$("#addschedule_button").unbind("click").click(function (e) {
    e.preventDefault();
    saveScheduleData();
});

/* Save Add Callo Log */
$("#addcalllog_button").unbind("click").click(function (e) {
    e.preventDefault();
    validCallLog();
})

/*Save Personal Note*/
$("#addpersonalnote_button").unbind("click").click(function (e) {
    e.preventDefault();
    saveNoteData();
});

/* Save To-Do */
$("#addToDo_button").unbind("click").click(function (e) {
    e.preventDefault();
    saveCustomerActivityData();
});

$("#editpotential_button").unbind("click").click(function (e) {
    e.preventDefault();
    if ($("#editpotential_button").text() != "Edit") {
        saveSummaryData();
    }
    else {
        $("#editpotential_button").text("Save");
        enabledPotentialDetailField();
        $("#cancelpotential_button").show();
    }
});



$("#addcontact_button").unbind("click").click(function (e) {
    e.preventDefault();
    ContactDecisionShow();
});

var resetcalllog = function () {
    $("#select_callresult").val("");
    $("#inputcalllog_spokewith").val("");
    $("#inputcalllog_callback").val("");
    $("#select_calllognote").val("");
    $("#txt_calllognote").val("");
}

var validCallLog = function () {

    var isValid = true;
    if ($("#select_statuslistid").val().trim() == "") {
        $("#select_statuslistid").css('border-color', 'red');
        if (isValid) {
            isValid = false;
        }
    }
    else {
        $("#select_statuslistid").css('border-color', '');
    }

    if ($("#select_callresult").val().trim() == "") {
        $("#select_callresult").css('border-color', 'red');
        if (isValid) {
            isValid = false;
        }
    }
    else {
        $("#select_callresult").css('border-color', '');
    }

    if ($("#inputcalllog_spokewith").val().trim() == "") {
        $("#inputcalllog_spokewith").css('border-color', 'red');
        if (isValid) {
            isValid = false;
        }
    }
    else {
        $("#inputcalllog_spokewith").css('border-color', '');
    }
    if ($("#inputcalllog_callback").val().trim() == "") {
        $("#inputcalllog_callback").css('border-color', 'red');
        if (isValid) {
            isValid = false;
        }
    }
    else {
        $("#inputcalllog_callback").css('border-color', '');
    }
    if ($("#select_calllognote").val().trim() == "") {
        $("#select_calllognote").css('border-color', 'red');
        if (isValid) {
            isValid = false;
        }
    }
    else {
        $("#select_calllognote").css('border-color', '');
    }
    if (isValid) {
        saveCalllog();
    }

}

var validFvPresentationDetail = function () {


    var isValid = true;
    if ($(".clsmeasurecontact").val().trim() == "") {
        $(".clsmeasurecontact").css('border-color', 'red'); $(window).scrollTop(0);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $(".clsmeasurecontact").css('border-color', '');
    }
    if ($(".input_measurefacility").val().trim() == "") {
        $(".input_measurefacility").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $(".input_measurefacility").css('border-color', '');
    }
    if ($(".input_numoffloor").val().trim() == "") {
        $(".input_numoffloor").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; }
    }
    else {
        $(".input_numoffloor").css('border-color', '');
    }
    if ($(".input_budgetamount").val().trim() == "") {
        $(".input_budgetamount").css('border-color', 'red'); $(window).scrollTop(0);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $(".input_budgetamount").css('border-color', '');
    }
    if ($(".input_cleantime").val().trim() == "") {
        $(".input_cleantime").css('border-color', 'red'); $(window).scrollTop(0);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $(".input_cleantime").css('border-color', '');
    }
    if ($("#select_potentialfrequency").val().trim() == "") {
        $("#select_potentialfrequency").css('border-color', 'red'); $(window).scrollTop(0);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $("#select_potentialfrequency").css('border-color', '');
    }
    if ($("#select_potentialservicetype").val().trim() == "") {
        $("#select_potentialservicetype").css('border-color', 'red'); $(window).scrollTop(0);
        if (isValid == true) {
            isValid = false;
        }
    }
    else {
        $("#select_potentialservicetype").css('border-color', '');
    }
    if ($("#select_potentialservicelevel").val().trim() == "") {
        $("#select_potentialservicelevel").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; }
    }
    else {
        $("#select_potentialservicelevel").css('border-color', '');
    }
    if ($("#select_potentialcleanfrequency").val() == "1") {
        if (($(".input_mon").prop('checked') == false) && ($(".input_tue").prop('checked') == false) && ($(".input_wed").prop('checked') == false) && ($(".input_thu").prop('checked') == false) && ($(".input_fri").prop('checked') == false) && ($(".input_sat").prop('checked') == false) && ($(".input_sun").prop('checked') == false)) {
            $(".cleningdayclass").css('color', 'red');
            $(window).scrollTop(0);
            if (isValid == true) {
                isValid = false;
            }
        }
        else {
            $(".cleningdayclass").css('color', '');
        }
    }
    if (isValid == true) {
        // saveContactInfo();
        saveFvSummaryData(getFvPresentationDataToPost());
    }
}

var validPdAppointmentDetail = function () {

    var isValid = true;
    if ($(".clsmeetwith").val().trim() == "") { $(".clsmeetwith").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".clsmeetwith").css('border-color', ''); }
    if ($(".clsconphone").val().trim() == "") { $(".clsconphone").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".clsconphone").css('border-color', ''); }
    if ($(".clsconemail").val().trim() == "") { $(".clsconemail").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".clsconemail").css('border-color', ''); }
    if ($(".check_bidpresentedproposal").val() == "") { $("#div_presentproposal").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $("#div_presentproposal").css('border-color', ''); }

    if ($(".pd_followup_time_div").is(":visible")) {
        if ($(".input_pdstartdate").val() == "") { $(".input_pdstartdate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".input_pdstartdate").css('border-color', ''); }
        if ($(".input_pdenddate").val() == "") { $(".input_pdenddate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".input_pdenddate").css('border-color', ''); }

        isValid = validateNextMeetingDateAgainstPrevious(isValid, "#input_pdfollow_potential_schedulestartdate", "#input_pdfollow_potential_schedulestarttime");
    }

    if ($("#checkboxpdpotential_presentproposal").is(":checked")) {
        if ($(".select_overcomeobjection option:selected").val().trim() == "") { $(".select_overcomeobjection").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
        else {
            $(".select_overcomeobjection").css('border-color', '');

            if ($("#select_pdpotentialovercome").val() == "1") { // yes
                if ($("#select_pdpotential_scheduleresult").val().trim() == "") { $("#select_pdpotential_scheduleresult").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
            } else if ($("#select_pdpotentialovercome").val() == "0") { // no
                if ($("#select_pdpotential_schedulereason").val().trim() == "") { $("#select_pdpotential_schedulereason").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
            }
        }
    } else {
        if ($("#select_pdpotential_nopresentreason").val().trim() == "") { $("#select_pdpotential_nopresentreason").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } }
    }

    if ($('.pd_nodecisionmaker').is(':checked')) {
        if ($("#input_potential_decisioncontactname").val().trim() == "") { $("#input_potential_decisioncontactname").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $("#input_potential_decisioncontactname").css('border-color', ''); }
        if ($("#input_potential_decisioncontactphone").val().trim() == "") { $("#input_potential_decisioncontactphone").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $("#input_potential_decisioncontactphone").css('border-color', ''); }
        if ($("#input_potential_decisioncontactemail").val().trim() == "") { $("#input_potential_decisioncontactemail").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $("#input_potential_decisioncontactemail").css('border-color', ''); }
    }
    else {
        $("#input_potential_decisioncontactname").css('border-color', '');
        $("#input_potential_decisioncontactphone").css('border-color', '');
        $("#input_potential_decisioncontactemail").css('border-color', '');

    }

    //if ($(".select_pdpurpose option:selected").val().trim() == "") { $(".select_pdpurpose").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".select_pdpurpose").css('border-color', ''); }

    if (isValid == true) {

        if ($(".pd_followup_time_div").is(":visible")) {
            validateScheduleAvailability(
                "#input_pdfollow_potential_schedulestartdate", "#input_pdfollow_potential_schedulestarttime",
                "#input_pdfollow_potential_scheduleenddate", "#input_pdfollow_potential_scheduleendtime",
                savePdAppointmentSummaryData);
        }
        else {
            savePdAppointmentSummaryData();
        }
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

var validFollowupDetail = function (value) {

    var isValid = true;
    if ($(".select_closetype option:selected").val().trim() == "") { $(".select_closetype").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".select_closetype").css('border-color', ''); }

    var needsFollowup = $(".select_closetype option:selected").val().trim() == "" || ($(".select_closetype option:selected").val().trim() != "1" && $(".select_closetype option:selected").val().trim() != "2" && $(".select_closetype option:selected").val().trim() != "5");

    if (needsFollowup) {

        if ($(".input_followup_reschedule_startdate").val().trim() == "") { $(".input_followup_reschedule_startdate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".input_followup_reschedule_startdate").css('border-color', ''); }
        if ($(".input_followup_reschedule_enddate").val().trim() == "") { $(".input_followup_reschedule_enddate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".input_followup_reschedule_enddate").css('border-color', ''); }
        if ($(".select_followup_reschedulePurpose").val().trim() == "") { $(".select_followup_reschedulePurpose").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".select_followup_reschedulePurpose").css('border-color', ''); }

        isValid = validateNextMeetingDateAgainstPrevious(isValid, "#input_followup_reschedule_startdate", "#input_followup_reschedule_starttime");
    }

    if (isValid == true) {
        if (needsFollowup) {
            validateScheduleAvailability(
                "#input_followup_reschedule_startdate", "#input_followup_reschedule_starttime",
                "#input_followup_reschedule_enddate", "#input_followup_reschedule_endtime",
                saveFollowUpSummaryData);
        } else {
            saveFollowUpSummaryData();
        }

    }

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

    if (!startTime.isValid() || !endTime.isValid())
        return;

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

var validCloseDetail = function (doSave) {

    if (doSave == null)
    {
        doSave = true;
    }
    var isValid = true;

    if ($('input[name=rdbCloseContractAgreement]:checked').length == 0) { $("#lblCloseContractAgreement").css('color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $("#lblCloseContractAgreement").css('color', ''); }
    if ($('input[name=rdbCloseGetSignedAgreement]:checked').length == 0) { $("#lblCloseGetSignedAgreement").css('color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $("#lblCloseGetSignedAgreement").css('color', ''); }
    if ($('input[name=rdbCloseDocuments]:checked').length == 0) { $("#lblCloseDocuments").css('color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $("#lblCloseDocuments").css('color', ''); }
    if ($('input[name=rdbCloseNotifyOperations]:checked').length == 0) { $("#lblCloseNotifyOperations").css('color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $("#lblCloseNotifyOperations").css('color', ''); }


    if ($(".sold_propamount").val().trim() == "") { $(".sold_propamount").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_propamount").css('border-color', ''); }
    if ($(".sold_initialamount").val().trim() == "") { $(".sold_initialamount").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_initialamount").css('border-color', ''); }


    if ($(".sold_contractamount").val().trim() == "") { $(".sold_contractamount").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_contractamount").css('border-color', ''); }
    if ($(".sold_initialcleanamount").val().trim() == "") { $(".sold_initialcleanamount").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_initialcleanamount").css('border-color', ''); }


    if ($(".sold_signdate").val().trim() == "") { $(".sold_signdate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_signdate").css('border-color', ''); }
    if ($(".sold_startdate").val().trim() == "") { $(".sold_startdate").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_startdate").css('border-color', ''); }



    //if ($(".sold_ponumber").val().trim() == "") { $(".sold_ponumber").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_ponumber").css('border-color', ''); }
    if ($(".sold_contractterm").val().trim() == "") { $(".sold_contractterm").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_contractterm").css('border-color', ''); }



    if ($(".sold_propamount").val().trim() == "") { $(".sold_propamount").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_propamount").css('border-color', ''); }
    if ($(".sold_initialamount").val().trim() == "") { $(".sold_initialamount").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_initialamount").css('border-color', ''); }
    if ($(".sold_cleantime").val().trim() == "") { $(".sold_cleantime").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_cleantime").css('border-color', ''); }






    if ($(".sold_ServiceTypeListModel").val().trim() == "") { $(".sold_ServiceTypeListModel").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_ServiceTypeListModel").css('border-color', ''); }
    if ($(".sold_FrequencyListModel").val() == "") { $(".sold_FrequencyListModel").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_FrequencyListModel").css('border-color', ''); }
    if ($(".sold_ContractDetail_CleanFrequency").val() == "") { $(".sold_ContractDetail_CleanFrequency").css('border-color', 'red'); $(window).scrollTop(0); if (isValid == true) { isValid = false; } } else { $(".sold_ContractDetail_CleanFrequency").css('border-color', ''); }

    //isValid = validateNextMeetingDateAgainstPrevious(isValid, "#sold_startdate", null);
    //isValid = validateNextMeetingDateAgainstPrevious(isValid, "#sold_signdate", null);

    if (doSave == true && isValid == true) {
        saveCloseSummaryData(getCloseDataToPost());
    }
    else {
        return isValid;
    }
}

var getFvPresentationDataToPost = function () {

    var sel = $(".fvDays input:checked").map(function (_, el) {
        return $(el).val();
    }).get();
    postData = {
        "CRM_AccountId": $("#hdfSelectedAccountId").val(),
        "CRM_AccountCustomerDetailId": $("#hdfselectedaccountdetailid").val(),
        "MeasureContactPerson": $("#input_potentialmeasurecontactperson").val(),
        "MeasureFacility": $("#input_potentialmeasurefacility").val(),
        "NumberOfFloors": $("#input_potentialnumoffloors").val(),
        "Frequency": $("#select_potentialfrequency").val(),
        "CleaningDay": sel,
        "ServiceType": $("#select_potentialservicetype").val(),
        "ServiceLevel": $("#select_potentialservicelevel").val(),
        "CleanFrequency": $("#select_potentialcleanfrequency").val(),
        "CleanTime": $("#input_potentialcleantimes").val(),
        "Budget": $("#input_potentialbudget").val(),
        "Note": $("#text_potentialgeneralnote").val(),

        "Mon": $("#chkweekday_mon").is(':checked'),
        "Tue": $("#chkweekday_tue").is(':checked'),
        "Wed": $("#chkweekday_wed").is(':checked'),
        "Thu": $("#chkweekday_thu").is(':checked'),
        "Fri": $("#chkweekday_fri").is(':checked'),
        "Sat": $("#chkweekday_sat").is(':checked'),
        "Sun": $("#chkweekday_sun").is(':checked'),
        "Weekend": $("#chkweekday_Weekend").is(':checked')


    };

    return postData;

}

var getBiddingDataToPost = function (value) {
    postData = "";

    postData = {
        "CRM_AccountId": $("#hdfSelectedAccountId").val(),
        "CRM_AccountCustomerDetailId": $("#hdfselectedaccountdetailid").val(),
        "IsAnalysisWorkBook": $("#checkboxcontactinfo_accountworkbook").is(":checked"),
        "MeetWithPerson": $("#input_bidding_meetwith").val(),
        "ContactPhone": $("#input_bidding_contactphone").val(),
        "ContactEmail": $("#input_bidding_contactemail").val(),

        "ScheduleStartDate": $("#input_bidding_schedulestartdate").val(),
        "ScheduleStartTime": $("#input_bidding_schedulestarttime").val(),
        "ScheduleEndDate": $("#input_bidding_scheduleenddate").val(),
        "ScheduleEndTime": $("#input_bidding_scheduleendtime").val(),
        "PruposeId": $("#select_Biddingcontactinfo_Biddingpurpose").val(),
        "Prupose": $("#select_Biddingcontactinfo_Biddingpurpose option:selected").text(),

        "MonthlyPrice": $("#input_Biddingcontactinfo_monthlyprice").val(),
        "PriceApproved": $("#select_Biddingcontactinfo_priceapproved").val(),
        "IncludePrice": $("#input_Biddingcontactinfo_includeicprice").val(),
        "Note": $("#text_Biddingcontactinfo_bidgeneralnote").val(),
        "IsBidSheet": $("#checkboxcontactinfo_BidSheetbook").is(":checked"),
        "IsCancellation": $("#checkboxcontactinfo_Cleaningbook").is(":checked"),
    };


    return postData;
}

var getpdAppointmentDataToPost = function (value) {
    postData = "";

    var purposeDropdown = "";
    var purposeId = "";
    var purpose = "";

    if ($("#checkboxpdpotential_presentproposal").is(":checked")) // did present
    {
        if ($("#select_pdpotentialovercome").val() == "1") // yes overcome
            purposeDropdown = "#select_pdpotential_scheduleresult";
        else if ($("#select_pdpotentialovercome").val() == "0") // no overcome
            purposeDropdown = "#select_pdpotential_schedulereason";
    }
    else  // did not present, no overcome
        purposeDropdown = "#select_pdpotential_nopresentreason";

    if (purposeDropdown != "") {
        purposeId = $(purposeDropdown).val();
        purpose = $(purposeDropdown + " :selected").text();
    }

    postData = {
        "CRM_AccountId": $("#hdfSelectedAccountId").val(),
        "CRM_AccountCustomerDetailId": $("#hdfselectedaccountdetailid").val(),

        "DMeetWithPerson": $("#input_potential_decisioncontactname").val(),
        "DContactPhone": $("#input_potential_decisioncontactphone").val(),
        "DContactEmail": $("#input_potential_decisioncontactemail").val(),

        "MeetWithDecisionMaker": $("#pd_yesdecisionmaker").is(":checked") ? true : false,
        "PresentProposal": $("#checkboxpdpotential_presentproposal").is(":checked"),
        "OverComeObjection": $("#select_pdpotentialovercome").val(),
        //"ProposalDetail": $("#checkboxpdpotential_proposaldetail").is(":checked") ? true : false,

        //"ScheduleStartDate": $("#input_pdbid_potential_schedulestartdate").val(),
        //"ScheduleStartTime": $("#input_pdbid_potential_schedulestarttime").val(),
        //"ScheduleEndDate": $("#input_pdbid_potential_scheduleenddate").val(),
        //"ScheduleEndTime": $("#input_pdbid_potential_scheduleendtime").val(),

        "CallBackStartDate": $("#input_pdfollow_potential_schedulestartdate").val(),
        "CallBackStartTime": $("#input_pdfollow_potential_schedulestarttime").val(),
        "CallBackEndDate": $("#input_pdfollow_potential_scheduleenddate").val(),
        "CallBackEndTime": $("#input_pdfollow_potential_scheduleendtime").val(),
        "CallBack_PurposeId": purposeId,
        "CallBack_Purpose": purpose,

        "Note": $("#text_pdpotential_note").val()
    };
    return postData;
}

var getfollowUpDataToPost = function (value) {
    postData = "";
    postData = {
        "CRM_AccountId": $("#hdfSelectedAccountId").val(),
        "CRM_AccountCustomerDetailId": $("#hdfselectedaccountdetailid").val(),
        "CloseType": $("#select_closetype").val(),

        "ScheduleStartDate": $("#input_followup_reschedule_startdate").val(),
        "ScheduleStartTime": $("#input_followup_reschedule_starttime").val(),
        "ScheduleEndDate": $("#input_followup_reschedule_enddate").val(),
        "ScheduleEndTime": $("#input_followup_reschedule_endtime").val(),

        "PurposeAgainId": $("#selectpotential_followmeetagainpurpose").val(),
        "PurposeAgain": $("#selectpotential_followmeetagainpurpose option:selected").text(),
        "Note": $("#textarea_followupnote").val()
    };
    return postData;
}

var getCloseDataToPost = function (value) {



    var postData = new FormData();
    var sel = $(".sold input:checked").map(function (_, el) {
        return $(el).val();
    }).get();
    for (var i = 0; i < 12; i++) {
        var filetype = (1 + i).toString();
        if ($("#file" + filetype)[0].files.length != 0) { postData.append("file" + filetype, $("#file" + filetype)[0].files[0]); }
    }

    postData.append("CRM_AccountId", $("#hdfSelectedAccountId").val());
    postData.append("CRM_AccountCustomerDetailId", $("#hdfselectedaccountdetailid").val());
    postData.append("HaveBackgroundCheck", $(".sold_yes_contract").is(":checked"));
    postData.append("SignedAgreement", $(".sold_yes_signed").is(":checked"));
    postData.append("DocumentSalesCRM", $(".sold_yes_documentsalescrm").is(":checked"));
    postData.append("NotifyAccountPlacement", $(".sold_yes_accountplacement").is(":checked"));

    postData.append("PropAmount", $(".sold_propamount").val());
    postData.append("InitialClean", $(".sold_initialamount").val());
    postData.append("ContractAmount", $(".sold_contractamount").val());
    postData.append("InitialCleanAmount", $(".sold_initialcleanamount").val());

    postData.append("SignDate", $(".sold_signdate").val());
    postData.append("StartDate", $(".sold_startdate").val());
    postData.append("PhoneNumber", $(".sold_ponumber").val());
    postData.append("ContractTerm", $(".sold_contractterm").val());

    postData.append("ServiceType", $(".sold_ServiceTypeListModel option:selected").val());
    postData.append("BillingFrequency", $(".sold_FrequencyListModel option:selected").val());

    postData.append("CleanTime", $(".sold_cleantime").val());
    postData.append("CleaningDay", sel);
    postData.append("CleanFrequency", $(".sold_ContractDetail_CleanFrequency option:selected").val());
    postData.append("Note", $(".textarea_soldnote").val());

    return postData;

}

var saveFvSummaryData = function (postData) {

    //Make a Ajax Request
    JK.CRM.CRMApp.showActivity("#fv_summary_action");

    $.ajax({
        url: "/CRM/CustomerSales/CreateFvPresentationSummaryData",
        type: "POST",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (response) {
            refreshfv(response);

        },
        error: function (error) {
            console.log(error);
        }
    });

}

var saveBiddingSummaryData = function () {

    var postData = getBiddingDataToPost();
    console.log(postData);

    //Make a Ajax Request
    JK.CRM.CRMApp.showActivity("#bid_summary_action");

    setTimeout(function () {

        $.ajax({
            url: "/CRM/CustomerSales/CreateBiddingSummaryData",
            type: "POST",
            data: postData,
            dataType: "json",
            cache: false,
            success: function (response) {
                console.log(response);
                refreshbid(response);
            },
            error: function (error) {
                console.log(error);
            }

        });
    }, 1000);


}

var savePdAppointmentSummaryData = function () {

    var postData = getpdAppointmentDataToPost();

    //Make a Ajax Request
    JK.CRM.CRMApp.showActivity("#pd_summary_action");

    $.ajax({
        url: "/CRM/CustomerSales/CreatePdAppointmentSummaryData",
        type: "POST",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (response) {
            refreshpd(response);
        },
        error: function (error) {
            console.log(error);
        }

    });
}

var saveFollowUpSummaryData = function () {

    var postData = getfollowUpDataToPost();

    //Make a Ajax Request
    JK.CRM.CRMApp.showActivity("#follow_summary_action");

    $.ajax({
        url: "/CRM/CustomerSales/CreateFollowUpSummaryData",
        type: "POST",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (response) {

            if (response.stage == 5) {
                //location.reload();
                location = '/CRM/CRMLeadCustomer/QualifiedLeads';
            }
            else {
                refreshfollow(response);
            }

        },
        error: function (error) {
            //console.log(error);
        }
    });
}

var saveCloseSummaryData = function (postData) {

    //Make a Ajax Request
    JK.CRM.CRMApp.showActivity("#follow_summary_action");

    $.ajax({
        url: "/CRM/CustomerSales/CreateCloseSummaryData",
        type: "POST",
        data: postData,
        dataType: "json",
        cache: false,
        processData: false,
        contentType: false,
        success: function (response) {
            location.reload();
            // console.log(response);
            // refreshSold(response);
        },
        error: function (error) {
            alert(error);
            console.log(error);
        }
    });

}

var saveNoteData = function () {
    var noteForm = $("#addNote_form");
    if (!noteForm.valid()) {
        return;
    } else {
        $("#addNote_modalview").modal('hide');
        var postData = {
            "CRM_AccountCustomerDetailId": $("#hdfselectedaccountdetailid").val(),
            "Title": $("#input_notetitle").val(),
            "Description": $("#input_notedescription").val()
        }
        //Make a Ajax Request
        $.ajax({
            url: "/CRM/CustomerSales/SaveAccountCustomerNote",
            type: "POST",
            data: postData,
            dataType: "json",
            cache: false,
            success: function (response) {
                refreshNote(response);
            },
            error: function (error) {
                console.log(error);
            }
        });
    }

}

var saveCalllog = function () {
    postData = ""
    postData = {
        "CRM_AccountId": $("#hdfSelectedAccountId").val(),
        "CRM_AccountCustomerDetailId": $("#hdfselectedaccountdetailid").val(),
        "StatusId": $("#select_statuslistid option:selected").val(),
        "CallResultId": $("#select_callresult option:selected").val(),
        "SpokeWith": $("#inputcalllog_spokewith").val(),
        "CallBack": $("#inputcalllog_callback").val(),
        "LeadSource": $("#select_providertype option:selected").val(),
        "NoteType": $("#select_calllognote option:selected").val(),
        "Note": $("#txt_calllognote").val(),
    }        

    //Make a Ajax Request
    $.ajax({
        url: "/CRM/CustomerSales/SaveAccountCustomerCallLog",
        type: "POST",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (response) {
            if (!isNaN(response.result.CRM_AccountId)) {
                $("#calllog_modalview").modal('hide');
                resetcalllog();
                refreshCallLog(response.result.CRM_AccountId);
            }           
        },
        error: function (error) {            
            console.log(error.message);
        }
    });
}

var saveSummaryData = function () {

    var summaryForm = $("#form_summary");
    if (!summaryForm.valid()) {
        return;
    }
    else {
        var postData = {
            "CRM_AccountId": $("#hdfSelectedAccountId").val(),
            "CRM_AccountCustomerDetailId": $("#hdfselectedaccountdetailid").val(),
            "StageStatus": $("#select_stagestatus").val(),
            "ProviderType": $("#select_providertype").val(),
            "ProviderSource": $("#select_providersource").val(),
            "BudgetAmount": $("#input_budgetamount").val(),

        };

        JK.CRM.CRMApp.showActivity("#summary_action");
        $("#potential_edit_button").prop("disabled", true);
        $("#potential_cancel_button").prop("disabled", true);

        //Make a Ajax Request
        $.ajax({
            url: "/CRM/CustomerSales/SaveCustomerAccountSummary",
            type: "POST",
            data: postData,
            dataType: "json",
            cache: false,
            success: function (response) {
                $("#potential_edit_button").text("Edit");
                $("#potential_cancel_button").hide();
                $("#potential_edit_button").prop("disabled", false);
                JK.CRM.CRMApp.hideActivity("#summary_action");
                refreshpotential(response);
            },
            error: function (error) {
                JK.CRM.CRMApp.hideActivity("#summary_action");
                console.log(error.message);
            }
        });
    }
}

var saveCustomerActivityData = function () {
    var activityForm = $("#addActivity_form");
    if (!activityForm.valid()) {
        return;
    }

    else {
        var postData = {
            "CRM_AccountCustomerDetailId": $("#hdfselectedaccountdetailid").val(),
            "Note": $("#form_description").val(),
            "ActivityType": $("#select_Type option:selected").val(),
            "OutComeType": $("#select_OutCome option:selected").val(),
            "StartDate": $("#todo_startdate").val(),
            "StartTime": $("#todo_starttime").val(),
            "EndDate": $("#todo_enddate").val(),
            "EndTime": $("#todo_endtime").val(),
        };

        // Make a request
        $("#cancel_customeraddactivity_button").prop("disabled", true);
        $("#customeraddactivity_button").prop("disabled", true);
        JK.CRM.CRMApp.showActivity("#addActivity_modalview_footer");


        $.ajax({
            url: "/CRM/CustomerSales/SaveCustomerActivitySummary",
            type: "POST",
            data: postData,
            dataType: "json",
            cache: false,
            success: function (response) {

                $("#customer_addActivity_modalview").modal('hide');
                $("#cancel_customeraddactivity_button").prop("disabled", false);
                $("#customeraddactivity_button").prop("disabled", false);

                resetCustomerAddActivityForm();
                refreshToDo(response);
                JK.CRM.CRMApp.hideActivity("#addActivity_modalview_footer");
            },
            error: function (error) {
                JK.CRM.CRMApp.hideActivity("#addActivity_modalview_footer");
                console.log(error.message);
            }
        });
    }
}

var saveContactInfo = function () {
    var contactInfoForm = $("#editcontactinfo_form");
    if (!contactInfoForm.valid()) {
        return;
    } else {
        var postData = {
            "CRM_AccountCustomerDetailId": $("#hdfselectedaccountdetailid").val(),
            "CRM_AccountId": $("#hdfSelectedAccountId").val(),
            "ContactName": $("#input_contactname").val(),
            "Title": $("#input_contacttitle").val(),
            "PhoneNumber": $("#input_contactphonenumber").val(),
            "EmailAddress": $("#input_contactemail").val(),

            "CompanyName": $("#input_companyname").val(),
            "IndustryType": $("#select_industrytype option:selected").val(),
            "NumberOfLocations": $("#input_numberoflocations").val(),
            "CompanyPhoneNumber": $("#input_companyphonenumber").val(),

            "CompanyFaxNumber": $("#input_companyfaxnumber").val(),
            "CompanyEmailAddress": $("#input_companyemail").val(),
            "CompnayWebSite": $("#input_companywebsite").val(),


            "CompanyAddressLine1": $("#input_addressline1").val(),
            "CompanyAddressLine2": $("#input_addressline2").val(),
            "CompanyCity": $("#input_city").val(),
            "CompanyCounty": $("#input_county").val(),

            "CompanyState": $("#select_state option:selected").val(),
            "CompanyZipCode": $("#input_zipcode").val(),
            "Sqft": $("#input_sqft").val(),
            "LineofBusiness": $("#input_lineofbusiness").val(),
            "SalesVolume": $("#input_salesvolume").val()
        }

        $("#contactinfo_edit_button").prop("disabled", true);
        $("#contactinfo_cancel_button").prop("disabled", true);

        $.ajax({
            url: "/CRM/CustomerSales/SaveCustomerContactInfo",
            type: "POST",
            data: postData,
            dataType: "json",
            cache: false,
            success: function (response) {
                $("#contactinfo_edit_button").text("Edit");
                $("#contactinfo_cancel_button").hide();
                $("#contactinfo_edit_button").prop("disabled", false);
                // JK.CRM.CRMApp.hideActivity("#addschedule_modalview_footer");
                refreshcontactinfo(response);
            },
            error: function (error) {
                // JK.CRM.CRMApp.hideActivity("#addschedule_modalview_footer");
                console.log(error.message);
            }
        });
    }

}

var resetCustomerAddActivityForm = function () {
    $("#select_Type").val("");
    $("#select_OutCome").val("");
    $("#dtp_Timestamp").val("");
    $("#form_description").val("");
}

var resetAddNoteForm = function () {
    $("#input_notetitle").val("");
    $("#input_notedescription").val("");
}

var resetAddDocumentForm = function () {
    $("#").val("");
}

function addNewSchedule() {

    $.ajax({
        url: "/CRM/CustomerSales/SaveCustomerScheduleSummary",
        type: "POST",
        dataType: "json",
        data: $("#form_addnewschedule").serialize(),
        cache: false,
        success: function (response) {
            resetAddScheduleForm();
            getSchedules(response.id);
        },
        error: function (error) {
            console.log(error);
        }
    });

}

function resetAddScheduleForm() {

    $("#input_scheduletitle").val("");
    $("#input_schedulelocation").val("");
    $("#input_schedulestartdate").val("");
    //$("#checkbox_alldayevent").prop("checked", false);
    $("#input_scheduleenddate").val("");
    $("#input_scheduledescription").val("");
}

var uploadDocument = function () {

    var formData = new FormData();

    for (var i = 0; i < 12; i++) {
        var filetype = (1 + i).toString();
        if ($("#file" + filetype)[0].files.length === 0) { console.log((1 + i.toString() + "File is empty")); continue; }


        formData.append("file" + filetype, $("#file" + filetype)[0].files[0]);
    }


}


var hideDocModal = function () {

    $("#uploaddocument_modalview").modal('hide');

    var formdata = new FormData();
    formdata.append("CRM_AccountId", $("#hdfSelectedAccountId").val());
    formdata.append("CRM_AccountCustomerDetailId", $("#hdfselectedaccountdetailid").val());
    formdata.append("document", $('#fileDocument')[0].files[0]);
    formdata.append("Description", $("#input_filedescription").val());
    formdata.append("File_Title", $("#input_filename").val());

    $.ajax({
        url: "/CRM/CustomerSales/SaveDocument",
        type: "POST",
        data: formdata,
        dataType: "json",
        cache: false,
        processData: false,
        contentType: false,
        success: function (response) {
            refreshDocuments(response);
        },
        error: function (error) {
            console.log(error);
        }
    });
    $("#uploaddocument_modalview").modal('hide');

}

var hideBidDocModal = function () {

    $("#uploadbiddocument_modalview").modal('hide');

    var formdata = new FormData();
    formdata.append("CRM_AccountId", $("#hdfSelectedAccountId").val());
    formdata.append("CRM_AccountCustomerDetailId", $("#hdfselectedaccountdetailid").val());
    formdata.append("document", $('#bidfileDocument')[0].files[0]);
    formdata.append("Description", $("#input_bidfiledescription").val());
    formdata.append("File_Title", $("#input_bidfilename").val());
    formdata.append("IsWorkBook", true);

    $.ajax({
        url: "/CRM/CustomerSales/SaveDocument",
        type: "POST",
        data: formdata,
        dataType: "json",
        cache: false,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                enableLink();
                refreshDocuments(response);
            } else {
                disableLink();
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

var GetPotential = function () {
    $("#PotentialLeadList").DataTable().ajax.reload();
}

function getSchedules(id) {

    $.ajax({
        url: "/CRM/CustomerSales/GetLeadSchedules?id=" + id,
        type: "GET",
        dataType: "json",
        cache: false,
        success: function (response) {
            refreshCalendar(response.result);
            refreshSchedule(response);
            $("#hdnMostRecent").val(response.mostrecent);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function getSchedulesForAssignee(id) {


    $.ajax({
        url: "/CRM/CustomerSales/GetSchedulesForLeadAssignee?userId=" + id,
        type: "GET",
        dataType: "json",
        cache: false,
        success: function (response) {
            refreshCalendar(response.result);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

var setDetailData = function (data) {
    $.blockUI();
    $("#content_activity").empty();
    $("#content_todo").empty();
    $("#content_note").empty();
    $("#content_schedule").empty();
    $("#content_document").empty();

    /**
     * Refresh the table
     **/
    //var tblPotential = $('#PotentialLeadList').DataTable();
    //tblPotential.ajax.url(getCurrentUrl()).load();

    /**
    *Set Report Detail
    */

    //  setReportData(data);

    /**
    *Stages
    */



    if (data.result["StageStatus"] == 24) {
        /*Set Schedule */
        if (data.stagestatus != null)
            setfollowup(data.stagestatus);

        resetStageClass();
        $(".follow-up").addClass("max-opp-stage-selected");

        $(".pd-appointment").addClass("max-opp-stage-selected");
        $(".bidding").addClass("max-opp-stage-selected");
        $(".fv-presentation").addClass("max-opp-stage-selected");
        $(".initial-communication").addClass("max-opp-stage-selected");
        if (data.contact != null && data.bidSchedule != null) { updateContact(data); }
        if (data.pdContact != null && data.pdSchedule != null) { updateContact(data); }
    }

    if (data.result["StageStatus"] == 25) {
        resetStageClass();
        enableContent(25);
        $(".close-potential").addClass("max-opp-stage-selected");
        $(".sold").addClass("max-opp-stage-selected");
        $(".follow-up").addClass("max-opp-stage-selected");
        $(".pd-appointment").addClass("max-opp-stage-selected");
        $(".bidding").addClass("max-opp-stage-selected");
        $(".fv-presentation").addClass("max-opp-stage-selected");
        $(".initial-communication").addClass("max-opp-stage-selected");
        if (data.sold != null) { setSoldDataInClose(data.sold); }
        /*if (data.contact != null && data.bidSchedule != null) { updateContact(data); }
        if (data.pdContact != null && data.pdSchedule != null) { updateContact(data); }*/
    }

    if (data.result["StageStatus"] == 30) {
        resetStageClass();

        $(".sold").addClass("max-opp-stage-selected");
        $(".follow-up").addClass("max-opp-stage-selected");
        $(".pd-appointment").addClass("max-opp-stage-selected");
        $(".bidding").addClass("max-opp-stage-selected");
        $(".fv-presentation").addClass("max-opp-stage-selected");
        $(".initial-communication").addClass("max-opp-stage-selected");
        if (data.fvpresentation != null) { setFvPresentationDataInClose(data.fvpresentation); }
        if (data.contact != null && data.bidSchedule != null) { updateContact(data); }
        if (data.pdContact != null && data.pdSchedule != null) { updateContact(data); }
    }

    /**
     *contact
     */

    $("#scheduleaccountdetailid").val(data.result["CRM_AccountCustomerDetailId"]);
    $("#accountdetailid").val(data.result["CRM_AccountCustomerDetailId"]);
    $("#input_contactname").val(data.result["ContactName"]);
    $("#input_contacttitle").val(data.result["Title"]);
    $("#input_contactphonenumber").val(data.result["PhoneNumber"]);
    $("#input_contactemail").val(data.result["EmailAddress"]);
    /**
    *company Information
    */
    $("#input_companyname").val(data.result["CompanyName"]);
    $("#select_industrytype").val(data.result["AccountTypeListId"]);
    $("#input_numberoflocations").val(data.result["NumberOfLocations"]);
    $("#input_companyphonenumber").val(data.result["CompanyPhoneNumber"]);
    $("#input_companyfaxnumber").val(data.result["CompanyFaxNumber"]);
    $("#input_companyemail").val(data.result["CompanyEmailAddress"]);
    $("#input_companywebsite").val(data.result["CompanyWebSite"]);
    /**
    * Company Address
    */

    $("#input_addressline1").val(data.result["CompanyAddressLine1"]);
    $("#input_addressline2").val(data.result["CompanyAddressLine2"]);
    $("#input_city").val(data.result["CompanyCity"]);
    $("#input_county").val(data.result["CompanyCounty"]);
    $("#select_state").val(data.result["CompanyState"]);
    $("#input_zipcode").val(data.result["CompanyZipCode"]);

    $("#input_sqft").val(data.result["SqFt"]);
    $("#input_lineofbusiness").val(data.result["LineofBusiness"]);
    $("#input_salesvolume").val(data.result["SalesVolume"]);

    /**
    * Lead Detail
    */
    $("#accountid").val(data.result["CRM_AccountId"]);
    $("#select_stagestatus").val(data.result["StageStatus"]);
    $("#select_providertype").val(data.result["ProviderType"]);
    $("#select_providersource").val(data.result["ProviderSource"]);
    $("#input_budgetamount").val(data.result["BudgetAmount"]);






    $("#hdfSelectedAccountId").val(data.result.CRM_AccountId);
    $("#hdfselectedaccountdetailid").val(data.result.CRM_AccountCustomerDetailId);

    /**
    * Show Activity
    */
    $.each(data.activity, function (key, item) {

        var contentActivity = '<div class="mt-comment-body">';
        contentActivity += '<div class="mt-comment-info space-note">';
        if (item.ActivityType == 1)
            contentActivity += '<i class="fa fa-phone" style="float: left;"></i>';
        if (item.ActivityType == 2)
            contentActivity += '<i class="fa fa-envelope" style="float: left;"></i>';
        if (item.ActivityType == 3)
            contentActivity += '<i class="fa fa-calendar" style="float: left;"></i>';

        contentActivity += '<span class="mt-activitytitle">' + item.ActivityTypeName + '</span>';

        if (item.OutComeTypeName != null)
            contentActivity += '<span class="item-label badge-status">' + item.OutComeTypeName + '</span>';
        contentActivity += '<span class="mt-comment-date">' + formatJSONDate(item.TimeStamp) + '</span>';
        contentActivity += '</div>';
        contentActivity += '<div class="mt-Note-text ">' + item.Note + '</div>';
        contentActivity += '</div>';
        contentActivity += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#content_todo").append(contentActivity);
    });



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
        contentSchedule += '<span class="mt-scheduletime">' + ScheduleTime(item.StartDate) + ' Hour </span>';
        contentSchedule += '</div>';
        contentSchedule += '<div class="mt-Note-text">' + item.Description + '</div>';
        contentSchedule += '</div>';
        contentSchedule += '</div>';
        contentSchedule += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#content_schedule").append(contentSchedule);
    });



    /**
   * Notes
   */
    $.each(data.note, function (key, item) {

        var contentNote = '<div class="mt-comment">';
        contentNote += '<div class="mt-comment-body">';
        contentNote += '<div class="mt-comment-info">';
        contentNote += '<span class="mt-title">' + item.Title + '</span>';
        contentNote += '</div>';
        contentNote += '<div class="mt-Note-text">' + item.Description + '</div>';
        contentNote += '</div>';
        contentNote += '</div>';
        contentNote += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#content_note").append(contentNote);
    });


    $.each(data.document, function (key, item) {

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
        $("#content_document").append(contentDocument);
    });



    if (data.initial != null) {
        updateinitialActivity(data);
    }

    if (data.fvpresentation != null) {
        updatefvpresentationActivity(data);
    }

    if (data.bidding != null) {
        updateBiddingActivity(data);

    }

    if (data.pdappointment != null) {

        var contentpdappointment = '<div class="mt-comment">';
        contentpdappointment += '<div class="mt-comment-img">';
        contentpdappointment += '<h3 class="mt-date-text text-center"></h3>';
        contentpdappointment += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
        contentpdappointment += '</div>';
        contentpdappointment += '<div class="mt-comment-body">';
        contentpdappointment += '<div class="mt-comment-info">';
        contentpdappointment += '<span class="mt-title">' + "Pd Appointment" + '</span>';
        if (data.pdSchedule != null)
            contentpdappointment += '<span class="mt-comment-date">' + formatJSONDate(data.pdSchedule["StartDate"]) + '</span>';
        else
            contentpdappointment += '<span class="mt-comment-date">' + formatJSONDate(data.pdappointment["CreatedDate"]) + '</span>';

        contentpdappointment += '</div>';
        contentpdappointment += '<div class="mt-Note-text" style="margin-top:4px;">' + data.pdappointment["Note"] + '</div>';
        contentpdappointment += '<div class="mt-comment-info">';
        contentpdappointment += '<span class="mt-comment-date" style="margin-top:8px;"><a onclick="OpenPDAppointmentPopup(' + data.initial["CRM_AccountCustomerDetailId"] + ')" data-toggle="modal">View Detail</a></span>';
        contentpdappointment += '</div>';
        contentpdappointment += '</div>';
        contentpdappointment += '</div>';
        contentpdappointment += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#content_activity").append(contentpdappointment);

        data.pdappointment["MeetDecisionMaker"] == true ? $("#modal_yesdecisionmaker").prop("checked", true) : $("#modal_nodecisionmaker").prop("checked", true);

        if (!(data.pdappointment["MeetDecisionMaker"])) {
            $("#schedule_modal_decision").css("display", "block");
        }

        data.pdappointment["PresentProposal"] == true ? $("#checkboxModal_pdproposal").prop("checked", true) : $("#checkboxModal_pdproposal").prop("checked", false);

        data.pdappointment["OverComeObjections"] == true ? $("#selectModal_pdovercome").val("1") : $("#selectModal_pdovercome").val("0")



        data.pdappointment["EnteredProposalDetail"] == true ? $("#checkboxModal_proposaldetail").prop("checked", true) : $("#checkboxModal_proposaldetail").prop("checked", false);




        $("#textModal_pd_note").val(data.pdappointment["Note"]);
    }

    if (data.followup != null) {
        updateFollowupActivity(data);
    }

    if (data.sold != null) {
        updateSoldActivity(data);
    }

    getSchedules(data.result["CRM_AccountCustomerDetailId"]);

    //BindFVPR(data.result.CRM_AccountCustomerDetailId, data.result.CRM_AccountId, data.result.StageStatus)

    if (data.result["StageStatus"] == 16) {
        resetStageClass(); $(".initial-communication").addClass("max-opp-stage-selected");

    }
    if (data.result["StageStatus"] == 21) {
        resetStageClass(); $(".fv-presentation").addClass("max-opp-stage-selected"); $(".initial-communication").addClass("max-opp-stage-selected");

    }
    if (data.result["StageStatus"] == 22) {
        resetStageClass(); $(".bidding").addClass("max-opp-stage-selected"); $(".fv-presentation").addClass("max-opp-stage-selected"); $(".initial-communication").addClass("max-opp-stage-selected");

    }
    if (data.result["StageStatus"] == 23) {
        resetStageClass();
        $(".pd-appointment").addClass("max-opp-stage-selected");
        $(".bidding").addClass("max-opp-stage-selected");
        $(".fv-presentation").addClass("max-opp-stage-selected");
        $(".initial-communication").addClass("max-opp-stage-selected");

        if (data.contact != null && data.bidSchedule != null) {
            updateContact(data);
        }
    }
    $.unblockUI();
}

var setReportData = function (data) {
    //Hide Report
    hideReport();
    //Company Info

    $("#hdfselectedaccountdetailid").val(data.result["CRM_AccountCustomerDetailId"])

    $("#accountdetailid").val(data.result["CRM_AccountCustomerDetailId"])
    $("#input_reportcontactname").val(data.result["ContactName"]);
    //$("#input_reportcontactlastname").val(data.result["LastName"]);
    $("#input_reportcontactphonenumber").val(data.result["PhoneNumber"]);
    $("#input_reportcontactemail").val(data.result["EmailAddress"]);

    //Address

    $("#input_reportcompanyname").val(data.result["CompanyName"]);
    $("#select_reportindustrytype").val(data.result["AccountTypeListId"]);
    $("#input_reportnumberoflocations").val(data.result["NumberOfLocations"]);
    $("#input_reportcompanyphonenumber").val(data.result["CompanyPhoneNumber"]);
    $("#input_reportcompanyfaxnumber").val(data.result["CompanyFaxNumber"]);
    $("#input_reportcompanyemail").val(data.result["CompanyEmailAddress"]);
    $("#input_reportcompanywebsite").val(data.result["CompanyWebSite"]);

    //Contact Info
    $("#input_reportaddressline1").val(data.result["CompanyAddressLine1"]);
    $("#input_reportaddressline2").val(data.result["CompanyAddressLine2"]);
    $("#input_reportcity").val(data.result["CompanyCity"]);
    $("#input_reportcounty").val(data.result["CompanyCounty"]);
    $("#select_reportstate").val(data.result["CompanyState"]);
    $("#input_reportzipcode").val(data.result["CompanyZipCode"]);



    //Initial Communication
    if (data.initial != null) {
        $("#report_initial").show();

        $("#input_reportcontactperson").val(data.initial["ContactPerson"]);
        data.initial["AvailableToMeet"] != null ? $("#inputschedule_reportavailabletomeet").val(formatJSONDate(data.initial["AvailableToMeet"])) : $("#inputreport_availabletomeetmodal").val("");

        $("#textschedule_report").val(data.initial["Note"]);
        data.initial["InterestedInPerposal"] == true ? $("#select_reportinterestedinproposal").val(1) : $("#select_reportinterestedinproposal").val(0);
        $("#selectschedule_reportschedulepurpose").val(data.initial["PURPOSE"]);
    }

    //Fv Presentation
    if (data.fvpresentation != null) {
        $("#report_fv").show();

        $("#input_reportmeasurecontactperson").val(data.fvpresentation["MeasureContactPerson"]);
        $("#input_reportmeasurefacility").val(data.fvpresentation["MeasureFacility"]);
        $("#input_reportnumoffloors").val(data.fvpresentation["NumberOfFloors"]);


        /*Cleaning Days*/

        data.fvpresentation["Mon"] == true ? $("#report_mon").prop("checked", true) : $("#Mon").prop("checked", false);
        data.fvpresentation["Tue"] == true ? $("#report_tue").prop("checked", true) : $("#Tue").prop("checked", false);
        data.fvpresentation["Wed"] == true ? $("#report_wed").prop("checked", true) : $("#Wed").prop("checked", false);
        data.fvpresentation["Thu"] == true ? $("#report_thu").prop("checked", true) : $("#Thu").prop("checked", false);
        data.fvpresentation["Fri"] == true ? $("#report_fri").prop("checked", true) : $("#Fri").prop("checked", false);
        data.fvpresentation["Sat"] == true ? $("#report_sat").prop("checked", true) : $("#Sat").prop("checked", false);
        data.fvpresentation["Sun"] == true ? $("#report_sun").prop("checked", true) : $("#Sun").prop("checked", false);

        $("#select_reportfrequency").val(data.fvpresentation["BillingFrequency"]);
        $("#select_reportservicelevel").val(data.fvpresentation["ServiceLevel"]);
        $("#select_reportcleanfrequency").val(data.fvpresentation["CleanFrequency"]);
        $("#input_reportdocumentbudget").val(data.fvpresentation["BudgetAmount"]);
        $("#text_reportdocument").val(data.fvpresentation["Note"]);
    }


    //Bidding

    if (data.bidding != null) {
        $("#report_bidding").show();

        data.bidding["AnalysisWorkBook"] == true ? $("#checkboxreport_accountworkbook").prop("checked", true) : $("#checkboxreport_accountworkbook").prop("checked", false);
        $("#input_reportmonthlyprice").val(data.bidding["MonthlyPrice"]);
        $("#select_reportpriceapproved").val(data.bidding["PriceApproved"]);
        data.bidding["IfBidOver"] == true ? $("#report_yesbidover").prop("checked", true) : $("#report_nobidover").prop("checked", true);
        $("#input_reporticewprice").val(data.bidding["IncludePrice"]);
        $("#text_bidreport").val(data.bidding["Note"]);
    }


    //Pd Appointment
    if (data.pdappointment != null) {
        $("#report_pd").show();

        $("#inputreport_pdschedulemeetwith").val(data.pdappointment["MeetPersonName"]);
        /* $("#inputreport_pdschedule").val(data.stagestatuschedules[1].Schedule1Format); */

        data.pdappointment["MeetDecisionMaker"] == true ? $("#pdschedulereport_yesdecisionmaker").prop("checked", true) : $("#pdschedulereport_nodecisionmaker").prop("checked", true);

        data.pdappointment["PresentProposal"] == true ? $("#checkboxpdschedulereport_presentproposal").prop("checked", true) : $("#checkboxpdschedulereport_presentproposal").prop("checked", false);

        data.pdappointment["OverComeObjections"] == true ? $("#selectreport_pdscheduleovercome").val("1") : $("#selectreport_pdscheduleovercome").val("0")

        $("#textreport_pdcommentschedule").val(data.pdappointment["Comment"]);

        data.pdappointment["EnteredProposalDetail"] == true ? $("#checkboxpdschedulereport_proposaldetail").prop("checked", true) : $("#checkboxpdschedulereport_proposaldetail").prop("checked", false);

        /*  $("#inputreport_pdschedule_callback").val(data.stagestatuschedules[1].Schedule2Format);
  
          $("#selectreport_pdschedule_purpose").val(data.stagestatuschedules[1].Purpose2);  */

        $("#textreport_pdschedule_note").val(data.pdappointment["Note"]);
    }

    //Follow up
    if (data.followup != null) {
        $("#report_follow").show();

        $("#inputreport_followupavailabletomeet").val(data.stagestatuschedules[1].Schedule2Format);

        $("#selectreport_followupavailablemeetpurpose").val(data.stagestatuschedules[1].Purpose2);

        $("#selectreport_closetype").val(data.followup["Close"]);
        if (data.followup["Close"] != 2) {
            $("#divreport_reschedule").show();
            $("#inputreport_followmeetagain").val(data.stagestatuschedules[2].Schedule1Format);
            $("#selectreport_followmeetagainpurpose").val(data.stagestatuschedules[2].Purpose1);
        }
        else { $("#divreport_reschedule").hide(); }
        $("#textfollowupreport_initialcommunication").val(data.followup["Note"]);
    }




    //Close
}

var setSoldDataInClose = function (data) {

    /* Contactinfo Tab */
    data["Mon"] == true ? $("#close_mon").prop("checked", true) : $("#close_mon").prop("checked", false);
    data["Tue"] == true ? $("#close_tue").prop("checked", true) : $("#close_tue").prop("checked", false);
    data["Wed"] == true ? $("#close_wed").prop("checked", true) : $("#close_wed").prop("checked", false);
    data["Thu"] == true ? $("#close_thu").prop("checked", true) : $("#close_thu").prop("checked", false);
    data["Fri"] == true ? $("#close_fri").prop("checked", true) : $("#close_fri").prop("checked", false);
    data["Sat"] == true ? $("#close_sat").prop("checked", true) : $("#close_sat").prop("checked", false);
    data["Sun"] == true ? $("#close_sun").prop("checked", true) : $("#close_sun").prop("checked", false);

    $("#close_FrequencyListModel").val(data["BillingFrequency"]);
    $("#close_ServiceTypeListModel").val("5");
    $("#close_ContractDetail_CleanFrequency").val(data["CleanFrequency"]);
    $("#close_cleantime").val(data["CleanTime"]);

    data["HaveBackgroundCheck"] == true ? $("#close_yes_contract").prop("checked", true) : $("#close_no_contract").prop("checked", true);
    data["SignedAgreement"] == true ? $("#close_yes_signed").prop("checked", true) : $("#close_no_signed").prop("checked", true);
    data["DocumentSalesCRM"] == true ? $("#close_yes_documentsalescrm").prop("checked", true) : $("#close_no_documentsalescrm").prop("checked", true);
    data["NotifyAccountPlacement"] == true ? $("#close_yes_accountplacement").prop("checked", true) : $("#close_no_accountplacement").prop("checked", true);

    $("#close_propamount").val(data["PropAmount"]);
    $("#close_initialamount").val(data["InitialClean"]);
    $("#close_contractamount").val(data["ContractAmount"]);
    $("#close_initialcleanamount").val(data["InitialCleanAmount"]);

    $("#close_signdate").val(formatJSONDate(data["SignDate"]));
    $("#close_startdate").val(formatJSONDate(data["StartDate"]));
    $("#close_ponumber").val(data["PhoneNumber"]);
    $("#close_contractTerm").val(data["ContractTerm"]);
    $("#text_closegeneralnote").val(data["Note"]);








    /* Potential Detail Tab*/
    data["Mon"] == true ? $("#close_potential_mon").prop("checked", true) : $("#close_potential_mon").prop("checked", false);
    data["Tue"] == true ? $("#close_potential_tue").prop("checked", true) : $("#close_potential_tue").prop("checked", false);
    data["Wed"] == true ? $("#close_potential_wed").prop("checked", true) : $("#close_potential_wed").prop("checked", false);
    data["Thu"] == true ? $("#close_potential_thu").prop("checked", true) : $("#close_potential_thu").prop("checked", false);
    data["Fri"] == true ? $("#close_potential_fri").prop("checked", true) : $("#close_potential_fri").prop("checked", false);
    data["Sat"] == true ? $("#close_potential_sat").prop("checked", true) : $("#close_potential_sat").prop("checked", false);
    data["Sun"] == true ? $("#close_potential_sun").prop("checked", true) : $("#close_potential_sun").prop("checked", false);

    $("#close_potential_FrequencyListModel").val(data["BillingFrequency"]);
    $("#close_potential_ServiceTypeListModel").val("5");
    $("#close_potential_ContractDetail_CleanFrequency").val(data["CleanFrequency"]);
    $("#close_potential_cleantime").val(data["CleanTime"]);


    data["HaveBackgroundCheck"] == true ? $("#close_potential_yes_contract").prop("checked", true) : $("#close_potential_no_contract").prop("checked", true);
    data["SignedAgreement"] == true ? $("#close_potential_yes_signed").prop("checked", true) : $("#close_potential_no_signed").prop("checked", true);
    data["DocumentSalesCRM"] == true ? $("#close_potential_yes_documentsalescrm").prop("checked", true) : $("#close_potential_no_documentsalescrm").prop("checked", true);
    data["NotifyAccountPlacement"] == true ? $("#close_potential_yes_accountplacement").prop("checked", true) : $("#close_potential_no_accountplacement").prop("checked", true);

    $("#close_potential_propamount").val(data["PropAmount"]);
    $("#close_potential_initialamount").val(data["InitialClean"]);
    $("#close_potential_contractamount").val(data["ContractAmount"]);
    $("#close_potential_initialcleanamount").val(data["InitialCleanAmount"]);

    $("#close_potential_signdate").val(SetJSONDate(data["SignDate"]));
    $("#close_potential_startdate").val(SetJSONDate(data["StartDate"]));
    $("#close_potential_ponumber").val(data["PhoneNumber"]);
    $("#close_potential_contractterm").val(data["ContractTerm"]);
    $("#text_potential_closegeneralnote").val(data["Note"]);

    /* Activity Tab */
    data["Mon"] == true ? $("#close_activity_mon").prop("checked", true) : $("#close_activity_mon").prop("checked", false);
    data["Tue"] == true ? $("#close_activity_tue").prop("checked", true) : $("#close_activity_tue").prop("checked", false);
    data["Wed"] == true ? $("#close_activity_wed").prop("checked", true) : $("#close_activity_wed").prop("checked", false);
    data["Thu"] == true ? $("#close_activity_thu").prop("checked", true) : $("#close_activity_thu").prop("checked", false);
    data["Fri"] == true ? $("#close_activity_fri").prop("checked", true) : $("#close_activity_fri").prop("checked", false);
    data["Sat"] == true ? $("#close_activity_sat").prop("checked", true) : $("#close_activity_sat").prop("checked", false);
    data["Sun"] == true ? $("#close_activity_sun").prop("checked", true) : $("#close_activity_sun").prop("checked", false);


    $("#close_activity_FrequencyListModel").val(data["BillingFrequency"]);
    $("#close_activity_ServiceTypeListModel").val("5");
    $("#close_activity_ContractDetail_CleanFrequency").val(data["CleanFrequency"]);
    $("#close_activity_cleantime").val(data["CleanTime"]);

    data["HaveBackgroundCheck"] == true ? $("#close_activity_yes_contract").prop("checked", true) : $("#close_activity_no_contract").prop("checked", true);
    data["SignedAgreement"] == true ? $("#close_activity_yes_signed").prop("checked", true) : $("#close_activity_no_signed").prop("checked", true);
    data["DocumentSalesCRM"] == true ? $("#close_activity_yes_documentsalescrm").prop("checked", true) : $("#close_activity_no_documentsalescrm").prop("checked", true);
    data["NotifyAccountPlacement"] == true ? $("#close_activity_yes_accountplacement").prop("checked", true) : $("#close_activity_no_accountplacement").prop("checked", true);

    $("#close_activity_propamount").val(data["PropAmount"]);
    $("#close_activity_initialamount").val(data["InitialClean"]);
    $("#close_activity_contractamount").val(data["ContractAmount"]);
    $("#close_activity_initialcleanamount").val(data["InitialCleanAmount"]);

    $("#close_activity_signdate").val(formatJSONDate(data["SignDate"]));
    $("#close_activity_startdate").val(formatJSONDate(data["StartDate"]));
    $("#close_activity_ponumber").val(data["PhoneNumber"]);
    $("#close_activity_contractterm").val(data["ContractTerm"]);
    $("#text_activity_closegeneralnote").val(data["Note"]);


    /* Tasks Tab To-Do Tab*/
    data["Mon"] == true ? $("#close_todo_mon").prop("checked", true) : $("#close_todo_mon").prop("checked", false);
    data["Tue"] == true ? $("#close_todo_tue").prop("checked", true) : $("#close_todo_tue").prop("checked", false);
    data["Wed"] == true ? $("#close_todo_wed").prop("checked", true) : $("#close_todo_wed").prop("checked", false);
    data["Thu"] == true ? $("#close_todo_thu").prop("checked", true) : $("#close_todo_thu").prop("checked", false);
    data["Fri"] == true ? $("#close_todo_fri").prop("checked", true) : $("#close_todo_fri").prop("checked", false);
    data["Sat"] == true ? $("#close_todo_sat").prop("checked", true) : $("#close_todo_sat").prop("checked", false);
    data["Sun"] == true ? $("#close_todo_sun").prop("checked", true) : $("#close_todo_sun").prop("checked", false);

    $("#close_todo_FrequencyListModel").val(data["BillingFrequency"]);
    $("#close_todo_ServiceTypeListModel").val("5");
    $("#close_todo_ContractDetail_CleanFrequency").val(data["CleanFrequency"]);
    $("#close_todo_cleantime").val(data["CleanTime"]);

    data["HaveBackgroundCheck"] == true ? $("#close_todo_yes_contract").prop("checked", true) : $("#close_todo_no_contract").prop("checked", true);
    data["SignedAgreement"] == true ? $("#close_todo_yes_signed").prop("checked", true) : $("#close_todo_no_signed").prop("checked", true);
    data["DocumentSalesCRM"] == true ? $("#close_todo_yes_documentsalescrm").prop("checked", true) : $("#close_todo_no_documentsalescrm").prop("checked", true);
    data["NotifyAccountPlacement"] == true ? $("#close_todo_yes_accountplacement").prop("checked", true) : $("#close_todo_no_accountplacement").prop("checked", true);

    $("#close_todo_propamount").val(data["PropAmount"]);
    $("#close_todo_initialamount").val(data["InitialClean"]);
    $("#close_todo_contractamount").val(data["ContractAmount"]);
    $("#close_todo_initialcleanamount").val(data["InitialCleanAmount"]);

    $("#close_todo_signdate").val(formatJSONDate(data["SignDate"]));
    $("#close_todo_startdate").val(formatJSONDate(data["StartDate"]));
    $("#close_todo_ponumber").val(data["PhoneNumber"]);
    $("#close_todo_contractterm").val(data["ContractTerm"]);
    $("#text_todo_closegeneralnote").val(data["Note"]);

    /* Personal Note Tab Note Tab */
    data["Mon"] == true ? $("#close_note_mon").prop("checked", true) : $("#close_note_mon").prop("checked", false);
    data["Tue"] == true ? $("#close_note_tue").prop("checked", true) : $("#close_note_tue").prop("checked", false);
    data["Wed"] == true ? $("#close_note_wed").prop("checked", true) : $("#close_note_wed").prop("checked", false);
    data["Thu"] == true ? $("#close_note_thu").prop("checked", true) : $("#close_note_thu").prop("checked", false);
    data["Fri"] == true ? $("#close_note_fri").prop("checked", true) : $("#close_note_fri").prop("checked", false);
    data["Sat"] == true ? $("#close_note_sat").prop("checked", true) : $("#close_note_sat").prop("checked", false);
    data["Sun"] == true ? $("#close_note_sun").prop("checked", true) : $("#close_note_sun").prop("checked", false);

    $("#close_note_FrequencyListModel").val(data["BillingFrequency"]);
    $("#close_note_ServiceTypeListModel").val("5");
    $("#close_note_ContractDetail_CleanFrequency").val(data["CleanFrequency"]);
    $("#close_note_cleantime").val(data["CleanTime"]);

    data["HaveBackgroundCheck"] == true ? $("#close_note_yes_contract").prop("checked", true) : $("#close_note_no_contract").prop("checked", true);
    data["SignedAgreement"] == true ? $("#close_note_yes_signed").prop("checked", true) : $("#close_note_no_signed").prop("checked", true);
    data["DocumentSalesCRM"] == true ? $("#close_note_yes_documentsalescrm").prop("checked", true) : $("#sold_note_no_documentsalescrm").prop("checked", true);
    data["NotifyAccountPlacement"] == true ? $("#close_note_yes_accountplacement").prop("checked", true) : $("#close_note_no_accountplacement").prop("checked", true);

    $("#close_note_propamount").val(data["PropAmount"]);
    $("#close_note_initialamount").val(data["InitialClean"]);
    $("#close_note_contractamount").val(data["ContractAmount"]);
    $("#close_note_initialcleanamount").val(data["InitialCleanAmount"]);

    $("#close_note_signdate").val(formatJSONDate(data["SignDate"]));
    $("#close_note_startdate").val(formatJSONDate(data["StartDate"]));
    $("#close_note_ponumber").val(data["PhoneNumber"]);
    $("#close_note_contractterm").val(data["ContractTerm"]);
    $("#text_note_closegeneralnote").val(data["Note"]);

    /* Document Tab */
    data["Mon"] == true ? $("#close_document_mon").prop("checked", true) : $("#close_note_mon").prop("checked", false);
    data["Tue"] == true ? $("#close_document_tue").prop("checked", true) : $("#close_note_tue").prop("checked", false);
    data["Wed"] == true ? $("#close_document_wed").prop("checked", true) : $("#close_note_wed").prop("checked", false);
    data["Thu"] == true ? $("#close_document_thu").prop("checked", true) : $("#close_note_thu").prop("checked", false);
    data["Fri"] == true ? $("#close_document_fri").prop("checked", true) : $("#close_note_fri").prop("checked", false);
    data["Sat"] == true ? $("#close_document_sat").prop("checked", true) : $("#close_note_sat").prop("checked", false);
    data["Sun"] == true ? $("#close_document_sun").prop("checked", true) : $("#close_note_sun").prop("checked", false);


    $("#close_document_FrequencyListModel").val(data["BillingFrequency"]);
    $("#close_document_ServiceTypeListModel").val("5");
    $("#close_document_ContractDetail_CleanFrequency").val(data["CleanFrequency"]);
    $("#close_document_cleantime").val(data["CleanTime"]);

    data["HaveBackgroundCheck"] == true ? $("#close_document_yes_contract").prop("checked", true) : $("#close_document_no_contract").prop("checked", true);
    data["SignedAgreement"] == true ? $("#close_document_yes_signed").prop("checked", true) : $("#close_document_no_signed").prop("checked", true);
    data["DocumentSalesCRM"] == true ? $("#close_document_yes_documentsalescrm").prop("checked", true) : $("#close_document_no_documentsalescrm").prop("checked", true);
    data["NotifyAccountPlacement"] == true ? $("#close_document_yes_accountplacement").prop("checked", true) : $("#close_document_no_accountplacement").prop("checked", true);

    $("#close_document_propamount").val(data["PropAmount"]);
    $("#close_document_initialamount").val(data["InitialClean"]);
    $("#close_document_contractamount").val(data["ContractAmount"]);
    $("#close_document_initialcleanamount").val(data["InitialCleanAmount"]);

    $("#close_document_signdate").val(formatJSONDate(data["SignDate"]));
    $("#close_document_startdate").val(formatJSONDate(data["StartDate"]));
    $("#close_document_ponumber").val(data["PhoneNumber"]);
    $("#close_document_contractterm").val(data["ContractTerm"]);
    $("#text_document_closegeneralnote").val(data["Note"]);

    /* Schedule Tab */
    data["Mon"] == true ? $("#close_schedule_mon").prop("checked", true) : $("#close_note_mon").prop("checked", false);
    data["Tue"] == true ? $("#close_schedule_tue").prop("checked", true) : $("#close_note_tue").prop("checked", false);
    data["Wed"] == true ? $("#close_schedule_wed").prop("checked", true) : $("#close_note_wed").prop("checked", false);
    data["Thu"] == true ? $("#close_schedule_thu").prop("checked", true) : $("#close_note_thu").prop("checked", false);
    data["Fri"] == true ? $("#close_schedule_fri").prop("checked", true) : $("#close_note_fri").prop("checked", false);
    data["Sat"] == true ? $("#close_schedule_sat").prop("checked", true) : $("#close_note_sat").prop("checked", false);
    data["Sun"] == true ? $("#close_schedule_sun").prop("checked", true) : $("#close_note_sun").prop("checked", false);


    $("#close_schedule_FrequencyListModel").val(data["BillingFrequency"]);
    $("#close_schedule_ServiceTypeListModel").val("5");
    $("#close_schedule_ContractDetail_CleanFrequency").val(data["CleanFrequency"]);
    $("#close_schedule_cleantime").val(data["CleanTime"]);

    data["HaveBackgroundCheck"] == true ? $("#close_schedule_yes_contract").prop("checked", true) : $("#close_schedule_no_contract").prop("checked", true);
    data["SignedAgreement"] == true ? $("#close_schedule_yes_signed").prop("checked", true) : $("#close_schedule_no_signed").prop("checked", true);
    data["DocumentSalesCRM"] == true ? $("#close_schedule_yes_documentsalescrm").prop("checked", true) : $("#close_schedule_no_documentsalescrm").prop("checked", true);
    data["NotifyAccountPlacement"] == true ? $("#close_schedule_yes_accountplacement").prop("checked", true) : $("#close_schedule_no_accountplacement").prop("checked", true);

    $("#close_schedule_propamount").val(data["PropAmount"]);
    $("#close_schedule_initialamount").val(data["InitialClean"]);
    $("#close_schedule_contractamount").val(data["ContractAmount"]);
    $("#close_schedule_initialcleanamount").val(data["InitialCleanAmount"]);

    $("#close_schedule_signdate").val(formatJSONDate(data["SignDate"]));
    $("#close_schedule_startdate").val(formatJSONDate(data["StartDate"]));
    $("#close_schedule_ponumber").val(data["PhoneNumber"]);
    $("#close_schedule_contractterm").val(data["ContractTerm"]);
    $("#text_schedule_closegeneralnote").val(data["Note"]);

}

var setFvPresentationDataInClose = function (data) {

    /* Potential Tab */
    data["Mon"] == true ? $(".sold_mon").prop("checked", true) : $(".sold_mon").prop("checked", false);
    data["Tue"] == true ? $(".sold_tue").prop("checked", true) : $(".sold_tue").prop("checked", false);
    data["Wed"] == true ? $(".sold_wed").prop("checked", true) : $(".sold_wed").prop("checked", false);
    data["Thu"] == true ? $(".sold_thu").prop("checked", true) : $(".sold_thu").prop("checked", false);
    data["Fri"] == true ? $(".sold_fri").prop("checked", true) : $(".sold_fri").prop("checked", false);
    data["Sat"] == true ? $(".sold_sat").prop("checked", true) : $(".sold_sat").prop("checked", false);
    data["Sun"] == true ? $(".sold_sun").prop("checked", true) : $(".sold_sun").prop("checked", false);
    data["Weekend"] == true ? $(".sold_weekend").prop("checked", true) : $(".sold_weekend").prop("checked", false);

    $(".sold_FrequencyListModel").val(data["BillingFrequency"]);
    $(".sold_ServiceTypeListModel").val("5");
    $(".sold_ContractDetail_CleanFrequency").val(data["CleanFrequency"]);
    $(".sold_cleantime").val(data["CleanTime"]);
}

var setfollowup = function (value) {
    $("#input_followupavailabletomeet").val(formatJSONDate(value["StartDate"]));
    $("#select_followupavailablemeetpurpose").val(value["Purpose"]);

    $("#inputpotential_followupavailabletomeet").val(formatJSONDate(value["StartDate"]));
    $("#selectpotential_followupavailablemeetpurpose").val(value["Purpose"]);

    $("#inputactivity_followupavailabletomeet").val(formatJSONDate(value["StartDate"]));
    $("#selectactivity_followupavailablemeetpurpose").val(value["Purpose"]);

    $("#inputtodo_followupavailabletomeet").val(formatJSONDate(value["StartDate"]));
    $("#selecttodo_followupavailablemeetpurpose").val(value["Purpose"]);

    $("#inputnote_followupavailabletomeet").val(formatJSONDate(value["StartDate"]));
    $("#selectnote_followupavailablemeetpurpose").val(value["Purpose"]);

    $("#inputdocument_followupavailabletomeet").val(formatJSONDate(value["StartDate"]));
    $("#selectdocument_followupavailablemeetpurpose").val(value["Purpose"]);

    $("#inputschedule_followupavailabletomeet").val(value["StartDate"]);
    $("#selectschedule_followupavailablemeetpurpose").val(value["Purpose"]);
}

var refreshDocuments = function (data) {

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



var refreshSchedule = function (data) {

    if (data != null) {

        //Empty the Schedule html
        $("#content_schedule").empty();

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
            $("#content_schedule").append(contentSchedule);
        });

        /**
*Stage Status Schedules
*/

        $.each(data.stagestatuschedules, function (key, item) {
            if (item.Schedule1 != null) {
                var contentStageSchedule = '<div class="mt-comment">';
                contentStageSchedule += '<div class="mt-comment-img">';
                contentStageSchedule += '<h3 class="mt-date-text text-center">' + ScheduleDate(item.Schedule1) + '</h3>';
                contentStageSchedule += '<h5 class="mt-date-text text-center mt-schedule-monthSpace">' + ScheduleMonth(item.Schedule1) + '</h5>';
                contentStageSchedule += '</div>';
                contentStageSchedule += '<div class="mt-comment-body">';
                contentStageSchedule += '<div class="mt-comment-info">';
                contentStageSchedule += '<span class="mt-title">' + item.Title + '</span>';
                contentStageSchedule += '</div>';
                contentStageSchedule += '<div class="mt-comment-info">';
                contentStageSchedule += '<span class="mt-scheduletime">' + ScheduleTime(item.Schedule1) + '</span>';
                contentStageSchedule += '</div>';
                contentStageSchedule += '<div class="mt-Note-text"></div>';
                contentStageSchedule += '</div>';
                contentStageSchedule += '</div>';
                contentStageSchedule += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
                $("#content_schedule").append(contentStageSchedule);
            }

            if (item.Schedule2 != null) {
                var contentStageSchedule = '<div class="mt-comment">';
                contentStageSchedule += '<div class="mt-comment-img">';
                contentStageSchedule += '<h3 class="mt-date-text text-center">' + ScheduleDate(item.Schedule2) + '</h3>';
                contentStageSchedule += '<h5 class="mt-date-text text-center mt-schedule-monthSpace">' + ScheduleMonth(item.Schedule2) + '</h5>';
                contentStageSchedule += '</div>';
                contentStageSchedule += '<div class="mt-comment-body">';
                contentStageSchedule += '<div class="mt-comment-info">';
                contentStageSchedule += '<span class="mt-title">' + 'Available To Meet' + '</span>';
                contentStageSchedule += '</div>';
                contentStageSchedule += '<div class="mt-comment-info">';
                contentStageSchedule += '<span class="mt-scheduletime">' + ScheduleTime(item.Schedule2) + '</span>';
                contentStageSchedule += '</div>';
                contentStageSchedule += '<div class="mt-Note-text"></div>';
                contentStageSchedule += '</div>';
                contentStageSchedule += '</div>';
                contentStageSchedule += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
                $("#content_schedule").append(contentStageSchedule);
            }
        });
    }

    resetAddScheduleForm();
    JK.CRM.CRMApp.hideActivity("#schedule_modalview_footer");


}

var refreshInitial = function (data) {

    GetPotential();
    if (data.stage == 21) {
        resetStageClass(); $(".fv-presentation").addClass("max-opp-stage-selected");
        //enableContent(21);
    }
    JK.CRM.CRMApp.hideActivity("#initial_summary_action");
    refreshCalendar(data.calenderDates);
    refreshSchedule(data);

}

var refreshfv = function (data) {
    $.blockUI();
    getLeadDetail($('#hdfSelectedAccountId').val(), '0');
    //GetPotential();    //Refresh the Potential Table
    //$.unblockUI();
}

var refreshbid = function (data) {
    $.blockUI();
    getLeadDetail($('#hdfSelectedAccountId').val(), '0');
    //GetPotential();    //Refresh the Potential Table
    //$.unblockUI();


}

var refreshpd = function (data) {
    $.blockUI();
    getLeadDetail($('#hdfSelectedAccountId').val(), '0');
    //GetPotential();    //Refresh the Potential Table
    //$.unblockUI();

}

var refreshSold = function (data) {
    $.blockUI();
    getLeadDetail($('#hdfSelectedAccountId').val(), '0');
    //GetPotential();    //Refresh the Potential Table
    //$.unblockUI();
}

var refreshfollow = function (data) {
    $.blockUI();
    getLeadDetail($('#hdfSelectedAccountId').val(), '0');
    //GetPotential();    //Refresh the Potential Table
    //$.unblockUI();
}

var refreshNote = function (data) {

    $.blockUI();
    getLeadDetail($('#hdfSelectedAccountId').val(), '0');
    $.unblockUI();
}

var refreshToDo = function (data) {
     
    $.blockUI();
    getLeadDetail($('#hdfSelectedAccountId').val(), '0');
    $.unblockUI();
}

var refreshcontactinfo = function (data) {
    /**
    *contact
    */
    $("#accountdetailid").val(data.accountcustomer["CRM_AccountCustomerDetailId"])
    $("#input_contactname").val(data.result["ContactName"]);
    $("#input_contacttitle").val(data.accountcustomer["Title"]);
    $("#input_contactphonenumber").val(data.result["PhoneNumber"]);
    $("#input_contactemail").val(data.result["EmailAddress"]);
    /**
    *company Information
    */
    $("#input_companyname").val(data.accountcustomer["CompanyName"]);
    $("#select_industrytype").val(data.accountcustomer["AccountTypeListId"]);
    $("#input_numberoflocations").val(data.accountcustomer["NumberOfLocations"]);
    $("#input_companyphonenumber").val(data.accountcustomer["CompanyPhoneNumber"]);
    $("#input_companyfaxnumber").val(data.accountcustomer["CompanyFaxNumber"]);
    $("#input_companyemail").val(data.accountcustomer["CompanyEmailAddress"]);
    $("#input_companywebsite").val(data.accountcustomer["CompanyWebSite"]);
    /**
    * Company Address
    */
    $("#input_addressline1").val(data.accountcustomer["CompanyAddressLine1"]);
    $("#input_addressline2").val(data.accountcustomer["CompanyAddressLine2"]);
    $("#input_city").val(data.accountcustomer["CompanyCity"]);
    $("#input_county").val(data.accountcustomer["CompanyCounty"]);
    $("#select_state").val(data.accountcustomer["CompanyState"]);
    $("#input_zipcode").val(data.accountcustomer["CompanyZipCode"]);

    $("#input_sqft").val(data.accountcustomer["SqFt"]);
    $("#input_lineofbusiness").val(data.accountcustomer["LineofBusiness"]);
    $("#input_salesvolume").val(data.accountcustomer["SalesVolume"]);

    /*
    Save btn text into Edit ,Cancel hide ,Disable field
    */
    $("#editcontactinfo_button").text("Edit");
    $("#cancelcontactinfo_button").hide();

    disabledfield();
    $("#editcontactinfo_button").prop("disabled", false);
    $("#cancelcontactinfo_button").prop("disabled", false);

}

var refreshpotential = function (data) {
    /**
   * Lead Detail
   */
    $("#accountid").val(data.result["CRM_AccountId"]);
    $("#select_stagestatus").val(data.result["StageStatus"]);
    $("#select_providertype").val(data.result["ProviderType"]);
    $("#select_providersource").val(data.result["ProviderSource"]);
    $("#input_budgetamount").val(data.result["BudgetAmount"]);

    /*
   Save btn text into Edit ,Cancel hide ,Disable field
   */
    $("#editpotential_button").text("Edit");
    $("#cancelpotential_button").hide();

    disabledPotentialDetailField();
    $("#editpotential_button").prop("disabled", false);
    $("#cancelpotential_button").prop("disabled", false);
}

var resetUploadform = function () {
    $("#input_filename").val("");
    $("#input_filedescription").val("");
    $("#fileDocument").val("");
}

function UploadDocument() {
    $('#ModelCustomerUploadDocumentPopup').modal('show');
}


function OpenCRMUploadDocumentPopup() {
    debugger;
    var CRMAccCustDetailstage = $("#hdnCRMAccCustDetailStage").val();
    var aId = $("#hdfselectedaccountdetailid").val();
    if (Id = !"") {
        var sURL = "/CRM/CustomerSales/CRMUploadDocumentPopup?id=" + aId + "&sold=" + CRMAccCustDetailstage;
        $.ajax({
            type: "GET",
            url: sURL,
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                $('#RenderCRMUploadDocumentPopup').html(data);
                $("#ModelCRMUploadDocumentPopup").modal({ backdrop: 'static' });
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
}


function onClickCRMSaveDoc() {

    var CRMAccCustDetailId = $("#hdnCRMAccCustDetailId").val();
    var CRMAccCustDetailstage = $("#hdnCRMAccCustDetailStage").val();
    if (CRMAccCustDetailId != "") {

        if ($("#CRM_file_1").val() == "" && $("#hdndocFile_1").val() == "") {
            $('#errdoc_1').show();
            return false;
        }
        if ($("#CRM_file_2").val() == "" && $("#hdndocFile_2").val() == "") {
            $('#errdoc_2').show();
            return false;
        }
        if ($("#CRM_file_4").val() == "" && $("#hdndocFile_4").val() == "") {
            $('#errdoc_4').show();
            return false;
        }
        if ($("#CRM_file_6").val() == "" && $("#hdndocFile_6").val() == "" && CRMAccCustDetailstage == 1) {
            $('#errdoc_6').show();
            return false;
        }



        $('#errdoc_1').hide();
        $('#errdoc_2').hide();
        $('#errdoc_4').hide();
        $('#errdoc_6').hide();

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

            for (var i = 1; i <= documentCount; i++) {

                console.log(i);
                var CRM_file = $("#doc_file_" + i);
                console.log(CRM_file);

                if (CRM_file.length > 0 && CRM_file != null && CRM_file[0].files[0] != "" && CRM_file[0].files[0] != undefined) {
                    console.log('ok');
                    fileData.append("doc_file_" + i, CRM_file[0].files[0]);
                    fileData.append("txtDocName_" + $("#ddlDocFileType_" + i).val(), $("#txtDocName_" + i).val());
                    if (selIds == "") {
                        selIds = "Other_" + $("#ddlDocFileType_" + i).val();
                    }
                    else {
                        selIds += "," + "Other_" + $("#ddlDocFileType_" + i).val();
                    }
                }
            }

            fileData.append("selIds", selIds);
            fileData.append("CRMAccCustDetailId", CRMAccCustDetailId);


            $.ajax({
                url: "/CRM/CustomerSales/CRMSaveDocuments",
                type: "POST",
                contentType: false,
                processData: false,
                data: fileData,
                async: false,
                success: function (response) {
                    console.log(response);
                    refreshDocuments(response);
                    //OpenCRMUploadDocumentPopup();
                    refreshCRMDocumentDisplay(response.document);
                    //$(".clseditlnk").show();
                    $("#ModelCRMUploadDocumentPopup").modal("hide");

                    $(".checkboxcontactinfo_accountworkbook").prop("checked", true);
                    $(".checkboxcontactinfo_BidSheetbook").prop("checked", true);
                    $(".checkboxcontactinfo_Cleaningbook").prop("checked", true);


                },
                error: function (error) {
                    console.log(error);
                }
            });
        }
    }
}

function RemoveCRMDocument(Id, AccId) {
    $.ajax({
        url: "/CRM/CustomerSales/RemoveCRM_Document?Id=" + Id + "&CRMAccId=" + AccId,
        type: "POST",
        contentType: false,
        processData: false,
        async: false,
        success: function (response) {
            OpenCRMUploadDocumentPopup();
            refreshCRMDocumentDisplay(response.document);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function RemoveCRMDisplayDocument(Id, AccId) {
    $.ajax({
        url: "/CRM/CustomerSales/RemoveCRM_Document?Id=" + Id + "&CRMAccId=" + AccId,
        type: "POST",
        contentType: false,
        processData: false,
        async: false,
        success: function (response) {
            refreshCRMDocumentDisplay(response.document);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

var refreshCRMDocumentDisplay = function (data) {

    $(".CRMDocumentDisplay").html('');
    $.each(data, function (index, value) {
        var filepath = $("#hdnsiteurl").val() + "/Areas/CRM/Documents/" + value.File_Name;
        if (value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'pdf') {
            $(".CRMDocumentDisplay").append('<div class="col-md-3" style="margin-left: 30px;"><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_pdf.png" /></a></div>');
        }
        else if (value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'doc' || value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'docx') {
            $(".CRMDocumentDisplay").append('<div class="col-md-3" ><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_Doc.png" /></a></div>');
        }
        else if (value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'xls' || value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'xlsx' || value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'xlsm') {
            $(".CRMDocumentDisplay").append('<div class="col-md-3"><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_xls.png" /></a></div>');
        }
        else if (value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'txt') {
            $(".CRMDocumentDisplay").append('<div class="col-md-3"><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_Text.png" /></a></div>');
        }
        else if (value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'jpg' || value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'png' || value.File_Name.substr(value.File_Name.lastIndexOf('.') + 1) == 'jpeg') {
            $(".CRMDocumentDisplay").append('<div class="col-md-3"><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_image.png" /></a></div>');
        }
        else {
            $(".CRMDocumentDisplay").append('<div class="col-md-3"><a target="_blank" href=' + encodeURI(filepath) + '><img src="/Images/if_defult.png" /></a></div>');
        }
    });
    $.unblockUI();
}

var updateinitialActivity = function (data) {


    /*Set initial Data in Activity */
    if (data.initial != null) {

        var contentInitial = '<div class="mt-comment">';
        contentInitial += '<div class="mt-comment-img">';
        contentInitial += '<h3 class="mt-date-text text-center"></h3>';
        contentInitial += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
        contentInitial += '</div>';
        contentInitial += '<div class="mt-comment-body">';
        contentInitial += '<div class="mt-comment-info">';
        contentInitial += '<span class="mt-title">' + "Initial Communication" + '</span>';
        contentInitial += '<span class="mt-comment-date">' + formatJSONDate(data.initial["StartDate"]) + '</span>';
        contentInitial += '</div>';
        contentInitial += '<div class="mt-Note-text" style="margin-top:4px;">' + data.initial["Note"] + '</div>';
        contentInitial += '<div class="mt-comment-info">';
        contentInitial += '<span class="mt-comment-date" style="margin-top:8px;"><a onclick="OpenInitialCommunicationPopup(' + data.initial["CRM_AccountCustomerDetailId"] + ')" data-toggle="modal">View Detail</a></span>';
        contentInitial += '</div>';
        contentInitial += '</div>';
        contentInitial += '</div>';
        contentInitial += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#content_activity").append(contentInitial);

        $("#inputinitial_contactpersonmodal").val(data.initial["ContactPerson"]);
        data.initial["AvailableToMeet"] != null ? $("#inputinitial_availabletomeetmodal").val(formatJSONDate(data.initial["AvailableToMeet"])) : $("#inputinitial_availabletomeetmodal").val("");
        $("#text_initialcommunicationmodal").val(data.initial["Note"]);
        data.initial["InterestedInPerposal"] == true ? $("#selectinitial_interestedinproposalmodal").val(1) : $("#selectinitial_interestedinproposalmodal").val(0);
        $("#selectinitial_schedulepurposemodal").val(data.initial["PURPOSE"]);
    }
}


var updatefvpresentationActivity = function (data) {

    /*Set fvpresentation Data in Activity */
    if (data.fvpresentation != null) {

        var contentfv = '<div class="mt-comment">';
        contentfv += '<div class="mt-comment-img">';
        contentfv += '<h3 class="mt-date-text text-center"></h3>';
        contentfv += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
        contentfv += '</div>';
        contentfv += '<div class="mt-comment-body">';
        contentfv += '<div class="mt-comment-info">';
        contentfv += '<span class="mt-title">' + "Fv Presentation" + '</span>';
        contentfv += '<span class="mt-comment-date">' + formatJSONDate(data.initial["StartDate"]) + '</span>';
        contentfv += '</div>';
        contentfv += '<div class="mt-Note-text" style="margin-top:4px;">' + data.fvpresentation["Note"] + '</div>';
        contentfv += '<div class="mt-comment-info">';
        contentfv += '<span class="mt-comment-date" style="margin-top:8px;"><a onclick="OpenFVPresentationPopup(' + data.initial["CRM_AccountCustomerDetailId"] + ')" data-toggle="modal">View Detail</a></span>';
        contentfv += '</div>';
        contentfv += '</div>';
        contentfv += '</div>';
        contentfv += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#content_activity").append(contentfv);


        $("#input_measurecontactpersonmodal").val(data.fvpresentation["MeasureContactPerson"]);
        $("#input_measurefacilitymodal").val(data.fvpresentation["MeasureFacility"]);
        $("#input_numoffloorsmodal").val(data.fvpresentation["NumberOfFloors"]);
        /*Cleaning Days*/


        data.fvpresentation["Mon"] == true ? $("#Mon").prop("checked", true) : $("#Mon").prop("checked", false);
        data.fvpresentation["Tue"] == true ? $("#Tue").prop("checked", true) : $("#Tue").prop("checked", false);
        data.fvpresentation["Wed"] == true ? $("#Wed").prop("checked", true) : $("#Wed").prop("checked", false);
        data.fvpresentation["Thu"] == true ? $("#Thu").prop("checked", true) : $("#Thu").prop("checked", false);
        data.fvpresentation["Fri"] == true ? $("#Fri").prop("checked", true) : $("#Fri").prop("checked", false);
        data.fvpresentation["Sat"] == true ? $("#Sat").prop("checked", true) : $("#Sat").prop("checked", false);
        data.fvpresentation["Sun"] == true ? $("#Sun").prop("checked", true) : $("#Sun").prop("checked", false);

        $("#select_frequencymodal").val(data.fvpresentation["BillingFrequency"]);
        $("#select_servicetypemodal").val("5");
        $("#select_servicelevelmodal").val(data.fvpresentation["ServiceLevel"]);
        $("#select_cleanfrequencymodal").val(data.fvpresentation["CleanFrequency"]);
        $("#input_cleantimesmodal").val(data.fvpresentation["CleanTime"]);
        $("#input_budgetmodal").val(data.fvpresentation["BudgetAmount"]);
        $("#text_fvpresentationmodal").val(data.fvpresentation["Note"]);
    }
}


var updateBiddingActivity = function (data) {

    /*Set bidding Data in Activity */
    if (data.bidding != null) {

        var contentbidding = '<div class="mt-comment">';
        contentbidding += '<div class="mt-comment-img">';
        contentbidding += '<h3 class="mt-date-text text-center"></h3>';
        contentbidding += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
        contentbidding += '</div>';
        contentbidding += '<div class="mt-comment-body">';
        contentbidding += '<div class="mt-comment-info">';
        contentbidding += '<span class="mt-title">' + "Bidding" + '</span>';
        if (data.bidSchedule != null)
            contentbidding += '<span class="mt-comment-date">' + formatJSONDate(data.bidSchedule["StartDate"]) + '</span>';
        else
            contentbidding += '<span class="mt-comment-date">' + formatJSONDate(data.bidding["CreatedDate"]) + '</span>';
        contentbidding += '</div>';
        contentbidding += '<div class="mt-Note-text" style="margin-top:4px;">' + data.bidding["Note"] + '</div>';
        contentbidding += '<div class="mt-comment-info">';
        contentbidding += '<span class="mt-comment-date" style="margin-top:8px;"><a onclick="OpenBiddingPopup(' + data.initial["CRM_AccountCustomerDetailId"] + ')"  data-toggle="modal">View Detail</a></span>';
        contentbidding += '</div>';
        contentbidding += '</div>';
        contentbidding += '</div>';
        contentbidding += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#content_activity").append(contentbidding);

        data.bidding["AnalysisWorkBook"] == true ? $("#checkboxcontactinfoModal_accountworkbook").prop("checked", true) : $("#checkboxcontactinfoModal_accountworkbook").prop("checked", false)
        $("#input_monthlypricemodal").val(data.bidding["MonthlyPrice"]);
        $("#select_priceapprovedmodal").val(data.bidding["PriceApproved"]);
        data.bidding["IfBidOver"] == true ? $("#yes_bidovermodal").prop("checked", true) : $("#no_bidovermodal").prop("checked", true);
        $("#input_icewpricemodal").val(data.bidding["IncludePrice"]);
        $("#text_biddingmodal").val(data.bidding["Note"]);
    }
}

var updatePdAppointmentActivity = function (data) {

    /*Set pdappointment Data in Activity */
    if (data.pdappointment != null) {

        var contentpdappointment = '<div class="mt-comment">';
        contentpdappointment += '<div class="mt-comment-img">';
        contentpdappointment += '<h3 class="mt-date-text text-center"></h3>';
        contentpdappointment += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
        contentpdappointment += '</div>';
        contentpdappointment += '<div class="mt-comment-body">';
        contentpdappointment += '<div class="mt-comment-info">';
        contentpdappointment += '<span class="mt-title">' + "Pd Appointment" + '</span>';

        if (data.pdSchedule != null)
            contentpdappointment += '<span class="mt-comment-date">' + formatJSONDate(data.pdSchedule["StartDate"]) + '</span>';
        else
            contentpdappointment += '<span class="mt-comment-date">' + formatJSONDate(data.pdappointment["CreatedDate"]) + '</span>';


        contentpdappointment += '</div>';
        contentpdappointment += '<div class="mt-Note-text" style="margin-top:4px;">' + data.pdappointment["Note"] + '</div>';
        contentpdappointment += '<div class="mt-comment-info">';
        contentpdappointment += '<span class="mt-comment-date" style="margin-top:8px;"><a onclick="OpenPDAppointmentPopup(' + data.initial["CRM_AccountCustomerDetailId"] + ')" data-toggle="modal">View Detail</a></span>';
        contentpdappointment += '</div>';
        contentpdappointment += '</div>';
        contentpdappointment += '</div>';
        contentpdappointment += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#content_activity").append(contentpdappointment);


        $("#inputModal_pdmeetwith").val(data.pdappointment["MeetPersonName"]);
        /* $("#inputmodal_pdschedule").val(data.stagestatuschedules[1].Schedule1Format); */

        data.pdappointment["MeetDecisionMaker"] == true ? $("#modal_yesdecisionmaker").prop("checked", true) : $("#modal_nodecisionmaker").prop("checked", true);

        if (!(data.pdappointment["MeetDecisionMaker"])) {
            $("#schedule_modal_decision").css("display", "block");
        }

        data.pdappointment["PresentProposal"] == true ? $("#checkboxModal_pdproposal").prop("checked", true) : $("#checkboxModal_pdproposal").prop("checked", false);

        data.pdappointment["OverComeObjections"] == true ? $("#selectModal_pdovercome").val("1") : $("#selectModal_pdovercome").val("0")

        //$("#textModal_pdcomment").val(data.pdappointment["Comment"]);

        data.pdappointment["EnteredProposalDetail"] == true ? $("#checkboxModal_proposaldetail").prop("checked", true) : $("#checkboxModal_proposaldetail").prop("checked", false);


        /* $("#inputModal_pd_callback").val(data.stagestatuschedules[1].Schedule2Format);
 
         $("#selectModal_pd_schedulepurpose").val(data.stagestatuschedules[1].Purpose2); */

        $("#textModal_pd_note").val(data.pdappointment["Note"]);
    }
}

var updateFollowupActivity = function (data) {
    /*Set followup Data in Activity */
    if (data.followup != null) {

        var contentfollowup = '<div class="mt-comment">';
        contentfollowup += '<div class="mt-comment-img">';
        contentfollowup += '<h3 class="mt-date-text text-center"></h3>';
        contentfollowup += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
        contentfollowup += '</div>';
        contentfollowup += '<div class="mt-comment-body">';
        contentfollowup += '<div class="mt-comment-info">';
        contentfollowup += '<span class="mt-title">' + "Follow-Up" + '</span>';

        if (data.followSchedule != null)
            contentfollowup += '<span class="mt-comment-date">' + formatJSONDate(data.followSchedule["StartDate"]) + '</span>';
        else
            contentfollowup += '<span class="mt-comment-date">' + formatJSONDate(data.followup["CreatedDate"]) + '</span>';

        contentfollowup += '</div>';
        contentfollowup += '<div class="mt-Note-text" style="margin-top:4px;">' + data.followup["Note"] + '</div>';
        contentfollowup += '<div class="mt-comment-info">';
        contentfollowup += '<span class="mt-comment-date" style="margin-top:8px;"><a onclick="OpenPDFollowUpPopup(' + data.initial["CRM_AccountCustomerDetailId"] + ')"  data-toggle="modal">View Detail</a></span>';
        contentfollowup += '</div>';
        contentfollowup += '</div>';
        contentfollowup += '</div>';
        contentfollowup += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#content_activity").append(contentfollowup);

        if (data.followSchedule != null) {
            $("#inputModal_followupvailabletomeet").val(formatJSONDate(data.followSchedule["StartDate"]));
            $("#selectModal_followupavailablemeetpurpose").val(data.followSchedule["Purpose"]);
        }
        else {
            $("#inputModal_followupvailabletomeet").val("March 7, 2018, 2:30 PM");
            $("#selectModal_followupavailablemeetpurpose").val(3);
        }

        $("#selectModal_closetype").val(data.followup["Close"]);
        if (data.followup["Close"] != 2) {
            $("#divModal_reschedule").show();
            //$("#inputModal_followmeetagain").val(data.stagestatuschedules[2].Schedule1Format);
            //$("#selectModal_followmeetagainpurpose").val(data.stagestatuschedules[2].Purpose1);
        }
        else { $("#divModal_reschedule").hide(); }
        $("#textfollowupModal_initialcommunication").val(data.followup["Note"]);
    }

}

var updateSoldActivity = function (data) {

    /*Set Sold Data in Activity */
    if (data.sold != null) {


        var contentsold = '<div class="mt-comment">';
        contentsold += '<div class="mt-comment-img">';
        contentsold += '<h3 class="mt-date-text text-center"></h3>';
        contentsold += '<h5 class="mt-date-text text-center mt-schedule-monthSpace"></h5>';
        contentsold += '</div>';
        contentsold += '<div class="mt-comment-body">';
        contentsold += '<div class="mt-comment-info">';
        contentsold += '<span class="mt-title">' + "Sold" + '</span>';
        if (data.sold != null)
            contentsold += '<span class="mt-comment-date">' + formatJSONDate(data.sold["StartDate"]) + '</span>';
        else
            contentsold += '<span class="mt-comment-date">' + formatJSONDate(data.sold["CreatedDate"]) + '</span>';
        contentsold += '</div>';
        contentsold += '<div class="mt-Note-text" style="margin-top:4px;">' + (data.sold["Note"] == null ? " " : data.sold["Note"]) + '</div>';
        contentsold += '<div class="mt-comment-info">';
        contentsold += '<span class="mt-comment-date" style="margin-top:8px;"><a  onclick="OpenSoldPopup(' + data.initial["CRM_AccountCustomerDetailId"] + ')"  data-toggle="modal">View Detail</a></span>';
        contentsold += '</div>';
        contentsold += '</div>';
        contentsold += '</div>';
        contentsold += '<h5 style="border-bottom: 1px solid #e7ecf1;"></h5>';
        $("#content_activity").append(contentsold);


        data.sold["HaveBackgroundCheck"] == true ? $("#soldmodal_yes_contract").prop("checked", true) : $("#soldmodal_no_contract").prop("checked", true);
        data.sold["SignedAgreement"] == true ? $("#soldmodal_yes_signed").prop("checked", true) : $("#soldmodal_no_signed").prop("checked", true);
        data.sold["DocumentSalesCRM"] == true ? $("#soldmodal_yes_documentsalescrm").prop("checked", true) : $("#soldmodal_no_documentsalescrm").prop("checked", true);
        data.sold["NotifyAccountPlacement"] == true ? $("#soldmodal_yes_accountplacement").prop("checked", true) : $("#soldmodal_no_accountplacement").prop("checked", true);


        $("#soldmodal_propamount").val(data.sold["PropAmount"]);
        $("#soldmodal_initialamount").val(data.sold["InitialClean"]);
        $("#soldmodal_contractamount").val(data.sold["ContractAmount"]);
        $("#soldmodal_initialcleanamount").val(data.sold["InitialCleanAmount"]);

        $("#soldmodal_signdate").val(formatJSONDate(data.sold["SignDate"]));
        $("#soldmodal_startdate").val(formatJSONDate(data.sold["StartDate"]));
        $("#soldmodal_ponumber").val(data.sold["PhoneNumber"]);
        $("#soldmodal_contractTerm").val(data.sold["ContractTerm"]);
        $("#text_soldmodal").val(data.sold["Note"]);

        $("#soldmodal_FrequencyListModel").val(data.sold["BillingFrequency"]);
        $("#soldmodal_ServiceTypeListModel").val("5");
        $("#soldmodal_ContractDetail_CleanFrequency").val(data.sold["CleanFrequency"]);
        $("#soldmodal_cleantime").val(data.sold["CleanTime"]);

        data.sold["Mon"] == true ? $("#soldmodal_mon").prop("checked", true) : $("#soldmodal_mon").prop("checked", false);
        data.sold["Tue"] == true ? $("#soldmodal_tue").prop("checked", true) : $("#soldmodal_tue").prop("checked", false);
        data.sold["Wed"] == true ? $("#soldmodal_wed").prop("checked", true) : $("#soldmodal_wed").prop("checked", false);
        data.sold["Thu"] == true ? $("#soldmodal_thu").prop("checked", true) : $("#soldmodal_thu").prop("checked", false);
        data.sold["Fri"] == true ? $("#soldmodal_fri").prop("checked", true) : $("#soldmodal_fri").prop("checked", false);
        data.sold["Sat"] == true ? $("#soldmodal_sat").prop("checked", true) : $("#soldmodal_sat").prop("checked", false);
        data.sold["Sun"] == true ? $("#soldmodal_sun").prop("checked", true) : $("#soldmodal_sun").prop("checked", false);

    }
}

var updateContact = function (data) {

    $("#hdfselectedaccountdetailid").val(data.result.CRM_AccountCustomerDetailId);
    //BindFVPR(data.result.CRM_AccountCustomerDetailId, data.result.CRM_AccountId, data.result.StageStatus, data)



}

var updateContactU = function (data) {
    $.blockUI();
    if (data.contact != null && data.bidSchedule != null) {
        debugger;

        /*Bidding Modal */
        $("#input_bidding_modal_meetwith").val(data.contact["ContactName"]);
        $("#input_bidding_modal_contactphone").val(data.contact["ContactPhone"]);
        $("#input_bidding_modal_contactemail").val(data.contact["ContactEmail"]);
        $("#input_bidding_model_schedule").val(SetJSONDate(data.bidSchedule["StartDate"]));

        /*PdAppointment Modal */
        $("#input_pdpotentialmeetwith").val(data.contact["ContactName"]);
        $("#input_pdcontactphonepotential").val(data.contact["ContactPhone"]);
        $("#input_pdcontactemailpotential").val(data.contact["ContactEmail"]);
        $("#input_pdbid_potential_schedulestartdate").val(SetJSONDate(data.bidSchedule["StartDate"]));
        $("#input_pdbid_potential_schedulestarttime").val(SetJSONtime(data.bidSchedule["StartDate"]));
        $("#input_pdbid_potential_scheduleenddate").val(SetJSONDate(data.bidSchedule["EndDate"]));
        $("#input_pdbid_potential_scheduleendtime").val(SetJSONtime(data.bidSchedule["EndDate"]));
        $("#select_pdbid_pdpotential_Schedulepurpose").val(data.bidSchedule.PurposeId);



        $("#input_pdpotentialmeetwith").prop("disabled", true)
        $("#input_pdcontactphonepotential").prop("disabled", true)
        $("#input_pdcontactemailpotential").prop("disabled", true)
        $("#inputmodal_pdschedule").prop("disabled", true)
        $("#input_pdbid_potential_schedulestartdate").prop("disabled", true)
        $("#input_pdbid_potential_schedulestarttime").prop("disabled", true)
        $("#input_pdbid_potential_scheduleenddate").prop("disabled", true)
        $("#input_pdbid_potential_scheduleendtime").prop("disabled", true)
        $("#select_pdbid_pdpotential_Schedulepurpose").prop("disabled", true)



        /*Set Contact in PdAppointment*/
        /* Contact info */
        //$(".input_bidcontactname").val(data.contact["ContactName"]);
        //$(".input_bidcontactphone").val(data.contact["ContactPhone"]);
        //$(".input_bidcontactemail").val(data.contact["ContactEmail"]);

        //$("#input_pdbid_potential_schedulestartdate").val(SetJSONDate(data.bidSchedule["StartDate"]));
        //$("#input_pdbid_potential_schedulestarttime").val(SetJSONtime(data.bidSchedule["StartDate"]));
        //$("#input_pdbid_potential_scheduleenddate").val(SetJSONDate(data.bidSchedule["EndDate"]));
        //$("#input_pdbid_potential_scheduleendtime").val(SetJSONtime(data.bidSchedule["EndDate"]));
        // $("#input_pdschedulecontactinfo").val(formatJSONDate(data.bidSchedule["StartDate"]));

    }

    // allow followup to use previous followup as data source

    var scheduleForFollowUp = data.pdSchedule;
    if (data.followup != null && data.followSchedule != null)
        scheduleForFollowUp = data.followSchedule;

    if (scheduleForFollowUp != null) {



        /*PdAppointment Modal */

        $("#input_followupPd_startdate").val(SetJSONDate(scheduleForFollowUp["StartDate"]));
        $("#input_followupPd_starttime").val(SetJSONtime(scheduleForFollowUp["StartDate"]));
        $("#input_followupPd_enddate").val(SetJSONDate(scheduleForFollowUp["EndDate"]));
        $("#input_followupPd_endtime").val(SetJSONtime(scheduleForFollowUp["EndDate"]));
        $("#select_followupPdpurpose").val(data.pdSchedule.PurposeId);



        $("#input_followupPd_startdate").prop("disabled", true)
        $("#input_followupPd_starttime").prop("disabled", true)
        $("#input_followupPd_enddate").prop("disabled", true)
        $("#input_followupPd_endtime").prop("disabled", true)
        $("#select_followupPdpurpose").prop("disabled", true)


        /*Activity Pd Appointment Modal */
        //if (data.pdContact != null) {
        //    $("#input_modal_decisioncontactname").val(data.pdContact["ContactName"]);
        //    $("#input_modal_decisioncontactphone").val(data.pdContact["ContactPhone"]);
        //    $("#input_modal_decisioncontactemail").val(data.pdContact["ContactEmail"]);
        //}

        //$("#inputModal_pd_callback").val(formatJSONDate(data.pdSchedule["StartDate"]));
        //$("#selectModal_pd_schedulepurpose").val(data.pdSchedule["Purpose"]);

        ///*follow up contactinfo*/
        //$("#input_followupavailabletomeet").val(formatJSONDate(data.pdSchedule["StartDate"]));
        //$("#select_followupavailablemeetpurpose").val(data.pdSchedule["Purpose"]);
    }

    if (data.contact != null && data.bidding != null) {
        $("#sold_propamount").val(data.bidding["MonthlyPrice"]);
        $("#sold_contractamount").val(data.bidding["MonthlyPrice"]);
        $("#sold_initialamount").val(data.bidding["IncludePrice"]);
        //$('#sold_ServiceTypeListModel').val()


        if (data.fvpresentation != null) {
            $("#sold_ContractDetail_CleanFrequency").val(data.fvpresentation["CleanFrequency"]);
            $("#sold_cleantime").val(data.fvpresentation["CleanTime"]);

            $("#sold_mon").prop('checked', data.fvpresentation["Mon"]);
            $("#sold_tue").prop('checked', data.fvpresentation["Tue"]);
            $("#sold_wed").prop('checked', data.fvpresentation["Wed"]);
            $("#sold_thu").prop('checked', data.fvpresentation["Thu"]);
            $("#sold_fri").prop('checked', data.fvpresentation["Fri"]);
            $("#sold_sat").prop('checked', data.fvpresentation["Sat"]);
            $("#sold_sun").prop('checked', data.fvpresentation["Sun"]);
            $("#sold_weekend").prop('checked', data.fvpresentation["Weekend"]);

            $("#sold_ServiceTypeListModel").val(data.fvpresentation["ServiceTypeListId"]);


        }



    }


    if (data.document.length > 0) {

        //$(".clseditlnk").show();
        refreshCRMDocumentDisplay(data.document);
        $.each(data.document, function (index, value) {
            if (value.FileTypeListId == "1") {
                $("#checkboxcontactinfo_accountworkbook").prop("checked", true);
            }
            if (value.FileTypeListId == "2") {
                $("#checkboxcontactinfo_BidSheetbook").prop("checked", true);
            }
            if (value.FileTypeListId == "4") {
                $("#checkboxcontactinfo_Cleaningbook").prop("checked", true);
            }
        });
    }
    else {
        //$(".clseditlnk").hide();
        $("#CRMDocumentDisplay").html('');
    }



    $("#input_potentialbudget").val(data.result["BudgetAmount"]);
     $.unblockUI();
}

$(".clssamecontact").click(function () {
    if ($(this).is(":checked")) {
        $(".clsmeasurecontact").val($("#input_contactname").val());
        $(".clsmeasurecontact").attr("readonly", "readonly");
        $(".clssamecontact").attr("Checked", true);
    }
    else {
        $(".clsmeasurecontact").val("");
        $(".clsmeasurecontact").removeAttr("readonly", "readonly");
        $(".clssamecontact").attr("Checked", false);
    }
});

$(".clssameContmeetingper").click(function () {
    if ($(this).is(":checked")) {
        $(".clsmeetwith").val($("#input_contactname").val());
        $(".clsconphone").val($("#input_contactphonenumber").val());
        $(".clsconemail").val($("#input_contactemail").val());


        $(".clsmeetwith").attr("readonly", "readonly");
        $(".clsconphone").attr("readonly", "readonly");
        $(".clsconemail").attr("readonly", "readonly");

        $(".clssameContmeetingper").prop("checked", true);
    }
    else {
        $(".clsmeetwith").val("");
        $(".clsconphone").val("");
        $(".clsconemail").val("");

        $(".clsmeetwith").removeAttr("readonly", "readonly");
        $(".clsconphone").removeAttr("readonly", "readonly");
        $(".clsconemail").removeAttr("readonly", "readonly");

        $(".clssameContmeetingper").prop("checked", false);
    }
});

$(".clssamebiddingmeetingper").click(function () {
    if ($(this).is(":checked")) {
        $(".clsbiddingconper").val($("#input_contactname").val());
        $(".clsbiddingconphone").val($("#input_contactphonenumber").val());
        $(".clsbiddingconemail").val($("#input_contactemail").val());


        $(".clsbiddingconper").attr("readonly", "readonly");
        $(".clsbiddingconphone").attr("readonly", "readonly");

        if ($("#input_bidding_contactemail").val() != "")
            $(".clsbiddingconemail").attr("readonly", "readonly");

        $(".clssamebiddingmeetingper").prop("checked", true);
    }
    else {
        $(".clsbiddingconper").val("");
        $(".clsbiddingconphone").val("");
        $(".clsbiddingconemail").val("");

        $(".clsbiddingconper").removeAttr("readonly", "readonly");
        $(".clsbiddingconphone").removeAttr("readonly", "readonly");
        $(".clsbiddingconemail").removeAttr("readonly", "readonly");

        $(".clssamebiddingmeetingper").prop("checked", false);
    }
});

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


