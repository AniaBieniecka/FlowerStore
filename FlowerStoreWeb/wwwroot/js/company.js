﻿var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url:'/admin/company/getall'},
        "columns": [
        { data: 'name', "width": "15%" },
        { data: 'streetAddress', "width": "30%" },
        { data: 'city', "width": "5%" },
        { data: 'state', "width": "20%" },
        { data: 'postalCode', "width": "20%" },
        { data: 'phoneNumber', "width": "20%" },
        {
                data: 'companyID',
            "render": function (data) {
                return `<div class="w-75 btn-group" role="group">
                            <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2 ">
                            <i class="bi bi-pen"></i>  Edit
                            </a>
                            <a onClick=Delete('/admin/product/delete?id=${data}') class="btn btn-primary mx-2 ">
                            <i class="bi bi-trash3"></i>  Delete
                            </a>
                </div>`
            }
            , "width": "25%"
            }
    ]
    });
}

function Delete(url) {

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}