var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Admin/Order/getall' },
        "columns": [
            { data: 'orderHeaderId', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "15%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'productID',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                            <a href="/admin/order/details?orderHeaderId=${data}" class="btn btn-primary mx-2 ">
                            <i class="bi bi-pen"></i>Edit  
                            </a>
                </div>`
                }
                , "width": "25%"
            }
        ]
    });
}

