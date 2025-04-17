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



@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TranslateModule,
    ConnectionLostComponent
  ],
  template: `
    <ng-container *ngIf="isConnected; else disconnected">
  <!-- 🔝 Horní menu -->
  <nav class="top-menu w-full bg-green-600 text-white p-3 flex justify-end items-center space-x-4 shadow-md">
  <!-- 🌍 Jazykový výběr s vlajkami -->
  <select
    (change)="switchLanguage($any($event.target).value)"
    class="bg-green-700 text-white rounded-full px-3 py-1 outline-none hover:bg-green-800 cursor-pointer text-xl"
  >
    <option value="cs">🇨🇿</option>
    <option value="en">🇬🇧</option>
    <option value="ja">🇯🇵</option>
  </select>
    <!-- 🚪 Odhlášení -->
    <button
      *ngIf="auth.isLoggedIn()"
      (click)="logout()"
      class="bg-red-600 hover:bg-red-700 text-white rounded px-3 py-1"
    >
      🚪 Logout
    </button>
  </nav>

  <!-- 🔁 Obsah aplikace -->
  <router-outlet></router-outlet>
    </ng-container>

    <ng-template #disconnected>
      <app-connection-lost></app-connection-lost>
    </ng-template>
  `,
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
