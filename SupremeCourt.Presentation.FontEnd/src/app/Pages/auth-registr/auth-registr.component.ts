// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Registration page component
// Finle Name: auth-registr.component.ts

import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-auth-registr',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './auth-registr.component.html',
  styleUrl: './auth-registr.component.scss'
})
export class AuthRegistrPagesComponent {
  username: string = '';
  password: string = '';

  message: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService) { }

  OnSubmit() {
    this.authService.register(this.username, this.password).subscribe({
      next: (response) => {
        // ✅ Zde vypíšeš návratové hodnoty
        this.message = response.message || 'Registrace proběhla úspěšně.';
        this.errorMessage = ''; // smažeme předchozí chybu
      },
      error: (err) => {
        // ❌ Vypsání chybové zprávy
        this.errorMessage = err.error?.message || 'Nastala chyba při registraci.';
        this.message = '';
      }
    });
  }
}