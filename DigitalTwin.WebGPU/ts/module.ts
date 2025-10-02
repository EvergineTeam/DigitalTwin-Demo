
class EvergineModule {

    get canvas(): HTMLCanvasElement {
        if (Blazor && Blazor.runtime && Blazor.runtime.Module) {
            return Blazor.runtime.Module.canvas;
        } else {
            return globalThis.Module.canvas;
        }
    }

    set canvas(canvas: HTMLCanvasElement) {
        if (Blazor && Blazor.runtime && Blazor.runtime.Module) {
            Blazor.runtime.Module.canvas = canvas;
        } else {
            globalThis.Module.canvas = canvas;
        }
    }

    constructor(module: object) {
        Object.assign(this);
        globalThis.evergineSetProgressCallback = this.setProgress;
    }

    locateFile(base: string) {
        return `evergine/${base}`;
    }

    setProgress(progress: number) {
        let percentage = Math.round(progress);

        let loadingBar = document.getElementById("loading-bar-percentage");
        loadingBar.style.width = percentage + "%";
      
        if (percentage === 100) {
          loadingBar.classList.add("progress-infinite");
        }
    }
}