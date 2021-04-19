export const PHONE_PATTERN = '^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$';

export function exportFile(blob: Blob, fileName: string) {
  const anchor = document.createElement('a');
  const url = window.URL.createObjectURL(blob);

  anchor.href = url;
  anchor.download = fileName + '.csv';
  anchor.click();
  window.URL.revokeObjectURL(url);
  anchor.remove();
}
