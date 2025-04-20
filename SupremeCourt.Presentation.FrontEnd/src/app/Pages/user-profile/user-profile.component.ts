// Autor: Petr Ondra
// Date: 19.4.2025
// Description: User profile management – přezdívka, heslo, automatická změna obrázku po výběru
// File Name: user-profile.component.ts

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../Services/auth.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
  profileForm: FormGroup;
  passwordForm: FormGroup;

  currentImageUrl: string | null = null;

  message = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private authService: AuthService,
    private router: Router
  ) {
    this.profileForm = this.fb.group({
      nickname: ['', Validators.required]
    });

    this.passwordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    const nickname = this.authService.getUserName();
    this.profileForm.patchValue({ nickname });

    this.authService.refreshProfileImageUrl();

    this.authService.currentImageUrl$.subscribe(url => {
      this.currentImageUrl = url;
    });
  }

  // 📸 Výběr a okamžité nahrání profilového obrázku
  onFileSelectedAndUpload(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const selectedImage = input.files[0];
    const formData = new FormData();
    formData.append('profileImage', selectedImage);

    this.http.put(`${environment.apiUrl}/auth/me`, formData, {
      headers: this.authService.getAuthHeaders()
    }).subscribe({
      next: () => {
        const userId = this.authService.getUserId();
        const imageUrl = `${environment.apiUrl}/player/${userId}/profile-picture?${Date.now()}`;
        this.authService.setProfileImageUrl(imageUrl);
        this.message = 'Profilový obrázek byl úspěšně změněn.';
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'Chyba při nahrávání profilového obrázku.';
        this.message = '';
      }
    });
  }

  // 🔁 ZMĚNA PŘEZDÍVKY
  onChangeNickname(): void {
    const formData = new FormData();
    formData.append('nickname', this.profileForm.get('nickname')?.value);

    this.http.put(`${environment.apiUrl}/auth/me`, formData, {
      headers: this.authService.getAuthHeaders()
    }).subscribe({
      next: () => {
        const newNickname = this.profileForm.get('nickname')?.value;
        this.authService.setUserName(newNickname);
        this.message = 'Přezdívka byla změněna.';
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'Chyba při změně přezdívky.';
        this.message = '';
      }
    });
  }

  // 🔁 ZMĚNA HESLA
  onChangePassword(): void {
    const oldPw = this.passwordForm.get('oldPassword')?.value;
    const newPw = this.passwordForm.get('newPassword')?.value;
    const confirm = this.passwordForm.get('confirmPassword')?.value;

    if (newPw !== confirm) {
      this.errorMessage = 'Nová hesla se neshodují.';
      this.message = '';
      return;
    }

    this.http.post(`${environment.apiUrl}/auth/change-password`, {
      currentPassword: oldPw,
      newPassword: newPw
    }, {
      headers: this.authService.getAuthHeaders()
    }).subscribe({
      next: () => {
        this.authService.logout();
        this.router.navigate(['/login']);
      },
      error: () => {
        this.errorMessage = 'Staré heslo není správné.';
        this.message = '';
      }
    });
  }
}
