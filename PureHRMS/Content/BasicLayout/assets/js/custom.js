
$(document).ready(function () {

  
    /*======================================
   TEXT EDITOR SCRIPTS
   ========================================*/
    $('.textEditorCustom').summernote({
        height: 350,// set height for editor
    });
    /*======================================
   DATEPICKER SCRIPTS
   ========================================*/
    $(".datepicker").datepicker({
        dateFormat: 'mm/dd/yy',
        changeYear: true,
        changeMonth: true,
       // maxDate: 0,
       // minDate:-2000 // MINIMUM DATE -2000 days
    });
   
    
});
      