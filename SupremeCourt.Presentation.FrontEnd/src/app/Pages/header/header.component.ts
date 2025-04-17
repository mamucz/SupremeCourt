import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  currentLang: string = 'cs';
  dropdownOpen = false; // ← TADY je ta proměnná!
  languages = [
    { code: 'cs', name: 'Čeština' },
    { code: 'en', name: 'English' },
    { code: 'ja', name: '日本語' }
  ];

  constructor(
    public auth: AuthService,
    private translate: TranslateService
  ) {
    const savedLang = localStorage.getItem('lang') || 'cs';
    this.translate.setDefaultLang(savedLang);
    this.translate.use(savedLang);
    this.currentLang = savedLang;
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
}
