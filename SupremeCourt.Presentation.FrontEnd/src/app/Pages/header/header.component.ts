import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../Services/auth.service';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  currentLang: string = 'cs';
  dropdownOpen = false;
  avatarUrl: string | null = null;

  languages = [
    { code: 'cs', name: 'Čeština' },
    { code: 'en', name: 'English' },
    { code: 'ja', name: '日本語' }
  ];

  constructor(
    public auth: AuthService,
    private translate: TranslateService,
    private router: Router
  ) {
    const savedLang = localStorage.getItem('lang') || 'cs';
    this.translate.setDefaultLang(savedLang);
    this.translate.use(savedLang);
    this.currentLang = savedLang;
  }

  ngOnInit(): void {
    this.auth.refreshProfileImageUrl(); // 👈 načti z localStorage
    this.auth.currentImageUrl$.subscribe(url => {
      this.avatarUrl = url;
    });
  }

  logout(): void {
    this.auth.logout();
  }

  switchLanguage(lang: string): void {
    this.translate.use(lang);
    localStorage.setItem('lang', lang);
    this.currentLang = lang;
  }

  getFlag(code: string): string {
    const map: Record<string, string> = {
      cz: 'cz',
      cs: 'cz',
      en: 'gb',
      ja: 'jp'
    };
    return map[code] || code;
  }

  getUserName(): string {
    return this.auth.getUserName() ?? 'Uživatel';
  }

  goToProfile(): void {
    this.router.navigate(['/profile']);
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }

  goHome(): void {
    if (this.auth.isLoggedIn()) {
      this.router.navigate(['/waiting-rooms']);
    } else {
      this.router.navigate(['/']);
    }
  }
}
