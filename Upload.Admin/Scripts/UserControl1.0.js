/*
* Author:       Lê Công Hoàng
* Date:         25.11.2017 00:00:00
* Discription:  File khởi tạo các control
*               Custom một số control mặc định
*/

var AjaxConfig = {
    timeout: 60 * 60000
}
var UC = [];
var ThemeConfig = {
    colorICheck: "purple"
    , uploadImageExt: [".gif", ".jpg", ".png"]
}
$(document).ready(function () {
    createCheckBox({ ctrlID: '.check-box' });
});

function createKendoUpload(allData) {
    var upload = $("#" + allData.ctrlID).kendoUpload({
        async: {
            saveUrl: "upload/save",
            removeUrl: "upload/remove",
            autoUpload: true
        },
        validation: {
            allowedExtensions: ThemeConfig.uploadImageExt
            , maxFileSize: 4194304
        },
        cancel: allData.onCancel,
        complete: allData.onComplete,
        error: allData.onError,
        progress: allData.onProgress,
        remove: allData.onRemove,
        select: allData.onSelect,
        success: allData.onSuccess,
        upload: allData.onUpload
    });
    UC[allData.ctrlID] = upload;
}

function createCKEditor(allData) {
    var ctrlID = allData.ctrlID;
    var config = allData.config || {};
    try {
        var ckEditor = CKEDITOR.replace(ctrlID, config);
        UC[ctrlID] = ckEditor;
    } catch (err) {
        //console.log(err);
    }
}
function createCheckBox(allData) {
    try {
        $(allData.ctrlID).iCheck({
            checkboxClass: 'icheckbox_square-' + ThemeConfig.colorICheck,
            radioClass: 'iradio_square-' + ThemeConfig.colorICheck,
            increaseArea: '20%' // optional
        });
    } catch (err) {
        // console.log(err);
    }
}
function createDataTable(allData) {
    var dom = "lfrtip";
    var dataTable = $(allData.ctrlID).DataTable({
        "oLanguage": {
            "oPaginate": {
                "sFirst": "<<",
                "sLast": ">>",
                "sNext": ">",
                "sPrevious": "<",
            },
            "sLengthMenu": "Hiển thị _MENU_ mục",
            "sZeroRecords": "Không tìm thấy dữ liệu",
            "sSearch": "Tìm kiếm:",
            "sLoadingRecords": "Đang tải...",
            "sProcessing": "Đang xử lý...",
            "sEmptyTable": "Không có dữ liệu để hiển thị",
            "sInfo": "Hiển thị _START_ đến _END_ của _TOTAL_ mục ",
            "sInfoEmpty": "Hiển thị 0 đến 0 của 0 dòng ",
            "sInfoFiltered": "(Tìm kiếm dữ liệu trong _MAX_ mục) "
        }
        , select: true
        , "data": allData.data || null
        , "columns": allData.columns || null
        , "columnDefs": allData.columnDefs || null
        , "sScrollX": allData.scrollX || false
        , "scrollCollapse": allData.scrollX || false
        , "sScrollY": "400px"
        , "scrollCollapse": true
        , "pageLength": allData.length || 10
        , "sDom": allData.dom || dom
        , fnRowCallback: allData.fnRowCallback
    });

    UC["currDataTable"] = dataTable;
}
function createDataCombobox(allData) {
    if ($(allData.ctrlID) == null)
        return;
    var defaultValue = allData.defaultValue || "0";
    getDataObject({
        type: allData.type || "GET",
        url: allData.url,
        onSuccess: function (data) {
            var dropTruongDay = $(allData.ctrlID);
            dropTruongDay.empty();
            if (allData.placeHolder != undefined)
                dropTruongDay.append('<option selected="selected" value="' + defaultValue + '">---' + allData.placeHolder + '---</option>');
            $.each(data, function () {
                var value = this[allData.value];
                var text = this[allData.text];
                dropTruongDay.append($("<option></option>").val(value).html(text));
            });
        },
        onError: function (x, e) {
            var d = getError({ x: x, e: e });
            d.title = d.status;
            d.icon = "error";
            alert(d);
        }
    })
}

function createDataUpload(allData) {
    var date = new Date();
    $("#" + allData.ctrID).fileUpload({
        'uploader': '../Content/upload/uploader.swf',
        'cancelImg': '../Content/upload/cancel.png',
        'buttonText': allData.buttonText || 'Browse Files',
        'script': allData.url || '/ParitalView/AddNewVideoPartial/',
        'folder': allData.folder || 'Uploaded/videos/' + date.getFullYear() + date.getMonth() + date.getDate(),
        'fileDesc': 'Video Files',
        'fileExt': '*.mp4;*.mov;*.wmv;*.avi',
        'multi': true,
        'auto': true
    });
}
function getDataObject(allData) {
    var t = AjaxConfig;
    var onSuccess = allData.onSuccess;
    var onError = allData.onError;
    var url2 = allData.url;
    var filter = allData.filter || {};
    var urlType = jQuery.type(url2);
    if (urlType == "object") {
        var jsonFilter = ConvertDataToJsonString({ data: filter });
        filter = deepCopy({ data: url2 });
        delete filter.url;
        filter.jsonFilter = jsonFilter;
        url2 = url2.url;
    } else
        allData.type = allData.type || "GET";
    var a = $.ajax({
        type: allData.type || "post"
        , headers: allData.headers
        , url: url2
        , data: JSON.stringify(filter)
        , contentType: allData.contentType || "application/json; charset=utf-8"
        , datatype: allData.datatype || "json"
        , traditional: true
        , timeout: t.timeout
	    , async: true
        , success: function (data) {
            a.abort();
            if (onSuccess != null) { onSuccess(data); }
        }
        , error: function (x, e) {
            a.abort();
            if (onError != null) { onError(x, e); }
            var d = getError({ x: x, e: e });
            d.icon = "error";
            d.title = d.status;
            alert(d);
        }
    });
}

function alert(allData) {
    var icon = allData.icon || "";//"error", "success", "info", "warning"
    var title = allData.title || "Thông báo";
    var message = allData.message || "";
    if (message == "") {
        icon = "success";
        message = allData;
    }
    var btnOk = allData.OkText || "Xác nhận";
    swal({
        title: title,
        text: message,
        icon: icon,
        button: btnOk
    });
}
function alertOK(allData) {
    var icon = allData.icon || "";//"error", "success", "info", "warning"
    var title = allData.title || "Thông báo";
    var message = allData.message || "";
    if (message == "") {
        icon = "success";
        message = allData;
    }
    var btnOk = allData.OkText || "Xác nhận";
    swal({
        title: title,
        text: message,
        icon: icon,
        button: btnOk
    }).then((value) => {
        if (value) {
            allData.fnExecute();
        } else {

        }
    });
}
function alertConfirm(allData) {
    var icon = allData.icon || "";//"error", "success", "info", "warning"
    var title = allData.title || "Thông báo";
    var message = allData.message || "";
    var btn = { ok: allData.OkText || "Xác nhận", cancel: allData.CancelText || "Hủy bỏ" };
    swal({
        title: title,
        text: message,
        icon: icon,
        buttons: btn,
        dangerMode: true,
    })
    .then((willDelete) => {
        if (willDelete) {
            allData.fnExecute();
        } else {

        }
    });
}
function getError(allData) {
    var x = allData.x;
    var e = allData.e;
    var statusText = x.statusText;
    var d = {
        title: e
        , status: x.status.toString()
        , message: statusText
    };
    return d;
}

function ConvertDataToJsonString(allData) {
    var data = allData.data;
    if (data == null || data.length == 0) { return ""; }
    var jsonString = JSON.stringify(data);
    jsonString = htmlEncode({ htmlText: jsonString });

    return jsonString;
}

function ConvertDataToXmlString(allData) {
    var data = allData.data;
    var IsEncode = true;
    if (allData.IsEncode != null) { IsEncode = allData.IsEncode; }

    if (data == null || data.length == 0) { return ""; }
    var xmlString = "";
    for (var i = 0; i < data.length; i++) {
        var item = data[i];
        xmlString += "<row";
        for (var key in item) {
            //var ot = "<" + key + ">";
            //var ct = "</" + key + ">";
            //xmlString += ot + item[key] + ct;
            xmlString += " " + key + "=\"" + item[key] + "\"";
        }
        xmlString += "/>";
    }
    if (IsEncode) {
        xmlString = htmlEncode({ htmlText: xmlString });
    }

    return xmlString;
}

function htmlEncode(allData) {
    var htmlText = allData.htmlText;
    var htmlReturn = $('<div/>').text(htmlText).html();
    return htmlReturn;
}

function htmlDecode(allData) {
    var htmlText = allData.htmlText;
    var htmlReturn = $('<div/>').html(htmlText).text();
    return htmlReturn;
}

function deepCopy(allData) {
    var data = allData.data;
    if (data == null) { return []; }
    var jsonString = JSON.stringify(data);
    var newData = JSON.parse(jsonString);

    return newData;
}

function parseDate(jsonDate) {
    var re = /-?\d+/;
    var m = re.exec(jsonDate);
    if (m == null)
        return "";
    var date = new Date(parseInt(m[0]));
    return date.getDate() + '-' + date.getMonth() + '-' + date.getFullYear();
}

function parseDateTime(jsonDate) {
    var re = /-?\d+/;
    var m = re.exec(jsonDate);
    if (m == null)
        return "";
    var date = new Date(parseInt(m[0]));
    return date.getDate() + '-' + date.getMonth() + '-' + date.getFullYear() + ' ' + date.getHours() + ':' + date.getMinutes() + ':' + date.getSeconds();
}
//Kiểm tra Email
function validateEmail2(emails) {
    var myEmails = emails.split(";");

    var isValid = false;
    var re = /^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$/;
    var n = myEmails.length;
    for (var i = 0; i < n; i++) {
        var myEmail = myEmails[i];
        if (!(i == n - 1 && myEmail.length == 0)) {
            isValid = re.test(myEmail);
            if (!isValid) {
                break;
            }
        }
    }
    return isValid;
}
//Nhập số không nhập chữ
function keypressNumberOnly(allData) {
    //Nhập số không nhập chữ
    $(allData.ctrlID).keypress(function (event) {
        var re = /^[+-]?\d+(\.\d+)?$/;
        var regex = new RegExp(re);
        var key = event.key;
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    });
}
function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, 'g'), replace);
}
function enableButton(allData) {
    var ctrID = "#" + allData.ctrlID;
    var enable = allData.disabled;
    if (enable)
        $(ctrID).attr("disabled", "disabled");
    else
        $(ctrID).removeAttr("disabled");
    // $(ctrID).prop('disabled', allData.disabled);
}

function stringFormat(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function EncodeVietNamese(str) {
    if (str == "" || str == undefined)
        return str;
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, 'a');
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, 'e');
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, 'i');
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, 'o');
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, 'u');
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, 'y');
    str = str.replace(/đ/g, 'd');
    // str = str.replace(/\W+/g, ' ');
    // str = str.replace(/\s/g, '-');
    return str;
}