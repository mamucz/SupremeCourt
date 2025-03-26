// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Module registration
// File Name: app.component.ts

import { Component } from '@angular/core';
import { RouterModule } from '@angular/router'; // ✅ přidáno pro router-outlet
import { CommonModule } from '@angular/common';
import { AuthService } from './Services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true, // ✅ protože používáš standalone
  imports: [CommonModule, RouterModule], // ✅ zde přidat RouterModule
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(public auth: AuthService) {}

  logout() {
    this.auth.logout();
  }
}

