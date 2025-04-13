import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, interval, map, of, switchMap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HealthCheckService {
    private readonly healthUrl = environment.apiUrl + '/health';

  constructor(private http: HttpClient) {}

  checkHealth() {
    return this.http.get(this.healthUrl, { responseType: 'text' }).pipe(
      map(() => true),
      catchError(() => of(false))
    );
  }

  // Volání každých X sekund
  pollHealth(intervalMs = 1000) {
    return interval(intervalMs).pipe(
      switchMap(() => this.checkHealth())
    );
  }
}
