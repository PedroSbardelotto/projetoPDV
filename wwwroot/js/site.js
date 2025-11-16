// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {

    // --- Bloco 1: Lógica do Modal de DELEÇÃO (Seu código original) ---
    var deleteModal = document.getElementById('deleteModal');

    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;

            var itemId = button.getAttribute('data-bs-id');
            var itemName = button.getAttribute('data-bs-name');
            var formUrl = button.getAttribute('data-bs-url');
            var nameBase = button.getAttribute('data-nome-base');

            var modalBodyName = deleteModal.querySelector('#modalItemName');
            var deleteFormInput = deleteModal.querySelector('#modalItemId');
            var deleteForm = deleteModal.querySelector('#deleteForm');
            var modalNameBase = deleteModal.querySelector('#modalNameBase');

            modalBodyName.textContent = itemName;
            modalNameBase.textContent = nameBase;
            deleteFormInput.value = itemId;

            deleteForm.action = formUrl;
        });
    }

    // --- Bloco 2: Lógica do Modal de ATIVAÇÃO (Novo código) ---
    // A lógica é idêntica, mas mira o '#activateModal'
    var activateModal = document.getElementById('activateModal');

    if (activateModal) {
        activateModal.addEventListener('show.bs.modal', function (event) {
            // Pega o botão que disparou o modal
            var button = event.relatedTarget;

            // Extrai os dados 'data-bs-*' do botão
            var itemId = button.getAttribute('data-bs-id');
            var itemName = button.getAttribute('data-bs-name');
            var formUrl = button.getAttribute('data-bs-url');
            var nameBase = button.getAttribute('data-nome-base');

            // Encontra os elementos DENTRO do modal de ativação
            // (Os IDs dos spans/inputs internos são os mesmos do seu modal de delete)
            var modalBodyName = activateModal.querySelector('#modalItemName');
            var activateFormInput = activateModal.querySelector('#modalItemId');
            var activateForm = activateModal.querySelector('#activateForm'); // <-- Única mudança de ID interno
            var modalNameBase = activateModal.querySelector('#modalNameBase');

            // Preenche os elementos do modal com os dados
            modalBodyName.textContent = itemName;
            modalNameBase.textContent = nameBase;
            activateFormInput.value = itemId;

            // Define o 'action' do formulário de ativação
            activateForm.action = formUrl;
        });
    }

});