var DXagent = navigator.userAgent.toLowerCase();
var DXopera = (DXagent.indexOf("opera") > -1);
var DXsafari = DXagent.indexOf("safari") > -1;
var DXie = (DXagent.indexOf("msie") > -1 && !DXopera);
var DXns = (DXagent.indexOf("mozilla") > -1 || DXagent.indexOf("netscape") > -1 || DXagent.indexOf("firefox") > -1) && !DXsafari && !DXie && !DXopera;
var resizeTimeout = null;

function DXattachEventToElement(element, eventName, func) {
    if(element) {
        if(DXns || DXsafari)
            element.addEventListener(eventName, func, true);
        else {
            if(eventName.toLowerCase().indexOf("on") != 0)
                eventName = "on" + eventName;
            element.attachEvent(eventName, func);
        }
    }
}
function DXGetElement(id) {
    if(document.getElementById != null) {
        return document.getElementById(id);
    }
    if(document.all != null) {
        return document.all[id];
    }
    if(document.layers != null) {
        return document.layers[id];
    }
    return null;
}
function DXGetElementHeight(id) {
    var el = DXGetElement(id);
    if(el) {
        return parseInt(el.offsetHeight);
    }
    return 0;
}
function DXGetWindowHeight() {
    var height = 0;
    if(typeof (window.innerHeight) == 'number') {
        height = window.innerHeight;
    } else if(document.documentElement && document.documentElement.clientHeight) {
        height = document.documentElement.clientHeight;
    } else if(document.body && document.body.clientHeight) {
        height = document.body.clientHeight;
    }
    var margin = 0;
    if(document.body.currentStyle) {
        margin = parseInt(document.body.currentStyle.margin);
    }
    return parseInt(height) - (margin * 2);
}
function DXGetWindowWidth() {
    var width = 0;
    if(typeof (window.clientWidth) == 'number') {
        width = window.clientWidth;
    } else if(document.documentElement && document.documentElement.clientWidth) {
        width = document.documentElement.clientWidth;
    } else if(document.body && document.body.clientWidth) {
        width = document.body.clientWidth;
    }
    var margin = 0;
    if(document.body.currentStyle) {
        margin = parseInt(document.body.currentStyle.margin);
    }
    return parseInt(width) - (margin * 2);
}
function DXMoveFooter() {
    if(window.splitter) {
        SetupPane();
        var leftPaneContentHeight = DXGetElementHeight('LeftPane');
        var rightPaneContentHeight = DXGetElementHeight('ContentPane');
        var maxPaneContentHeight = (leftPaneContentHeight > rightPaneContentHeight ? leftPaneContentHeight : rightPaneContentHeight);
        var currentSplitterHeight = splitter.GetHeight();
        var bodyVisibleHeight = document.body.offsetHeight;
        var splitterHeightForSmallContent = DXGetWindowHeight() - (bodyVisibleHeight - currentSplitterHeight);
        var newSplitterHeight = splitterHeightForSmallContent > maxPaneContentHeight ? splitterHeightForSmallContent : maxPaneContentHeight;

        if(__aspxIE) {
            if(__aspxBrowserVersion != 7) {
                var size = newSplitterHeight - DXGetElementHeight("ContentPane") - 20;
                size = ((size >= 0) ? size : 0);
                var sizableFooter = document.getElementById("SizableFooter");
                if(sizableFooter) {
                    if(size != 0) {
                        sizableFooter.style.display = "block";
                        sizableFooter.style.height = size + "px";
                    }
                    else {
                        sizableFooter.style.display = "none";
                    }
                }
            }
            else {
                var contentPaneElement = document.getElementById("ContentPane")
                var widestElement = FindWidestChildElement(contentPaneElement);
                var width = widestElement.scrollWidth;
                var paddings = GetPaddings(widestElement, contentPaneElement.parentElement);
                var leftPaneWidth = 0;
                var leftPane = splitter.GetPaneByName("Left");
                if(leftPane) {
                    if(!leftPane.isSizePx) {
                        leftPaneWidth = leftPane.helper.GetContentContainerElement().style.width;
                    }
                    else {
                        leftPaneWidth = leftPane.GetSize()
                    }
                }
                var contentPane = splitter.GetPaneByName("Content");
                if(contentPane) {
                    contentPane.helper.GetContentContainerElement().style.width = width + paddings;
                    contentPaneElement.style.width = width + paddings;
                    if(currentSplitterHeight != newSplitterHeight) {
                        splitter.SetHeight(newSplitterHeight);
                    }
                }
                if(leftPane) {
                    leftPane.SetSize(leftPaneWidth);
                }
            }
        }
        else {
            splitter.SetHeight(newSplitterHeight);
        }
        SetupPane();
    }
    resizeTimeout = null;
}
function GetPaddings(childElement, rootElement) {
    var result = 0;
    while(childElement != rootElement) {
        if(childElement.currentStyle.paddingLeft != '') {
            result += parseInt(childElement.currentStyle.paddingLeft);
        }
        if(childElement.currentStyle.paddingRight != '') {
            result += parseInt(childElement.currentStyle.paddingRight);
        }
        childElement = childElement.parentElement;
    }
    return result;
}
function FindWidestChildElement(element) {
    var widestElement = element;
    var result = widestElement;
    for(var i = 0; i < element.children.length; i++) {
        widestElement = FindWidestChildElement(element.children[i]);
        if(widestElement.scrollWidth > result.scrollWidth) {
            result = widestElement;
        }
    }
    return result;
}
function SetupPane() {
    if(!(__aspxIE && __aspxBrowserVersion == 7)) {
        var contentPaneDiv = splitter.GetPaneByName("Content").helper.GetContentContainerElement();
        contentPaneDiv.style.width = "100%";
        contentPaneDiv.style.height = "100%";
        contentPaneDiv.style.display = "table";
        var leftPane = splitter.GetPaneByName("Left");
        if(leftPane) {
            var leftPaneDiv = leftPane.helper.GetContentContainerElement();
            leftPaneDiv.style.display = leftPaneDiv.style.display == "none" ? "none" : "table";
        }
    }
}

function SupressApplyingScrollPosition() {
    splitter.Init.AddHandler(SetupPane);
    splitter.PaneResizeCompleted.AddHandler(DXMoveFooter);
    GetXAFClientGlobalEvents().PageControlActiveTabChanged.AddHandler(DXMoveFooter);

    if(__aspxIE) {
        splitter.UpdatePanesVisible = function() { return; };
    }
    if(!(__aspxIE && __aspxBrowserVersion == 7)) {
        splitter.OnWindowResize = function() { return; };
        splitter.ApplyScrollPosition = function() { return; };
        var leftPane = splitter.GetPaneByName("Left");
        if(leftPane) {
            leftPane.ApplyScrollPosition = function() { return; };
        }
        var contentPane = splitter.GetPaneByName("Content");
        if(contentPane) {
            contentPane.ApplyScrollPosition = function() { return; };
        }
    }
}
function DXWindowOnResize(evt) {
    if(resizeTimeout) {
        window.clearTimeout(resizeTimeout);
    }
    resizeTimeout = window.setTimeout(DXMoveFooter, 100);
}
function CallbackHandler() {
    splitter.AdjustControl();
    DXWindowOnResize();
}
