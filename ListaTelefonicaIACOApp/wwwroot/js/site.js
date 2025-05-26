// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    $('.telefone-fixo').mask('(00) 0000-0000');
    $('.telefone-celular').mask('(00) 000000000');
    $('.telefone-comercial').mask('(00) 0000-0000');
    $('.CEP').mask('00000-000');
});