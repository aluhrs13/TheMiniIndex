export function perfMark(markName: string) {
  performance.mark(markName);
}

export function perfMeasure(
  measureName: string,
  startMark: string,
  endMark: string
) {
  performance.measure(measureName, startMark, endMark);
}

export function logError(errorDetails: string) {
  console.error(errorDetails);
}

export function logMsg(text: string) {
  console.log(text);
}
