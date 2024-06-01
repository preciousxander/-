// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var tokenKey = "accessToken";

function get_jwt(request) {
    request.setRequestHeader("jwt_token", sessionStorage.getItem(tokenKey));
}