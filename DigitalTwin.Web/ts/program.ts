class Program {

    assemblyName: string;
    className: string;

    constructor(assemblyName: string, className: string) {
        this.assemblyName = assemblyName;
        this.className = className;
    }

    Run(canvasId: string) {
        this.invoke("Run", canvasId);
    }

    UpdateCanvasSize(canvasId: string) {
        this.invoke("UpdateCanvasSize", canvasId);

    }

    UpdateData(command: any) {
        this.invoke("UpdateData", command);
    }

    invoke(methodName: string, ...args: any[]) {
        window.BINDING.call_static_method(
            `[${this.assemblyName}] ${this.className}:${methodName}`,
            args
        );
    }
}