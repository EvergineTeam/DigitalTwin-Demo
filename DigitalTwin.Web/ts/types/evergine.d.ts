declare global {
    var areAllAssetsLoaded: any;
    var evergineSetProgressCallback: (progress: number) => void;
    var Blazor: any;
    var DotNet: any;
    var $: any;
    interface Window {
        (src: any, event: any): void;
        BINDING: {
            call_static_method: (method: string, args?: unknown[]) => unknown;
        };
        EGL: any;
    }
    interface Module {
        canvasId: HTMLCanvasElement;
        locateFile: (base: string) => string;
        setProgress: (progress: number) => void;

    }
}

export {};