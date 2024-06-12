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

  Destroy(canvasId: string) {
    this.invoke("Destroy", canvasId);
  }

  UpdateCanvasSize(canvasId: string) {
    this.invoke("UpdateCanvasSize", canvasId);
  }

  invoke(methodName: string, ...args: any[]) {
    DotNet.invokeMethod(
        `${this.assemblyName}`,
        `${this.className}:${methodName}`,
        ...args
    );
  }
}
