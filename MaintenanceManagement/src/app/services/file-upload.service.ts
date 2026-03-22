import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class FileUploadService {
  private readonly base = `${environment.apiUrl}/files`;
  constructor(private http: HttpClient) {}

  upload(files: File[]): Observable<any[]> {
    const formData = new FormData();
    files.forEach(f => formData.append('files', f, f.name));
    return this.http.post<any[]>(`${this.base}/upload`, formData);
  }

  getUrl(fileName: string): string {
    return `${environment.apiUrl}/files/${fileName}`;
  }

  delete(fileName: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${fileName}`);
  }
}
