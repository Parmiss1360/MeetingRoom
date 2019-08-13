
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
   
