$(document).ajaxStart(function () {
    sSessionSecond = 7000;
    //isloading.start();
    $('#jqxLoader').jqxLoader('open');
}).ajaxStop(function () {
    $('#jqxLoader').jqxLoader('close');
    //isloading.stop();
});

function addEvent(obj, evt, fn) {
    if (obj.addEventListener) {
        obj.addEventListener(evt, fn, false);
    } 
    else if (obj.attachEvent) {
        obj.attachEvent("on" + evt, fn);
    }
}


var menuModel = function (arrayList, rootId) {
    if (!(arrayList instanceof Array)) { return null; }
    var rootNodes = [];
    var traverse = function (nodes, item, index) {
        if (nodes instanceof Array) {
            return nodes.some(function (node) {
                if (node.menu_idx === item.group_idx) {
                    node.children = node.children || [];
                    return node.children.push(arrayList.splice(index, 1)[0]);
                }

                return traverse(node.children, item, index);
            });
        }
    };

    while (arrayList.length > 0) {
        arrayList.some(function (item, index) {
            if (item.group_idx === rootId) {
                return rootNodes.push(arrayList.splice(index, 1)[0]);
            }
            return traverse(rootNodes, item, index);
        });
    }

    return rootNodes;
};

var deptModel = function (arrayList, rootId) {
    if (!(arrayList instanceof Array)) { return null; }
    var rootNodes = [];
    var traverse = function (nodes, item, index) {
        if (nodes instanceof Array) {
            return nodes.some(function (node) {
                if (node.dept_idx === item.high_dept_idx) {
                    node.children = node.children || [];
                    return node.children.push(arrayList.splice(index, 1)[0]);
                }

                return traverse(node.children, item, index);
            });
        }
    };

    while (arrayList.length > 0) {
        arrayList.some(function (item, index) {
            if (item.high_dept_idx === rootId) {
                return rootNodes.push(arrayList.splice(index, 1)[0]);
            }
            return traverse(rootNodes, item, index);
        });
    }

    return rootNodes;
};
/*
var ReplytreeModel = function (arrayList, rootId, son, parent) {
    if (!(arrayList instanceof Array)) { return null; }
    var rootNodes = [];
    var traverse = function (nodes, item, index) {
        if (nodes instanceof Array) {
            return nodes.some(function (node) {
                if (node.dept_idx === item.bbs_reply_idx) {
                    node.children = node.children || [];
                    return node.children.push(arrayList.splice(index, 1)[0]);
                }

                return traverse(node.children, item, index);
            });
        }
    };

    while (arrayList.length > 0) {
        arrayList.some(function (item, index) {
            if (item[parent] === rootId) {
                return rootNodes.push(arrayList.splice(index, 1)[0]);
            }
            return traverse(rootNodes, item, index);
        });
    }

    return rootNodes;
};
*/

var treeModel = function (arrayList, rootId, son, parent) {
    if (!(arrayList instanceof Array)) { return null; }
    var rootNodes = [];
    var traverse = function (nodes, item, index) {
        if (nodes instanceof Array) {
            return nodes.some(function (node) {
                if (node[son] === item[parent]) {
                    node.children = node.children || [];
                    return node.children.push(arrayList.splice(index, 1)[0]);
                }

                return traverse(node.children, item, index);
            });
        }
    };

    while (arrayList.length > 0) {
        arrayList.some(function (item, index) {
            if (item[parent] === rootId) {
                return rootNodes.push(arrayList.splice(index, 1)[0]);
            }
            return traverse(rootNodes, item, index);
        });
    }

    return rootNodes;
};

var createDepartmentTreeUl = function (deptTreeModel) {
    var ul = document.createElement("ul");
    deptTreeModel.forEach(function (item, index, array) {

        var li = document.createElement("li");
        li.id = item.dept_idx;
        li.setAttribute("item-expanded", "true");

        var treeImg = document.createElement("img");
        treeImg.style.cssFloat = "left";
        treeImg.style.marginRight = "10px";
        treeImg.style.width = "18px";
        treeImg.src = item.dept_icon;

        var treeSpan = document.createElement("span");

        treeSpan.setAttribute("item-title", "true");
        treeSpan.innerText = item.dept_nm;

        li.appendChild(treeImg);
        li.appendChild(treeSpan);

        if (item.children !== undefined) {
            li.appendChild(createDepartmentTreeUl(item.children));
        }

        ul.appendChild(li);
    });

    return ul;
}

function isRightClick(event) {
    var rightclick;
    if (!event) event = window.event;
    if (event.which) rightclick = event.which === 3 ? true : false;
    else if (event.button) rightclick = event.button === 2 ? true : false;
    return rightclick;
}

/*
 -- 조회된 Array 내용을 JqxCombo 데이터 형식으로 변환
 data : Array
 valField : value 값이 될 FieldName
 txtField : text 값이 될 FieldName 
 isAll : 전제조회 유무 bool 값
*/
var ConvComboData = function (data, valField, txtField, isAll) {
    var retData = data.map(function (v, i, a) {
        var valueData = v[valField];
        var textData = v[txtField];
        return { value: valueData, text: textData };
    });

    if (data === undefined || data.length === 0) {
        var NoData = {
            value: null,
            text: "내용 없음"
        };

        retData.push(NoData);
    }

    if (isAll) {
        var allViewData = {
            value: "",
            text: ""
        };

        retData.unshift(allViewData);
    }

    return retData;
};

var ConvTextArray = function (data, txtField) {
    var retData = data.map(function (v, i, a) {
        var textData = v[txtField];
        
        return textData;
    });

    if (data === undefined || data.length === 0) {      
        retData.push("내용 없음");
    }

    return retData.reduce(function (a, b) { if (a.indexOf(b) < 0) a.push(b); return a; }, []);
};

function GetDate(num, type, tag) {
    if (tag === undefined || tag === null) {
        tag = "-";
    }

    var d = new Date(); //오늘날짜 데이터 세팅

    var retDate = new Date();

    if (type === "y") {
        retDate.setYear(d.getFullYear() + num);
    }
    else if (type === "m") {
        retDate.setMonth(d.getMonth() + num);
    }
    else if (type === "d") {
        retDate.setDate(d.getDay() + num);
    }

    var yy = retDate.getFullYear();
    var mm = (retDate.getMonth() + 1).toString().length === 1 ? "0" + (retDate.getMonth() + 1) : (retDate.getMonth() + 1);
    var dd = (retDate.getDate()).toString().length === 1 ? "0" + (retDate.getDate()) : (retDate.getDate());

    return yy + tag + mm + tag + dd;
}

function ConvModelToGoogleData(list, dataName) {
    var colLength = dataName.length;
    var retData = list.map(function (v, i, a) {
        var cellData = [];
        for (var x = 0; x < colLength; x++) {
            var data = {
                "v": v[dataName[x].toString()]
            };

            cellData.push(data);
        }


        return {
            "c": cellData
        }
    });

    return retData;
}

function getTextElementByColor(color) {
    if (color === 'transparent' || color.hex === "") {
        return $("<div style='text-shadow: none; position: relative; margin-bottom: 2px; margin-top: 2px;'height:20px;>transparent</div>");
    }
    var element = $("<div style='text-shadow: none; position: relative; margin-bottom: 2px; margin-top: 2px;height:20px;'>#" + color.hex + "</div>");
    var nThreshold = 105;
    var bgDelta = (color.r * 0.299) + (color.g * 0.587) + (color.b * 0.114);
    var foreColor = (255 - bgDelta < nThreshold) ? 'Black' : 'White';
    element.css('color', foreColor);
    element.css('background', "#" + color.hex);
    element.addClass('jqx-rc-all');
    return element;
}


function PrintGridView(source, gridObject, jsonResult) {
    source.localdata = jsonResult;

    var Adapter = new $.jqx.dataAdapter(source);
    Adapter.dataBind();

    gridObject.jqxGrid({ source: Adapter });
    Adapter = null;
}

function GetDownloadFile(file_idx, is_itf) {
    location.href = "/Common/FileDownload?link_file_idx=" + file_idx + "&is_itf=" + is_itf;
}

function GetDistDownloadFile(distfileidx) {
    location.href = "/Common/DistFileDownload?dist_file_idx=" + distfileidx;
}


function GetDistDownloadAllFile(dist_idx) {
    location.href = "/Common/DistAllFileDownload?dist_idx=" + dist_idx;
}


function GetPdfViewer(file_idx, is_itf) {
    var url = "/Common/PdfViewer?link_file_idx=" + file_idx + "&is_itf=" + is_itf;
    window.open(url, '_blank');
}   

function GetDistPdfViewer(distfileidx) {   
    var url = "/Common/DistPdfViewer?dist_file_idx=" + distfileidx;
    window.open(url, '_blank');
}


var renderWord = function (row, columnfield, value, defaulthtml, columnproperties) {
    return "<span class='renderText' title='" + value + "'>" + value + "</span>";
}