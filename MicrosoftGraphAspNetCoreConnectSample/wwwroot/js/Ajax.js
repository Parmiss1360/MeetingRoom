
$(function()

{
 var placeholder=$("#modal-placeholder");
 $(document).on("click","button[data-toggle='ajax-modal']" ,(function(){

    var url =$(this).data('url');

$.ajax({
    type: "GET",
    url:url,
    beforesend:function(){$('body').preloader();},
    complete:function(){$('body').preloader('remove');},
    success: function (response) {
    placeholder.html(response);
  
    placeholder.find('#myModal').modal('show');
    }
});


    }));


placeholder.on('click','button[data-save="modal"]', function(){


  $("body").preloader();
var form=$(this).parents("div").find('form');

 //$(this).parents(".modal-content").find('form').css({"color": "red", "border": "2px solid red"});
//var form= placeholder.find('form');

var url=form.attr('action');
 var senddata=new FormData(form.get(0));

	
$.ajax({
    type: "post",
    url: url,
    data: senddata,
    contentType:false,
    processData: false,
    success: function (response) {

     //$("#test").html(response).css({"color": "red", "border": "2px solid red"});

//ما در اینجا نیازمد دانستن اطلاعات بیشتری هستیم در مرحله اول ما همیشه یک دایو داریم به نام پلیس هولدر که دورم فرمی که میخواهیم 
// مدال در ان باز شود قرار میدهیم 
//در این مرحله فرم مدال ما از طریق همین دایو به دکمه ان دسترسی داریم دکمه را گیدا کرده و یا متد کلیک در ان شروع میکنیم 
//مرحله پیدا کردن فرم ما است که ما درون ایتمهای دایو پرنتهاش دنبال ان میگردیم 
// موقعیکه پیدا شد اتریبیوت ما 
// فرم که همان اکشن است را مقدارش در یک متغییز ذخیره میکنیم 
// و بعد فرم را مقدارش را از ظریق formdata میگیریم 
//حالا نوبت فعال کردن گست  است که اطلاعات مرتبط ارسال شده 
// اما ما نیاز به اعتبار سنجی هم داریم چیکار باید کنیم برای اعتبار سنجی باید ابتدا یک متغییر در نظر بگیرم و از طریق viewDATA.statemodel.isvalid
//این مقدار رو درش بریزیم  اما نکته مهم این است که دیتای برگشتی رو از توی مدل بادی در میاریم و توی یک  متیغرر ریخته و بعد 
       
     var newBody = $(".modal-body", response);
        placeholder.find(".modal-body").replaceWith(response);
      

       var isvalid=$(".modal-body").find("#hid").val();
       var check=$(".modal-body").find("#check").val();
   
        if(isvalid==="True")
     

{
    

      var notificationpalceholder=$("#notification");
       var url_n=notificationpalceholder.data('url');


$.ajax({
   type: "get",
   url: url_n,
   success: function (response) {
       notificationpalceholder.html(response);
   }
});
var table=$("#myTable");
var tblurl=table.data('url'); 
$.ajax({
type: "get",
url: tblurl,

//codehaye partialview
success: function (table) {
$("#content").html(table);  


}
});


placeholder.find("#myModal").modal('hide');
 if(check==="1")
    {
    $(".card_Site").css({"background-color": "darkred"});

    }

    else
    {
       $(".card_Site").css({"background-color": "forestgreen"});


    }          
}  
$("body").preloader('remove');


    }
});

});

})
   
