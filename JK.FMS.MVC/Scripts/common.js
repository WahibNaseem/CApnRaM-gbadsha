
$(document).ajaxSuccess(function (xhr, props) {
    if (props.responseText === "501") {
        window.location.href = '/JKControl/User/Index?returnUrl=' + window.location.href;
    }
});

$(document).ajaxError(function (xhr, props) {    
    if (props.responseText === "501") {
        window.location.href = '/JKControl/User/Index?returnUrl=' + window.location.href;
    }

});


var idleTime = 0;
$(document).ready(function () {
    //Increment the idle time counter every minute.
    var idleInterval = setInterval(timerIncrement, 60000); // 1 minute

    //Zero the idle timer on mouse movement.
    $(this).mousemove(function (e) {
        idleTime = 0;
    });
    $(this).keypress(function (e) {
        idleTime = 0;
    });
});

function timerIncrement() {
    idleTime = idleTime + 1;
    if (idleTime > 719) { // 720 minutes
        window.location.reload();
    }
}