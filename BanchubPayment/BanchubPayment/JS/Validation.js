$(document).ready(function () {

    //  /***********************************************
    //* Show more or less content
    //***********************************************/

    //  // Configure/customize these variables.
    //  var showChar = 135;  // How many characters are shown by default
    //  var ellipsestext = "...";
    //  var moretext = "Show more";
    //  var lesstext = "Show less";

    //  $('.more').each(function () {
    //      var content = $(this).html();
    //      if (content.length > showChar) {
    //          var c = content.substr(0, showChar);
    //          var h = content.substr(showChar, content.length - showChar);
    //          var html = c + '<span class="moreellipses">' + ellipsestext + '&nbsp;</span><span class="morecontent"><span>' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink">' + moretext + '</a></span>';
    //          $(this).html(html);
    //      }
    //  });

    //  $(".morelink").click(function () {
    //      if ($(this).hasClass("less")) {
    //          $(this).removeClass("less");
    //          $(this).html(moretext);
    //      } else {
    //          $(this).addClass("less");
    //          $(this).html(lesstext);
    //      }
    //      $(this).parent().prev().toggle();
    //      $(this).prev().toggle();
    //      return false;
    //  });

    //  function ShowLess() {

    //      $('.more').each(function () {
    //          var content = $(this).html();
    //          if (content.length > showChar) {
    //              var c = content.substr(0, showChar);
    //              var h = content.substr(showChar, content.length - showChar);
    //              var html = c + '<span class="moreellipses">' + ellipsestext + '&nbsp;</span><span class="morecontent"><span>' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink">' + moretext + '</a></span>';
    //              $(this).html(html);
    //          }
    //      });

    //      $(".morelink").click(function () {
    //          if ($(this).hasClass("less")) {
    //              $(this).removeClass("less");
    //              $(this).html(moretext);
    //          } else {
    //              $(this).addClass("less");
    //              $(this).html(lesstext);
    //          }
    //          $(this).parent().prev().toggle();
    //          $(this).prev().toggle();
    //          return false;
    //      });
    //  }

    //  /***********************************************
    //   * Show more or less content ends
    //   ***********************************************/

    /***********************************************
     * Popup invoking
     ***********************************************/
    function InvokePop() {
        $('[data-toggle="popover"]').popover()
    }
    /* end */

    /* Disabling the source */
    $(document).keydown(function (event) {
        if (event.keyCode == 123) {
            return false;
        }
        else if (event.ctrlKey && event.shiftKey && event.keyCode == 73) {
            return false;
        }
    });

    $(document).on("contextmenu", function (e) {
        e.preventDefault();
    });

    /* end */

    /***********************************************
     * Keypress events
     ***********************************************/
    $(document).on('keypress', '#CreditCardHolderName', function (event) {
        var regex = new RegExp("^[a-zA-Z ]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
        else
            return true;
    });

    $(document).on('keypress', '#DebitCardHolderName', function (event) {
        var regex = new RegExp("^[a-zA-Z ]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
        else
            return true;
    });

    $(document).on('keypress', '#AccountName', function (event) {
        var regex = new RegExp("^[a-zA-Z ]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
        else
            return true;
    });

    $(document).on('keypress', '#CreditCardNumber', function (event) {

        //var reg = new RegExp("^[0-9,\b][0-9,\b]*$");

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        //if (!reg.test(key)) {
        //    event.preventDefault();
        //    return false;
        //}
        //else {
        if (key >= 0 || key <= 9) {
            var len = ($('#CreditCardNumber').val()).length;
            if (len == 4 || len == 9 || len == 14 || len == 19) {
                $('#CreditCardNumber').val($('#CreditCardNumber').val() + "-");
            }

            var new_img = getcardtype(($('#CreditCardNumber').val()));
            $("#cardimg").empty();
            var imgtag = $('<img id="ctype">');
            imgtag.attr('src', new_img);
            imgtag.appendTo('#cardimg');
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#DebitCardNumber', function (event) {

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            var len = ($('#DebitCardNumber').val()).length;
            if (len == 4 || len == 9 || len == 14 || len == 19) {
                $('#DebitCardNumber').val($('#DebitCardNumber').val() + "-");
            }
            var new_img1 = getcardtype(($('#DebitCardNumber').val()));
            $("#cardimg1").empty();
            var imgtag = $('<img id="dtype">');
            imgtag.attr('src', new_img1);
            imgtag.appendTo('#cardimg1');
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#AccountNumber', function (event) {
        //var reg = new RegExp("^[0-9,\b][0-9,\b]*$");
        //var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        //if (!reg.test(key)) {
        //    event.preventDefault();
        //    return false;
        //}
        //else
        //    return true;

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#RoutingNumber', function (event) {
        //var reg = new RegExp("^[0-9,\b][0-9,\b]*$");
        //var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        //if (!reg.test(key)) {
        //    event.preventDefault();
        //    return false;
        //}
        //else
        //    return true;

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#WireAccountNumber', function (event) {
        //var reg = new RegExp("^[0-9,\b][0-9,\b]*$");
        //var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        //if (!reg.test(key)) {
        //    event.preventDefault();
        //    return false;
        //}
        //else
        //    return true;

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#WireRoutingNumber', function (event) {
        //var reg = new RegExp("^[0-9,\b][0-9,\b]*$");
        //var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        //if (!reg.test(key)) {
        //    event.preventDefault();
        //    return false;
        //}
        //else
        //    return true;

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#CreditZipCode', function (event) {
        //var reg = new RegExp("^[0-9,\b][0-9,\b]*$");
        //var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        //if (!reg.test(key)) {
        //    event.preventDefault();
        //    return false;
        //}
        //else
        //    return true;

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#DebitZipCode', function (event) {
        //var reg = new RegExp("^[0-9,\b][0-9,\b]*$");
        //var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        //if (!reg.test(key)) {
        //    event.preventDefault();
        //    return false;
        //}
        //else
        //    return true;

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#ZipCode', function (event) {
        //var reg = new RegExp("^[0-9,\b][0-9,\b]*$");
        //var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        //if (!reg.test(key)) {
        //    event.preventDefault();
        //    return false;
        //}
        //else
        //    return true;

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#WireZipCode', function (event) {
        //var reg = new RegExp("^[0-9,\b][0-9,\b]*$");
        //var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        //if (!reg.test(key)) {
        //    event.preventDefault();
        //    return false;
        //}
        //else
        //    return true;

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#CreditCvv', function (event) {

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            return true;
        }
        return false;
    });

    $(document).on('keypress', '#DebitCvv', function (event) {

        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (key >= 0 || key <= 9) {
            return true;
        }
        return false;
    });

    $(document).on('keydown', '#Phone', function (event) {
        var regex = new RegExp("^[0-9]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (event.which == 8 || (event.which >= 37 && event.which <= 40) || event.which == 46) {
            return;
        }
        if (event.key == 16 || event.shiftKey == true) {
            return false;
        }

        if (event.which >= 96 && event.which <= 105) {            
            key = String.fromCharCode(event.which -= 48);
        }

        if (!regex.test(event.key) && event.which != 229) {
            return false;
        }

        if (key >= 0 || key <= 9 || event.which == 229) {            
            var len = ($('#Phone').val()).length;
            if (len == 3 || len == 7) {                
                $('#Phone').val($('#Phone').val() + "-");
            }
            var dlen = $("#debitForm").find("#Phone").val().length;
            if (dlen == 3 || dlen == 7) {
                $("#debitForm").find("#Phone").val(($("#debitForm").find("#Phone").val() + "-"));
            }
            var alen = $("#achForm").find("#Phone").val().length;
            if (alen == 3 || alen == 7) {
                $("#achForm").find("#Phone").val(($("#achForm").find("#Phone").val() + "-"));
            }            
            return true;
        }
        return false;
    });

    
    //$('#Phone').on.keydown(function (e) {
    //    var key = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        
    //    if (key >= 0 || key <= 9) {
    //        var len = ($('#Phone').val()).length;
            
    //        if (len == 3 || len == 7) {
                
    //            $('#Phone').val($('#Phone').val() + "-");
    //        }
    //        var dlen = $("#debitForm").find("#Phone").val().length;
    //        if (dlen == 3 || dlen == 7) {
    //            $("#debitForm").find("#Phone").val(($("#debitForm").find("#Phone").val() + "-"));
    //        }
    //        var alen = $("#achForm").find("#Phone").val().length;
    //        if (alen == 3 || alen == 7) {
    //            $("#achForm").find("#Phone").val(($("#achForm").find("#Phone").val() + "-"));
    //        }
    //        return true;
    //    }
    //    return false;
    //});

    $(document).bind("cut copy paste", function (e) {
        e.preventDefault();
    });

    $(document).on('keydown', '#CaPhone', function (event) {
        var regex = new RegExp("^[0-9]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (event.which == 8 || (event.which >= 37 && event.which <= 40) || event.which == 46) {
            return;
        }
        if (event.key == 16 || event.shiftKey == true) {
            return false;
        }

        if (event.which >= 96 && event.which <= 105) {
            key = String.fromCharCode(event.which -= 48);
        }
        if (!regex.test(event.key) && event.which != 229) {
            return false;
        }
        if (key >= 0 || key <= 9 || event.which == 229) {
            var len = ($('#CaPhone').val()).length;
            if (len == 3 || len == 7) {
                $('#CaPhone').val($('#CaPhone').val() + "-");
            }
            return true;
        }
        return false;
    });
    
    /***********************************************
     * Keypress events end
     ***********************************************/

    /***********************************************
     * Card type
     ***********************************************/
    function getcardtype(num) {
      
        num = num.replace(/\-/g, '');
        if (num.match("^4[0-9]+$"))
            return "../images/imgcard/card_visa.png";
        if (num.match("^(5018|5020|5038|5612|5893|6304|6759|6761|6762|6763|0604|6390)[0-9]+$"))
            return "../images/imgcard/maestro_card.png";
        if (num.match("^(5019)[0-9]+$"))
            return "../images/imgcard/dankart.png";
        if (num.match("^(636)[0-9]+$"))
            return "../images/imgcard/interpay.png";
        if (num.match("^(62|88)[0-9]+$"))
            return "../images/imgcard/unionpay-card.png";
        if (num.match("^(51|52|53|54|55)[0-9]+$"))
            return "../images/imgcard/master_card.png";
        if (num.match("^(34|37)[0-9]+$"))
            return "../images/imgcard/amex_card.png";
        if (num.match("^3(?:0[0-5]|[68][0-9])[0-9]+$"))
            return "../images/imgcard/dinner_club.png";
        if (num.match("^6(?:011|5[0-9]{2})[0-9]+$"))
            return "../images/imgcard/discover_network_card.png";
        if (num.match("^(?:2131|1800|35[0-9]{3})[0-9]+$"))
            return "../images/imgcard/jcb_card.png";
        return "../images/blank.gif";
    }

    //< script src="~/Scripts/aes.js" ></script >
    /*
     AES encryption for client to server for key field
     */
    function aesencrypt(data, svalue) {
        //debugger
        //var key = CryptoJS.enc.Utf8.parse('8080808080808080');
        //var iv = CryptoJS.enc.Utf8.parse('8080808080808080');        
        var key = CryptoJS.enc.Utf8.parse(svalue.substring(0,16));
        var iv = CryptoJS.enc.Utf8.parse(svalue.substring(16,32));

        return CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(data), key,
            {
                keySize: 128 / 8,
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });        
    }

    function aesdecrypt(data, svalue) {
        var key = CryptoJS.enc.Utf8.parse(svalue.substring(0, 16));
        var iv = CryptoJS.enc.Utf8.parse(svalue.substring(16, 32));

        return (CryptoJS.AES.decrypt(data, key,
            {
                keySize: 128 / 8,
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            })).toString(CryptoJS.enc.Utf8);
    }

    /***********************************************
     * Card type end
     ***********************************************/

    /***********************************************
     * Credit card payment process
     ***********************************************/
    $(document).on('click', '#credit', function () {        
        //ENCRYPT THE DATA USING AES256  and assign the data to textbox.        
        var temp = $('#tempccvalue').val();
        if ($('#CreditCardHolderName').val() != "")
            $('#CreditCardHolderName').val(aesencrypt($('#CreditCardHolderName').val(), temp));
        if ($('#CreditCardNumber').val() != "")
            $('#CreditCardNumber').val(aesencrypt($('#CreditCardNumber').val(), temp));
        if ($('#CreditCvv').val() != "")
            $('#CreditCvv').val(aesencrypt($('#CreditCvv').val(), temp));        
        
        $('#creditcancel').prop('disabled', true);
        
        var $this = $('#credit');
        $this.button('loading');
        //select and serialize our small form
        var frm = $("#creditForm").serialize();
        // get form action
        var action = $("#creditForm").attr("action"); 

        try {
            $.ajax({
                url: action,
                cache: false,
                async: true,
                type: "POST",
                data: frm, //data to post to server
                timeout: 120000,
                error: function (jqXHR, exception) {
                    if (jqXHR.status === 0) {
                        alert('Network not available. You will receive status notification for the requested payment.');
                    } else if (jqXHR.status == 404) {
                        alert('Requested page not found. [404]');
                    } else if (jqXHR.status == 500) {
                        alert('Internal Server Error [500].  You will receive status notification for the requested payment.');
                    } else if (exception === 'parsererror') {
                        alert('Requested JSON parse failed.');
                    } else if (exception === 'timeout') {
                        alert('Time out due to network issue. You will receive status notification for the requested payment.');
                    } else if (exception === 'abort') {
                        alert('Request aborted.');
                    } else {
                        alert('Uncaught Error.n' + jqXHR.responseText);
                    }

                    $('#creditcancel').prop('disabled', false);
                    $this.button('reset');
                    $('#CreditCardHolderName').val(aesdecrypt($('#CreditCardHolderName').val(), temp));
                    $('#CreditCardNumber').val(aesdecrypt($('#CreditCardNumber').val(), temp));
                    InvokePop();
                    ShowLess();
                },
                success: function (res) {                    
                    if (res.redirectUrl != null) {
                        SetUrl(res.redirectUrl);
                    }
                    else {

                        $("#creditform").html($.parseHTML(res));
                        $('#creditcancel').prop('disabled', false);
                        $this.button('reset');
                        $('#CreditCardHolderName').val(aesdecrypt($('#CreditCardHolderName').val(), temp));
                        $('#CreditCardNumber').val(aesdecrypt($('#CreditCardNumber').val(), temp));
                        InvokePop();
                        ShowLess();
                    }

                }
            });
        } catch (e) {
            alert(e.message);
        }
    });

    /***********************************************
     * Credit card payment process ends
     ***********************************************/

    /***********************************************
     * Debit card payment process
     ***********************************************/
    $(document).on('click', '#debit', function () {
        //debugger
        //ENCRYPT THE DATA USING AES256  and assign the data to textbox.
        var temp = $('#tempddvalue').val();
        if ($('#DebitCardHolderName').val() != "")
            $('#DebitCardHolderName').val(aesencrypt($('#DebitCardHolderName').val(), temp));
        if ($('#DebitCardNumber').val() != "")
            $('#DebitCardNumber').val(aesencrypt($('#DebitCardNumber').val(), temp));
        if ($('#DebitCvv').val() != "")
            $('#DebitCvv').val(aesencrypt($('#DebitCvv').val(), temp));

        $('#debitcancel').prop('disabled', true);        
        var $this = $('#debit');
        $this.button('loading');
        //select and serialize our small form
        var frm = $("#debitForm").serialize();
        // get form action
        var action = $("#debitForm").attr("action");
        $.ajax({
            url: action,
            cache: false,
            async: true,
            type: "POST",
            data: frm, //data to post to server
            timeout: 120000,
            error: function (jqXHR, exception) {
                //debugger
                alert('error');
                if (jqXHR.status === 0) {
                    alert('Network not available. You will receive status notification for the requested payment.');
                } else if (jqXHR.status == 404) {
                    alert('Requested page not found. [404]');
                } else if (jqXHR.status == 500) {
                    alert('Internal Server Error [500].  You will receive status notification for the requested payment.');
                } else if (exception === 'parsererror') {
                    alert('Requested JSON parse failed.');
                } else if (exception === 'timeout') {
                    alert('Time out due to network issue. You will receive status notification for the requested payment.');
                } else if (exception === 'abort') {
                    alert('Request aborted.');
                } else {
                    alert('Uncaught Error.n' + jqXHR.responseText);
                }

                $('#debitcancel').prop('disabled', false);
                $this.button('reset');
                $('#DebitCardHolderName').val(aesdecrypt($('#DebitCardHolderName').val(), temp));
                $('#DebitCardNumber').val(aesdecrypt($('#DebitCardNumber').val(), temp));
                InvokePop();
                ShowLess();
            },
            success: function (res) {                
                if (res.redirectUrl != null) {                   
                        SetUrl(res.redirectUrl);                    
                }
                else {
                    $("#debitform").html($.parseHTML(res)); //replace our form content
                    $('#debitcancel').prop('disabled', false);
                    $this.button('reset');
                    $('#DebitCardHolderName').val(aesdecrypt($('#DebitCardHolderName').val(), temp));
                    $('#DebitCardNumber').val(aesdecrypt($('#DebitCardNumber').val(), temp));
                    InvokePop();
                    ShowLess();
                }
            }
        });        
    });

    /***********************************************
     * Debit card payment process ends
     ***********************************************/

    /***********************************************
     * ACH (USA) payment process
     ***********************************************/
    $(document).on('click', '#ach', function () {

        //ENCRYPT THE DATA USING AES256  and assign the data to textbox.
        var temp = $('#tempavalue').val();
        if ($('#AccountName').val() != "")
            $('#AccountName').val(aesencrypt($('#AccountName').val(), temp));
        if ($('#AccountNumber').val() != "")
            $('#AccountNumber').val(aesencrypt($('#AccountNumber').val(), temp));
        if ($('#RoutingNumber').val() != "")
            $('#RoutingNumber').val(aesencrypt($('#RoutingNumber').val(), temp));
        
        $('#achcancel').prop('disabled', true);
        //select and serialize our small form
        var frm = $("#achForm").serialize();
        var $this = $('#ach');
        $this.button('loading');
        // get form action
        var action = $("#achForm").attr("action");
        $.ajax({
            url: action,
            cache: false,
            async: true,
            type: "POST",
            data: frm, //data to post to server
            timeout: 120000,
            error: function (jqXHR, exception) {
                if (jqXHR.status === 0) {
                    alert('Network not available. You will receive status notification for the requested payment.');
                } else if (jqXHR.status == 404) {
                    alert('Requested page not found. [404]');
                } else if (jqXHR.status == 500) {
                    alert('Internal Server Error [500].  You will receive status notification for the requested payment.');
                } else if (exception === 'parsererror') {
                    alert('Requested JSON parse failed.');
                } else if (exception === 'timeout') {
                    alert('Time out due to network issue. You will receive status notification for the requested payment.');
                } else if (exception === 'abort') {
                    alert('Request aborted.');
                } else {
                    alert('Uncaught Error.n' + jqXHR.responseText);
                }

                $('#achcancel').prop('disabled', false);
                $this.button('reset');
                $('#AccountName').val(aesdecrypt($('#AccountName').val(), temp));
                $('#AccountNumber').val(aesdecrypt($('#AccountNumber').val(), temp));
                $('#RoutingNumber').val(aesdecrypt($('#RoutingNumber').val(), temp));
                InvokePop();
                ShowLess();
            },
            success: function (res) {
                
                if (res.redirectUrl != null) {                   
                    SetUrl(res.redirectUrl);
                }
                else {
                    $("#achform").html($.parseHTML(res)); //replace our form content
                    $this.button('reset');
                    $('#AccountName').val(aesdecrypt($('#AccountName').val(), temp));
                    $('#AccountNumber').val(aesdecrypt($('#AccountNumber').val(), temp));
                    $('#RoutingNumber').val(aesdecrypt($('#RoutingNumber').val(), temp));
                    $('#achcancel').prop('disabled', false);
                    ShowLess();
                }
            }
        });
    });

    /***********************************************
     * ACH payment process ends
     ***********************************************/

    /***********************************************
     * ACH (Canada) payment process
     ***********************************************/
    $(document).on('click', '#achca', function () {
        //ENCRYPT THE DATA USING AES256  and assign the data to textbox.
        var temp = $('#tempcavalue').val();
        if ($('#CaAccountName').val() != "")
            $('#CaAccountName').val(aesencrypt($('#CaAccountName').val(), temp));
        if ($('#CaAccountNumber').val() != "")
            $('#CaAccountNumber').val(aesencrypt($('#CaAccountNumber').val(), temp));
        if ($('#BankTransitNumber').val() != "")
            $('#BankTransitNumber').val(aesencrypt($('#BankTransitNumber').val(), temp));

        $('#achcacancel').prop('disabled', true);
        //select and serialize our small form
        var frm = $("#achCaForm").serialize();
        var $this = $('#achca');
        $this.button('loading');
        // get form action
        var action = $("#achCaForm").attr("action");
        $.ajax({
            url: action,
            cache: false,
            async: true,
            type: "POST",
            data: frm, //data to post to server
            timeout: 120000,
            error: function (jqXHR, exception) {
                if (jqXHR.status === 0) {
                    alert('Network not available. Verify the newtwork. You will receive status notification for the requested payment.');
                } else if (jqXHR.status == 404) {
                    alert('Requested page not found. Please check the requested url.');
                } else if (jqXHR.status == 500) {
                    alert('Internal Server Error [500].  You will receive status notification for the requested payment.');
                } else if (exception === 'parsererror') {
                    alert('Requested JSON parse failed.');
                } else if (exception === 'timeout') {
                    alert('Time out due to network issue. You will receive status notification for the requested payment.');
                } else if (exception === 'abort') {
                    alert('Request aborted.');
                } else {
                    alert('Uncaught Error.n' + jqXHR.responseText);
                }

                $('#achcacancel').prop('disabled', false);
                $this.button('reset');
                $('#CaAccountName').val(aesdecrypt($('#CaAccountName').val(), temp));
                $('#CaAccountNumber').val(aesdecrypt($('#CaAccountNumber').val(), temp));
                $('#BankTransitNumber').val(aesdecrypt($('#BankTransitNumber').val(), temp));
                InvokePop();
                ShowLess();
            },
            success: function (res) {
                if (res.redirectUrl != null) {
                    SetUrl(res.redirectUrl);
                } else {
                    $this.button('reset');
                    $('#achcacancel').prop('disabled', false);
                    $("#achcaform").html($.parseHTML(res)); //replace our form content
                    $('#CaAccountName').val(aesdecrypt($('#CaAccountName').val(), temp));
                    $('#CaAccountNumber').val(aesdecrypt($('#CaAccountNumber').val(), temp));
                    $('#BankTransitNumber').val(aesdecrypt($('#BankTransitNumber').val(), temp));
                    ShowLess();
                }
            }
        });
    });

    /***********************************************
     * ACH(Canada) payment process ends
     ***********************************************/

    $(document).on('click', '#achWire', function () {
        //select and serialize our small form
        //ENCRYPT THE DATA USING AES256  and assign the data to textbox.

        var temp = $('#tempwvalue').val();
        if ($('#WireAccountName').val() != "")
            $('#WireAccountName').val(aesencrypt($('#WireAccountName').val(), temp));
        if ($('#WireAccountNumber').val() != "")
            $('#WireAccountNumber').val(aesencrypt($('#WireAccountNumber').val(), temp));
        if ($('#WireAccountType').val() != "")
            $('#WireAccountType').val(aesencrypt($('#WireAccountType').val(), temp));
        if ($('#WireRoutingNumber').val() != "")
            $('#WireRoutingNumber').val(aesencrypt($('#WireRoutingNumber').val(), temp));

        var frm = $("#wireForm").serialize();
        // get form action
        var action = $("#wireForm").attr("action");
        $.ajax({
            url: action, 
            cache: false,
            async: true,
            type: "POST",
            data: frm, //data to post to server
            error: function (edata) {
                if (statusCode.status == 0) {
                    alert("Please check your internet connection and try again.");
                }
                else {
                    alert(edata);
                }
            }, //something goes wrong...
            success: function (res) {
                if (res.redirectUrl != null) {
                    SetUrl(res.redirectUrl);
                }
                else {
                    $("#wireform").html($.parseHTML(res)); //replace our form content
                    $('#WireAccountName').val(aesdecrypt($('#WireAccountName').val(), temp));
                    $('#WireAccountNumber').val(aesdecrypt($('#WireAccountNumber').val(), temp));
                    $('#WireAccountType').val(aesdecrypt($('#WireAccountType').val(), temp));
                    $('#WireRoutingNumber').val(aesdecrypt($('#WireRoutingNumber').val(), temp));
                }
            }
        });
    });

    /***********************************************
     * Payment response url redirection
     ***********************************************/


/***********************************************
* Amazon Pay Payment Process
***********************************************/
    $("#AmazonPay").on('click', function () {
        debugger;
        $('#AmazonPaycancel').prop('disabled', true);

        var $this = $('#AmazonPay');
        $this.button('loading');

        //select and serialize our small form
        var frm = $("#AmazonPayForm").serialize()
            + '&'
            + this.name
            + '='
            + this.value
            ;

        // get form action
        var action = $("#AmazonPayForm").attr("action");

        try {
            $.ajax({
                url: action,
                cache: false,
                async: true,
                type: "POST",
                data: frm, //data to post to server
                timeout: 120000,
                error: function (jqXHR, exception) {
                    debugger;
                    if (jqXHR.status === 0) {
                        alert('Network not available. You will receive status notification for the requested payment.');
                    } else if (jqXHR.status == 404) {
                        alert('Requested page not found. [404]');
                    } else if (jqXHR.status == 500) {
                        alert('Internal Server Error [500].  You will receive status notification for the requested payment.');
                    } else if (exception === 'parsererror') {
                        alert('Requested JSON parse failed.');
                    } else if (exception === 'timeout') {
                        alert('Time out due to network issue. You will receive status notification for the requested payment.');
                    } else if (exception === 'abort') {
                        alert('Request aborted.');
                    } else {
                        alert('Uncaught Error.n' + jqXHR.responseText);
                    }

                    $('#AmazonPaycancel').prop('disabled', false);
                    $this.button('reset');

                    //InvokePop();
                    //ShowLess();
                },
                success: function (res) {

                    if (res.redirectUrl != null) {

                        var url = res.redirectUrl;


                        $("#AddressAndWallet").attr("hidden");
                        $("#addressBookWidgetDiv").empty();
                        $("#walletWidgetDiv").empty();
                        $("#PaymentStatus").removeAttr("hidden");
                        $("#AmazonPay").addClass("hidden");
                        $("#AmazonPaycancel").addClass("Hidden");


                        $('#ConfirmOrder').append($("#PaymentStatus"));

                        amazon.Login.logout();

                        window.top.location.href = url;
                    }


                }
            });
        }
        catch (e) {

        }
    });  
/***********************************************
*  Amazon Pay Payment Process end
***********************************************/
    function SetUrl(url) {

        $url = $('<a/>')
        $url.attr('href', url);
        
        document.body.appendChild($url[0]);
        $url[0].click();
        document.body.removeChild($url[0]);
    }

    //function InvalidURLException() {
    //    this.message = "An attempt was made to open a webpage of foreign domain. No allowed.";
    //    this.toString = function () {
    //        return this.message
    //    };
    //}

    //function validateURL(surl) {
    //    var url = parseURL(surl);
    //    var urlHostname = url.hostname.trim();

    //    if (urlHostname == '') {
    //        return true;
    //    }
    //    else {
    //        if (urlHostname.toUpperCase() == location.hostname.trim().toUpperCase()) {
    //            return true;
    //        }
    //        else
    //            return false;
    //    }
    //}

    function parseURL(url) {
        var a = document.createElement('a');
        a.href = url;
        return {
            source: url,
            protocol: a.protocol.replace(':', ''),
            hostname: a.hostname,
            host: a.host,
            port: a.port,
            query: a.search,
            params: (function () {
                var ret = {},
                    seg = a.search.replace(/^\?/, '').split('&'),
                    len = seg.length, i = 0, s;
                for (; i < len; i++) {
                    if (!seg[i]) { continue; }
                    s = seg[i].split('=');
                    ret[s[0]] = s[1];
                }
                return ret;
            })(),
            file: (a.pathname.match(/\/([^\/?#]+)$/i) || [, ''])[1],
            hash: a.hash.replace('#', ''),
            path: a.pathname.replace(/^([^\/])/, '/$1'),
            relative: (a.href.match(/tps?:\/\/[^\/]+(.+)/) || [, ''])[1],
            segments: a.pathname.replace(/^\//, '').split('/')
        };
    }
    
    //var saferInnerHTML = function (t, e, n) { "use strict"; var r = null, o = function (t, e) { e.forEach((function (e) { "class" === e.att ? t.className = e.value : "data-" === e.att.slice(0, 5) ? t.setAttribute(e.att, e.value || "") : t[e.att] = e.value || "" })) }, a = function (t) { return Array.from(t).map((function (t) { return { att: t.name, value: t.value } })) }, c = function (t) { var e = "text" === t.type ? document.createTextNode(t.content) : document.createElement(t.type); return o(e, t.atts), t.children.length > 0 ? t.children.forEach((function (t) { e.appendChild(c(t)) })) : "text" !== t.type && (e.textContent = t.content), e }, i = function (t) { var e = []; return Array.from(t.childNodes).forEach((function (t) { e.push({ content: t.childNodes && t.childNodes.length > 0 ? null : t.textContent, atts: 3 === t.nodeType ? [] : a(t.attributes), type: 3 === t.nodeType ? "text" : t.tagName.toLowerCase(), children: i(t) }) })), e }; if (!t) throw new Error("safeInnerHTML: Please provide a valid element to inject content into"); if (!(function () { if (!Array.from || !window.DOMParser) return !1; r = r || new DOMParser; try { r.parseFromString("x", "text/html") } catch (t) { return !1 } return !0 })()) throw new Error("safeInnerHTML: Your browser is not supported."); !(function (e) { n || (t.innerHTML = ""), e.forEach((function (e, n) { t.appendChild(c(e)) })) })(i(function (t) { return r = r || new DOMParser, r.parseFromString(t, "text/html").body }(e))) };

    function message(title, text, type) {
        new PNotify({
            title: title,
            text: text,
            type: type,
            styling: 'bootstrap3'
        });
    }


   
})

    