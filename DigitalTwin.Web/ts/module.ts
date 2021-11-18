
class EvergineModule {

    canvas: HTMLCanvasElement;

    constructor(module: object) {
        Object.assign(this);
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