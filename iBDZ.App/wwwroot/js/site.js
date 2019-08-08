// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function postDataAsJSON(href, data, onloadstart) {
    let xhr = new XMLHttpRequest();
    xhr.open('POST', href, true);
    xhr.setRequestHeader('Content-Type', 'application/json; charset=UTF-8');

    xhr.onloadstart = onloadstart
    xhr.onloadend = function () { window.location.href = xhr.responseURL; }

    xhr.send(JSON.stringify(data));
}