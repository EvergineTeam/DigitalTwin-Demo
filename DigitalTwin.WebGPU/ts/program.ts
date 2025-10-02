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
    return DotNet.invokeMethod(
      `${this.assemblyName}`,
      `${this.className}:${methodName}`,
      ...args);
  }

  invokeAsync<T>(methodName: string, ...args: any[]): Promise<T> {
    return DotNet.invokeMethodAsync(
      `${this.assemblyName}`,
      `${this.className}:${methodName}`,
      ...args);
  }
}
