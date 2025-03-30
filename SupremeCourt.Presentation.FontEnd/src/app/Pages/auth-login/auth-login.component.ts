// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Registration page component
// File Name: auth-login.component.ts

import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core'; // ✅ Import

@Component({
  selector: 'app-auth-login',
  standalone: true,
  imports: [FormsModule, CommonModule, TranslateModule], // ✅ Přidat sem
  templateUrl: './auth-login.component.html',
  styleUrls: ['./auth-login.component.scss']
})
export class AuthLoginComponent {
  username = '';
  password = '';
  message = '';
  errorMessage = '';

  constructor(
    private auth: AuthService,
    private router: Router,
    private translate: TranslateService) {
      const savedLang = localStorage.getItem('lang') || 'cs';
      this.translate.setDefaultLang(savedLang);
      this.translate.use(savedLang);
    }

  onSubmit() {
    this.auth.login(this.username, this.password).subscribe({
      next: (res) => {
        this.auth.saveSession(res.token, res.userId);
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
