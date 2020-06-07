window.onload = function () {
    document.getElementById("select-region-crop").onclick = isClick;
}

let cropper

function isClick() {
    document.clicked = !document.clicked;

    if (document.clicked) {
        isCrop();
        document.getElementById("select-region-crop").className = "btn btn-outline-primary";
        document.getElementById("crop-image").className = "btn btn-primary";
    } else {
        isDestroy();
        document.getElementById("select-region-crop").className = "btn btn-primary";
        document.getElementById("crop-image").className = "btn btn-primary disabled";
    }
}

function isCrop() {
    var image = document.getElementById("saved-image");
    cropper = new Cropper(image, {
        aspectRatio: NaN,
        dragMode: 'none',
        cropBoxMovable: 'false',
        cropBoxResizable: 'false',
        crop(event) {
            document.getElementById("imageY").value = event.detail.y;
            document.getElementById("imageX").value = event.detail.x;
            document.getElementById("imageW").value = event.detail.width;
            document.getElementById("imageH").value = event.detail.height;
        },
    });
}

function isDestroy() {
    cropper.destroy();
} 