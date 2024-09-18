$('#Document').change(function (e) {
    $('.ImageDisplayRow').attr("hidden", "hidden");
    var ext = $('#Document').val().split('.').pop().toLowerCase();
    var size = document.getElementById('Document').files[0].size;
    var filename = $('input[type=file]').val().replace(/C:\\fakepath\\/i, '');
    if ($.inArray(ext, ['jpeg', 'jpg', 'pdf', 'png', 'doc', 'docx', 'xls', 'xlsx', 'txt', '3gp', 'amr', 'mp3', 'wav', 'wma', 'aiff', 'opus', 'mp4a', 'gsm', 'webm']) == -1) {
        $('.ImageDisplayRow').attr("hidden", "hidden");
        alert('Error! File type allowed : jpeg, jpg, pdf, png, doc, docx, xls, xlsx, txt, 3gp, amr, mp3, wav, wma, aiff, opus, mp4a, gsm, webm');
        $('#Document').val('');
        $('#AttachDocument').text('Choose Document....');
    }
    else if (size > (20000 * 1024)) {
        $('.ImageDisplayRow').attr("hidden", "hidden");
        alert('Error! Maximum File size must be 20 mb');
        $('#Document').val('');
        $('#AttachDocument').text('Choose Document....');
    }
    else {

        //Get count of selected files
        var countFiles = $(this)[0].files.length;
        var imgPath = $(this)[0].value;
        var extn = imgPath.substring(imgPath.lastIndexOf('.') + 1).toLowerCase();
        $('#AttachDocument').text(filename);
        if (extn == "png" || extn == "jpg" || extn == "jpeg") {
            $('.ImageDisplayRow').removeAttr("hidden");
            if (typeof (FileReader) != "undefined") {

                //loop for each file selected for uploaded.
                for (var i = 0; i < countFiles; i++) {

                    var reader = new FileReader();
                    reader.onload = function (e) {

                        $("#DocumentImage").prop("src", e.target.result);
                        $('.fileDiv').removeClass('state-error');
                        $('.invalid-File').remove();
                    }
                    //image_holder.show();
                    reader.readAsDataURL($(this)[0].files[i]);
                }

            }
            else {
                alert("This browser does not support FileReader.");
            }
        }
        else {
            $('.ImageDisplayRow').attr("hidden", "hidden");
            $('#AttachDocument').val(filename);
        }
    }
});
$(function () {
    $('#btnSaveDocument').html('Upload');
    //$('.deleteTd').hide();
    $('.ImageDisplayRow').attr("hidden", "hidden");
    //GetTodayDate2();
    getDocumentsList();
})
$('#documentForm').on('submit', function (e) {
    e.preventDefault();
    var document = $('#Document').get(0).files;
    var documentFile = document[0];
    if ($('#DocumentId').val() == 0) {
        if (documentFile == '' || documentFile == undefined) {
            $('.fileDiv').removeClass('state-error');
            $('.invalid-File').remove();
            $('.fileDiv').addClass('state-error');
            $('.fileDiv').append('<label style="color:#cf564a;font-size:11px" for="Document" class="invalid-File">Document is required.</label>');
            return false;
        }
        else {
            $('.fileDiv').removeClass('state-error');
            $('.invalid-File').remove();
        }
    }
    //if (!$("#documentForm").valid()) {
    //    return false;
    //}
    var typeId = $('#TypeId').val();
    var documentType = $('#DocumentType').val();
    var documentId = $('#DocumentId').val();
    //var date = $('#DocumentDate').val();
    var remarks = $('#DocumentRemarks').val();
    var extension = $('#Document').val().split('.').pop().toLowerCase();
    var document = {
        DocumentId: documentId,
        DocumentType: documentType,
        TypeId: typeId,
        //Date: date,
        Remarks: remarks,
        DocumentExtension: extension,
        Document: document[0],
    }
    $('#btnSaveDocument').prop('disabled', true);
    $('#btnSaveDocument').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");
    $.ajax({
        type: "POST",
        url: '/admin/documents/attachdocumentsforpost/',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ 'doc': document }),
        success: function (data) {
            if (documentId > 0) {
                $('#btnSaveDocument').html("Save Changes");
            }
            else {
                $('#btnSaveDocument').html("Upload");
            }
            $('#btnSaveDocument').prop('disabled', false);
            if (data.status) {
                var bannerImage = $("#Document").val();
                var file = $('#Document').get(0).files;
                var documentFile = file[0];
                var extension = $('#Document').val().split('.').pop().toLowerCase();
                var documentIdWithDocumentTypeWithExtension = data.documentId + "_" + data.documentType + "_" + data.typeId + "_" + extension;
                var formData = new FormData();
                formData.append(file.name, documentFile);
                formData.append("DocumentName", $("#Document").text());

                var xhr = new XMLHttpRequest();
                var url = '/admin/documents/uploaddocument/' + documentIdWithDocumentTypeWithExtension;
                xhr.open('POST', url, true);

                xhr.onload = function (e) {
                    //var response = $.parseJSON(e.target.response);
                    toastr.success("Document Saved Succesfully.", "Success", { timeOut: 3000, "closeButton": true });
                    $('#DocumentId').val(0);
                    $('.ImageDisplayRow').attr("hidden", "hidden");
                    $('#documentForm')[0].reset();
                    $('#AttachDocument').text('Choose Document....');
                    $('.fileDiv').removeClass('state-error');
                    $('.invalid-File').remove();
                    getDocumentsList();
                    getDocumentsList2();
                    //GetTodayDate2();
                };
                xhr.send(formData);  // multipart/form-data
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})
function getDocumentsList() {
    var typeId = $('#TypeId').val();
    var documentType = $('#DocumentType').val();
    $.ajax({
        type: 'POST',
        url: '/admin/documents/documentslist/',
        dataType: 'json',
        data: { 'type': documentType, 'typeId': typeId },
        success: function (response) {
            var length = response.data.length;
            if (length == 0) {
                div = '<tr style="border-bottom: 1px solid #D8D8D8">' +
                    //'<th  style="text-align:center;padding:10px" colspan="6">' +
                    '<th  style="text-align:center;padding:10px" colspan="5">' +
                    'No document available.'
                '</th>' +
                    '</tr>';
            }
            else {
                var i = 0;
                var div = '';
                for (var key in response.data) {
                    i = i + 1;
                    div += '<tr style="border-bottom: 1px solid #D8D8D8">' +
                        '<td style="text-align: center;padding:7px;vertical-align:middle ">' +
                        '<input type="hidden" class="OldDocumentId" value="' + response.data[key].DocumentId + '" />' +
                        '<input type="hidden" class="OldDocumentExtension" value="' + response.data[key].DocumentExtension + '" />' +
                        '<input type="hidden" class="OldDocumentType" value="' + response.data[key].DocumentType + '" />' +
                        '<input type="hidden" class="OldRemarks" value="' + response.data[key].Remarks + '" />' +
                        //'<input type="hidden" class="OldDate" value="' + response.data[key].Date + '" />' +
                        '<input type="hidden" class="OldTypeId" value="' + response.data[key].TypeId + '" />' +
                        '<input type="hidden" class="OldImagePath" value="' + response.data[key].ImagePath + '" />' +
                        '' + i + '' +
                        '</td>' +
                        //'<th style="text-align: center; padding: 3px">' +
                        //    '' + getDate2(response.data[key].Date) + '' +
                        //'</th>' +
                        '<td style="text-align: left; padding: 7px;padding-left:8px;vertical-align:middle">' +
                        '' + response.data[key].Remarks + '' +
                        '</td>' +
                        '<td style="text-align: center; padding: 7px;vertical-align:middle !important">' +
                        '<a title="View Document" class="viewDocument btn btn-secondary" href="' + response.data[key].ImagePath + '" target="_blank" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-eye"></i></a>&nbsp' +
                        '</td>' +
                        '<td style="text-align: center; padding: 7px;vertical-align:middle !important">' +
                        '<a title="Edit Document" class="editDocument btn btn-primary" href="javascript:void(0)" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-edit"></i></a>&nbsp' +
                        '</td>' +
                        '<td style="text-align: center; padding: 7px;vertical-align:middle !important">' +
                        '<a title="Delete Document" class="btnDeleteDocument btn btn-danger" href="javascript:void(0)" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-times-circle"></i></a>&nbsp' +
                        '</td>' +
                        //'<th style="text-align: center; padding: 3px">' +
                        //    '<a download href="' + response.data[key].ImagePath +  '" target="_blank" class="btn btn-success downloadDocument" style="padding:5px;color:#fff !important;background-color: #428BCA !important;border-color: #428BCA !important">' +
                        //        ' Download' +
                        //    ' </a>' +
                        //'</th>' +
                        '</tr>';
                }
            }
            $('#DocumentsListTable').html(div);
        }
    });
}
$(document).off("click", ".editDocument").on("click", ".editDocument", function () {
    var documentId = $(this).closest("tr").find('.OldDocumentId').val();
    var remarks = $(this).closest("tr").find('.OldRemarks').val();
    var DocumentExtension = $(this).closest("tr").find('.OldDocumentExtension').val();
    //var datee = $(this).closest("tr").find('.OldDate').val();
    $('#DocumentId').val(documentId);
    $('#btnSaveDocument').html('Save Changes');
    //$('#DocumentDate').val(getDate3(datee));
    $('#DocumentRemarks').val(remarks);
    $('#Document').val('');
    if (DocumentExtension == "png" || DocumentExtension == "jpg" || DocumentExtension == "jpeg") {
        var imagePath = '/Areas/Admin/FormsDocuments/' + $('#DocumentType').val() + '/' + documentId + '.' + DocumentExtension;
        $('.ImageDisplayRow').removeAttr("hidden");
        $("#DocumentImage").attr("src", imagePath);
    }
    else {
        $('.ImageDisplayRow').attr("hidden", "hidden");
        $("#DocumentImage").attr("src", '');
    }
    $('#AttachDocument').text('Choose Document....');
    $('.fileDiv').removeClass('state-error');
    $('.invalid-File').remove();
})
$(document).off("click", ".btnCancelDocument").on("click", '.btnCancelDocument', function () {
    $('#DocumentId').val(0);
    $('#btnSaveDocument').prop('disabled', false);
    $('#btnSaveDocument').html("Upload");
    $('.ImageDisplayRow').attr("hidden", "hidden");
    $('#documentForm')[0].reset();
    $('#AttachDocument').text('Choose Document....');
    $('.fileDiv').removeClass('state-error');
    $('.invalid-File').remove();
})
$(document).off("click", ".btnDeleteDocument").on("click", '.btnDeleteDocument', function () {
    var documentId = $(this).closest("tr").find('.OldDocumentId').val();
    DeleteDocument(documentId);
})
function DeleteDocument(id) {
    var a = $.confirm({
        title: 'Delete Document',
        content: 'Are you sure you want to delete this Document',
        type: 'white',
        buttons: {
            ok: {
                text: "Yes",
                btnClass: 'btnConfirm',
                keys: ['enter'],
                action: function () {
                    $.ajax({
                        url: '/admin/documents/DeleteDocument/' + id,
                        type: "POST",
                        dataType: 'json',
                        success: function (data) {
                            if (data.status == true) {
                                toastr.success("Document Deleted Successfully", "Success", { timeOut: 3000, "closeButton": true });
                                getDocumentsList();
                                $('#DocumentId').val(0);
                                $('#btnSaveDocument').prop('disabled', false);
                                $('#btnSaveDocument').html("Upload");
                                $('.ImageDisplayRow').attr("hidden", "hidden");
                                $('#documentForm')[0].reset();
                                $('#AttachDocument').text('Choose Document....');
                                $('.fileDiv').removeClass('state-error');
                                $('.invalid-File').remove();
                                getDocumentsList2();

                            }
                            else {
                                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
                            }
                        }
                    })
                }
            },
            cancel: {
                text: "No",
                btnClass: 'btn-default',
                keys: ['esc'],
                action: function () {
                    a.close();
                }
            },
        }
    });
}