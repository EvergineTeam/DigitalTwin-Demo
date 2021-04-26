var Module = typeof Module !== 'undefined' ? Module : {};

Module['locateFile'] = function (base) {
    return `waveengine/${base}`;
}

Module['setProgress'] = function (loadedBytes, totalBytes) {
    let percentage = Math.round((loadedBytes / totalBytes) * 100);
    $('#loading-bar').children().css('width', percentage + '%');

    if (percentage === 100) {
        $('#loading-bar').addClass('progress-infinite');
    }
};

let App = {
    mainCanvasId: undefined,
    configure: function (canvasId, assemblyName, className) {
        this.mainCanvasId = canvasId;
        this.Program.assemblyName = assemblyName;
        this.Program.className = className;
    },
    init: function () {
        let canvas = document.getElementById(this.mainCanvasId);
        if (!canvas) {
            alert('Initialization failed: WebGL canvas element not found.');
        }

        // As a default initial behavior, pop up an alert when webgl context is lost. To make your
        // application robust, you may want to override this behavior before shipping!
        // See http://www.khronos.org/registry/webgl/specs/latest/1.0/#5.15.2
        canvas.addEventListener('webglcontextlost', function (e) {
            alert('WebGL context lost. You will need to reload the page.');
            e.preventDefault();
        }, false);

        Module.canvas = canvas;

        this.updateCanvasSize();
        this.Program.Main(this.mainCanvasId);
        window.addEventListener('resize', this.resizeAppSize.bind(this));
    },
    resizeAppSize: function () {
        this.updateCanvasSize();
        this.Program.UpdateCanvasSize(this.mainCanvasId);
    },
    hideSplash: function () {
        $('#splash').fadeOut(function () { $(this).remove(); });
    },
    updateCanvasSize: function () {
        let devicePixelRatio = window.devicePixelRatio || 1;
        $(`#${this.mainCanvasId}`)
            .css('width', window.innerWidth + 'px')
            .css('height', window.innerHeight + 'px')
            .prop('width', window.innerWidth * devicePixelRatio)
            .prop('height', window.innerHeight * devicePixelRatio);
    },
    warnUnsupportedBrowser: function () {
        App.hideSplash();
        let unsupportedBrowserErrorMessage = "Browser not supported";

        // Supposing Canvas2D is available
        let waveEngineCanvas = document.getElementById(App.mainCanvasId);
        waveEngineCanvas.style = "image-rendering: crisp-edges";
        let context = waveEngineCanvas.getContext("2d");
        context.fillStyle = "black";
        context.font = "16pt Arial";
        context.fillText(unsupportedBrowserErrorMessage, 4, 20);
    },
    Program: {
        assemblyName: undefined,
        className: undefined,
        Main: function (canvasId) {
            this.invoke('Main', [canvasId]);
        },
        UpdateCanvasSize: function (canvasId) {
            this.invoke('UpdateCanvasSize', [canvasId]);
        },
        UpdateData: function (command) {
            this.invoke('UpdateData', [command]);
        },
        invoke: function (methodName, args) {
            BINDING.call_static_method(`[${this.assemblyName}] ${this.className}:${methodName}`, args);
        }
    },
    openDialog: () => {
        $("#dialog").dialog("open");
    }
};

let WaveEngine = {
    init: function () {
        App.hideSplash();
    },
    onEvent: function (id) {
        console.log(id);
        var trackerName = document.getElementById('trackerName');
        trackerName.innerHTML = id;
        App.openDialog();
    },
    onTrackerAngleUpdated: function (angle) {
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
};

let MathHelper = {
    getRandomNumber: function (min, max) {
        return Math.random() * (max - min) + min;
    }
};

let isWebGL2Supported = function () {
    // Some browsers (e.g. Safari on macOS) pass this test, although its implementation it's not fully functional
    var elem = document.createElement('canvas');
    return !!(elem.getContext && elem.getContext('webgl2'));
}

// Will do the heavy lifting only on supported browsers
if (isWebGL2Supported()) {
    // Add script references
    var contentTag = document.createElement('script');
    contentTag.src = "waveengine/content.js";
    contentTag.defer = true;
    contentTag.async = false;

    var dotnetTag = document.createElement('script');
    dotnetTag.src = "waveengine/dotnet.js";
    dotnetTag.defer = true;
    dotnetTag.async = false;

    document.head.appendChild(contentTag);
    document.head.appendChild(dotnetTag);
} else {
    console.log("WebGL2 is NOT supported on this browser");
    $(document).ready(App.warnUnsupportedBrowser);
}
