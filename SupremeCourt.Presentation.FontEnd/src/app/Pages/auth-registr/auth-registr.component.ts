// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Registration page component
// File Name: auth-registr.component.ts

import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-auth-registr',
  standalone: true,
  imports: [FormsModule, CommonModule, TranslateModule],
  templateUrl: './auth-registr.component.html',
  styleUrl: './auth-registr.component.scss'
})
export class AuthRegistrPagesComponent {
  username: string = '';
  password: string = '';

  message: string = '';
  errorMessage: string = '';

  constructor(
    private authService: AuthService,
    private translate: TranslateService
  ) {
    const savedLang = localStorage.getItem('lang') || 'cs';
    this.translate.setDefaultLang(savedLang);
    this.translate.use(savedLang);
  }

  onSubmit() {
    this.authService.register(this.username, this.password).subscribe({
      next: () => {
        this.translate.get('REGISTER.SUCCESS').subscribe((text) => {
          this.message = text;
          this.errorMessage = '';
        });
      },
      error: () => {
        this.translate.get('REGISTER.ERROR').subscribe((text) => {
          this.errorMessage = text;
          this.message = '';
        });
      }
    });
  }
}
