// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Authentication Service
// File Name: auth.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = 'https://localhost:7078/api/auth';

  constructor(private http: HttpClient, private router: Router) {}

  // 🔐 Registrace
  register(username: string, password: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/register`, {
      username,
      password
    });
  }

  // 🔐 Přihlášení
  login(username: string, password: string): Observable<{ token: string; userId: number }> {
    return this.http.post<{ token: string; userId: number }>(`${this.baseUrl}/login`, {
      username,
      password
    });
  }

  // 💾 Uloží token i userId do localStorage
  saveSession(token: string, userId: number): void {
    localStorage.setItem('token', token);
    localStorage.setItem('userId', userId.toString());
  }

  // 🔐 Odhlášení
  logout(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.http.post(`${this.baseUrl}/logout`, {}, {
        headers: { Authorization: `Bearer ${token}` }
      }).subscribe();
    }

    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    this.router.navigate(['/login']);
  }

  // ✅ Vrací true, pokud je uživatel přihlášen
  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  // 📦 Získá token z localStorage
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  // 📦 Získá ID uživatele
  getUserId(): number | null {
    const id = localStorage.getItem('userId');
    return id ? parseInt(id, 10) : null;
  }

  // 📦 Vytvoří autorizované hlavičky (např. pro API volání)
  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }
}
