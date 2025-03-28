// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Registration page component
// Finle Name: auth-login.component.ts

import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './auth-login.component.html',
  styleUrl: './auth-login.component.scss'
})
export class AuthLoginComponent {
  username = '';
  password = '';
  message = '';
  errorMessage = '';

  constructor(
    private auth: AuthService, // ✅ přidej "private" nebo "public"
    private router: Router
  ) {}

  onSubmit() {
    this.auth.login(this.username, this.password).subscribe({
      next: (res) => {
        this.auth.saveSession(res.token, res.userId); // Uloží token i userId
        this.message = 'Přihlášení bylo úspěšné.';
        this.router.navigate(['/waiting-rooms']);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Login failed.';
        this.message = '';
      }
    });
  }  
}
