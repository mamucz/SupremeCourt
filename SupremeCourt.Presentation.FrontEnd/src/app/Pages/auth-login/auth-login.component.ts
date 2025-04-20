// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Login page component with profile image sync
// File Name: auth-login.component.ts

import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-auth-login',
  standalone: true,
  imports: [FormsModule, CommonModule, TranslateModule],
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
    private translate: TranslateService
  ) {
    const savedLang = localStorage.getItem('lang') || 'cs';
    this.translate.setDefaultLang(savedLang);
    this.translate.use(savedLang);
  }

  onSubmit(): void {
    this.auth.login(this.username, this.password).subscribe({
      next: () => {
        this.message = 'Přihlášení bylo úspěšné.';
        this.errorMessage = '';
        this.router.navigate(['/waiting-rooms']);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Přihlášení se nezdařilo.';
        this.message = '';
      }
    });
  }
}
