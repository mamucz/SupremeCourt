// Autor: Petr Ondra
// Date: 9.3.2021 (aktualizováno 19.4.2025)
// Description: Authentication Service (včetně registrace s obrázkem, správa session, logout, token access)
// File Name: auth.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = `${environment.apiUrl}/auth`;
  private currentUserSubject = new BehaviorSubject<string | null>(this.getUserName());
  currentUser$ = this.currentUserSubject.asObservable();

  private currentImageUrlSubject = new BehaviorSubject<string | null>(this.getProfileImageUrl());
  currentImageUrl$ = this.currentImageUrlSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  // 🔐 Registrace bez obrázku (JSON payload)
  register(username: string, password: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/register`, {
      username,
      password
    });
  }

  // 🔐 Registrace s obrázkem (FormData payload)
  registerWithImage(formData: FormData): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/register`, formData);
  }

  // 🔐 Přihlášení
  login(username: string, password: string): Observable<{
    token: string;
    userId: number;
    userName: string;
    profileImageUrl?: string;
  }> {
    return this.http.post<{
      token: string;
      userId: number;
      userName: string;
      profileImageUrl?: string;
    }>(`${this.baseUrl}/login`, {
      username,
      password
    }).pipe(
      tap(response => {
        this.saveSession(response.token, response.userId, response.userName);

        // 🖼️ Vytvoř trvalou URL na profilový obrázek
        const imageUrl = `${this.baseUrl.replace('/auth', '')}/player/${response.userId}/profile-picture?${Date.now()}`;
        this.setProfileImageUrl(imageUrl);
      })
    );
  }

  // 💾 Uloží token, userId a userName do localStorage
  saveSession(token: string, userId: number, userName: string, profileImageUrl?: string): void {
    localStorage.setItem('token', token);
    localStorage.setItem('userId', userId.toString());
    localStorage.setItem('userName', userName);
    this.currentUserSubject.next(userName);
  }

  // 🔐 Odhlášení
  logout(): void {
    const token = this.getToken();
    if (token) {
      this.http.post(`${this.baseUrl}/logout`, {}, {
        headers: this.getAuthHeaders()
      }).subscribe({
        next: () => {
          // OK – token smazán
        },
        error: () => {
          // I tak smaž data lokálně
        }
      });
    }

    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    localStorage.removeItem('userName');
    localStorage.removeItem('profileImageUrl');
    this.currentUserSubject.next(null);
    this.currentImageUrlSubject.next(null);

    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUserId(): number | null {
    const id = localStorage.getItem('userId');
    return id ? parseInt(id, 10) : null;
  }

  getUserName(): string | null {
    return localStorage.getItem('userName');
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  setUserName(userName: string): void {
    localStorage.setItem('userName', userName);
    this.currentUserSubject.next(userName);
  }

  setProfileImageUrl(url: string): void {
    localStorage.setItem('profileImageUrl', url);
    this.currentImageUrlSubject.next(url);
  }

  getProfileImageUrl(): string | null {
    return localStorage.getItem('profileImageUrl');
  }

  refreshProfileImageUrl(): void {
    const url = this.getProfileImageUrl();
    this.currentImageUrlSubject.next(url);
  }
}
