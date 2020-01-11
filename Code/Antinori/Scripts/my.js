function vai(url) {
    window.location.href = url;
}

function vaiTarget(url, target) {
    window.open(url, target)
}

function fileImage_into_IMG(input, target) {

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#' + target).attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}


function rootPath() {
    var path = "";
    if (location.pathname.split('/').length > 1) {
        for (var i = 0; i < location.pathname.split('/').length - 1; i++) {
            path += "../";
        }
    }
    return path;

}


function SUCCESS_function(titolo, testo, btn_ok_function, btn_ok_title, btn_cancel_function, btn_cancel_title) {
    MODAL_function(titolo, testo, btn_ok_function, btn_ok_title, btn_cancel_function, btn_cancel_title, "modal-success");
}

function DANGER_function(titolo, testo, btn_ok_function, btn_ok_title, btn_cancel_function, btn_cancel_title) {       
    MODAL_function(titolo, testo, btn_ok_function, btn_ok_title, btn_cancel_function, btn_cancel_title, "modal-danger");
}

function ALERT_function(titolo, testo, btn_ok_function, btn_ok_title, btn_cancel_function, btn_cancel_title) {  
    MODAL_function(titolo, testo, btn_ok_function, btn_ok_title, btn_cancel_function, btn_cancel_title, "modal-warning");
}

function MODAL_function(titolo, testo, btn_ok_function, btn_ok_title, btn_cancel_function,btn_cancel_title, tipo) {
    REFRESH_hide("MODAL_Refresh");
    $('#MODAL_ID').removeClass("modal-warning");
    $('#MODAL_ID').removeClass("modal-danger");
    if (tipo !== "") {
            $('#MODAL_ID').addClass(tipo);
    }    
    $("#MODAL_titolo").html(titolo);
    $("#MODAL_testo").html(testo);
    if (btn_cancel_title !== undefined && btn_cancel_title !== "" && btn_cancel_title !== null) {
        $("#MODAL_BTN_close").html(btn_cancel_title);
        $("#MODAL_BTN_close").show();
    }
    else {
        $("#MODAL_BTN_close").hide();
    }
   
    if (btn_ok_function !== undefined && btn_ok_function !== "" && btn_ok_function !== null) {
        $("#MODAL_BTN_ok").show();
        $("#MODAL_BTN_ok").html(btn_ok_title);
        $("#MODAL_BTN_ok").attr('onClick', btn_ok_function);
    }
    else {
        $("#MODAL_BTN_ok").hide();
    }
    if (btn_cancel_function !== undefined && btn_cancel_function !== "" && btn_cancel_function !== null) {
        $("#MODAL_BTN_close").attr('onClick', btn_cancel_function);
    }
    else {
        $("#MODAL_BTN_close").attr('onClick', '$("#MODAL_ID").hide();');
    }
    $("#MODAL_BTN_plus").html($("#BTN_Plus").html());
    $('#MODAL_ID').modal();
}

function REFRESH_show(id) {
    $("#" + id).show();   
}

function REFRESH_hide(id) {
    $("#" + id).hide();
}


var DataTablesLanguage =
{
    "sEmptyTable": "Nessun dato presente nella tabella",
    "sInfo": "Vista da _START_ a _END_ di _TOTAL_ elementi",
    "sInfoEmpty": "Vista da 0 a 0 di 0 elementi",
    "sInfoFiltered": "(filtrati da _MAX_ elementi totali)",
    "sInfoPostFix": "",
    "sInfoThousands": ",",
    "sLengthMenu": "Visualizza _MENU_ elementi",
    "sLoadingRecords": "Caricamento...",
    "sProcessing": "Elaborazione...",
    "sSearch": "Cerca:",
    "sZeroRecords": "La ricerca non ha portato alcun risultato.",
    "oPaginate": {
        "sFirst": "Inizio",
        "sPrevious": "Precedente",
        "sNext": "Successivo",
        "sLast": "Fine"
    },
    "oAria": {
        "sSortAscending": ": attiva per ordinare la colonna in ordine crescente",
        "sSortDescending": ": attiva per ordinare la colonna in ordine decrescente"
    }
}
