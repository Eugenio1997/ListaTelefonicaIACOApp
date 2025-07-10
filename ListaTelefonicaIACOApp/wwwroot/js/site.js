// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function aplicarMascaras() {
    console.log("✅ aplicando máscaras...");

    //sendo aplicados nos formularios
    $('.telefone-fixo').mask('(00) 0000-0000', {
        placeholder: '(00) ____-____',
        clearIfNotMatch: true,
        selectOnFocus: true,
    });
    $('.telefone-comercial').mask('0000', {
        placeholder: '0000',
        clearIfNotMatch: true,
        selectOnFocus: true,
    });
    $('.telefone-celular').mask('(00) 00000-0000', {
        clearIfNotMatch: true,
        selectOnFocus: true,
    });

}

function aplicarReadonly() {
    $('.telefone-fixo, .telefone-celular, .telefone-comercial').attr('readonly', true);
}

$(document).ready(function () {
    aplicarMascaras();
});

//Ativando o tooltip do Bootstrap
$(function () {
    $('[data-bs-toggle="tooltip"]').tooltip();
});


// 🔁 Função para gerar paginação
function gerarPaginacao(paginaAtual, totalPaginas) {
    let paginacaoHtml = '';

    // << Pular 10 páginas para trás
    if (paginaAtual > 10) {
        paginacaoHtml += `<li class="page-item">
            <a class="page-link" href="javascript:void(0)" data-pagina="${paginaAtual - 10}">&laquo;</a>
        </li>`;
    } else {
        paginacaoHtml += `<li class="page-item disabled">
            <a class="page-link" href="javascript:void(0)">&laquo;</a>
        </li>`;
    }

    // < Página anterior
    paginacaoHtml += paginaAtual > 1
        ? `<li class="page-item"><a class="page-link" href="javascript:void(0)" data-pagina="${paginaAtual - 1}"><span aria-hidden="true">&lsaquo;</span></a></li>`
        : `<li class="page-item disabled"><a class="page-link" href="javascript:void(0)"><span aria-hidden="true">&lsaquo;</span></a></li>`;

    // Página atual / total
    paginacaoHtml += `
        <li class="page-item disabled"><a class="page-link" href="#">${paginaAtual}</a></li>
        <li class="page-item disabled"><a class="page-link" href="#">de</a></li>
        <li class="page-item disabled"><a class="page-link" href="#">${totalPaginas}</a></li>`;

    // > Próxima página
    paginacaoHtml += paginaAtual < totalPaginas
        ? `<li class="page-item"><a class="page-link" href="javascript:void(0)" data-pagina="${paginaAtual + 1}"><span aria-hidden="true" >&rsaquo;</span ></a></li>`
        : `<li class="page-item disabled"><a class="page-link" href="javascript:void(0)"><span aria-hidden="true" >&rsaquo;</span ></a></li>`;

    // >> Pular 10 páginas para frente
    if (paginaAtual + 10 <= totalPaginas) {
        paginacaoHtml += `<li class="page-item">
            <a class="page-link" href="javascript:void(0)" data-pagina="${paginaAtual + 10}">&raquo;</a>
        </li>`;
    } else {
        paginacaoHtml += `<li class="page-item disabled">
            <a class="page-link" href="javascript:void(0)">&raquo;</a>
        </li>`;
    }

    $('.pagination').html(paginacaoHtml);
}


function mostrarToast(titulo, mensagem, tipo = 'danger') {
    const cor = tipo === 'success' ? 'bg-success' : tipo === 'warning' ? 'bg-warning' : 'bg-danger';
    const icone = tipo === 'success' ? '✔️' : tipo === 'warning' ? '⚠️' : '❌';

    const toastHtml = `
            <div class="toast align-items-center text-white ${cor} border-0 mb-2" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <strong>${icone} ${titulo}</strong><br>
                            <span>${mensagem}</span>
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
            `;

    const $toast = $(toastHtml).appendTo("#toastContainer");
    const toast = new bootstrap.Toast($toast[0], { delay: 4000 });
    toast.show();
}

