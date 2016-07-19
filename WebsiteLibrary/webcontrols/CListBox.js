var scrollbarWidth = null

function ClientID(prefix) {
	// Utility function to find an unused ID
	var count = 0;
	while (document.all[prefix + (++count)] != null);
	return prefix + count;
}

var tempID = ClientID("temp")
var str = "<DIV  ID=\"" + tempID + "\" STYLE=\"position: absolute; top: -500; width: 100;overflow: scroll\"><DIV STYLE=\"width: 500\">&nbsp;</DIV></DIV>"
if (document.readyState != "complete"){
	document.write(str)
} else {
	document.body.insertAdjacentHTML("beforeEnd", str);
}
var nWidth = (document.all[tempID].offsetWidth - document.all[tempID].clientWidth) 
scrollbarWidth = nWidth // cache value 

function XList_ResizeSelect(objSelect){
	//check to see if the object is visible
	if (objSelect.offsetHeight == 0){
		return;
	}
	
	//remove the onresize event so that it doesn't loop forever
	objSelect.onresize = null;
	
	//make sure it's a listbox and not a dropdown
	objSelect.size = objSelect.options.length < 2 ? 2 : (objSelect.options.length - 1);

	if (objSelect.offsetHeight < objSelect.parentElement.offsetHeight - scrollbarWidth){
		objSelect.style.height = objSelect.parentElement.offsetHeight - scrollbarWidth / 2;
	}

	objSelect.style.width = "";
	if (objSelect.offsetWidth < objSelect.parentElement.offsetWidth - scrollbarWidth){
		objSelect.style.width = (objSelect.parentElement.offsetWidth - scrollbarWidth) + "px";
	} else {
		objSelect.style.width = "auto";
	}
}

function XList_ShowOption(objSelect){
	idx = objSelect.selectedIndex
	if (idx == -1){
		return;
	}
	if (objSelect.length == 0){
		return;
	}
	objDiv = objSelect.parentElement;

	HeightOfSelect = objSelect.clientHeight;
	OptionsInSelect = objSelect.options.length;
	HeightOfOption = HeightOfSelect / OptionsInSelect;
	HeightOfDiv = objDiv.clientHeight;
	OptionsInDiv = HeightOfDiv / HeightOfOption;
	OptionTopFromTopOfSelect = HeightOfOption * idx;
	OptionTopFromTopOfDiv = OptionTopFromTopOfSelect - objDiv.scrollTop;
	OptionBottomFromBottomOfDiv = HeightOfDiv - OptionTopFromTopOfDiv - HeightOfOption;

	if (OptionTopFromTopOfDiv < 0) {
		objDiv.scrollTop = OptionTopFromTopOfSelect;
	} else if (OptionBottomFromBottomOfDiv < 0 && OptionBottomFromBottomOfDiv > 0 - HeightOfOption) {
		objDiv.scrollTop = objDiv.scrollTop + HeightOfOption;
	} else if (OptionBottomFromBottomOfDiv < 0) {
		objDiv.scrollTop = OptionTopFromTopOfSelect;
	}
}
