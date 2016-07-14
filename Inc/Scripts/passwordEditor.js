function valComparePassword (sender, args){
	    args.IsValid = true;
	    
        var password = document.getElementById(sender.id.replace("custvComparePassword", "txtPassword"));
        var confirmPassword = document.getElementById(sender.id.replace("custvComparePassword", "txtPasswordConfirm"));
        
        if (password.value != confirmPassword.value) 
        {
            args.IsValid = false;
        }
        //alert("passwords match compare:" + args.IsValid);
    }
function valPasswordLength(sender, args) {
    args.IsValid = true;
    var password = args.Value;
 
    if (password.length <3)
        args.IsValid = false;
}

function valPasswordPattern(sender, args) {
    args.IsValid = true;
    var password = args.Value;
    
    // NUMBERS
    //alert(!password.match(/\d/));
	if (!password.match(/\d/)) {             	// [verified] at least one number
		args.IsValid = false;
	}
    //alert(!password.match(/[a-z]/) && !password.match(/[A-Z]/));
    if (!password.match(/[a-z]/) && !password.match(/[A-Z]/)) {              // [verified] at least one lower case letter or one upper case letter
		args.IsValid = false;
	}
}

var newWidth;
var div2;

function CheckPsdStrength(password, div1ID, div2ID, passwordInputID)
{
	var score = 0 
    var p = password;
				
	var nScore = calcStrength(p);
	
	// Set new width
	var nRound = Math.round(nScore * 2);

	if (nRound > 100) {
		nRound = 100;
	}

    div2 = document.getElementById(div2ID);
	var maxWidth = document.getElementById(div1ID).offsetWidth - 2;
	//var maxWidth = div1.offsetWidth - 2;
	var scoreWidth = (maxWidth / 100) * nRound;
	newWidth = scoreWidth;
	
	if (nScore <20) 
	    document.getElementById(div2ID).title = "Your password is very weak!";
	else if (nScore > 20 && nScore <40) 
	    document.getElementById(div2ID).title = "Your password is medium strength.";
	else if (nScore > 40)
	    document.getElementById(div2ID).title = "Strong password!";
	document.getElementById(passwordInputID).title = document.getElementById(div2ID).title ;
	SetWidth();
}		

function SetWidth()
{			
    //var div = document.getElementById("div2");
	var oldWidth = parseInt(div2.style.width.replace("px", ""));

	div2.style.width = oldWidth  + 1;
	oldWidth  = oldWidth  + 1;
	if (newWidth > oldWidth )
		setTimeout(SetWidth,20);	
	else
	    div2.style.width = newWidth;		
}

function calcStrength(p) {
    if (p.length == 0) { 
        return 0; 
    } 
	var intScore = 0;

	// PASSWORD LENGTH
	intScore += p.length;
		
	if(p.length > 0 && p.length <= 4) {                    // length 4 or less
		intScore += p.length;
	}
	else if (p.length >= 5 && p.length <= 7) {	// length between 5 and 7
		intScore += 6;
	}
	else if (p.length >= 8 && p.length <= 15) {	// length between 8 and 15
		intScore += 12;
		//alert(intScore);
	}
	else if (p.length >= 16) {               // length 16 or more
		intScore += 18;
		//alert(intScore);
	}
	
	// LETTERS (Not exactly implemented as dictacted above because of my limited understanding of Regex)
	if (p.match(/[a-z]/)) {              // [verified] at least one lower case letter
		intScore += 1;
	}
	if (p.match(/[A-Z]/)) {              // [verified] at least one upper case letter
		intScore += 5;
	}
	// NUMBERS
	if (p.match(/\d/)) {             	// [verified] at least one number
		intScore += 5;
	}
	if (p.match(/.*\d.*\d.*\d/)) {            // [verified] at least three numbers
		intScore += 5;
	}
	
	// SPECIAL CHAR
	if (p.match(/[!,@,#,$,%,^,&,*,?,_,~]/)) {           // [verified] at least one special character
		intScore += 5;
	}
	// [verified] at least two special characters
	if (p.match(/.*[!,@,#,$,%,^,&,*,?,_,~].*[!,@,#,$,%,^,&,*,?,_,~]/)) {
		intScore += 5;
	}
	
	// COMBOS
	if (p.match(/(?=.*[a-z])(?=.*[A-Z])/)) {        // [verified] both upper and lower case
		intScore += 2;
	}
	if (p.match(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])/)) { // [verified] both letters and numbers
		intScore += 2;
	}
	// [verified] letters, numbers, and special characters
	if (p.match(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!,@,#,$,%,^,&,*,?,_,~])/)) {
		intScore += 2;
	}

	return intScore;

}