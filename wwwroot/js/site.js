// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {

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
});