/*Config*/
var BC = {
    dfUrl: "/PartialView/LoadData"
}
var AjaxConfig = {
    timeout: 60 * 60000
}
var UC = {};
$(document).ready(function () {

    $('input[type=checkbox],input[type=radio],input[type=file]').uniform();

    $('select').select2();

    $("span.icon input:checkbox, th input:checkbox").click(function () {
        var checkedStatus = this.checked;
        var checkbox = $(this).parents('.widget-box').find('tr td:first-child input:checkbox');
        checkbox.each(function () {
            this.checked = checkedStatus;
            if (checkedStatus == this.checked) {
                $(this).closest('.checker > span').removeClass('checked');
            }
            if (this.checked) {
                $(this).closest('.checker > span').addClass('checked');
            }
        });
    });
});

function oLanguage() {
    return {
        "oPaginate": {
            "sFirst": "<<",
            "sLast": ">>",
            "sNext": ">",
            "sPrevious": "<",
        },
        "sEmptyTable": "Không có dữ liệu",
        "sLengthMenu": "Hiển thị _MENU_ ",
        "sZeroRecords": "Không tìm thấy dữ liệu",
        "sSearch": "Tìm kiếm:",
        "sLoadingRecords": "Đang tải...",
        "sProcessing": "Đang xử lý...",
        "sInfoEmpty": "Không có dữ liệu để hiển thị"
    };
}

function twoAction(data) {
    var html = '';
    html += '<button onclick="R.events.editAction({data: \'' + data + '\'})" class="btn btn-warning"><span class="icon"><i class="icon-edit"></i></span></button>';
    html += '<button onclick="R.events.trashAction({data: \'' + data + '\'})" class="btn btn-danger"><span class="icon"><i class="icon-trash"></i></span></button>';
    html += '';
    return html;
}
function threeAction(data) {
    var html = '';
    html += '<button onclick="R.events.detailsAction({data: \'' + data + '\'})" class="btn btn-success"><span class="icon"><i class="icon-list-alt"></i></span></button>';
    html += '<button onclick="R.events.editAction({data: \'' + data + '\'})" class="btn btn-warning"><span class="icon"><i class="icon-edit"></i></span></button>';
    html += '<button onclick="R.events.trashAction({data: \'' + data + '\'})" class="btn btn-danger"><span class="icon"><i class="icon-trash"></i></span></button>';
    html += '';
    return html;
}

function createGridDataTable(allData) {
    var ctrID = allData.ctrID;
    var ctrID2 = "#" + allData.ctrID;
    var bJQueryUI = allData.jQueryUI || true;
    var sPaginationType = allData.page || "full_numbers";
    var sDom = allData.dom || '<""l>t<"F"fp>'
    var fnRowCallback = allData.fnRowCallback || null;
    var fnFooterCallback = allData.fnFooterCallback || null;
    //use store
    var url2 = allData.url;
    var spName = url2.spName || "";
    var jsonFilter = url2.jsonFilter || "";
    //filter
    var url = url2.url || "";
    var filter = allData.filter || { SPName: spName, JsonFilter: JSON.stringify(jsonFilter) };
    //column
    var colDefs = allData.colDefs || [];
    var columns = allData.columns;
    //Refix Data 
    var refix = allData.refix;
    var rkey = refix.key || 0;
    var rid = refix.id || "";

    var dataTable = $(ctrID2).dataTable({
        "bJQueryUI": bJQueryUI,
        "sPaginationType": sPaginationType,
        "sDom": sDom,
        "oLanguage": oLanguage(),
        "aoColumnDefs": colDefs,
        "aoColumns": columns,
        "aaData": [],
        "bFilter": true,
        "sScrollX": "100%",
        "sScrollXInner": "110%",
        "bScrollCollapse": true,
        "fnRowCallback": fnRowCallback,
        "fnFooterCallback": fnFooterCallback,
        "sScrollY": "100%",
    });

    getDataObject({
        url: url,
        filter: filter,
        onSuccess: function (data) {
            var da = [];
            da = refixData({ data: data, key: rkey, id: rid });
            dataTable.fnAddData(da)
        },
        onError: function (x, e) {
            var d = getError({ x: x, e: e });
            d.title = d.status;
            d.icon = "error";
            alert(d);
        }
    });
    UC.ctrID = dataTable;
}

function refixData(allData) {
    var d = [];
    var data = allData.data;
    var key = allData.key || 0;
    var id = allData.id;
    if (data.length == 0)
        return d;
    switch (key) {
        case 1:
            for (var i = 0; i < data.length; i++) {
                data[i].Stt = i + 1;
                data[i].Actions = twoAction(data[i][id]);
                d[i] = data[i];
            }
            break;
        case 2:
            for (var i = 0; i < data.length; i++) {
                data[i].Stt = i + 1;
                data[i].Actions = threeAction(data[i][id]);
                d[i] = data[i];
            }
            break;
        default:
            for (var i = 0; i < data.length; i++) {
                data[i].Stt = i + 1;
                d[i] = data[i];
            }
            break;
    }
    return d;
}

function getDataObject(allData) {
    var t = AjaxConfig;
    var onSuccess = allData.onSuccess;
    var onError = allData.onError;
    var url2 = allData.url;
    var filter = allData.filter;
    var urlType = jQuery.type(url2);
    if (urlType == "object") {
        var jsonFilter = ConvertDataToJsonString({ data: filter });
        filter = deepCopy({ data: url2 });
        delete filter.url;
        filter.jsonFilter = jsonFilter;
        url2 = url2.url;
    }
    var a = $.ajax({
        type: allData.type || "post"
        , headers: allData.headers
        , url: url2
        , data: filter || {}
        , ContentType: "application/json;charset=utf-8"
        , datatype: allData.datatype || "json"
        , traditional: true
        , timeout: t.timeout
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
    var btnOk = allData.OkText || "Xác nhận";
    swal(title, message, icon, { buttons: btnOk });
}
function alertConfirm(allData) {
    var icon = allData.icon || "";//"error", "success", "info", "warning"
    var title = allData.title || "Thông báo";
    var message = allData.message || "";
    var btn = { ok: allData.OkText || "Xác nhận", cancel: allData.CancelText || "Hủy bỏ" };
    swal(title.toString(), message.toString(), icon, { buttons: btn })
        .then((value) => {
        switch (value) {
            case "ok":
                allData.fnExecute;
                break;
            case "cancel":
                break;
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