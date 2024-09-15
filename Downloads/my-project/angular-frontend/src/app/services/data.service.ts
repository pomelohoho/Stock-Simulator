import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private apiUrl = '/api/tasks';

  constructor(private http: HttpClient) { }

  getTasks(): Observable<string[]> {
    return this.http.get<string[]>(this.apiUrl);
  }

  addTask(task: string): Observable<string> {
    return this.http.post<string>(this.apiUrl, task);
  }

  editTask(index: number, updatedTask: string): Observable<string> {
    return this.http.put<string>(`${this.apiUrl}/${index}`, updatedTask);
  }

  deleteTask(index: number): Observable<string> {
    return this.http.delete<string>(`${this.apiUrl}/${index}`);
  }
}
