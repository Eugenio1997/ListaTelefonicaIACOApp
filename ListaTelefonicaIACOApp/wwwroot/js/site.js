// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    // Landline or commercial phone format: (99) 9999-9999
    $('#Fixo').mask('(00) 0000-0000');
    $('#Comercial').mask('(00) 0000-0000');

    // Mobile phone format: (99) 999999999
    $('#Celular').mask('(00) 000000000');
});
