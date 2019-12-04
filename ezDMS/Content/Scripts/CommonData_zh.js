
var jqxtheme = "ispark";

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
    { value: "", text: "전체_zh" },
    { value: "Y", text: "사용 중_zh" },
    { value: "N", text: "사용 안함_zh" }];

var overFlag = [
    { value: "Y", text: "외부작성_zh" },
    { value: "N", text: "내부작성_zh" }];

var userGroupType = [
    { value: "", text: "" },
    { value: "10", text: "협력사_zh" },
    { value: "20", text: "사내_zh" }];

var waterMarkPos = [
    { value: "", text: "" },
    { value: "TR", text: "상단우측_zh" }, //상단우측
    { value: "TC", text: "상단 중앙_zh" },
    { value: "TL", text: "상단 좌측_zh" },
    { value: "CC", text: "중앙_zh" },
    { value: "BR", text: "하단 우측_zh" },
    { value: "BC", text: "하단 중앙_zh" },
    { value: "BL", text: "하단 좌측_zh" }];


var latestFlag = [
    { value: "", text: "전체_zh" },
    { value: "Y", text: "최신_zh" }];

var DateFormat = {
    culture: 'zh-cn',
    formatString: 'yyyy-MM-dd',
    width: 140,
    clearString: 'Clear'
};

var roleGroup = [];
var positionGroup = [];
var eoTypeGroup = [];
var distStatusGroup = [];