
(function ($, window) {

    var _clickTable = {
        init: function () {
            $('table.DataGridViewStyle').find('tr').not('.tr-header').not('.emptyrow').click(function () {
                $(this).siblings().removeClass('tr-selected');
                $(this).addClass('tr-selected');

                //change dropboxlist
                //$('select[id$=DropDownListBaseType]')

                //show button
                $('.DataArea').find('a.hidden').removeClass('hidden');
                //remember index
                $('input[id$=selectedIndex]').val($(this).index() - 1);
            });
        }
    }

    window.$mingzhang = $.extend(true, window.$mingzhang, { clickTable: _clickTable });

    $(document).ready(function () {
        $mingzhang.clickTable.init();

//        $('.DataGridViewStyle').find('tr').not('.tr-header').not('.emptyrow').hover(function () {
//            $(this).addClass('hover');
//            $(this).siblings().removeClass('hover');
//        });
    });


})($, window)