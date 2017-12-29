$(function () {
    Teechip.Controls.CreateControls();
})

var Teechip = {
    Controls: {
        CreateControls: function () {
            sessionStorage.initialFiles = "[]";
            var sessionStorageData = [];
            var initialFiles = JSON.parse(sessionStorage.initialFiles);
            keypressNumberOnly({ ctrlID: '#txtPrice' });

            $("#btnLogin").on('click', function (e) { Teechip.Events.LoginServerTeechip(e) });
            $("#btnUpload").on('click', function (e) { Teechip.Events.UploadServerTeechip(e) });
            $("#btnUploadUsingFile").on('click', function (e) { Teechip.Events.UploadServerTeechipUseFile(e) });
            createKendoUpload({
                ctrlID: "fInputFile"
                , onSuccess: Teechip.Events.ChangeLoadFile
            })
            createKendoUpload({
                ctrlID: "fInputFileData"
                , allowedExt: ThemeConfig.uploadExcelExt
                , async: {
                    saveUrl: "upload/saveexcel",
                    removeUrl: "upload/removeexcel",
                    autoUpload: true
                }
                , onSuccess: Teechip.Events.OnSuccessLoadFileData
            })
        },
        LoadAllProduct: function (allData) {
            var product = allData.product;
            var image = product.Colors[0].Image;
            var code = product.Code;
            var html = '';
            html += '<div class="col-md-12">';
            html += '   <div class="displayShop">';
            html += '       <div class="row">';
            html += '           <div class="col-md-3">';
            html += '               <img width="115" height="140" src="' + image + '" alt="Image" />';
            html += '           </div>';
            html += '           <div class="col-md-9">';
            html += '               <div class="row">';
            html += '                   <div class="col-md-9">';
            html += '                       <h4 style="font-weight:700; color: peru;">' + product.Name + '</h4>';
            html += '                   </div>';
            html += '               <div class="col-md-3">';
            html += '                   <input type="text" id="' + product.Id + '" value="' + product.Msrp + '" class="form-control k-textbox" />';
            html += '               </div>';
            html += '           </div>';
            html += '               <hr />';
            html += '               <div class="row">';
            html += '                   <ul id="show-color" class="show-color">';
            html += '                       @Color';
            html += '                   </ul>';
            html += '               </div>';
            html += '           </div>';
            html += '       </div>';
            html += '   </div>';
            html += '</div>';

            var color = '';
            color += '<li style="border-radius:50%; width:30px;height:30px;">';
            color += '  <div class="round">';
            color += '      <input name="@Code" type="checkbox" id="@Code-ck-@color" value="@color" />';
            color += '      <label for="@Code-ck-@color" style="background-color: #@color;"></label>';
            color += '  </div>';
            color += '</li>';

            var htmlColor = ''
            for (var i = 0; i < product.Colors.length; i++) {
                var x = color;
                htmlColor += replaceAll(x, "@color", product.Colors[i].Hex);
            }
            html = html.replace("@Color", htmlColor);
            html = replaceAll(html, "@Code", code);
            $("#show-product").append(html);
        },
        Filter: function () {
            var Image = "C:\\Users\\HoangLe\\Desktop\\Tool up Teechipshirt\\Upload\\98.png";

            var Title = EncodeVietNamese($("#txtTitle").val());
            var Description = EncodeVietNamese($("#txtDescription").val());
            var Category = EncodeVietNamese($("#txtCategory").val());
            var Url = EncodeVietNamese($("#txtUrl").val());

            var d = {
                Image: Image
                , Title: Title
                , Description: Description
                , Category: Category
                , Url: Url
            }
            return d;
        }
    },
    Events: {
        LoginServerTeechip: function (e) {
            var username = $("#txtUsername").val();// "hoangbap1595@gmail.com";
            var password = $("#txtPassword").val();//"Thienan@111";
            enableButton({ ctrlID: "btnLogin", disabled: true });
            getDataObject({
                url: "Teechip/ExecuteLogin"
               , type: "POST"
               , filter: { username: username, password: password }
               , onSuccess: function (data) {
                   if (data.result == 1) {
                       alert(data);
                       Teechip.Events.WriteLog({ data: "Login success!", work: 1 })
                       $("#modal-default").modal('toggle');
                       var htmUpload = '<button class="k-button k-primary" style="width:150px;" id="btnUpload">Upload</button>';
                       var htmUploadFile = '<button class="k-button k-primary" style="width:150px;" id="btnUploadUsingFile">Upload</button>';
                       $(".btnShowLogin").remove();
                       $("#action-button").append(htmUpload);
                       $("#action-button-file").append(htmUploadFile);
                       $("#btnUpload").on('click', function (e) { Teechip.Events.UploadServerTeechip(e) });
                       $("#btnUploadUsingFile").on('click', function (e) { Teechip.Events.UploadServerTeechipUseFile(e) });
                       $("#show-product").empty();
                       var email = data.email;
                       $(".lblID").html(email);
                       var products = data.product;
                       for (var i = 0; i < products.length; i++) {
                           Teechip.Controls.LoadAllProduct({ product: products[i] });
                       }
                   } else {
                       alert(data);
                       enableButton({ ctrlID: "btnLogin", disabled: false });
                   }
               }
                  , onError: function (error) {
                      alert(error);
                  }
            })
        },
        UploadServerTeechip: function (e) {
            var filter = Teechip.Controls.Filter();
            var dataImage = JSON.parse(sessionStorage.initialFiles);
            if (filter.Title == "") {
                alert({ title: "Message", icon: "warning", message: "Please enter field name!" });
                return
            }
            if (filter.Description == "") {
                alert({ title: "Message", icon: "warning", message: "Please enter field description!" });
                return
            }
            if (filter.Tag == "") {
                alert({ title: "Message", icon: "warning", message: "Please enter field tag!" });
                return
            } else if (filter.Tag.split(',').length < 3) {
                alert({ title: "Message", icon: "warning", message: "Please enter field tag more than 3 keyword (Ex: tag1, tag2, tag3)" });
                return
            }
            if (filter.Price > 20 || filter.Price < 0) {
                alert({ title: "Message", icon: "warning", message: "Please enter price between $00.00 and $20.00" });
                return
            }
            if (dataImage.length < 1) {
                alert({ title: "Message", icon: "warning", message: "Please choose image design !" });
                return
            }
            $("#imgUploading").removeAttr("hidden");
            $("html, body").animate({ scrollTop: $(document).height() }, 1000);
            enableButton({ ctrlID: "btnUpload", disabled: true });
            Teechip.Events.ProgressUpload({ sessionIndexUpload: 0 });
        },
        ProgressUpload: function (allData) {
            var dataImage = JSON.parse(sessionStorage.initialFiles);
            var index = allData.sessionIndexUpload;
            var filter = Teechip.Controls.Filter();
            try {
                if (index >= dataImage.length) {
                    $("#imgUploading").attr("hidden", "hidden");
                    sessionStorage.initialFiles = "[]";
                    enableButton({ ctrlID: "btnUpload", disabled: false });
                    return;
                }
                var imageName = dataImage[index].name;
                Teechip.Events.WriteLog({ data: "Uploading: " + imageName, work: 3 })
                //Config filter
                filter.Image = imageName;
                filter.Title = filter.Title.replace("$name", imageName.split('.')[0])
                filter.Description = filter.Description.replace("$name", imageName.split('.')[0]);
                getDataObject({
                    url: "TeechipShirt/UploadProgress"
                   , type: "POST"
                   , filter: filter
                   , onSuccess: function (data) {
                       Teechip.Events.WriteLog({ data: data.data, work: 1 })
                       index++; //Upload next Image
                       Teechip.Events.ProgressUpload({ sessionIndexUpload: index });
                   }
                      , onError: function (error) {
                          //console.log(error);
                          Teechip.Events.WriteLog({ data: error, work: 2 })
                          index++; //Upload next Image
                          Teechip.Events.ProgressUpload({ sessionIndexUpload: index });
                      }
                })
            } catch (error) {
                Teechip.Events.WriteLog({ data: error, work: 2 });
                index++; //Upload next Image
                Teechip.Events.ProgressUpload({ sessionIndexUpload: index });
            }
        },
        UploadServerTeechipUseFile: function (e) {
            var dataUpload = sessionStorageData;
            if (dataUpload.length < 1) {
                alert({ title: "Message", icon: "warning", message: "Please choose file data import!" });
                return
            }
            $("#imgUploading").removeAttr("hidden");
            enableButton({ ctrlID: "btnUploadUsingFile", disabled: true });
            $("html, body").animate({ scrollTop: $(document).height() }, 1000);
            Teechip.Events.ProgressUploadUseFile({ sessionIndexUpload: 0 });
        },
        ProgressUploadUseFile: function (allData) {
            var dataUpload = sessionStorageData;
            var index = allData.sessionIndexUpload;
            var filter = Teechip.Controls.Filter();
            try {
                if (index >= dataUpload.length) {
                    $("#imgUploading").attr("hidden", "hidden");
                    enableButton({ ctrlID: "btnUploadUsingFile", disabled: false });
                    return;
                }
                var imageName = dataUpload[index].Image;
                var title = dataUpload[index].Title;
                var description = dataUpload[index].Description;
                var tag = dataUpload[index].Tag;
                var shop = dataUpload[index].Shop;
                var price = dataUpload[index].Price;
                Teechip.Events.WriteLog({ data: "Uploading: " + imageName, work: 3 })
                //Config filter
                filter.Image = imageName;
                filter.Title = title;
                filter.Description = description;
                filter.Tag = tag;
                filter.Shop = shop;
                filter.Price = price;
                getDataObject({
                    url: "TeechipShirt/UploadProgress"
                   , type: "POST"
                   , filter: filter
                   , onSuccess: function (data) {
                       Teechip.Events.WriteLog({ data: data.data, work: 1 })
                       index++; //Upload next Image
                       Teechip.Events.ProgressUploadUseFile({ sessionIndexUpload: index });
                   }
                      , onError: function (error) {
                          //console.log(error);
                          Teechip.Events.WriteLog({ data: error, work: 2 })
                          index++; //Upload next Image
                          Teechip.Events.ProgressUploadUseFile({ sessionIndexUpload: index });
                      }
                })
            } catch (error) {
                Teechip.Events.WriteLog({ data: error, work: 2 });
                index++; //Upload next Image
                Teechip.Events.ProgressUploadUseFile({ sessionIndexUpload: index });
            }
        },
        ChangeLoadFile: function (e) {
            var currentInitialFiles = JSON.parse(sessionStorage.initialFiles);
            for (var i = 0; i < e.files.length; i++) {
                var current = {
                    name: e.files[i].name,
                    extension: e.files[i].extension,
                    size: e.files[i].size
                }

                if (e.operation == "upload") {
                    currentInitialFiles.push(current);
                } else {
                    var indexOfFile = currentInitialFiles.indexOf(current);
                    currentInitialFiles.splice(indexOfFile, 1);
                }
            }
            sessionStorage.initialFiles = JSON.stringify(currentInitialFiles);
        },
        ShopServerTeechip: function () {
            var items = [];
            var values = "";
            $("input.select-item-shop:checked:checked").each(function (index, item) {
                items[index] = item.value;
            });
            if (items.length < 1) {
                values = "";
            } else {
                values = items.join(',');
                console.log(values);
            }
            return values;
        },
        WriteLog: function (allData) {
            var log = $("#show-progress");
            var date = new Date();
            var d_Time = '[' + (parseInt(date.getHours()) < 0 ? '0' : '') + date.getHours() + ':' + (parseInt(date.getMinutes()) < 0 ? '0' : '') + date.getMinutes() + ':' + (parseInt(date.getSeconds()) < 0 ? '0' : '') + date.getSeconds() + ']';
            var d_Work = parseInt(allData.work);

            var data = '<li class="@cWork">' + d_Time + ' ' + allData.data + '.............................@work</li>';
            switch (d_Work) {
                case 1:
                    data = data.replace("@work", "Done").replace("@cWork", "in-done");
                    break;
                case 2:
                    data = data.replace("@work", "Faild").replace("@cWork", "in-error");
                    break;
                case 3:
                    data = data.replace("@work", "InProgress").replace("@cWork", "in-progress");
                    break;
            }

            log.prepend(data);
        },
        OnSuccessLoadFileData: function (e) {
            try {
                var fileName = Teechip.Events.GetFileInfo(e);
                var upload = $("#fInputFileData").data("kendoUpload");
                var files = upload.getFiles();

                /* set up XMLHttpRequest */
                var parentFolder = "/Uploaded/FileUpload/";
                var url = parentFolder + fileName;
                var oReq = new XMLHttpRequest();
                oReq.open("GET", url, true);
                oReq.responseType = "arraybuffer";

                oReq.onload = function (e) {
                    var arraybuffer = oReq.response;

                    /* convert data to binary string */
                    var data = new Uint8Array(arraybuffer);
                    var arr = new Array();
                    for (var i = 0; i != data.length; ++i)
                        arr[i] = String.fromCharCode(data[i]);
                    var bstr = arr.join("");

                    /* Call XLSX */
                    var workbook = XLSX.read(bstr, { type: "binary" });

                    /* DO SOMETHING WITH workbook HERE */
                    var first_sheet_name = workbook.SheetNames[0];
                    /* Get worksheet */
                    var worksheet = workbook.Sheets[first_sheet_name];

                    var data = XLSX.utils.sheet_to_json(worksheet);
                    console.log(data);
                    sessionStorageData = data;
                    Teechip.Events.WriteLog({ data: "Open file success! " + data.length + " record(s)", work: 1 })
                }

                oReq.send();
            } catch (error) {
                sessionStorageData = [];
            }
        },
        GetFileInfo: function (e) {
            return $.map(e.files, function (file) {
                var info = file.name;
                return info;
            }).join(", ");
        },
        GetProductSelected: function () {
            var values = new Array();
            $.each($("input[name='TC0']:checked"), function () {
                values.push($(this).val());
                // or you can do something to the actual checked checkboxes by working directly with  'this'
                // something like $(this).hide() (only something useful, probably) :P
            });
            console.log(values)
        }
    }
}