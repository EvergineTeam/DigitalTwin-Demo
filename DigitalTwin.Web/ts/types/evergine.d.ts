declare global {

    var DotNet: any;
    var $: any;
    interface Window {
        (src: any, event: any): void;
        BINDING: {
            call_static_method: (method: string, args?: unknown[]) => unknown;
        };
    }
    interface Module {
        canvasId: HTMLCanvasElement;
        locateFile: (base: string) => string;
        setProgress: (progress: number) => void;

    }
}

export { };