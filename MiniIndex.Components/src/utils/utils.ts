export function fixCDN(thumbnail: string) {
  if (thumbnail.includes('miniindex.blob.core.windows.net')) {
    return thumbnail.replace('miniindex.blob.core.windows.net', 'miniindexblobakamai.azureedge.net');
  } else {
    return thumbnail;
  }
}
