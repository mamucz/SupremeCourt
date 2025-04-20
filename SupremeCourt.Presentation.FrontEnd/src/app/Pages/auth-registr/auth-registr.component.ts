// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Registration page component (Reactive Form + obrázek)
// File Name: auth-registr.component.ts

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-auth-registr',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './auth-registr.component.html',
  styleUrl: './auth-registr.component.scss'
})
export class AuthRegistrPagesComponent {
  registerForm: FormGroup;
  selectedImage: File | null = null;
  imagePreviewUrl: string | null = null;

  message: string = '';
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private translate: TranslateService
  ) {
    this.registerForm = this.fb.group({
      nickname: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });

    const savedLang = localStorage.getItem('lang') || 'cs';
    this.translate.setDefaultLang(savedLang);
    this.translate.use(savedLang);
  }

  get passwordsDoNotMatch(): boolean {
    const pw = this.registerForm.get('password')?.value;
    const confirm = this.registerForm.get('confirmPassword')?.value;
    return pw && confirm && pw !== confirm;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedImage = input.files[0];
      this.imagePreviewUrl = URL.createObjectURL(this.selectedImage);
    }
  }

  onSubmit(): void {
    if (this.registerForm.invalid || this.passwordsDoNotMatch) {
      return;
    }

    const nickname = this.registerForm.get('nickname')?.value;
    const password = this.registerForm.get('password')?.value;

    const formData = new FormData();
    formData.append('UserName', nickname); // ← důležité pro backend
    formData.append('Password', password);

    if (this.selectedImage) {
      formData.append('ProfileImage', this.selectedImage);
    }

    this.authService.registerWithImage(formData).subscribe({
      next: () => {
        this.translate.get('REGISTER.SUCCESS').subscribe((text) => {
          this.message = text;
          this.errorMessage = '';
        });

        this.registerForm.reset();
        // Necháme obrázek a náhled, jak sis přál
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
