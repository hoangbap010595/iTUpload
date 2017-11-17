var SPRD = {
    VERSION: '2.21.7',
    API_PATH: '/api/v1',
    API_KEY: '1c711bf5-b82d-40de-bea6-435b5473cf9b',
    API_SECRET: 'fd9f23cc-2432-4a69-9dad-bbd57b7b9fdd',
    API_RELATIVE_SELF_LINKS: false,
    FORCE_HTTPS_LINKS: true,
    HTML5_ROUTES: true,
    LOGGER_SERVER_URL: '',
    LOGGER_CLIENT_LOGGING: false,
    IMAGE_SERVER_PATH: '/image-server',
    RAYGUN_KEY: '942bVUYrpgpw1jUqiLEx3A==',
    RAYGUN_ACTIVE: true,
    LOCALE_URL: './locales/',
    LOCALE_DEFAULT: 'us_US',
    ENVIRONMENT: 'ops',
    PLATFORM: 'na',
    TABLOMAT_SHOPID: '1048679',
    LIVECHAT_ACTIVE: true
}

function SHA1(msg) {
    function rotate_left(n, s) {
        var t4 = ( n << s ) | (n >>> (32 - s));
        return t4;
    }

    function lsb_hex(val) {
        var str = "";
        var i;
        var vh;
        var vl;

        for (i = 0; i <= 6; i += 2) {
            vh = (val >>> (i * 4 + 4)) & 0x0f;
            vl = (val >>> (i * 4)) & 0x0f;
            str += vh.toString(16) + vl.toString(16);
        }
        return str;
    }

    function cvt_hex(val) {
        var str = "";
        var i;
        var v;

        for (i = 7; i >= 0; i--) {
            v = (val >>> (i * 4)) & 0x0f;
            str += v.toString(16);
        }
        return str;
    }

    function Utf8Encode(string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";

        for (var n = 0; n < string.length; n++) {

            var c = string.charCodeAt(n);

            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }

        }

        return utftext;
    }

    var blockstart;
    var i, j;
    var W = new Array(80);
    var H0 = 0x67452301;
    var H1 = 0xEFCDAB89;
    var H2 = 0x98BADCFE;
    var H3 = 0x10325476;
    var H4 = 0xC3D2E1F0;
    var A, B, C, D, E;
    var temp;

    msg = Utf8Encode(msg);

    var msg_len = msg.length;

    var word_array = new Array();
    for (i = 0; i < msg_len - 3; i += 4) {
        j = msg.charCodeAt(i) << 24 | msg.charCodeAt(i + 1) << 16 |
            msg.charCodeAt(i + 2) << 8 | msg.charCodeAt(i + 3);
        word_array.push(j);
    }

    switch (msg_len % 4) {
        case 0:
            i = 0x080000000;
            break;
        case 1:
            i = msg.charCodeAt(msg_len - 1) << 24 | 0x0800000;
            break;

        case 2:
            i = msg.charCodeAt(msg_len - 2) << 24 | msg.charCodeAt(msg_len - 1) << 16 | 0x08000;
            break;

        case 3:
            i = msg.charCodeAt(msg_len - 3) << 24 | msg.charCodeAt(msg_len - 2) << 16 | msg.charCodeAt(msg_len - 1) << 8 | 0x80;
            break;
    }

    word_array.push(i);

    while ((word_array.length % 16) != 14) word_array.push(0);

    word_array.push(msg_len >>> 29);
    word_array.push((msg_len << 3) & 0x0ffffffff);


    for (blockstart = 0; blockstart < word_array.length; blockstart += 16) {

        for (i = 0; i < 16; i++) W[i] = word_array[blockstart + i];
        for (i = 16; i <= 79; i++) W[i] = rotate_left(W[i - 3] ^ W[i - 8] ^ W[i - 14] ^ W[i - 16], 1);

        A = H0;
        B = H1;
        C = H2;
        D = H3;
        E = H4;

        for (i = 0; i <= 19; i++) {
            temp = (rotate_left(A, 5) + ((B & C) | (~B & D)) + E + W[i] + 0x5A827999) & 0x0ffffffff;
            E = D;
            D = C;
            C = rotate_left(B, 30);
            B = A;
            A = temp;
        }

        for (i = 20; i <= 39; i++) {
            temp = (rotate_left(A, 5) + (B ^ C ^ D) + E + W[i] + 0x6ED9EBA1) & 0x0ffffffff;
            E = D;
            D = C;
            C = rotate_left(B, 30);
            B = A;
            A = temp;
        }

        for (i = 40; i <= 59; i++) {
            temp = (rotate_left(A, 5) + ((B & C) | (B & D) | (C & D)) + E + W[i] + 0x8F1BBCDC) & 0x0ffffffff;
            E = D;
            D = C;
            C = rotate_left(B, 30);
            B = A;
            A = temp;
        }

        for (i = 60; i <= 79; i++) {
            temp = (rotate_left(A, 5) + (B ^ C ^ D) + E + W[i] + 0xCA62C1D6) & 0x0ffffffff;
            E = D;
            D = C;
            C = rotate_left(B, 30);
            B = A;
            A = temp;
        }

        H0 = (H0 + A) & 0x0ffffffff;
        H1 = (H1 + B) & 0x0ffffffff;
        H2 = (H2 + C) & 0x0ffffffff;
        H3 = (H3 + D) & 0x0ffffffff;
        H4 = (H4 + E) & 0x0ffffffff;

    }

    var temp = cvt_hex(H0) + cvt_hex(H1) + cvt_hex(H2) + cvt_hex(H3) + cvt_hex(H4);
    return temp.toLowerCase();
}

function updateTime() {
    var url = document.getElementById('url').value;
    if (!isUrlValid(url)) {
        alert("Please provide a valid api key!");
        return;
    }

    var xmlHttp = createXMLHTTPRequest();
    if (xmlHttp == null) {
        document.getElementById('time').value = new Date().getTime();
        encodeUrl();
    } else {
        var timeUrl = null;
        var index = url.indexOf("/", url.indexOf("http://") + 7);
        if (index != -1)
            index = url.indexOf("/v", index + 1);
        if (index != -1)
            index = url.indexOf("/", index + 2);
        if (index != -1)
            timeUrl = url.substring(0, index + 1);

        if (timeUrl == null) {
            document.getElementById('time').value = new Date().getTime();
            encodeUrl();
        } else {
            timeUrl += "serverTime";

            xmlHttp.onreadystatechange = function()
            {
                if (xmlHttp.readyState == 4) {
                    if (xmlHttp.status == 200) {
                        document.getElementById('time').value = xmlHttp.responseText;
                    }
                    else {
                        document.getElementById('time').value = new Date().getTime();
                    }
                    encodeUrl();
                }
            }
            xmlHttp.onerror = function() {
                document.getElementById('time').value = new Date().getTime();
                encodeUrl();
            }
            try {
                xmlHttp.open("GET", timeUrl, true);
                xmlHttp.send(null);
            } catch (e) {
                document.getElementById('time').value = new Date().getTime();
                encodeUrl();
            }
        }
    }
}

function createXMLHTTPRequest() {
    try {
        return new XMLHttpRequest();
    }
    catch (e) {
        try {
            return new ActiveXObject("Msxml2.XMLHTTP");
        }
        catch (e) {
            try {
                return new ActiveXObject("Microsoft.XMLHTTP");
            }
            catch (e) {
                return null;
            }
        }
    }
}

function encodeUrl() {
    //var apiKey = document.getElementById('apiKey').value;
    //var secret = document.getElementById('secret').value;
    //var sessionId = document.getElementById('sessionId').value;
    //var method = document.getElementById('method').value;
    //var url = document.getElementById('url').value;
    //var time = document.getElementById('time').value;
    var apiKey = SPRD.API_KEY;
    var secret = SPRD.API_SECRET;
    var sessionId = "123";
    var method = "PUST"
    var url = "https://partner.spreadshirt.com/api/v1/users/302719724/ideas/5a0a5c17aa0c6d5484aef460";
    var time = new Date().getTime();

    if (!isAPIKeyValid(apiKey)) {
        alert("Please provide a valid api key!");
        return;
    }
    if (!isSecretValid(secret)) {
        alert("Please provide a valid secret!");
        return;
    }
    if (!isUrlValid(url)) {
        alert("Please provide a valid url!");
        return;
    }

    var index = url.indexOf('?');
    var urlNoQuery = (index == -1) ? url : url.substring(0, index);
    var data = method + " " + urlNoQuery + " " + time + " " + secret;

    var dataForEncoding = data;
    //document.getElementById('dataForEncoding').value = data;

    var sig = SHA1(data);
    var encodedUrl = url + ((index == -1) ? "?" : "&") +
                     "apiKey=" + apiKey + "&locale=us_US&mediaType=json&sig=" + sig + "&time=" + time;
    if (sessionId != null && sessionId != "")
        encodedUrl += "&sessionId=" + sessionId;

    var tm_Encoded = encodedUrl;
    var tm_EncodedUrl = encodedUrl;
    var tm_Url = url;
}

function encodeUrlHost(allData) {
    //var apiKey = document.getElementById('apiKey').value;
    //var secret = document.getElementById('secret').value;
    //var sessionId = document.getElementById('sessionId').value;
    //var method = document.getElementById('method').value;
    //var url = document.getElementById('url').value;
    //var time = document.getElementById('time').value;
    var apiKey = SPRD.API_KEY;
    var secret = SPRD.API_SECRET;
    var sessionId = allData.sessionId || "";
    var method = allData.method || "POST"
    var url = allData.url || "";
    var time = new Date().getTime();

    if (!isAPIKeyValid(apiKey)) {
        alert("Please provide a valid api key!");
        return;
    }
    if (!isSecretValid(secret)) {
        alert("Please provide a valid secret!");
        return;
    }
    if (!isUrlValid(url)) {
        alert("Please provide a valid url!");
        return;
    }

    var index = url.indexOf('?');
    var urlNoQuery = (index == -1) ? url : url.substring(0, index);
    var data = method + " " + urlNoQuery + " " + time + " " + secret;

    var sig = SHA1(data);
    var encodedUrl = url + ((index == -1) ? "?" : "&") +
                     "apiKey=" + apiKey + "&locale=us_US&mediaType=json&sig=" + sig + "&time=" + time;
    if (sessionId != null && sessionId != "")
        encodedUrl += "&sessionId=" + sessionId;

    return encodedUrl;
}

function isAPIKeyValid(apiKey) {
    return apiKey != null && apiKey != "";
}

function isSecretValid(secret) {
    return secret != null && secret != "";
}

function isUrlValid(url) {
    return url != null && url != "" && (url.indexOf("http://") == 0 || url.indexOf("https://") == 0);
}