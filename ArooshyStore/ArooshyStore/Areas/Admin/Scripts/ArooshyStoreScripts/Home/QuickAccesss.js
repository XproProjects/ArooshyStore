$("#txtSearchModule").on("keyup", function () {
    SearchText();
});
function SearchText() {
    var value = $("#txtSearchModule").val().toLowerCase();
    $(".ModuleNameLi").filter(function () {
        $(this).toggle($(this).find('.ModuleNameSpan').html().toLowerCase().indexOf(value) > -1)
    });
}