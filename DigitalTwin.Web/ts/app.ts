class App {

    static mainCanvasId: string;
    program: Program;
    module: EvergineModule;
    static requestAnimationFrameCallback: (d:any) => void;

    constructor(canvasId: string, assemblyName: string, className: string, module: EvergineModule) {
        App.mainCanvasId = canvasId;
        this.program = new Program(assemblyName, className);
        this.module = module;
        let canvas = document.getElementById(App.mainCanvasId) as HTMLCanvasElement;
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

        this.module.canvas = canvas;

        this.updateCanvasSize();
        window.addEventListener("resize", this.resizeAppSize.bind(this));
    }
    run() {
        this.program.Run(App.mainCanvasId)
    }
    resizeAppSize() {
        this.updateCanvasSize();
        this.program.UpdateCanvasSize(App.mainCanvasId);
    }

    static openDialog(){
        $("#dialog").dialog("open");
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

    static warnUnsupportedBrowser () {
        App.hideSplash();
        let unsupportedBrowserErrorMessage = "Browser not supported";

        // Supposing Canvas2D is available
        let evergineCanvas = document.getElementById(App.mainCanvasId) as HTMLCanvasElement;
        evergineCanvas.setAttribute('style', 'image-rendering: crisp-edges');
        let context = evergineCanvas.getContext("2d");
        context.fillStyle = "black";
        context.font = "16pt Arial";
        context.fillText(unsupportedBrowserErrorMessage, 4, 20);
    }
}