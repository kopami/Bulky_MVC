﻿var dataTable;

$(document).ready(function () {
    LoadDataTable();
});

function LoadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'company.name', "width": "15%" },
            { data: 'role', "width": "15%" },
            {
                data: { id: 'id', lockoutEnd: 'lockoutEnd'},
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        //currently locked
                        return `
                        <div class="text-center d-flex">
                            <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:40%;">
                                <i class="bi bi-unlock-fill"></i> Unlock
                            </a>
                            <a class="btn btn-danger text-white" style="cursor:pointer; width:60%;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                        </div>
                        `
                    }
                    else {
                        return `
                        <div class="text-center d-flex">
                            <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:40%;">
                                <i class="bi bi-lock-fill"></i> Lock
                            </a>
                            <a class="btn btn-danger text-white" style="cursor:pointer; width:60%;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                        </div>
                        `
                    }
                    

                },
                "width": "25%"
            }
        ]
    });
}
function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toast.success(data.message);
                dataTable.ajax.reload();
            }
        }
    })
}