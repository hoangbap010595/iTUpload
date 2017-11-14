
$(document).ready(function () {

    var login = $('#loginform');
    var recover = $('#recoverform');
    var register = $('#registerform')
    var speed = 400;

    $('#to-recover').click(function () {

        $("#loginform").slideUp();
        $("#recoverform").fadeIn();
    });

    $('#to-login').click(function () {

        $("#recoverform").hide();
        $("#loginform").fadeIn();
    });
    if (recover != null)
        $("#recoverform").fadeIn();
    if (register != null)
        register.fadeIn();
    if ($.browser.msie == true && $.browser.version.slice(0, 3) < 10) {
        $('input[placeholder]').each(function () {

            var input = $(this);

            $(input).val(input.attr('placeholder'));

            $(input).focus(function () {
                if (input.val() == input.attr('placeholder')) {
                    input.val('');
                }
            });

            $(input).blur(function () {
                if (input.val() == '' || input.val() == input.attr('placeholder')) {
                    input.val(input.attr('placeholder'));
                }
            });
        });

    }

    //Kiểm tra Email
    function validateEmail2(emails) {
        var myEmails = emails.split(";");

        var isValid = false;
        var re = /^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$/;
        var n = myEmails.length;
        for (var i = 0; i < n; i++) {
            var myEmail = myEmails[i];
            if (!(i == n - 1 && myEmail.length == 0)) {
                isValid = re.test(myEmail);
                if (!isValid) {
                    break;
                }
            }
        }

        return isValid;
    }
    function validateEmail(emails) {
        var isValid = false;
        var re = /^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$/;
        if (!(emails.length == 0)) {
            isValid = re.test(emails);
        }
        return isValid;
    }
});