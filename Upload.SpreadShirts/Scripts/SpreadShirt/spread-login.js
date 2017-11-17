
function loginSpread() {

    getDataObjectAjax({
        url: "https://partner.spreadshirt.com/login"
       , method: "GET"
    })


    var url = 'https://partner.spreadshirt.com/api/v1/sessions?';
    var newUrl = encodeUrlHost({
        url: url
        , method: "POST"
    });

    setTimeout(
    getDataObjectAjax({
        url: newUrl
        , method: "POST"
        // , dataType: "text"
        , filter: '{"rememberMe":true,"username":"lchoang1995@gmail.com","password":"Omega@111"}'
        //, contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , headers: {
            "Host": "partner.spreadshirt.com"
            , "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0"
            , "Accept": "application/json, text/plain, */*"
            , "Accept-Language": "vi-VN,vi;q=0.8,en-US;q=0.5,en;q=0.3"
            , "Accept-Encoding": "gzip, deflate, br"
            , "Content-Type": "application/json;charset=utf-8"
            , "Referer": "https://partner.spreadshirt.com/login"
            , "Content-Length": "77"
        }
    }), 3000)
}

function getDataObjectAjax(allData) {
    $.ajax({
        url: allData.url,
        headers: allData.headers || null,
        type: allData.method || 'POST',
        dataType: allData.dataType || "text",
        data: allData.filter || null,
        contentType: allData.contentType || null,
        success: function (data, status, XmlHttpRequest) {
            var rpData = data; //content loads here
            var cookies = [];
            if (document.cookie)
                cookies = document.cookie.split('; ');
        },
        error: function (xhr, desc, err) {
            console.log("error");
            var cookies = [];
            if (document.cookie)
                cookies = document.cookie.split('; ');
        }
    });
}