
//add by  wzg 
//20090612
//页面 输入数据验证
(function($) {
    //添加提示框事件  
    $.fn.tooltip = function(options){
        var opts = $.extend({}, $.fn.tooltip.defaults, options);
        //生成提示框
        $('body').append('<div class="tooltipshowpanel"></div>');
        //$('.tooltipshowpanel').bgiframe();
        $(document).mouseover(function(){$('.tooltipshowpanel').hide();});
        this.each(function(){
            if($(this).attr('funs') != undefined)
            {
            //添加验证事件
                $(this).focus(function(){
                    $(this).removeClass('tooltipinputerr');
                //添加提示事件
                }).blur(function(){
                    this.value = jQuery.trim(this.value);
                    //获取验证 提示信息 
                    var tips = $(this).attr('tips').split('_');
                    var thisText = this;
                    var flag = true;
                    //获取验证 信息 (验证方法) 并循环
                    $($(this).attr('funs').split('_')).each(function(i,checkFun){
                        if (flag){
                                //验证
                                if ($.getCheckResult(checkFun.replace('@',$(thisText).attr('value')))){
                                    //验证正确 改变输入框显示样式
                                    $(thisText).removeClass('tooltipinputerr').addClass('tooltipinputok');
                                    //验证正确 取消输入框绑定事件
                                    $(thisText).unbind("mouseover mouseout");
                                    //判断 若有输入正确 则恢复选确定按钮
                                    if( $('.tooltipinputerr').size() == 0 ){
                                        //查找 id 以_ok 结尾   或者等于OkButton 的按钮 恢复
                                        ($("input[type=submit][id=OkButton],input[type=submit][id$=_ok]")).attr('disabled','');
                                    }
                                }else{
                                    //验证失败 改变输入框显示样式
                                    $(thisText).removeClass('tooltipinputok').addClass('tooltipinputerr');
                                    //设置提示框内容 与风格
                                    $('.tooltipshowpanel').css({left:$.getLeft(thisText)+'px',top:$.getTop(thisText)+'px'});
                                    $('.tooltipshowpanel').html(tips[i]);
                                    //验证失败 添加鼠标提示信息    
                                    $(thisText).bind("mouseover",function(){
                                        $('.tooltipshowpanel').slideDown("fast");
                                    });
                                    //验证失败 移出鼠标提示信息    
                                    $(thisText).bind("mouseout",function(){
                                        $('.tooltipshowpanel').hide();
                                    });
                                    //判断 若有错误输入 则灰选确定按钮
                                    if( $('.tooltipinputerr').size() > 0 ){
                                    //查找 id 以 _ok结尾   或者等于OkButton 的按钮 灰选
                                        ($("input[type=submit][id=OkButton],input[type=submit][id$=_ok]")).attr('disabled','disabled');
                                    }
                                    flag = false;
                                }
                        }
                    });
                });
            }
        });
    };
    //ActionArea  PopActionArea
    $.extend({
        //获取 输入框位置 
        getWidth : function(object) {
            return object.offsetWidth;
        },

        getLeft : function(object) {
            var go = object;
            var oParent,oLeft = go.offsetLeft;
            while(go.offsetParent!=null) {
                oParent = go.offsetParent;
                oLeft += oParent.offsetLeft;
                go = oParent;
            }
            return oLeft;
        },
        getTop : function(object) {
            var go = object;
            var oParent,oTop = go.offsetTop;
            while(go.offsetParent!=null) {
                oParent = go.offsetParent;
                oTop += oParent.offsetTop;
                go = oParent;
            }
            return oTop + 22;
        },
        //动态验证 输入框 数据   func 为验证方法名称 及参数
        getCheckResult : function(func) {
            return !eval(func); 
        },
        onsubmit : true
    });  
    $.fn.tooltip.defaults = { onsubmit: true };
    $(document).ready(function() {
        //添加删除提示
        jQuery('input[type=submit][id$=_del]').each(function(){
            $(this).click(function(){
                return confirm('确认删除吗？');
            });
        });
        //添加校验
        jQuery('input[tips],input[funs]').tooltip();
        jQuery('input[tips],input[funs]').each(function(){
            //日期类型
            if ($(this).attr('class')=='Wdate'){
                // this.focus();
                this.blur();
            }
            //若页面控件 不为失效
            else if (this.disabled == false){
                try{
                     this.focus();
                     this.blur();
                }
                catch(e)  {
                }
            }
        })
    });
})(jQuery)


// 判断为空
function notNull() {
	return (arguments[0] == null || arguments[0] == "");
}


// 判断为整数
function isInt() {
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /^(-|\+)?\d+$/;
	return pattern.test(arguments[0]);
}

// 判断为正整数
function isNumber() {
    if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /^\d+$/;
	return !pattern.test(arguments[0]);
}
//判断为正整数或字母
function isNumberOrAZ() {
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /[^0-9A-Z]/g;
	return !pattern.test(arguments[0]);
}
//判断为正整数或浮点数
function isNumberOrFloat() {
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern =/^\d+$/;
	var pattern2 =/^\d+(\.\d+)?$/;
	return !pattern.test(arguments[0])&&!pattern2.test(arguments[0]);
}

// 判断为负整数
function isNint() {
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /^-\d+$/;
	return pattern.test(arguments[0]);
}

//判断 浮点型数字
function isFloat() {
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var tempFloat = arguments[0];
	var aec =1;
	var dec =2;
	//12.23
	if (arguments[1] != null ) aec= arguments[1];
	if (arguments[2] != null ) dec= arguments[2];
	if( (tempFloat.indexOf(".") != -1 )  ){
		if ((tempFloat.length - tempFloat.indexOf('.')-1 > dec ))
		{
			return false;
		}
		
	}
	tempFloat = checkFloat(tempFloat,aec,dec);
	if ( tempFloat == 'error' ) return false;
   // /^\-?([1-9]\d*|0)(\.\d*)?$/;
   if(tempFloat.indexOf('.' == -1)){
   		tempFloat='0'+tempFloat;
   }
	var pattern = /^[-\+]?\d+(\,\d+)?\d+(\.\d+)?$/;
	return pattern.test(tempFloat);
}

//判断 浮点型数字
function checkFloat(f,aec,dec){
		f=f.replace(/,/g,''); 
		if (dec == null ) dec = 2; 
  		var result=parseInt(f);
  		var temp = result+'';
  		if (temp.length>aec ) return 'error';
  		result += (dec==0?"":".");
  		f-=parseInt(f);     
  		if(f==0)     
  			for(i=0;i<dec;i++)   result+='0';     
  		else {     
  			for(i=0;i<dec;i++)   f*=10;     
  				result+=parseInt(f);     
  			}     
  		return   result;   
}



// 判断短时间格式，形如(13:04:06)
function isTime() {
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var a = arguments[0].match(/^(\d{1,2})(:)?(\d{1,2})\2(\d{1,2})$/);
	return !(a == null || a[1]>24 || a[3]>60 || a[4]>60);
}

// 判断短日期格式，形如(2003-12-05)
function isDate() {
  if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
  var str = arguments[0];
  if ( str.length != 8 ) return false;
  var   r   =   str.match(/^(\d{1,4})(|\/)(\d{1,2})\2(\d{1,2})$/);     
  if(r==null) return   false;     
  var  d=  new  Date(r[1],   r[3]-1,   r[4]);
  return  (d.getFullYear()==r[1]&&(d.getMonth()+1)==r[3]&&d.getDate()==r[4]);   
}

// 判断长时间格式，形如(2003-12-05 13:04:06)
function isDateTime() {
	if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var reg = /^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$/; 
	var r = arguments[0].match(reg); 
    if(r == null) return false; 

	var d = new Date(r[1], r[3] - 1, r[4], r[5], r[6], r[7]); 
	return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4] && d.getHours() == r[5] && d.getMinutes() == r[6] && d.getSeconds() == r[7]);
}

// 判断汉字
function isChinese() {
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /^[\u0391-\uFFE5]+$/;
	return pattern.test(arguments[0]);
}

// 判断不是汉字
function isNotChinese() {
    if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /^[\u0391-\uFFE5]+$/;
	return !pattern.test(arguments[0]);
}

// 判断英文字母
function isAlpha() {
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /[^a-zA-Z]/g;
	return pattern.test(arguments[0]);
}

// 判断字母和数字组合
function isAlphanumeric() {
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /[^0-9a-zA-Z]/g;
	return pattern.test(arguments[0]);
}

// 判断字符由字母和数字，下划线,点号组成.且开头的只能是下划线和字母
function isAscii() {
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	  var thisReg = new RegExp(/[^0-9a-zA-Z]/g);
	    return thisReg.test(this.value);
}

// 判断URL地址格式
function isUrl() {
	if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /^http:\/\/[A-Za-z0-9]+\.[A-Za-z0-9]+[\/=\?%\-&_~`@[\]\':+!]*([^<>\"\"])*$/;
	return pattern.test(arguments[0]);
}

// 判断email地址格式
function isEmail() {
	if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var re = RegExp(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/);
	return re.test(arguments[0]);
}

// 判断IP地址
function isIP() {
	if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;
	return pattern.test(arguments[0]);
}

function isPhone() {
	if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var pattern = /(^((\d{1,6})-){0,2}(\d{3,9})$)|(^13(\d){9,9}$)/;
	return pattern.test(arguments[0]);
}

function isIdcardNo() {
	if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	var len = arguments[0].length;
	if(len == 15) {
		var reg = /^(\d{6})()?(\d{2})(\d{2})(\d{2})(\d{3})$/;
	} else if(len == 18) {
		var reg = /^(\d{6})()?(\d{4})(\d{2})(\d{2})(\d{3})(\d|X|x)$/;
	} else 
		return false;
	if(!reg.test(arguments[0])) return false;
	
	var part = arguments[0].match(reg);
	var year = (len == 15) ? ("19" + part[3]) : part[3];
	var date = new Date(year + "/" + part[4] + "/" + part[5]);
	if((date.getFullYear() != year) || 
	   ((date.getMonth() + 1) != (part[4] | 0)) ||
	   (date.getDate() != (part[5] | 0)))
		return false;
	
	if(len == 15) return true;
	var wi = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2];
	var lastcode = "10X98765432";
	var sum = 0;
	for(var i = 0; i < 17; i++) {
		sum += parseInt(arguments[0].charAt(i)) * wi[i];
	}
	return lastcode.charAt(sum % 11) == part[7].toUpperCase();
}
// 判断输入 长度
function lenBound() {
	if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	return (arguments[0].length > parseInt(arguments[1]) && arguments[0].length < parseInt(arguments[2]));
}


function checkLength(){
if (arguments[0] == null || arguments[0] == ""){
        return false;
    }
	return (arguments[0].length > parseInt(arguments[1]));
}