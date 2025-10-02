class App {
  containerId: string;
  canvasId: string;
  program: Program;
  module: EvergineModule;
  static requestAnimationFrameCallback: (d: any) => void;

  constructor(assemblyName: string, className: string, module: EvergineModule) {
    this.program = new Program(assemblyName, className);
    this.module = module;
  }

  startEvergine(
    containerId = "evergine-canvas-container",
    canvasId = "evergine-canvas"
  ) {
    this.containerId = containerId;
    this.canvasId = canvasId;

    let container = document.getElementById(this.containerId) as HTMLDivElement;
    if (!container) {
      alert(
        "Initialization failed: canvas container '" +
          this.containerId +
          "' not found."
      );
    }

    let canvas = document.createElement("canvas") as HTMLCanvasElement;
    canvas.id = this.canvasId;
    canvas.tabIndex = 1;
    canvas.addEventListener("contextmenu", (e) => e.preventDefault());

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
    container.appendChild(canvas);

    this.updateCanvasSize();
    window.addEventListener("resize", this.resizeAppSize.bind(this));

    this.waitAndRun();
  }

  destroyEvergine() {
    let container = document.getElementById(this.containerId) as HTMLDivElement;
    container.replaceChildren();
    this.program.Destroy(this.canvasId);
  }

  waitAndRun() {
    startAssetsDownloadIfNeeded();
    if (areAllAssetsLoaded()) {
      console.log("Running...");
      this.program.Run(this.canvasId);
    } else {
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
    let evergineCanvas = document.getElementById(
      this.canvasId
    ) as HTMLCanvasElement;
    evergineCanvas.setAttribute("style", "image-rendering: crisp-edges");
    let context = evergineCanvas.getContext("2d");
    context.fillStyle = "black";
    context.font = "16pt Arial";
    context.fillText(unsupportedBrowserErrorMessage, 4, 20);
  }
}
