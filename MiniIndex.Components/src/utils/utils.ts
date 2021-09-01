export function format(first: string, middle: string, last: string): string {
  return (first || '') + (middle ? ` ${middle}` : '') + (last ? ` ${last}` : '');
}

export function fixCDN(thumbnail: string) {
  if (thumbnail.includes('miniindex.blob.core.windows.net')) {
    return thumbnail.replace('miniindex.blob.core.windows.net', 'miniindexblobakamai.azureedge.net');
  } else {
    return thumbnail;
  }
}
