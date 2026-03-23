import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class FileUploadService {
  private readonly base = `${environment.apiUrl}/files`;
  private readonly baseUrl = environment.baseUrl;
  constructor(private http: HttpClient) {}

  upload(files: File[]): Observable<any[]> {
    const formData = new FormData();
    files.forEach(f => formData.append('files', f, f.name));
    return this.http.post<any[]>(`${this.base}/upload`, formData);
  }

  /** Upload image files to the dedicated photos folder. */
  uploadPhoto(files: File[]): Observable<any[]> {
    const formData = new FormData();
    files.forEach(f => formData.append('files', f, f.name));
    return this.http.post<any[]>(`${this.base}/photos/upload`, formData);
  }

  getUrl(fileName: string): string {
    return `${environment.apiUrl}/files/${fileName}`;
  }

  /** Build the full URL for a photo stored in the photos folder. */
  getPhotoUrl(relativeUrl: string): string {
    if (!relativeUrl) return '';
    if (relativeUrl.startsWith('http')) return relativeUrl;
    return `${this.baseUrl}${relativeUrl}`;
  }

  delete(fileName: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${fileName}`);
  }

  deletePhoto(fileName: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/photos/${fileName}`);
  }
}
