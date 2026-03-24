import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CsvExportService {
  /**
   * Builds a CSV string from headers + rows and triggers a file download.
   * The BOM (\uFEFF) ensures correct UTF-8 rendering in Microsoft Excel.
   */
  download(headers: string[], rows: (string | number)[][], filename: string): void {
    const csv = [
      headers.join(','),
      ...rows.map(r => r.join(','))
    ].join('\n');

    const blob = new Blob(['\uFEFF' + csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    URL.revokeObjectURL(url);
  }
}
