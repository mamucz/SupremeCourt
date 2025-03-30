// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Module registration
// File Name: app.component.ts

import { Component } from '@angular/core';
import { RouterModule } from '@angular/router'; // ✅ přidáno pro router-outlet
import { CommonModule } from '@angular/common';
import { AuthService } from './Services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core'; // ✅ Import

@Component({
  selector: 'app-root',
  standalone: true, // ✅ protože používáš standalone
  imports: [CommonModule, RouterModule, TranslateModule], // ✅ zde přidat RouterModule
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(public auth: AuthService,
    private translate: TranslateService) {
      const savedLang = localStorage.getItem('lang') || 'cs';
      this.translate.setDefaultLang(savedLang);
      this.translate.use(savedLang);
    }

  logout() {
    this.auth.logout();
  }
  switchLanguage(lang: string) {
    this.translate.use(lang);
    localStorage.setItem('lang', lang);
  }
}


