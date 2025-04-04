// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Routing for authentication pages
// File Name: app.routes.ts

import { Routes } from '@angular/router';
import { AuthRegistrPagesComponent } from './Pages/auth-registr/auth-registr.component';
import { AuthLoginComponent } from './Pages/auth-login/auth-login.component';
import { HomePageComponent } from './Pages/home/home.component'; // ✅ přidáno
import { WaitingRoomListComponent } from './Pages/waiting-room-list/waiting-room-list.component';
import { WaitingRoomComponent } from './Pages/waiting-room/waiting-room.component';

export const routes: Routes = [
  { path: '', component: HomePageComponent },         // ✅ Změna!
  { path: 'register', component: AuthRegistrPagesComponent },
  { path: 'login', component: AuthLoginComponent },
  { path: 'waiting-rooms', component: WaitingRoomListComponent }, // ✅ nový route
  { path: 'waiting-room/:id', component: WaitingRoomComponent } // ✅ přidáno
];
