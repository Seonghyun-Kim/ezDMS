function GetCommData(url, args, callBackFunc) {
    var retData = null;
    var param = args;

    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "post",
        dataType: 'json',
        url: url,
        data: JSON.stringify(param),
        success: function (res) {
            if (res.isError) {
                alert(res.resultMessage);
                console.log(res.resultDescription); 
                return;
            }

            retData = res;
        }, error: function (res) {
            console.log(res.responseText);
        }
    }).then(function () {
        callBackFunc(retData);
    });
}

var useFlag = [
    { value: "", text: "전체" },
    { value: "Y", text: "사용 중" },
    { value: "N", text: "사용 안함" }];

var overFlag = [
    { value: "Y", text: "외부작성" },
    { value: "N", text: "내부작성" }];

var userGroupType = [
    { value: "", text: "" },
    { value: "10", text: "협력사" },
    { value: "20", text: "사내" }];

var waterMarkPos = [
    { value: "", text: "" },
    { value: "TR", text: "상단 우측" },
    { value: "TC", text: "상단 중앙" },
    { value: "TL", text: "상단 좌측" },
    { value: "CC", text: "중앙" },
    { value: "BR", text: "하단 우측" },
    { value: "BC", text: "하단 중앙" },
    { value: "BL", text: "하단 좌측" }];


var latestFlag = [
    { value: "", text: "전체" },
    { value: "Y", text: "최신" }];

var DateFormat = {
    culture: 'ko-KR',
    formatString: 'yyyy-MM-dd',
    width: 140,
    clearString: 'Clear'
};




var roleGroup = [];
var positionGroup = [];
var eoTypeGroup = [];
var distStatusGroup = [];