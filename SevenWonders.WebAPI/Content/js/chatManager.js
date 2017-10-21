$(function () {
    var element = $('.messagesContainer');
    height = element[0].scrollHeight;
    element.scrollTop(height);    
});

$('.messagesContainer').bind("DOMSubtreeModified", function () {
    alert('changed');
});

//$(document).on('click', '.button-chat', function (e) {   
//    var i = $(this).attr('id');
//    var element = $('.messagesContainer');
//    height = element[2].scrollHeight;
//    element.scrollTop(i);
//});


$(document).on('click', '#btn-chat', function (e) {   
    var i;
    var element = $('.messagesContainer');
    for (var k = 0; k < element.length; ++k)
    {
        if(element[k].scrollHeight!=0)
        {
            i = k;
            break;
        }
    }   
    height = element[i].scrollHeight;
    element.scrollTop(height);
});