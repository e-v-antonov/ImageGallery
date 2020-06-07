$(function () {
    $("#field__file-upload").change(function () {
        if (typeof (FileReader) != "undefined") {
            var dvPreview = $("#img-preview");
            dvPreview.html("");
            var regex = /^([а-яА-Яa-zA-Z0-9\s_\\.\-:])+(.jpg|.jpeg|.gif|.png|.bmp|.ico)$/;
            $($(this)[0].files).each(function () {
                var file = $(this);
                if (regex.test(file[0].name.toLowerCase())) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        var div = document.createElement("div");
                        div.className = "col-md-3";
                        var img = document.createElement("img");
                        img.src = e.target.result;
                        img.style = "width: 100%";
                        img.className = "img-fluid";
                        img.style.marginBottom = "15px";
                        div.append(img);
                        dvPreview.append(div);
                    }
                    reader.readAsDataURL(file[0]);
                } else {
                    alert(file[0].name + " данный файл не является изображением.");
                    dvPreview.html("");
                    return false;
                }
            });
        } else {
            alert("Данный браузер не поддерживает функцию отобажения загружаемых изображений.");
        }
    });
});