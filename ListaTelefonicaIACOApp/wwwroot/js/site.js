// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function aplicarMascaras() {
    console.log("✅ aplicando máscaras...");
    $('.telefone-fixo').mask('(00) 0000-0000');
    $('.telefone-celular').mask('(00) 00000-0000');
    $('.telefone-comercial').mask('(00) 0000-0000');
    $('.CEP').mask('00000-000');

}

$(document).ready(function () {
    aplicarMascaras();
});

//Ativando o tooltip do Bootstrap
$(function () {
    $('[data-bs-toggle="tooltip"]').tooltip();
});


