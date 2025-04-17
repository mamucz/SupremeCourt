// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Module registration with server health monitoring
// File Name: app.component.ts

import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './Services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { HealthCheckService } from './Services/health-check.service';
import { ConnectionLostComponent } from './Pages/connection-lost/connection-lost.component';
import { HeaderComponent } from './Pages/header/header.component';
import { FooterComponent } from './Pages/footer/footer.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TranslateModule,
    ConnectionLostComponent,
    HeaderComponent,
    FooterComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  isConnected = true;

  constructor(
    public auth: AuthService,
    private translate: TranslateService,
    private health: HealthCheckService
  ) {
    const savedLang = localStorage.getItem('lang') || 'cs';
    this.translate.setDefaultLang(savedLang);
    this.translate.use(savedLang);

    // 🔄 Pravidelný health-check
    this.health.pollHealth(1000).subscribe(status => {
      this.isConnected = status;
    });
  }

  logout() {
    this.auth.logout();
  }

  switchLanguage(lang: string) {
    this.translate.use(lang);
    localStorage.setItem('lang', lang);
  }
}
