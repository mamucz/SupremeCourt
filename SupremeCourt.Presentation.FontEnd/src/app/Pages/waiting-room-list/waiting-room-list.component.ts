// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Component for displaying waiting rooms with i18n
// File: waiting-room-list.component.ts

import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../Services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core'; // ✅ Přidat
import * as signalR from '@microsoft/signalr';
import { Router } from '@angular/router';

interface WaitingRoomDto {
  waitingRoomId: number;
  createdAt: string;
  createdBy: string;
  playerCount: number;
}

@Component({
  selector: 'app-waiting-room-list',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  templateUrl: './waiting-room-list.component.html',
  styleUrls: ['./waiting-room-list.component.scss']
})
export class WaitingRoomListComponent implements OnInit, OnDestroy {
  waitingRooms: WaitingRoomDto[] = [];
  message: string = '';
  error: string = '';
  private hubConnection!: signalR.HubConnection;

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private translate: TranslateService,
    private router: Router
  ) {
    const savedLang = localStorage.getItem('lang') || 'cs';
    this.translate.setDefaultLang(savedLang);
    this.translate.use(savedLang);
  }

  ngOnInit(): void {
    this.loadWaitingRooms();
    this.setupSignalR();
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  loadWaitingRooms() {
    this.http.get<WaitingRoomDto[]>('https://localhost:7078/api/waitingroom/waitingrooms', {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: (data) => {
        this.waitingRooms = data || [];
        this.error = '';
        if (this.waitingRooms.length === 0) {
          this.translate.get('WAITINGROOM.EMPTY').subscribe(text => this.message = text);
        }
      },
      error: (err) => {
        this.waitingRooms = [];
        this.message = '';
        this.translate.get('WAITINGROOM.CREATE_ERROR').subscribe(text => {
          this.error = err.error?.message || text;
        });
      }
    });
  }

  createWaitingRoom() {
    const playerId = this.auth.getUserId();
    if (!playerId) {
      this.translate.get('WAITINGROOM.CREATE_ERROR').subscribe(text => this.error = text);
      return;
    }

    this.http.post<any>('https://localhost:7078/api/waitingroom/create', { playerId }, {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: res => {
        this.translate.get('WAITINGROOM.CREATED').subscribe(text => {
          this.message = `✅ ${text.replace('#', res.waitingRoomId)}`;
        });
        this.error = '';
      },
      error: err => {
        this.translate.get('WAITINGROOM.CREATE_ERROR').subscribe(text => {
          this.error = err.error?.message || text;
        });
        this.message = '';
      }
    });
  }

  joinRoom(gameId: number) {
    const playerId = this.auth.getUserId();
    if (!playerId) {
      this.error = 'Nejste přihlášen.';
      return;
    }
  
    this.http.post('https://localhost:7078/api/waitingroom/join', {
      waitingRoomId: gameId,
      playerId
    }, {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: () => {
        this.message = `✅ Připojeno k místnosti #${gameId}`;
        this.error = '';
        // přesměrování do detailu místnosti
        this.router.navigate([`/waiting-room/${gameId}`]);
      },
      error: err => {
        this.error = err.error?.message || 'Nepodařilo se připojit.';
        this.message = '';
      }
    });
  }
  

  private setupSignalR(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7078/waitingRoomListHub', {
        accessTokenFactory: () => this.auth.getToken() || ''
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection
      .start()
      .then(() => {
        console.log('✅ SignalR connected to WaitingRoomListHub');
        return this.hubConnection.invoke('JoinWaitingRoomList'); // ✅ Zavolá se až po připojení
      })
      .catch(err => {
        console.error('❌ SignalR connection error:', err);
      });

    this.hubConnection.on('NewWaitingRoomCreated', (newRoom: WaitingRoomDto) => {
      this.waitingRooms.push(newRoom);
      this.translate.get('WAITINGROOM.CREATED').subscribe(text => {
        this.message = `🆕 ${text.replace('#', newRoom.waitingRoomId)}`;
      });
    });
  }
}
