
let isWebGL2Supported = function () {
    // Some browsers (e.g. Safari on macOS) pass this test, although its implementation it's not fully functional
    var elem = document.createElement("canvas");
    return !!(elem.getContext && elem.getContext("webgl2"));
};


// Helper functions
function _evergine_getGlobalObject() {
    return window;
}

function _evergine_getObjectProperty(obj, property) {
    return obj[property];
}

function _evergine_setObjectProperty(obj, property, value) {
    obj[property] = value;
}

function _evergine_addSimpleEventListener(
    src,
    eventName,
    target,
    listenerName,
    options
) {
    src.addEventListener(
        eventName,
        (e) => {
            //let eref = DotNet.createJSObjectReference(e);
            target.invokeMethod(listenerName, e.type);
            //DotNet.disposeJSObjectReference(eref);
        },
        options
    );
}

function _evergine_addEventListener(
    src,
    eventName,
    target,
    listenerName,
    options
) {
    src.addEventListener(
        eventName,
        (e) => {
            let eref = DotNet.createJSObjectReference(e);
            target.invokeMethod(listenerName, e.type, eref);
            DotNet.disposeJSObjectReference(eref);
        },
        options
    );
}

function _evergine_addEventCustomListener(
    src,
    eventName,
    target,
    listenerName,
    summaryFn,
    options
) {
    src.addEventListener(
        eventName,
        (e) => {
            let output = window[summaryFn](src, e);
            target.invokeMethod(listenerName, e.type, output);
        },
        options
    );
}

function _evergine_getPointSummary(src, event) {
    return [
        Math.round(event.clientX),
        Math.round(event.clientY),
        Math.round(src.getBoundingClientRect().left),
        Math.round(src.getBoundingClientRect().top),
    ].join();
}

function _evergine_getTouchSummary(src, event) {
    event.preventDefault();
    let changed = event.changedTouches;
    let summ = [];
    for (let i = 0; i < changed.length; i++) {
        let touch = changed[i];
        summ.push(touch.identifier + "," + _evergine_getPointSummary(src, touch));
    }
    return summ.join(";");
}

function _evergine_getMouseButtonSummary(src, event) {
    return [event.button].join();
}

function _evergine_getWheelSummary(src, event) {
    // Hack for firefox double event bug
    let deltaX = "deltaX" in event ? event.deltaX : 0;
    let deltaY = "deltaY" in event ? event.deltaY : 0;
    return [deltaX, deltaY].join();
}

function _evergine_getKeySummary(src, event) {
    return [event.code, event.key].join();
}

function _evergine_removeEventListener(src, eventName, options) {
    src.addEventListener(eventName, null, options);
}

function _evergine_setRequestAnimationFrameCallback(
    targetInstance,
    callbackName
) {
    if (callbackName) {
        App.requestAnimationFrameCallback = function (d) {
            targetInstance.invokeMethod(callbackName, d);
            if (App.requestAnimationFrameCallback) {
                window.requestAnimationFrame(App.requestAnimationFrameCallback);
            }
        };
        window.requestAnimationFrame(App.requestAnimationFrameCallback);
    } else {
        App.requestAnimationFrameCallback = undefined;
    }
}

function fadeIn(elem, ms, cbk) {
    if (!elem) return;

    elem.style.opacity = 0;
    elem.style.filter = "alpha(opacity=0)";
    elem.style.display = "inline-block";
    elem.style.visibility = "visible";

    if (ms) {
        var opacity = 0;
        var timer = setInterval(function () {
            opacity += 50 / ms;
            if (opacity >= 1) {
                clearInterval(timer);
                opacity = 1;
                cbk(elem);
            }
            elem.style.opacity = opacity;
            elem.style.filter = "alpha(opacity=" + opacity * 100 + ")";
        }, 50);
    } else {
        elem.style.opacity = 1;
        elem.style.filter = "alpha(opacity=1)";
    }
}


function fadeOut(elem, ms, cbk) {
    if (!elem) return;

    if (ms) {
        var opacity = 1;
        var timer = setInterval(function () {
            opacity -= 50 / ms;
            if (opacity <= 0) {
                clearInterval(timer);
                opacity = 0;
                elem.style.display = "none";
                elem.style.visibility = "hidden";
                cbk(elem);
            }
            elem.style.opacity = opacity;
            elem.style.filter = "alpha(opacity=" + opacity * 100 + ")";
        }, 50);
    } else {
        elem.style.opacity = 0;
        elem.style.filter = "alpha(opacity=0)";
        elem.style.display = "none";
        elem.style.visibility = "hidden";
    }
}

function _evergine_ready() {
    App.hideSplash();
}

let MathHelper = {
    getRandomNumber: function (min, max) {
        return Math.random() * (max - min) + min;
    }
};

function _onEvent(id) {
    console.log(id);
    var trackerName = document.getElementById('trackerName');
    trackerName.innerHTML = id;
    App.openDialog();
}

function _onTrackerAngleUpdated(angle) {
    console.log(angle);
    var trackerName = document.getElementById('trackerPosition');
    trackerName.innerHTML = angle;

    for (var i = 1; i <= 3; i++) {
        var element = document.getElementById('trackerVoltage' + i);
        element.innerHTML = MathHelper.getRandomNumber(220, 245);
    }

    for (var i = 1; i <= 3; i++) {
        var element = document.getElementById('trackerIntensity' + i);
        element.innerHTML = MathHelper.getRandomNumber(275, 330);
    }
}