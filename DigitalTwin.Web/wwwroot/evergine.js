var Module = typeof Module !== "undefined" ? Module : {};

Module["locateFile"] = function (base) {
    return `evergine/${base}`;
};

Module["setProgress"] = function (progress) {
    let percentage = Math.round(progress);

    let loadingBarBar = document.getElementById("loading-bar-percentage");
    loadingBarBar.style.width = percentage + "%";

    let loadingBar = document.getElementById("loading-bar");
    if (percentage === 100) {
        loadingBar.classList.add("progress-infinite");
    }
};

let App = {
    mainCanvasId: undefined,
    configure: function (canvasId, assemblyName, className) {
        this.mainCanvasId = canvasId;
        this.Program.assemblyName = assemblyName;
        this.Program.className = className;

        let canvas = document.getElementById(this.mainCanvasId);
        if (!canvas) {
            alert("Initialization failed: WebGL canvas element not found.");
        }

        // As a default initial behavior, pop up an alert when webgl context is lost. To make your
        // application robust, you may want to override this behavior before shipping!
        // See http://www.khronos.org/registry/webgl/specs/latest/1.0/#5.15.2
        canvas.addEventListener(
            "webglcontextlost",
            function (e) {
                alert("WebGL context lost. You will need to reload the page.");
                e.preventDefault();
            },
            false
        );

        Module.canvas = canvas;

        this.updateCanvasSize();
        window.addEventListener("resize", this.resizeAppSize.bind(this));
    },
    run: function () {
        this.Program.Run(this.mainCanvasId);
    },
    resizeAppSize: function () {
        this.updateCanvasSize();
        this.Program.UpdateCanvasSize(this.mainCanvasId);
    },
    hideSplash: function () {
        let splash = document.getElementById("splash");
        fadeOut(splash, 400, (el) => {
            el.remove();
        });
    },
    updateCanvasSize: function () {
        let devicePixelRatio = window.devicePixelRatio || 1;

        Module.canvas.style.width = window.innerWidth + "px";
        Module.canvas.style.height = window.innerHeight + "px";
        Module.canvas.width = window.innerWidth * devicePixelRatio;
        Module.canvas.height = window.innerHeight * devicePixelRatio;
    },
    warnUnsupportedBrowser: function () {
        App.hideSplash();
        let unsupportedBrowserErrorMessage = "Browser not supported";

        // Supposing Canvas2D is available
        let evergineCanvas = document.getElementById(App.mainCanvasId);
        evergineCanvas.style = "image-rendering: crisp-edges";
        let context = evergineCanvas.getContext("2d");
        context.fillStyle = "black";
        context.font = "16pt Arial";
        context.fillText(unsupportedBrowserErrorMessage, 4, 20);
    },
    openDialog: () => {
        $("#dialog").dialog("open");
    },
    Program: {
        assemblyName: undefined,
        className: undefined,
        Run: function (canvasId) {
            this.invoke("Run", [canvasId]);
        },
        UpdateCanvasSize: function (canvasId) {
            this.invoke("UpdateCanvasSize", [canvasId]);
        },
        UpdateData: function (command) {
            this.invoke('UpdateData', [command]);
        },
        invoke: function (methodName, args) {
            BINDING.call_static_method(
                `[${this.assemblyName}] ${this.className}:${methodName}`,
                args
            );
        },
    },
};

let isWebGL2Supported = function () {
    // Some browsers (e.g. Safari on macOS) pass this test, although its implementation it's not fully functional
    var elem = document.createElement("canvas");
    return !!(elem.getContext && elem.getContext("webgl2"));
};

////// Will do the heavy lifting only on supported browsers
////if (isWebGL2Supported()) {
////    // Add script references
////    var contentTag = document.createElement('script');
////    contentTag.src = "evergine/content.js";
////    contentTag.defer = true;
////    contentTag.async = false;

////    var dotnetTag = document.createElement('script');
////    dotnetTag.src = "evergine/dotnet.js";
////    dotnetTag.defer = true;
////    dotnetTag.async = false;

////    document.head.appendChild(contentTag);
////    document.head.appendChild(dotnetTag);
////} else {
////    console.log("WebGL2 is NOT supported on this browser");
////    $(document).ready(App.warnUnsupportedBrowser);
////}

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
    for (i = 0; i < changed.length; i++) {
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

function _evergine_ready() {
    App.hideSplash();
}
;

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