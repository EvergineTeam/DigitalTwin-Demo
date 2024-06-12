class App {
    constructor(assemblyName, className, module) {
        this.program = new Program(assemblyName, className);
        this.module = module;
    }
    startEvergine(containerId = "evergine-canvas-container", canvasId = "evergine-canvas") {
        this.containerId = containerId;
        this.canvasId = canvasId;
        let container = document.getElementById(this.containerId);
        if (!container) {
            alert("Initialization failed: canvas container '" +
                this.containerId +
                "' not found.");
        }
        let canvas = document.createElement("canvas");
        canvas.id = this.canvasId;
        canvas.tabIndex = 1;
        canvas.addEventListener("contextmenu", (e) => e.preventDefault());
        // As a default initial behavior, pop up an alert when webgl context is lost. To make your
        // application robust, you may want to override this behavior before shipping!
        // See http://www.khronos.org/registry/webgl/specs/latest/1.0/#5.15.2
        canvas.addEventListener("webglcontextlost", function (e) {
            alert("WebGL context lost. You will need to reload the page.");
            e.preventDefault();
        }, false);
        this.module.canvas = canvas;
        container.appendChild(canvas);
        this.updateCanvasSize();
        window.addEventListener("resize", this.resizeAppSize.bind(this));
        this.waitAndRun();
    }
    destroyEvergine() {
        let container = document.getElementById(this.containerId);
        container.replaceChildren();
        this.program.Destroy(this.canvasId);
    }
    waitAndRun() {
        if (areAllAssetsLoaded()) {
            console.log("Running...");
            this.program.Run(this.canvasId);
        }
        else {
            setTimeout(this.waitAndRun.bind(this), 100);
        }
    }
    resizeAppSize() {
        this.updateCanvasSize();
        this.program.UpdateCanvasSize(this.canvasId);
    }
    static hideSplash() {
        let splash = document.getElementById("splash");
        fadeOut(splash, 400, (el) => {
            el.remove();
        });
    }
    updateCanvasSize() {
        let devicePixelRatio = window.devicePixelRatio || 1;
        this.module.canvas.style.width = window.innerWidth + "px";
        this.module.canvas.style.height = window.innerHeight + "px";
        this.module.canvas.width = window.innerWidth * devicePixelRatio;
        this.module.canvas.height = window.innerHeight * devicePixelRatio;
    }
    warnUnsupportedBrowser() {
        App.hideSplash();
        let unsupportedBrowserErrorMessage = "Browser not supported";
        // Supposing Canvas2D is available
        let evergineCanvas = document.getElementById(this.canvasId);
        evergineCanvas.setAttribute("style", "image-rendering: crisp-edges");
        let context = evergineCanvas.getContext("2d");
        context.fillStyle = "black";
        context.font = "16pt Arial";
        context.fillText(unsupportedBrowserErrorMessage, 4, 20);
    }
    static openDialog() {
        $("#dialog").dialog("open");
    }
}
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
function _evergine_addSimpleEventListener(src, eventName, target, listenerName, options) {
    src.addEventListener(eventName, (e) => {
        //let eref = DotNet.createJSObjectReference(e);
        target.invokeMethod(listenerName, e.type);
        //DotNet.disposeJSObjectReference(eref);
    }, options);
}
function _evergine_addEventListener(src, eventName, target, listenerName, options) {
    src.addEventListener(eventName, (e) => {
        let eref = DotNet.createJSObjectReference(e);
        target.invokeMethod(listenerName, e.type, eref);
        DotNet.disposeJSObjectReference(eref);
    }, options);
}
function _evergine_addEventCustomListener(src, eventName, target, listenerName, summaryFn, options) {
    src.addEventListener(eventName, (e) => {
        let output = window[summaryFn](src, e);
        target.invokeMethod(listenerName, e.type, output);
    }, options);
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
function _evergine_setRequestAnimationFrameCallback(targetInstance, callbackName) {
    if (callbackName) {
        App.requestAnimationFrameCallback = function (d) {
            targetInstance.invokeMethod(callbackName, d);
            if (App.requestAnimationFrameCallback) {
                window.requestAnimationFrame(App.requestAnimationFrameCallback);
            }
        };
        window.requestAnimationFrame(App.requestAnimationFrameCallback);
    }
    else {
        App.requestAnimationFrameCallback = undefined;
    }
}
function fadeIn(elem, ms, cbk) {
    if (!elem)
        return;
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
    }
    else {
        elem.style.opacity = 1;
        elem.style.filter = "alpha(opacity=1)";
    }
}
function fadeOut(elem, ms, cbk) {
    if (!elem)
        return;
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
    }
    else {
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
function _evergine_EGL(contextId, canvasId) {
    if (contextId && canvasId) {
        const canvas = document.getElementById(canvasId);
        canvas.getContext(contextId, { antialias: true, preserveDrawingBuffer: true });
    }
    else if (window.EGL) {
        window.EGL.contextAttributes.antialias = true;
        window.EGL.contextAttributes.preserveDrawingBuffer = true;
    }
    else {
        console.log("_evergine_EGL cannot set context properties");
    }
}
class EvergineModule {
    get canvas() {
        if (Blazor && Blazor.runtime && Blazor.runtime.Module) {
            return Blazor.runtime.Module.canvas;
        }
        else {
            return globalThis.Module.canvas;
        }
    }
    set canvas(canvas) {
        if (Blazor && Blazor.runtime && Blazor.runtime.Module) {
            Blazor.runtime.Module.canvas = canvas;
        }
        else {
            globalThis.Module.canvas = canvas;
        }
    }
    constructor(module) {
        Object.assign(this);
        globalThis.evergineSetProgressCallback = this.setProgress;
    }
    locateFile(base) {
        return `evergine/${base}`;
    }
    setProgress(progress) {
        let percentage = Math.round(progress);
        let loadingBar = document.getElementById("loading-bar-percentage");
        loadingBar.style.width = percentage + "%";
        if (percentage === 100) {
            loadingBar.classList.add("progress-infinite");
        }
    }
}
class Program {
    constructor(assemblyName, className) {
        this.assemblyName = assemblyName;
        this.className = className;
    }
    Run(canvasId) {
        this.invoke("Run", canvasId);
    }
    Destroy(canvasId) {
        this.invoke("Destroy", canvasId);
    }
    UpdateCanvasSize(canvasId) {
        this.invoke("UpdateCanvasSize", canvasId);
    }
    invoke(methodName, ...args) {
        DotNet.invokeMethod(`${this.assemblyName}`, `${this.className}:${methodName}`, ...args);
    }
}
