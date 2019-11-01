String.format = function () {
    var theString = arguments[0];

    for (var i = 1; i < arguments.length; i++) {
        var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
        theString = theString.replace(regEx, arguments[i]);
    }

    return theString;
}

String.prototype.trim = function () {
    return this.replace(/(^\s*)|(\s*$)/gi, "");
}
 
String.prototype.isNumber = function () {
    var regNumber = /^[0-9]*$/;
    if (!regNumber.test(this)) {
        return false;
    }
    return true;
}

String.prototype.replaceAll = function (str1, str2) {
    var temp_str = "";
    if (this.trim() != "" && str1 != str2) {
        temp_str = this.trim();
        while (temp_str.indexOf(str1) > -1) {
            temp_str = temp_str.replace(str1, str2);
        }
    }
    return temp_str;
};